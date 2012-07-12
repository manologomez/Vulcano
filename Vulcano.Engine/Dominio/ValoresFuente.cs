using System.Collections.Generic;

namespace Vulcano.Engine.Dominio{
	/// <summary>
	/// Representa los valores extraidos de la fuente de datos, es
	/// básicamente una tabla hash glorificada con más campos
	/// </summary>
	public class ValoresFuente : Dictionary<string, object> {
		public dynamic IdItem { get; set; }
		public string Tipo { get; set; }
		public string Codigo1 { get; set; }
		public string Codigo2 { get; set; }
		public bool Incompleto { get; set; }

		public ValoresFuente() { }

		public ValoresFuente(IDictionary<string, object> dictionary)
			: base(dictionary) { }

		public ValoresFuente(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer)
			: base(dictionary, comparer) { }

		public string GetString(string key, string defaultValue = null) {
			if (!ContainsKey(key))
				return defaultValue;
			return Utils.SafeString(this[key], defaultValue);
		}

	}
}