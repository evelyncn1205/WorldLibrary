using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Prism.Models;

namespace WorldLibrary.Prism.ViewModels
{
    public class BookDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private BookResponse _book;
        public BookDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
        }

        public BookResponse Book
        {
            get => _book;
            set => SetProperty(ref _book, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("book"))
            {
                Book = parameters.GetValue<BookResponse>("book");
                Title = Book.Title;
            }
        }
    }
}
