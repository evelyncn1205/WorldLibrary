using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Syncfusion.Licensing;
using WorldLibrary.Prism.Services;
using WorldLibrary.Prism.ViewModels;
using WorldLibrary.Prism.Views;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace WorldLibrary.Prism
{
    public partial class App 
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            SyncfusionLicenseProvider.RegisterLicense("Mjc1ODUzNkAzMjMzMmUzMDJlMzBMeEJ1Vk5qNHBkeUNsdk1NVk1PdkxqaE1wRzUwb1cvWWN1V3NXM0hpVEQwPQ==");

            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/BooksPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.RegisterForNavigation<NavigationPage>();            
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<BooksPage, BooksPageViewModel>();
            containerRegistry.RegisterForNavigation<BookDetailPage, BookDetailPageViewModel>();
        }
    }
}
