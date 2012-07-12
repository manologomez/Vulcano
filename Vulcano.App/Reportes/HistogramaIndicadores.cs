using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using Vulcano.Engine;
using Vulcano.Engine.Dominio;
using SqlDataClasses;

namespace Vulcano.App.Reportes {
	public class HistogramaIndicadores {
		public FichaIndicadores Ficha { get; set; }
		public Ciudad City { get; set; }
		protected SqlHelper helper;
		public float Tolerancia { get; set; }

		public HistogramaIndicadores() {
			Tolerancia = 1;
		}

		public void Construir(string plantilla, string destino) {
			using (var pck = new ExcelPackage(new FileInfo(plantilla))) {
				var wb = pck.Workbook;
				var indicadores = Ficha.Temas;
				var con = NHibernateHelper.GetCurrentSession().Connection;

				helper = new SqlHelper(con);
				var i = 0;
				foreach (var indicador in indicadores) {
					i++;
					LLenarHoja(i, wb, indicador);
				}

				pck.SaveAs(new FileInfo(destino));
				AppNotifier.Print("Exportado histograma");

			}
		}

		protected void LLenarHoja(int i, ExcelWorkbook wb, string indicador) {
			var tname = "Indicador" + i;
			var sheet = wb.Worksheets.FirstOrDefault(x => x.Name == tname);
			if (sheet == null)
				return;
			//sheet.Name = indicador;
			sheet.Name = indicador;
			sheet.Cells["B1"].Value = indicador;
			sheet.Cells["D1"].Value = Tolerancia;
			LlenarTabla(i, 3, sheet, "construccion");
			LlenarTabla(i, 19, sheet, "manzana");
			LlenarTabla(i, 35, sheet, "predio");
			foreach (var drawing in sheet.Drawings) {
				var chart = drawing as ExcelChart;
				if (chart == null) continue;
				var ser = chart.Series[0];
				var serie = chart.Series.Add(
					ser.Series.Replace(tname, indicador),
					ser.XSeries.Replace(tname, indicador)
				);
				var add = ser.HeaderAddress.Address;
				serie.HeaderAddress = new ExcelAddressBase(add.Replace(tname, indicador));
				chart.Series.Delete(0);
			}

		}

		protected void LlenarTabla(int i, int offset, ExcelWorksheet sheet, string tipo) {
			var sql = SqlConsulta("indicador" + i);
			var dt = helper.ExecuteDataTable(sql, new { ciudad = City.Nombre, tipo_item = tipo });
			for (int j = 0; j < 10; j++) {
				var v = dt.Rows[0][j];
				var real = j + offset;
				sheet.Cells["B" + real].Value = v;
			}
			sheet.Cells["B" + (offset + 10)].Value = dt.Rows[0]["total"];

		}


		public string SqlConsulta(string ind) {
			const string tpl = @"select 
			count(case when :indicador: >=0 and :indicador: <10 then 1 end) as p1,
			count(case when :indicador: >=10 and :indicador: <20 then 1 end) as p2,
			count(case when :indicador: >=20 and :indicador: <30 then 1 end) as p3,
			count(case when :indicador: >=30 and :indicador: <40 then 1 end) as p4,
			count(case when :indicador: >=40 and :indicador: <50 then 1 end) as p5,
			count(case when :indicador: >=50 and :indicador: <60 then 1 end) as p6,
			count(case when :indicador: >=60 and :indicador: <70 then 1 end) as p7,
			count(case when :indicador: >=70 and :indicador: <80 then 1 end) as p8,
			count(case when :indicador: >=80 and :indicador: <90 then 1 end) as p9,
			count(case when :indicador: >=90 and :indicador: <=100 then 1 end) as p10,
			count(:indicador:) as total
			from resultado
			where canton = @ciudad and tipo_item = @tipo_item :tolerancia:";
			var sql = tpl.Replace(":indicador:", ind);
			var rep = "";
			if (Tolerancia > 0) {
				rep = " and completo >= " + Tolerancia.ToString().Replace(",", ".");
			}
			return sql.Replace(":tolerancia:", rep);
		}
	}
}
