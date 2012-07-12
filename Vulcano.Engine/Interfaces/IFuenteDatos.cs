using System.Collections.Generic;
using Vulcano.Engine.Dominio;

namespace Vulcano.Engine.Interfaces {
	/// <summary>
	/// Representa una fuente de datos abstracta
	/// </summary>
	public interface IFuenteDatos {
		IList<ValoresFuente> Valores(Ciudad city, int limite, dynamic options = null);
		int Total(Ciudad city, dynamic options = null);
		string Name { get; }
		void CompletarDatos(Resultado res, ValoresFuente fuente);
	}
}