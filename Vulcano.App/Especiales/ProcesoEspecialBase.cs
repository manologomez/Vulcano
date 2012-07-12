using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vulcano.Engine;

namespace Vulcano.App.Especiales {
	/// <summary>
	/// Clase base con utilitarios para los procesos especiales de cálculo
	/// </summary>
	public abstract class ProcesoEspecialBase {
		public event Action<LogEvent> Mensaje;

		public float Tolerancia { get; set; }

		protected ProcesoEspecialBase() {
			Tolerancia = 1;
		}

		protected void Print(string msg, Exception error = null) {
			if (string.IsNullOrEmpty(msg) || Mensaje == null)
				return;
			var log = new LogEvent(msg, this);
			log.Error = error;
			Mensaje(log);
		}
		
	}
}
