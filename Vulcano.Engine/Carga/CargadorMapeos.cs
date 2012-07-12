using System;
using System.Collections.Generic;
using ISpreadsheet;
using Vulcano.Engine.Dominio;

namespace Vulcano.Engine.Carga {
	public class CargadorMapeos {
		public static IDictionary<string, MapeosFuente> Cargar(string file) {
			var excel = SpreadsheetFactory.GetWorkbook(file);
			var lista = new Dictionary<string, MapeosFuente>();
			foreach (var sheet in excel.Sheets) {
				if (sheet.Name.StartsWith("_"))
					continue;
				var mapa = new MapeosFuente { Fuente = sheet.Name };
				lista[mapa.Fuente] = mapa;
				for (int i = 2; i <= sheet.NumRows; i++) {
					var codigo = sheet.GetString("A", i);
					if (string.IsNullOrEmpty(codigo))
						break;
					if (codigo == "_reglas_") {
						mapa.ScriptReglas = sheet.GetString("B", i);
						break;
					}
					var mapeo = new MapeoCampo();
					mapeo.CodigoVariable = codigo;
					mapeo.Campo = sheet.GetString("B", i);
					mapeo.Fuente = sheet.GetString("C", i);
					mapeo.Tipo = sheet.GetString("D", i);
					mapeo.Catalogo = sheet.GetString("E", i);
					mapeo.Expresion = sheet.GetString("F", i);
					mapa.Campos[codigo] = mapeo;
				}
			}
			if (excel is IDisposable)
				(excel as IDisposable).Dispose();
			return lista;
		}
	}
}