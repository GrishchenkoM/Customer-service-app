using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Web.Models;

namespace Web
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        public NinjectControllerFactory()
        {
            _ninjectKernel = new StandardKernel();
            AddBinding();
        }

        private void AddBinding()
        {
            _ninjectKernel.Bind<ApplicationDbContext>().ToSelf()
                .WithConstructorArgument("DefaultConnection",
                    ConfigurationManager.ConnectionStrings[0].ConnectionString);
        }

        protected override IController GetControllerInstance(
            RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
                ? null
                : (IController)_ninjectKernel.Get(controllerType);
        }

        private readonly IKernel _ninjectKernel;
    }
}