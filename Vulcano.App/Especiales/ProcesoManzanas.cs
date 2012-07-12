using System;
using System.Collections.Generic;
using System.Linq;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;
using Vulcano.Engine.Interfaces;

namespace Vulcano.App.Especiales{
	public class ProcesoManzanas : ProcesoEspecialBase, IProcesoEspecial {
		public string Nombre { get { return "Calculo Manzanas"; } }
		public void Procesar(Ciudad ciudad) {
			// TODO: poner un unit of work en marcha
			Print("Proceso Manzanas, ciudad: " + ciudad.Nombre);
			try {
				var lista = CalcularManzanas(ciudad);
				Print("Resultados: " + lista.Count);
				if (lista.Count > 0) {
					var first = lista[0];
					Print("Borrando...");
					var borrados = RepoGeneral.BorrarResultados(first.Proceso, first.Tipo_item, ciudad);
					Print("Borrados: " + borrados);
					Print("Guardando...");
					Grabador.Guardar(lista, (num, msg) => Print("Guardados " + num));
					Print("Terminado");
				}
			} catch (Exception ex) {
				Print("Error: " + ex.Message, ex);
			}
		}

		public IList<Resultado> CalcularManzanas(Ciudad ciudad) {
			var lista = RepoGeneral.Resultados(new ResultadoForm {
				Ciudad = ciudad.Nombre, Orden = "Id_item", Tipo = "construccion"
			});
			var nuevos = new List<Resultado>();
			var grupos = lista.ToLookup(x => x.Codigo3);
			foreach (var grupo in grupos) {
				var res = ProcesosUtils.CrearRes(grupo.First());
				res.Nombre = grupo.Key;
				res.Tipo_item = "manzana";
				res.Codigo = grupo.Key;
				res.Numcomponentes = grupo.Count();
				res.Completo = grupo.Average(x => x.Completo);
				//var filtrados = grupo.Where(x => x.Completo.HasValue && x.Completo == 1);
				float area = 0;
				var map = new Dictionary<int, float>();
				//foreach (var dato in filtrados) {
				foreach (var dato in grupo) {
					if (dato.Area == null || dato.Completo == null || dato.Completo.Value < Tolerancia)
						continue;
					res.Numevaluados += 1;
					area += dato.Area.Value;
					// OJO que aquí se debe generalizar más para que coja todos los indicadores
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