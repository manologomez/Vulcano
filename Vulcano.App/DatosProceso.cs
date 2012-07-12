using System.Collections.Generic;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;

namespace Vulcano.App {
	public class DatosProceso {
		public IDictionary<string, MapeosFuente> MapeosCargados { get; set; }
		public IList<FichaIndicadores> Fichas { get; set; }
		public IDictionary<string, Catalogo> CatalogosFuentes { get; set; }
		public IList<Ciudad> Ciudades { get; set; }

		public DatosProceso() {
			Ciudades = new List<Ciudad>();
			Fichas = new List<FichaIndicadores>();
			MapeosCargados = new Dictionary<string, MapeosFuente>();
			CatalogosFuentes = new Dictionary<string, Catalogo>();
		}

	}
}