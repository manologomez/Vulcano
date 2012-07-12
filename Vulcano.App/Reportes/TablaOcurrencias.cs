using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Dapper;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;

namespace Vulcano.App.Reportes {
	public class TablaOcurrencias {
		public Ciudad City { get; set; }
		public FichaIndicadores Ficha { get; set; }
		public float Tolerancia { get; set; }

		public TablaOcurrencias() {
			Tolerancia = 1;
		}

		protected string NombreCol(decimal f) {
			return f == 0 ? "0" : f.ToString("##");
		}

		public void Generar(string destino) {
			var con = NHibernateHelper.GetCurrentSession().Connection;
			var sql = SqlConsulta();
			AppNotifier.Print("Creando tabla de ocurrencias para: " + City.Nombre);
			AppNotifier.Print("Tolerancia: " + Tolerancia);
			var lista = con.Query<DatoOcurrencia>(sql, new{ciudad = City.Nombre}).ToList();

			var posibles = lista.OrderBy(x => x.Valor_numerico)
				.Select(x => NombreCol(x.Valor_numerico)).Distinct()
				.ToList();
			//posibles.Sort();
			var variables = Ficha.Variables.Select(x => x.Value.Codigo);

			var grupos = lista.ToLookup(x => x.Contexto);
			var tablas = new Dictionary<string, Tuple<DataTable, DataTable>>();

			var def = new TableDef { TituloX = "Variable", EjeX = posibles, EjeY = variables.ToList() };
			foreach (var grupo in grupos) {
				def.Nombre = grupo.Key;
				var tab = def.CreateTable(typeof(int));
				foreach (var dato in grupo) {
					if (!tab.Item2.ContainsKey(dato.Variable))
						continue;
					var row = tab.Item2[dato.Variable];
					var col = NombreCol(dato.Valor_numerico);
					row[col] = dato.Cuenta;
				}
				var tablaPor = def.Porcentajes(tab.Item1);
				tablas[grupo.Key] = Tuple.Create(tab.Item1, tablaPor);
			}

			if (File.Exists(destino))
				File.Delete(destino);

			using (var pck = new ExcelPackage(new FileInfo(destino))) {
				foreach (var par in tablas) {
					var tuple = par.Value;
					var tabla = tuple.Item1;
					// tabla primaria
					var ws = pck.Workbook.Worksheets.Add(tabla.TableName);
					AddTable(1, 1, ws, tabla);
					var row = tabla.Rows.Count + 3;
					ws.Cells[row, 1].Value = "Tolerancia";
					ws.Cells[row, 1].Style.Font.Bold = true;
					ws.Cells[row, 2].Value = Tolerancia;
					ws.Cells[row, 2].Style.Font.Bold = true;
					ws.Row(1).Style.Font.Bold = true;

					// tabla porcentaje
					row += 2;
					AddTable(row, 1, ws, tuple.Item2, true);
					ws.Row(row).Style.Font.Bold = true;
				}
				pck.Save();
			}
			AppNotifier.Print("Guardada en: " + destino);
		}

		protected void AddTable(int row, int col, ExcelWorksheet ws, DataTable tabla, bool porcentaje = false) {
			ws.Cells[row, col].LoadFromDataTable(tabla, true);
			var hasta = col + tabla.Columns.Count - 1;
			var off = row + 1;
			foreach (object t in tabla.Rows) {
				var formula = string.Format("Sum({0})", new ExcelAddress(off, col + 1, off, hasta - 1).Address);
				ws.Cells[off, hasta].Formula = formula;
				if (porcentaje) {
					for (int i = col + 1; i <= hasta; i++) {
						ws.Cells[off, i].Style.Numberformat.Format = "#%";
					}
				}
				off++;
			}
			//ws.Cells[r, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
		}

		public string SqlConsulta() {
			const string tpl = @"select d.contexto, d.variable, d.valor_numerico, count(d.valor_numerico) as cuenta
			from detalle_res d
			where id_padre in (
			select id from resultado
			where canton = @ciudad and tipo_item = 'construccion'
			:tolerancia:
			)
			and contexto not in ('evaluacion','calidad')
			group by d.contexto, d.variable, d.valor_numerico
			order by d.contexto, d.variable, d.valor_numerico";
			var rep = "";
			if (Tolerancia > 0) {
				rep = " and completo >= " + Tolerancia.ToString().Replace(",", ".");
			}
			return tpl.Replace(":tolerancia:", rep);
		}

		public class DatoOcurrencia {
			public string Contexto { get; set; }
			public string Variable { get; set; }
			public decimal Valor_numerico { get; set; }
			public int Cuenta { get; set; }
		}
	}



}
