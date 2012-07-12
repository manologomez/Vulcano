using System.Collections.Generic;

namespace Vulcano.Engine {
	public class Catalogo {
		public string Nombre { get; set; }
		public Dictionary<string, string> Equivalencias { get; set; }

		public Catalogo() {
			Equivalencias = new Dictionary<string, string>();
		}

		public string Buscar(string catalogo, string codigo) {
			if (string.IsNullOrEmpty(catalogo))
				return "";
			var key1 = Utils.MakeKey(catalogo, codigo);
			return Equivalencias.ContainsKey(key1) ? Equivalencias[key1] : "";
		}

	}
}