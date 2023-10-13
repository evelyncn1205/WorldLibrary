using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Prism.Helpers;

namespace WorldLibrary.Prism.ViewModels
{
    public class ReservesPageViewModel : ViewModelBase
    {
        public ReservesPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Languages.Reserves;
        }
    }
}
