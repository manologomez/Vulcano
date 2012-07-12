using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PetaPoco;
using Vulcano.Engine.Dominio;

namespace Vulcano.Engine {
	public class RepoGeneral {
		public static IList<Ciudad> Lista() {
			using (var db = new Database("riesgos")) {
				return db.Fetch<Ciudad>("select * from ciudad order by nombre").ToList();
			}
		}

		public static string CrearCiudad(Ciudad ciudad) {
			try {
				using (var db = new Database("riesgos")) {
					var cuenta = db.SingleOrDefault<int>("select count(id) from ciudad where nombre = @0", ciudad.Nombre);
					if (cuenta > 0) return "Ya existe una ciudad con el mismo nombre";
					db.Save("ciudad", "id", ciudad);
					return "";
				}

			} catch (Exception ex) {
				return "Error de ejecución: " + ex.Message;
			}
		}

		public static IList<Resultado> Resultados(ResultadoForm form) {
			using (var session = NHibernateHelper.SessionFactory.OpenStatelessSession()) {
				var hql = "select r from Resultado r";
				var conds = new List<string>();
				if (!string.IsNullOrEmpty(form.Ciudad))
					conds.Add(string.Format("r.Canton = '{0}'", form.Ciudad));
				if (!string.IsNullOrEmpty(form.Tipo))
					conds.Add(string.Format("r.Tipo_item = '{0}'", form.Tipo));
				if (!string.IsNullOrEmpty(form.Where))
					conds.Add(form.Where);
				if (conds.Count > 0) {
					hql += " where " + string.Join(" and ", conds);
				}
				var orden = string.IsNullOrEmpty(form.Orden) ? "Id_item" : form.Orden;
				hql += " order by " + orden;
				var q = session.CreateQuery(hql);
				return q.List<Resultado>();
			}
		}

		public static int BorrarResultados(string proceso, string tipo, Ciudad ciudad) {
			var hql = @"delete from Resultado where Proceso = :proceso and Tipo_item=:tipo
			and Canton=:ciudad and Id_ciudad=:id_ciudad";
			var q = NHibernateHelper.GetCurrentSession().CreateQuery(hql);
			
			q.SetString("proceso", proceso)
				.SetString("tipo", tipo)
				.SetString("ciudad", ciudad.Nombre)
				.SetParameter("id_ciudad", ciudad.Id);
			return q.ExecuteUpdate();
		}

		public class Cond {
			public string query;
			public string parname;
			public object parameter;
			public Cond(string query, string parname, object parameter) {
				this.query = query;
				this.parname = parname;
				this.parameter = parameter;
			}
		}

		//TODO esto revisar a ver si se quita mejor o algo
		public static int BorrarResultados(ResultadoForm form) {
			var hql = @"delete from Resultado where ";
			var cond2 = new List<Cond>();
			if (!string.IsNullOrEmpty(form.Proceso))
				cond2.Add(new Cond("Proceso=:proceso", "proceso", form.Proceso));
			if (!string.IsNullOrEmpty(form.Tipo))
				cond2.Add(new Cond("Tipo_item=:tipo", "tipo", form.Tipo));
			if (!string.IsNullOrEmpty(form.Ciudad))
				cond2.Add(new Cond("Canton=:ciudad", "ciudad", form.Ciudad));
			if (cond2.Count == 0)
				return 0;
			hql += string.Join(" and ", cond2.Select(x => x.query));
			NHibernateHelper.BeginTransaction();
			var q = NHibernateHelper.GetCurrentSession().CreateQuery(hql);
			try {
				foreach (var cond in cond2) {
					q.SetParameter(cond.parname, cond.parameter);
				}
				int num = q.ExecuteUpdate();
				NHibernateHelper.CommitTransaction();
				return num;
			} catch (Exception) {
				NHibernateHelper.CommitTransaction();
				throw;
			}

		}

		public static int BorrarResultados(IDictionary<string, object> parametros) {
			var form = new ResultadoForm();
			foreach (var parametro in parametros) {
				if (Utils.IsSafeNull(parametro.Value))
					continue;
				var txt = parametro.Value.ToString();
				switch (parametro.Key) {
					case "Proceso":
						form.Proceso = txt;
						break;
					case "Tipo":
						form.Tipo = txt;
						break;
					case "Canton":
						form.Ciudad = txt;
						break;
				}
			}
			return BorrarResultados(form);
		}

	}
}
