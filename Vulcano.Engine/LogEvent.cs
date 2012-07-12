using System;

namespace Vulcano.Engine {
	/// <summary>
	/// Evento estandar de notificacion a la UI
	/// </summary>
	public class LogEvent : EventArgs {
		public string Mensaje { get; set; }
		public string Fuente { get; set; }
		public object Sender { get; set; }
		public dynamic Objeto { get; set; }
		public Exception Error { get; set; }

		public LogEvent() { }

		public LogEvent(string mensaje, object sender, string fuente = "", dynamic objeto = null) {
			Mensaje = mensaje;
			Fuente = fuente;
			Sender = sender;
			Objeto = objeto;
		}
	}
}