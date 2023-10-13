using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Prism.Helpers;

namespace WorldLibrary.Prism.ViewModels
{
    public class ModifyUserPageViewModel : ViewModelBase
    {
        public ModifyUserPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Languages.ModifyUser;
        }
    }
}
