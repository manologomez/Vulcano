using System.Collections.Generic;
using System.Linq;

namespace Vulcano.Engine.Dominio{
	public class FichaIndicadores {
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public List<string> Temas { get; set; }
		public Dictionary<string, Variable> Variables { get; set; }
		public Dictionary<string, int[]> Leyendas { get; set; }

		public FichaIndicadores() {
			Variables = new Dictionary<string, Variable>();
			Leyendas = new Dictionary<string, int[]>();
			Temas = new List<string>();
		}

		public string Leyenda(int total) {
			foreach (var par in Leyendas.Where(p => p.Value != null && p.Value.Length == 2)) {
				var rango = par.Value;
				if (total >= rango[0] && total < rango[1])
					return par.Key;
			}
			return "No definido";
		}
	}
}