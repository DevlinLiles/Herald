using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using ExampleMVC.Services;

namespace ExampleMVC
{
    public class MembershipInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IFormsAuthenticationService>().ImplementedBy<FormsAuthenticationService>(),
                               Component.For<IAccountRoleService>().ImplementedBy<AccountRoleService>(),
                               Component.For<IMembershipService>().ImplementedBy<AccountMembershipService>());
        }
    }
}
