using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vulcano.Engine.Dominio;

namespace Vulcano.Engine.Interfaces {
	public interface ICargadorFichas {
		IList<FichaIndicadores> CargarFichas();
	}

	public interface ICargadorMapeos {
		/// <summary>
		/// Retorna un mapa de configuraciones de mapeo, indexado por nombre de fuente
		/// </summary>
		/// <returns></returns>
		IDictionary<string, MapeosFuente> Cargar();

		MapeosFuente GetMapeos(string fuente);
	}
}
