using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ISpreadsheet;
using Vulcano.Engine.Dominio;

namespace Vulcano.App.Redes {
	public class FuenteRedes {
		public IList<FichaIndicadores> Fichas { get; set; }
		public Ciudad City { get; set; }
		public Dictionary<string, FichaIndicadores> MapaFichas { get; set; }

		public Resultado CrearResultado(ValoresFuente fuente) {
			var res = new Resultado {
				Id = Guid.NewGuid(),
				Codigo = fuente.Codigo1,
				Nombre = fuente.Codigo2,
				Fecha = DateTime.Now,
				Id_item = fuente.IdItem,
				Id_ciudad = City.Id,
				Canton = City.Nombre,
				Tipo_item = fuente.Tipo,
				Proceso = "REDES"
			};
			return res;
		}

		public Detalle_res CrearDetalle(Resultado padre, string contexto, string variable, string valorTexto) {
			return new Detalle_res {
				Id = Guid.NewGuid(), Id_padre = padre.Id, Contexto = contexto,
				Variable = variable, Valor = valorTexto
			};
		}

		public FuenteRedes(IList<FichaIndicadores> fichas, Ciudad city) {
			Fichas = fichas;
			City = city;
			MapaFichas = Fichas.Where(x => x.Nombre != "EDIFICACIONES")
				.ToDictionary(x => x.Nombre);
		}

		public IList<ValoresFuente> Obtener(string carpeta) {
			var filename = string.Format("Redes_{0}.xlsx", City.Nombre);
			var file = Path.Combine(carpeta, filename);
			if (!File.Exists(file))
				throw new ApplicationException("El archivo " + file + " no existe");
			var wb = SpreadsheetFactory.GetWorkbook(file);
			var fuentes = new List<ValoresFuente>();
			foreach (var ws in wb.Sheets) {
				if (!(MapaFichas.ContainsKey(ws.Name) || ws.Name == "Funcional"))
					continue;
				var posibles = BuscarTablas(ws, ws.Name == "Funcional");
				if (posibles.Count == 0)
					continue;
				foreach (var t in posibles) {
					if (!MapaFichas.ContainsKey(t.Item2))
						continue;
					var ficha = MapaFichas[t.Item2];
					var tablas = LlenarTabla(ws, t.Item1, ficha);
					fuentes.AddRange(tablas);
				}
			}
			return fuentes;
		}

		protected IList<ValoresFuente> LlenarTabla(IWorksheet sheet, int row, FichaIndicadores ficha) {
			var lista = new List<ValoresFuente>();
			var total = sheet.NumRows;
			var listaVars = ficha.Variables.Select(x => x.Value).OrderBy(x => x.Orden);
			for (int i = row; i <= total; i++) {
				var codigo = sheet.GetString("B", i);
				if (string.IsNullOrEmpty(codigo))
					break;
				// IMPORTANTE
				var f = new ValoresFuente { Tipo = ficha.Nombre, IdItem = total++, Codigo1 = codigo };
				var nombre = sheet.GetString("C", i);
				f.Codigo2 = string.IsNullOrEmpty(nombre) ? codigo : nombre;
				var off = 3;
				foreach (var v in listaVars) {
					off++;
					f[v.Codigo] = sheet.GetString(off, i);
				}
				lista.Add(f);
			}
			return lista;
		}

		protected List<Tuple<int, string>> BuscarTablas(IWorksheet sheet, bool buscarNombre = false) {
			var total = sheet.NumRows;
			var lista = new List<Tuple<int, string>>();
			for (int i = 2; i <= total; i++) {
				var txt = sheet.GetString("A", i) ?? "";
				if (txt != "Canton")
					continue;
				var nombre = sheet.Name;
				if (buscarNombre) {
					nombre = sheet.GetString("A", i - 1);
					if (string.IsNullOrEmpty(nombre))
						continue;
				}
				var t = Tuple.Create(i + 1, nombre);
				lista.Add(t);
			}
			return lista;
		}

	}
}
