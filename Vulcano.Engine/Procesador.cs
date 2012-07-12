using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;
using Vulcano.Engine.Dominio;
using ScriptingUtils.Streams;
using Vulcano.Engine.Interfaces;

namespace Vulcano.Engine {
	public enum EstadoProceso {
		Inicio, Corriendo, Fin, Cancelado, Error
	}

	public class ConfigProceso {
		public string ScriptFile { get; set; }
		public string CarpetaTrabajo { get; set; }
	}

	public class Procesador {
		public FichaIndicadores Ficha { get; set; }
		public IFuenteDatos Fuente { get; set; }
		public Catalogo Catalogo { get; set; }
		public MapeosFuente Mapeos { get; set; }
		public Ciudad City { get; set; }
		public ScriptEngine Engine { get; set; }
		public IList<Resultado> Resultados { get; set; }

		public event Action<LogEvent> OnMessage;
		public event Action<int, string> OnProgress;

		public Procesador() {
			Catalogo = new Catalogo();
			Mapeos = new MapeosFuente();
		}

		public void Progreso(int numero, string tipo) {
			if (OnProgress == null)
				return;
			OnProgress(numero, tipo);
		}

		public void Calcular(ConfigProceso config) {
			Engine = Utils.GetScriptingEngine(f => f.AddSearchPath(config.CarpetaTrabajo));
			var outs = new CallbackStream();
			outs.Callback += s => Print(s);
			Engine.Runtime.IO.SetOutput(outs, outs.OutEncoding);
			try {
				
				Calcular2(config);
			} catch (Exception ex) {
				//http://stackoverflow.com/questions/1465262/getting-traceback-information-from-ironpython-exceptions
				//engine.GetService<ExceptionOperations>().FormatException(exception);
				var error = Engine.GetService<ExceptionOperations>().FormatException(ex);
				Print(error, ex);
				//var msg = "Error de sintaxis " + Utils.FormatSyntaxError(exs);
			}
		}

		protected void Calcular2(ConfigProceso config) {
			// coger un script en python, precompilar y correr en cada parte
			var scope = Engine.CreateScope();
			Resultados = new List<Resultado>();
			scope.SetVariable("ficha", Ficha);
			scope.SetVariable("catalogo", Catalogo);
			scope.SetVariable("fuenteDatos", Fuente);
			scope.SetVariable("mapeos", Mapeos);
			scope.SetVariable("ciudad", City);
			scope.SetVariable("resultados", Resultados);
			scope.SetVariable("proc", this);

			var txt = File.ReadAllText(config.ScriptFile);
			var source = Engine.CreateScriptSourceFromString(txt);
			var compiled = source.Compile();
			compiled.Execute(scope);
			Print("Resultados: " + Resultados.Count);
			Print("Grabando");
			Progreso(Resultados.Count, "inicio");
			Grabador.Guardar(Resultados, (i, msg) => {

				if (i % 500 == 0) {
					Print("Guardados " + i);
					Progreso(i, msg);
				}
				if (msg == "total")
					Print("Fin guardar");
			});
		}

		public Detalle_res CrearDetalle(Resultado padre, string contexto, string variable, string valorTexto) {
			return new Detalle_res {
				Id = Guid.NewGuid(), Id_padre = padre.Id, Contexto = contexto,
				Variable = variable, Valor = valorTexto
			};
		}

		public Resultado CrearResultado(ValoresFuente fuente) {
			var res = new Resultado {
				Id = Guid.NewGuid(),
				Tipo_item = fuente.Tipo,
				Id_item = fuente.IdItem,
				Id_ciudad = City.Id,
				Proceso = Ficha.Nombre,
				Codigo = fuente.Codigo1,
				Codigo2 = fuente.Codigo2,
				Canton = City.Nombre,
				Fecha = DateTime.Now
			};
			return res;
		}

		public void Print(string message, Exception error = null) {
			if (OnMessage == null)
				return;
			if (string.IsNullOrEmpty(message))
				return;
			var msg = new LogEvent(message.Trim(), this, "Procesador") { Error = error };
			OnMessage(msg);
		}

	}

}
