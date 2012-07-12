using System;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using Vulcano.Engine;
using ScriptingUtils.Streams;

namespace Vulcano.App{
	public class CorrerScript {
		public event Action<LogEvent> OnMessage;
		public Dictionary<string, object> Data { get; set; }

		public CorrerScript() {
			Data = new Dictionary<string, object>();
		}

		public void Print(string message, Exception error = null) {
			if (OnMessage == null)
				return;
			if (string.IsNullOrEmpty(message))
				return;
			var msg = new LogEvent(message.Trim(), this, "Script") { Error = error };
			OnMessage(msg);
		}

		public void Correr(string script, string carpeta) {
			var engine = Utils.GetScriptingEngine(f => f.AddSearchPath(carpeta));
			var outs = new CallbackStream();
			outs.Callback += s => Print(s);
			engine.Runtime.IO.SetOutput(outs, outs.OutEncoding);
			try {
				var scope = engine.CreateScope(Data);
				var source = engine.CreateScriptSourceFromString(script);
				source.Execute(scope);
			} catch (Exception ex) {
				//http://stackoverflow.com/questions/1465262/getting-traceback-information-from-ironpython-exceptions
				//engine.GetService<ExceptionOperations>().FormatException(exception);
				var error = engine.GetService<ExceptionOperations>().FormatException(ex);
				Print(error, ex);
				//var msg = "Error de sintaxis " + Utils.FormatSyntaxError(exs);
			}
		}
	}
}