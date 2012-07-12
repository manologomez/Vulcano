namespace Vulcano.Engine.Dominio {
	/// <summary>
	/// Contiene las diferentes ponderaciones de una variable así como su descripción
	/// y una expresión de cálculo si fuera necesario
	/// </summary>
	public class ValorVariable {
		public string Descripcion { get; set; }
		public string Expresion { get; set; }
		public float?[] Puntajes { get; set; }

		public ValorVariable(int numeroTemas = 0) {
			Puntajes = new float?[numeroTemas];
		}
	}

}
