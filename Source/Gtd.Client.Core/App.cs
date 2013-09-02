using Cirrious.CrossCore.IoC;

namespace Gtd.Client.Core
{
    public class App : Cirrious.MvvmCross.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            // add a rule to register our Repository with IoC container
            CreatableTypes()
                .EndingWith("Repository")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            // default Mvx rule was here
            // "anything called Service gets registered as a LazySingleton"
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            // to start the clients with the InboxView experience
            RegisterAppStart<ViewModels.InboxViewModel>();
        }
    }
}