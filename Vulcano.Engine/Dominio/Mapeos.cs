using System.Collections.Generic;

namespace Vulcano.Engine.Dominio {
	/// <summary>
	/// Tiene la colección de mapeos de una fuente específica
	/// </summary>
	public class MapeosFuente {
		public string Ficha { get; set; }
		public string Fuente { get; set; }
		public Dictionary<string, MapeoCampo> Campos { get; set; }
		public string ScriptReglas { get; set; }

		public MapeosFuente() {
			Campos = new Dictionary<string, MapeoCampo>();
		}

		public static MapeosFuente CrearDeFicha(FichaIndicadores ficha) {
			var mapeos = new MapeosFuente();
			foreach (var v in ficha.Variables.Values) {
				var mapeo = new MapeoCampo { CodigoVariable = v.Codigo };
				mapeos.Campos[v.Codigo] = mapeo;
			}
			return mapeos;
		}
	}

	/// <summary>
	/// Representa el mapeo de un campo en particular, refiriéndose a como 
	/// extraer el valor de una variable desde un campo de la fuente de datos,
	/// contando con un posible catálogo y expresión de transformación
	/// </summary>
	public class MapeoCampo {
		public string CodigoVariable { get; set; }
		public string Campo { get; set; }
		public string Fuente { get; set; }
		public string Tipo { get; set; }
		public string Catalogo { get; set; }
		public string Expresion { get; set; }

		public IList<string> GetCatalogos() {
			if (string.IsNullOrEmpty(Catalogo))
				return new List<string>();
			return Catalogo.Replace(" ", "").Split(new[] { ',' });
		}

	}
}