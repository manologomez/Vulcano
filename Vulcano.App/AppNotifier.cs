using System;
using Vulcano.Engine;

namespace Vulcano.App {
	public class AppNotifier {
		public static event Action<LogEvent> OnMessage;

		public static void Print(string msg, Exception ex = null) {
			if (OnMessage == null)
				return;
			var ev = new LogEvent { Mensaje = msg, Error = ex };
			OnMessage(ev);
		}
	}
}