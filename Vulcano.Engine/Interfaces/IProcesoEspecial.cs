using System;
using Vulcano.Engine.Dominio;

namespace Vulcano.Engine.Interfaces {
	public interface IProcesoEspecial {
		string Nombre { get; }
		event Action<LogEvent> Mensaje;
		void Procesar(Ciudad ciudad);
	}
}