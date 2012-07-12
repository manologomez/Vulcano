using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;


namespace Vulcano.Tests {
	public class BootStrapper {

		public static IWindsorContainer Container { get; set; }
		private static bool initialized;

		public static void Initialize() {
			if (initialized)
				return;
			InitComponents();
			initialized = true;
		}

		public static void InitComponents() {
			Container = new WindsorContainer();
		}

	}
}
