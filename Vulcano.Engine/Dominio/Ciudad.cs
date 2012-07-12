namespace Vulcano.Engine.Dominio{
	/// <summary>
	/// Entidad Principal
	/// </summary>
	public class Ciudad {
		public virtual int Id { get; set; }
		public virtual string Codigo { get; set; }
		public virtual string Nombre { get; set; }
		public virtual string Contacto { get; set; }
		[PetaPoco.Column("script_load")]
		public virtual string ScriptLoad { get; set; }
		public virtual string Observaciones { get; set; }
	}
}