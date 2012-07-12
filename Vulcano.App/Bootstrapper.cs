using System;
using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Vulcano.Engine;
using Vulcano.Engine.Interfaces;

namespace Vulcano.App {
	public class Bootstrapper {

		public static IWindsorContainer Container { get; private set; }

		public static void Init() {
			var asm = Assembly.GetExecutingAssembly();
			Container = new WindsorContainer();
			Container.Kernel.Resolver.AddSubResolver(new ArrayResolver(Container.Kernel));
			Container.Kernel.Resolver.AddSubResolver(new ListResolver(Container.Kernel));
			Container.Register(
				AllTypes.FromThisAssembly()
					.BasedOn<IFuenteDatos>().WithService.FirstInterface()
					.LifestyleTransient()
				);
			Container.Register(
				AllTypes.FromThisAssembly()
					.BasedOn<IProcesoEspecial>().WithService.FirstInterface()
					.LifestyleTransient()
				);
			Container.Register(Component.For<FuentesFactory>().ImplementedBy<FuentesFactory>());
			Container.Register(Component.For<ProcesosFactory>().ImplementedBy<ProcesosFactory>());

			var path = Path.Combine(Environment.CurrentDirectory, "extensiones");
			if (Directory.Exists(path)) {
				Container.Install(FromAssembly.InDirectory(new AssemblyFilter(path)));
			}

			//var lista = Container.ResolveAll<IFuenteDatos>();
			//Console.WriteLine("sdf");
			var sef = NHibernateHelper.SessionFactory;
		}

		public static void Dispose() {
			if (Container != null)
				Container.Dispose();
		}
	}
}