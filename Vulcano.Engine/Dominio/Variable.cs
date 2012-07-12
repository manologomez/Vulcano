using System.Collections.Generic;
using System.Linq;

namespace Vulcano.Engine.Dominio {
	/// <summary>
	/// Variable de un indicador
	/// </summary>
	public class Variable {
		public string Nombre { get; set; }
		public string Codigo { get; set; }
		public float?[] Ponderaciones { get; set; }
		public List<ValorVariable> Valores { get; set; }
		public string Tipo { get; set; }
		public bool EsScript { get; set; }
		public int Orden { get; set; }

		public Variable(int numeroTemas = 0) {
			Valores = new List<ValorVariable>();
			Ponderaciones = new float?[numeroTemas];
			Tipo = "cadena";
		}

		public bool ValorEnPosibles(object valor) {
			if (Utils.IsSafeNull(valor))
				return false;
			var txt = valor.ToString();
			if (txt == "") return false;
			return Valores.Any(x => x.Expresion.Equals(txt));
		}

	}
}