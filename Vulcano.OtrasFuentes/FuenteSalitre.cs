using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;
using SqlDataClasses;
using Vulcano.Engine.Interfaces;

namespace Vulcano.OtrasFuentes {
	public class FuenteSalitre : IFuenteDatos {

		public string Name { get { return "Salitre"; } }

		public void CompletarDatos(Resultado res, ValoresFuente fuente) {
			res.Nombre = Utils.PruebeValor(fuente, new[] { "nombres", "direccion" });
			res.Codigo3 = CodManzana(res);
			if (fuente.ContainsKey("num_edif")) {
				var temp = fuente.GetString("num_edif");
				foreach (var detalle in res.Detalles) {
					detalle.Id_componente = temp;
				}
			}
			res.Area = Utils.GetNumero(fuente["area_bloque"]);
		}

		public string CodManzana(Resultado res) {
			return string.IsNullOrEmpty(res.Codigo) || res.Codigo.Length < 20
				? "S/N" : res.Codigo.Substring(0, 17);
		}

		public string formatoBloque = "bloque{0}_superficie";

		public void ResolverBloque(ValoresFuente fuente, string numero) {
			var key = string.Format(formatoBloque, numero);
			if (fuente.ContainsKey(key))
				fuente["area_bloque"] = fuente[key];
			else return;
			// borrar las otras areas
			for (var i = 1; i <= 6; i++) {
				if (i.ToString() == numero) continue;
				key = string.Format(formatoBloque, i);
				fuente.Remove(key);
			}
		}

		public int Total(Ciudad city, dynamic options = null) {
			const string sql = "select count(*) from v_reporte where manzana is not null ";
			var str = ConfigurationManager.ConnectionStrings["intelligov"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var total = new SqlHelper(conn).UniqueResult<int>(sql, new { id = city.Id });
			conn.Close();
			return total;
		}

		public IList<ValoresFuente> Valores(Ciudad city, int limite, dynamic options = null) {
			var top = limite > 0 ? "top " + limite : "";
			var sql = "select " + top + @" * from v_reporte v
			where manzana is not null
			order by predio_ID, num_edif, pisos, catalogo, categoria, valor_item";
			var str = ConfigurationManager.ConnectionStrings["intelligov"].ConnectionString;
			var conn = new SqlConnection(str);
			conn.Open();
			var reader = new SqlHelper(conn).ExecuteReader(sql);
			var mapa = new Dictionary<string, ValoresFuente>();
			while (reader.Read()) {
				var temp = new ValoresFuente();
				Utils.FillDict(reader, temp);
				var clave = temp.GetString("predio_codigo");
				var numero = temp.GetString("num_edif");
				var key = string.Format("{0}.{1}", clave, numero);
				if (!mapa.ContainsKey(key)) {
					temp.Tipo = "construccion";
					temp.Codigo1 = clave;
					temp.Codigo2 = numero;
					temp.IdItem = temp["predio_ID"];
					temp["clave"] = clave;
					ResolverBloque(temp, numero);
					temp.Remove("valor_item");
					mapa[key] = temp;
				}
				var cat = temp.GetString("categoria");
				var valor = temp.GetString("item_id", "").Trim();
				if (string.IsNullOrEmpty(valor))
					continue;
				if (mapa[key].ContainsKey(cat))
					mapa[key][cat] += "," + valor;
				else
					mapa[key][cat] = valor;
			}
			reader.Close();
			conn.Close();
			return mapa.Values.ToList();
		}

	}
}