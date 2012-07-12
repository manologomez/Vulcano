using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Vulcano.Engine.Dominio;

namespace Vulcano.Engine {
	public class Grabador {

		public static void Guardar(IList<Resultado> resultados, Action<int, string> reporte = null) {
			var ses = NHibernateHelper.SessionFactory.OpenStatelessSession();
			var tx = ses.BeginTransaction();
			try {
				var i = 1;
				foreach (var resultado in resultados) {
					ses.Insert(resultado);
					foreach (var detalle in resultado.Detalles) {
						detalle.Padre = resultado;
						ses.Insert(detalle);
					}
					if (reporte != null)
						reporte(i, "progreso");
					i++;
				}
				if (reporte != null)
					reporte(i, "total");
				tx.Commit();
			} catch (Exception ex) {
				tx.Rollback();
				throw ex;
			} finally {
				ses.Close();
			}
		}

	}
}
