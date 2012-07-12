using System;
using System.Collections.Generic;
using System.Linq;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;
using Vulcano.Engine.Interfaces;

namespace Vulcano.App.Especiales {
	public class ProcesoPredios : ProcesoEspecialBase, IProcesoEspecial {
		public string Nombre { get { return "Calculo Predios"; } }
		public void Procesar(Ciudad ciudad) {
			// TODO: poner un unit of work en marcha
			Print("Proceso Predios, ciudad: " + ciudad.Nombre);
			try {
				var lista = AgruparPredios(ciudad);
				Print("Resultados: " + lista.Count);
				if (lista.Count > 0) {
					var first = lista[0];
					Print("Borrando...");
					var borrados = RepoGeneral.BorrarResultados(first.Proceso, first.Tipo_item, ciudad);
					Print("Borrados: " + borrados);
					Print("Guardando...");
					Grabador.Guardar(lista, (num, msg) => {
						if (num % 100 == 0)
							Print("Guardados " + num);
					});
					Print("Terminado");
				}
			} catch (Exception ex) {
				Print("Error: " + ex.Message, ex);
			}
		}

		public IList<Resultado> AgruparPredios(Ciudad ciudad) {
			var lista = RepoGeneral.Resultados(new ResultadoForm {
				Ciudad = ciudad.Nombre, Orden = "Id_item", Tipo = "construccion"
			});
			var nuevos = new List<Resultado>();
			var grupos = lista.ToLookup(x => x.Id_item); // ojo, x.Codigo tal vez sea mas seguro, no se puede saber
			foreach (var grupo in grupos) {
				var first = grupo.First();
				var res = ProcesosUtils.CrearRes(first);
				res.Id_item = first.Id_item;
				res.Nombre = first.Nombre;
				res.Codigo = first.Codigo;
				res.Codigo2 = first.Codigo3;
				res.Tipo_item = "predio";
				res.Numcomponentes = grupo.Count();
				res.Completo = grupo.Average(x => x.Completo);
				float area = 0;
				var map = new Dictionary<int, float>();
				foreach (var dato in grupo) {
					if (dato.Area == null || dato.Completo == null || dato.Completo.Value < Tolerancia)
						continue;
					res.Numevaluados += 1;
					area += dato.Area.Value;
					ProcesosUtils.Adicionar(map, 1, dato.Indicador1, dato.Area);
					ProcesosUtils.Adicionar(map, 2, dato.Indicador2, dato.Area);
					ProcesosUtils.Adicionar(map, 3, dato.Indicador3, dato.Area);
					ProcesosUtils.Adicionar(map, 4, dato.Indicador4, dato.Area);
				}
				if (area > 0) {
					var keys = map.Keys.ToList();
					foreach (var key in keys) {
						map[key] = map[key] / area;
					}
					ProcesosUtils.LlenarResultado(map, res);
					res.Area = area;
				}
				nuevos.Add(res);
			}
			return nuevos;
		}
	}
}