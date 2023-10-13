using Example;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WorldLibrary.Prism.Models;
using WorldLibrary.Prism.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorldLibrary.Prism.ViewModels
{
    public class BooksPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private ObservableCollection<BookResponse> _books;
        private bool _isRunning;
        private string _search;
        private List<BookResponse> _myBooks;
        private DelegateCommand _searchCommand;
        public BooksPageViewModel(INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Book Page";
            LoadBooksAsync();
        }

        public ObservableCollection<BookResponse> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }
        public DelegateCommand SearchCommand => _searchCommand ?? (_searchCommand = new DelegateCommand(ShowBooks));

        public string Search
        {
            get => _search;
            set
            {
                SetProperty(ref _search, value);
                ShowBooks();
            }
        }
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }


        //public ObservableCollection<BookItemViewModel> Books
        //{
        //    get => _books;
        //    set => SetProperty(ref _books, value);
        //}
        private async void LoadBooksAsync()
        {

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert(
                        "Error",
                        "Check internet connection", "Accept");
                });
                return;
            }
            IsRunning = true;

            string url = App.Current.Resources["UrlAPI"].ToString();

            Response response = await _apiService.GetListAsync<BookResponse>(url, "/api", "/Books");

            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            _myBooks = (List<BookResponse>)response.Result;
            //Books = new ObservableCollection<BookResponse>(myBooks);
            ShowBooks();
        }
        private void ShowBooks()
        {
            if (string.IsNullOrEmpty(Search))
            {
                Books = new ObservableCollection<BookResponse>(_myBooks);
            }
            else
            {
                Books = new ObservableCollection<BookResponse>(
                    _myBooks.Where(p => p.Title.ToLower().Contains(Search.ToLower())));
            }
        }

        //private void ShowBooks()
        //{
        //    if (string.IsNullOrEmpty(Search))
        //    {
        //        Books = new ObservableCollection<BookItemViewModel>(_myBooks.Select(p =>
        //        new BookItemViewModel(_navigationService)
        //        {

        //        }));
        //    }
        //    else
        //    {
        //        Books = new ObservableCollection<BookItemViewModel>(
        //            _myBooks.Where(p => p.Name.ToLower().Contains(Search.ToLower())));
        //    }
        //}
    }
}


