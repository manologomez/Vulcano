using System;
using System.Collections.Generic;

namespace Vulcano.Engine.Dominio {
	public class Resultado {
		public virtual Guid Id { get; set; }
		public virtual int Id_ciudad { get; set; }
		public virtual string Canton { get; set; }
		public virtual string Tipo_item { get; set; }
		public virtual int? Id_item { get; set; }
		public virtual string Proceso { get; set; }
		public virtual string Nombre { get; set; }
		public virtual string Codigo { get; set; }
		public virtual string Codigo2 { get; set; }
		public virtual string Codigo3 { get; set; }
		public virtual string Indice { get; set; }
		public virtual string Informacion { get; set; }
		public virtual float? Completo { get; set; }
		public virtual int Numevaluados { get; set; }
		public virtual int Numcomponentes { get; set; }
		public virtual float? Indicador1 { get; set; }
		public virtual float? Indicador2 { get; set; }
		public virtual float? Indicador3 { get; set; }
		public virtual float? Indicador4 { get; set; }
		public virtual float? Indicador5 { get; set; }
		public virtual float? Indicador6 { get; set; }
		public virtual DateTime? Fecha { get; set; }
		public virtual string Serializado { get; set; }
		public virtual float? Area { get; set; }

		[PetaPoco.Ignore]
		public virtual IList<Detalle_res> Detalles { get; set; }

		public Resultado() {
			Detalles = new List<Detalle_res>();
		}

		public virtual void AddDetalle(Detalle_res det) {
			Detalles.Add(det);
		}
	}

}
