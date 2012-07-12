using System;
using System.Collections.Generic;
using ISpreadsheet;
using ISpreadsheet.Utils;
using Vulcano.Engine.Dominio;

namespace Vulcano.Engine.Carga {
	public class CargadorFichas {
		public static IList<FichaIndicadores> CargarFichas(string file) {
			var book = SpreadsheetFactory.GetWorkbook(file);
			var lista = new List<FichaIndicadores>();
			var valoresStart = CellUtils.LetterToColumnNumber("G");
			foreach (var sheet in book.Sheets) {
				var ficha = new FichaIndicadores {
					Nombre = sheet.Name,
					Descripcion = sheet.GetString("A3")
				};
				lista.Add(ficha);
				bool fin = false;
				var start = valoresStart;
				while (!fin) {
					var valor = sheet.GetString(start, 2);
					if (string.IsNullOrEmpty(valor) || valor == "valores" || ficha.Temas.Contains(valor))
						fin = true;
					else
						ficha.Temas.Add(valor);
					start++;
				}

				var last = "";
				var countAme = ficha.Temas.Count;
				var numFicha = 0;
				for (int i = 3; i <= sheet.NumRows; i++) {
					var expresion = sheet.GetString("F", i) ?? "";
					if (expresion == "")
						break;
					var codigo = sheet.GetString("B", i) ?? "";
					if (codigo != "" && codigo != last) {
						numFicha++;
						var v = new Variable(countAme) { Codigo = codigo, Orden = numFicha };
						v.Nombre = sheet.GetString("D", i);
						start = valoresStart + countAme + 1;
						for (int j = 0; j < countAme; j++) {
							v.Ponderaciones[j] = sheet.GetFloat(start, i);
							start += 2;
						}
						ficha.Variables[codigo] = v;
						var extra = sheet.GetString("C", i) ?? "";
						if (extra.Contains("int"))
							v.Tipo = "int";
						if (extra.Contains("script"))
							v.EsScript = true;
						last = codigo;
					}
					var valor = new ValorVariable(countAme);
					valor.Descripcion = sheet.GetString("E", i);
					valor.Expresion = expresion;
					for (int j = 0; j < countAme; j++) {
						valor.Puntajes[j] = sheet.GetFloat(j + valoresStart, i);
					}
					ficha.Variables[last].Valores.Add(valor);
				}

			}
			if (book is IDisposable)
				(book as IDisposable).Dispose();
			return lista;
		}
	}
}