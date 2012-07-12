using System.Collections.Generic;
using System.Linq;
using Vulcano.Engine.Interfaces;

namespace Vulcano.Engine{
	public class FuentesFactory {
		public IList<IFuenteDatos> Fuentes { get; set; }

		public FuentesFactory() {
			Fuentes = new List<IFuenteDatos>();
		}

		public IFuenteDatos GetFuente(string nombre) {
			return Fuentes.FirstOrDefault(x => x.Name == nombre);
		}
	}
}