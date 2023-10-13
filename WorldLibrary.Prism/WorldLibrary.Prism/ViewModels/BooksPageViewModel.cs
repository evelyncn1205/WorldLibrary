using Example;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WorldLibrary.Prism.Helpers;
using WorldLibrary.Prism.ItemViewModels;
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
        private ObservableCollection<BookItemViewModel> _books;
        private bool _isRunning;
        private string _search;
        private List<BookResponse> _myBooks;
        private DelegateCommand _searchCommand;
        public BooksPageViewModel(INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = Languages.Books;
            LoadBooksAsync();
        }

        public ObservableCollection<BookItemViewModel> Books
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

       
        private async void LoadBooksAsync()
        {

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert(
                       Languages.Error,
                       Languages.ConnectionError, Languages.Accept);
                });
                return;
            }
            IsRunning = true;

            string url = App.Current.Resources["UrlAPI"].ToString();

            Response response = await _apiService.GetListAsync<BookResponse>(url, "/api", "/Books");

            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            _myBooks = (List<BookResponse>)response.Result;
            ShowBooks();
        }


        private void ShowBooks()
        {
            if (string.IsNullOrEmpty(Search))
            {
                Books = new ObservableCollection<BookItemViewModel>(_myBooks.Select(p =>
                new BookItemViewModel(_navigationService)
                {
                    Id = p.Id,
                    Title = p.Title,    
                    Author = p.Author,
                    Category = p.Category,
                    Assessment = p.Assessment,
                    ImageId = p.ImageId,
                    Synopsis=p.Synopsis,
                    Year=p.Year,
                    Quantity=p.Quantity,
                    ImageFullPath=p.ImageFullPath,
                    User=p.User,
                }).ToList());
            }
            else
            {
                Books = new ObservableCollection<BookItemViewModel>(_myBooks.Select(p =>
                new BookItemViewModel(_navigationService)
                {
                    Id = p.Id,
                    Title = p.Title,
                    Author = p.Author,
                    Category = p.Category,
                    Assessment = p.Assessment,
                    ImageId = p.ImageId,
                    Synopsis=p.Synopsis,
                    Year=p.Year,
                    Quantity=p.Quantity,
                    ImageFullPath=p.ImageFullPath,
                    User=p.User,
                })
                .Where(p => p.Title.ToLower().Contains(Search.ToLower()))
                .ToList());
            }
        }
    }
}


