using RPA.ClaimStatements.Generator;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Web.Services;
using System;

using Unity;

namespace RPA.ClaimStatements.Web
{
    /// <summary>
    /// Unity registrations for the Generator services used by the web application.
    /// </summary>
    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        public static IUnityContainer Container => container.Value;

        public static void RegisterTypes(IUnityContainer container)
        {
            GeneratorServices.Register(container);
            container.RegisterType<ISuppressionService, SuppressionService>();
            container.RegisterType<ISampleService, SampleService>();
            container.RegisterType<ISampleXBService, SampleXBService>();
        }
    }
}