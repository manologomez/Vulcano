using System.Collections.Generic;
using System.Linq;
using Vulcano.Engine.Interfaces;

namespace Vulcano.Engine{
	public class ProcesosFactory {
		public IList<IProcesoEspecial> Procesos { get; set; }

		public ProcesosFactory() {
			Procesos = new List<IProcesoEspecial>();
		}

		public IProcesoEspecial GetProceso(string nombre) {
			return Procesos.FirstOrDefault(x => x.Nombre == nombre);
		}

	}
}