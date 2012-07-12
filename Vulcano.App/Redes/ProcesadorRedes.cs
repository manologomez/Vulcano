using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Scripting.Hosting;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;
using ScriptingUtils.Streams;

namespace Vulcano.App.Redes {
	public class ProcesadorRedes {
		public ScriptEngine Engine { get; set; }
		public IList<Resultado> Resultados { get; set; }

		public void Calcular(ConfigProceso config, FuenteRedes fuente) {
			var pathScript = Path.GetDirectoryName(config.ScriptFile);
			Engine = Utils.GetScriptingEngine(f => f.AddSearchPath(pathScript));
			var outs = new CallbackStream();
			outs.Callback += s => AppNotifier.Print(s);
			Engine.Runtime.IO.SetOutput(outs, outs.OutEncoding);
			try {
				Procesar(config, fuente);
			} catch (Exception ex) {
				var error = Engine.GetService<ExceptionOperations>().FormatException(ex);
				AppNotifier.Print(error, ex);
			}
		}

		protected void Procesar(ConfigProceso config, FuenteRedes fuente) {
			// coger un script en python, precompilar y correr en cada parte
			var txt = File.ReadAllText(config.ScriptFile);
			var source = Engine.CreateScriptSourceFromString(txt);
			var compiled = source.Compile();

			AppNotifier.Print("Cargando datos para " + fuente.City.Nombre);
			var datos = fuente.Obtener(config.CarpetaTrabajo);

			var scope = Engine.CreateScope();
			Resultados = new List<Resultado>();
			scope.SetVariable("fuentes", datos);
			scope.SetVariable("resultados", Resultados);
			scope.SetVariable("proc", fuente);

			compiled.Execute(scope);
			AppNotifier.Print("Resultados: " + Resultados.Count);
			int borrados = RepoGeneral.BorrarResultados(new ResultadoForm { Proceso = "REDES", Ciudad = fuente.City.Nombre });
			AppNotifier.Print("Borrados " + borrados);
			AppNotifier.Print("Grabando");
			Grabador.Guardar(Resultados, (i, msg) => {
				if (i % 500 == 0) {
					AppNotifier.Print("Guardados " + i);
				}
				if (msg == "total")
					AppNotifier.Print("Fin guardar");
			});
			AppNotifier.Print("Fin proceso redes");
		}

	}
}
