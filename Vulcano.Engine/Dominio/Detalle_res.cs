using System;

namespace Vulcano.Engine.Dominio {
	/// <summary>
	/// Detalle de un componente del resultado
	/// </summary>
	public class Detalle_res {
		public virtual Guid Id { get; set; }
		public virtual Guid Id_padre { get; set; }
		public virtual string Id_componente { get; set; }
		public virtual bool Es_base { get; set; }
		public virtual string Contexto { get; set; }
		public virtual string Variable { get; set; }
		public virtual string Valor { get; set; }
		public virtual decimal? Valor_numerico { get; set; }
		public virtual decimal? Calculado { get; set; }
		[PetaPoco.Ignore]
		public virtual Resultado Padre { get; set; }
	}
}