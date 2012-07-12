using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Vulcano.Engine.Dominio;

namespace Vulcano.App.Reportes {
	public class ExportadorExcel {
		public FichaIndicadores Ficha { get; set; }

		public void Guardar(IDictionary<string, IList<Resultado>> datos, string archivo) {
			using (var pck = new ExcelPackage(new FileInfo(archivo))) {
				try {
					foreach (var par in datos) {
						AppNotifier.Print("Creando hoja " + par.Key);
						CrearHoja(par.Value, par.Key, pck);
					}
					pck.Save();
				} catch (Exception ex) {
					AppNotifier.Print("Error " + ex.Message, ex);
				}
			}
		}

		public void GuardarRedes(IList<Resultado> lista, string archivo, IList<FichaIndicadores> fichas) {
			var map = fichas.ToDictionary(x => x.Nombre);
			var grupos = lista.ToLookup(x => x.Tipo_item);

			using (var pck = new ExcelPackage(new FileInfo(archivo))) {
				try {
					foreach (var grupo in grupos) {
						if (!map.ContainsKey(grupo.Key))
							continue;
						Ficha = map[grupo.Key];
						CrearHoja(grupo.ToList(), Ficha.Nombre, pck);
					}
					pck.Save();
				} catch (Exception ex) {
					AppNotifier.Print("Error " + ex.Message, ex);
				}
			}
		}

		public void CrearHoja(IList<Resultado> lista, string hoja, ExcelPackage pck) {

			var columnas = new List<string> { 
				"Canton","Tipo_item", "Proceso", "Nombre", "Codigo1", "Codigo2",
				"Completo","numEvaluados","numComponentes"
			};
			columnas.AddRange(Ficha.Temas);
			columnas.AddRange(new[] { "Fecha", "Area" });
			var sheet = pck.Workbook.Worksheets.Add(hoja);
			sheet.InsertRow(1, lista.Count);
			int i = 1;
			foreach (var columna in columnas) {
				sheet.Cells[1, i].Value = columna;
				i++;
			}
			i = 2;
			foreach (var res in lista) {
				var col = 1;
				SetValue(sheet.Cells[i, col++], res.Canton);
				SetValue(sheet.Cells[i, col++], res.Tipo_item);
				SetValue(sheet.Cells[i, col++], res.Proceso);
				SetValue(sheet.Cells[i, col++], res.Nombre);
				SetValue(sheet.Cells[i, col++], res.Codigo);
				SetValue(sheet.Cells[i, col++], res.Codigo2);
				SetValue(sheet.Cells[i, col++], res.Completo);
				SetValue(sheet.Cells[i, col++], res.Numevaluados);
				SetValue(sheet.Cells[i, col++], res.Numcomponentes);
				for (int j = 1; j <= Ficha.Temas.Count; j++) {
					var f = ValorInd(res, j);
					SetValue(sheet.Cells[i, col++], f);
				}
				SetValue(sheet.Cells[i, col++], res.Fecha);
				SetValue(sheet.Cells[i, col++], res.Area);
				//sheet.Cells[i, col++].Value = res.Fecha;
				//sheet.Cells[i, col++].Value = res.Area;
				if (i % 500 == 0)
					AppNotifier.Print("Generados " + i);
				i++;
			}
		}

		protected void SetValue(ExcelRange range, object value) {
			if (value == null)
				return;
			if (value is DateTime) {
				range.Style.Numberformat.Format = "mm-dd-yy";
			}
			if (value is float || value is double) {
				range.Style.Numberformat.Format = "#,##0.00";
			}
			range.Value = value;
		}

		public float? ValorInd(Resultado res, int num) {
			switch (num) {
				case 1: return res.Indicador1;
				case 2: return res.Indicador2;
				case 3: return res.Indicador3;
				case 4: return res.Indicador4;
				case 5: return res.Indicador5;
				case 6: return res.Indicador6;
			}
			return null;
		}

	}
}
