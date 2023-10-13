using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using WorldLibrary.Prism.Models;
using WorldLibrary.Prism.Views;

namespace WorldLibrary.Prism.ItemViewModels
{

    public class BookItemViewModel : BookResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectBookCommand;

        public BookItemViewModel(INavigationService navigationService)
        {
            _navigationService=navigationService;
            
        }

        public DelegateCommand SelectBookCommand =>
            _selectBookCommand??
            (_selectBookCommand = new DelegateCommand(SelectBookAsync));

        private async void SelectBookAsync()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                {"book",this}
            };
            await _navigationService.NavigateAsync(nameof(BookDetailPage), parameters);
        }
    }
}
