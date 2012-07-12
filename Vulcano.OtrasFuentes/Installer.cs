using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Vulcano.Engine.Interfaces;

namespace Vulcano.OtrasFuentes {
	public class Installer : IWindsorInstaller {
		public void Install(IWindsorContainer container, IConfigurationStore store) {
			container.Register(AllTypes.FromThisAssembly()
				.BasedOn<IFuenteDatos>().WithServiceBase().LifestyleTransient()
			);
		}
	}
}
