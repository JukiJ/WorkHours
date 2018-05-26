using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism;
using Xamarin.Forms;
using Prism.Unity;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Prism.Navigation;
using Prism.Common;
using WorkHours.Views;
using WorkHours.Services;

namespace WorkHours
{
	public partial class App : PrismApplication
	{
        public App(IPlatformInitializer initializer = null) : base(initializer) { }
        protected override void OnInitialized()
		{
			InitializeComponent();
            NavigationService.NavigateAsync("LoginPage");
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterForNavigation<NavigationPage>();

            containerRegistry.RegisterForNavigation<LoginPage>("LoginPage");
            containerRegistry.RegisterForNavigation<MainPage>("MainPage");
            containerRegistry.RegisterForNavigation<DaysPage>("DaysPage");
            containerRegistry.RegisterForNavigation<SingleDayPage>("SingleDay");
            containerRegistry.RegisterForNavigation<SearchPage>("SearchPage");
            containerRegistry.RegisterForNavigation<EditCIOPopupPage>("EditPopup");
            containerRegistry.RegisterForNavigation<NewPersonPage>("NewPersonPage");
            containerRegistry.RegisterForNavigation<AdminPage>("AdminPage");

            containerRegistry.Register<IPersonService, PersonService>();
            containerRegistry.Register<ICredentialsService, CredentialsService>();
            containerRegistry.Register<ICheckInOutService, CheckInOutService>();
            containerRegistry.Register<ICountHours,CountHours>();
        }		
	}
}
