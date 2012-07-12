using System;
using System.Collections.Generic;
using Vulcano.Engine.Dominio;

namespace Vulcano.App.Especiales{
	public class ProcesosUtils{
		public static void Adicionar(Dictionary<int, float> map, int numero, float? valor, float? ponderacion) {
			if (valor == null || ponderacion == null)
				return;
			if (!map.ContainsKey(numero)) map[numero] = 0;
			map[numero] += valor.Value * ponderacion.Value;
		}

		public static void LlenarResultado(Dictionary<int, float> map, Resultado res) {
			if (map.ContainsKey(1)) res.Indicador1 = map[1];
			if (map.ContainsKey(2)) res.Indicador2 = map[2];
			if (map.ContainsKey(3)) res.Indicador3 = map[3];
			if (map.ContainsKey(4)) res.Indicador4 = map[4];
			if (map.ContainsKey(5)) res.Indicador5 = map[5];
			if (map.ContainsKey(6)) res.Indicador6 = map[6];
		}

		public static Resultado CrearRes(Resultado tpl) {
			var res = new Resultado {
			                        	Id = Guid.NewGuid(),
			                        	Canton = tpl.Canton,
			                        	Id_ciudad = tpl.Id_ciudad,
			                        	Fecha = DateTime.Now,
			                        	Proceso = tpl.Proceso
			                        };
			return res;
		}
	}
}