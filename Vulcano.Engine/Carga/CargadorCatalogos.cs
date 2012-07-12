using System;
using System.Collections.Generic;
using ISpreadsheet;

namespace Vulcano.Engine.Carga {

	public class CargadorCatalogos {

		public static Dictionary<string, Catalogo> Cargar(string file) {
			IWorkbook book = null;
			int i = 0;
			try {
				book = SpreadsheetFactory.GetWorkbook(file);
				var sheet = book.Sheets[0];
				var catalogos = new Dictionary<string, Catalogo>();
				for (i = 2; i <= sheet.NumRows; i++) {
					var formato = sheet.GetString("C", i);
					if (string.IsNullOrEmpty(formato))
						break;
					var valor = sheet.GetString("G", i) ?? "";
					var codigo = sheet.GetString("E", i) ?? "";
					if (valor == "" || codigo == "")
						continue;
					if (!catalogos.ContainsKey(formato))
						catalogos[formato] = new Catalogo { Nombre = formato };
					var mapa = catalogos[formato];
					var key = Utils.MakeKey(
						sheet.GetString("D", i), // categoria
						codigo
						);
					mapa.Equivalencias[key] = valor;
					// TODO: esto podria crear choques y reemplazos, verificar o cambiar de estructura de datos
					var ambito1 = sheet.GetString("H", i);
					var ambito2 = sheet.GetString("I", i);
					if (!string.IsNullOrEmpty(ambito1)) {
						key = Utils.MakeKey(ambito1, codigo);
						mapa.Equivalencias[key] = valor;
					}
					if (!string.IsNullOrEmpty(ambito2)) {
						key = Utils.MakeKey(ambito2, codigo);
						mapa.Equivalencias[key] = valor;
					}
				}
				return catalogos;
			} catch (Exception ex) {
				throw new ApplicationException("Error en fila " + i, ex);
			} finally {
				if (book is IDisposable)
					(book as IDisposable).Dispose();
			}
		}

	}
}