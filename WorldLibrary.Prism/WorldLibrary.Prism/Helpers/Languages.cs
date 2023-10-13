using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WorldLibrary.Prism.Interfaces;
using WorldLibrary.Prism.Resources;
using Xamarin.Forms;

namespace WorldLibrary.Prism.Helpers
{
    public static class Languages
    {
        static Languages()
        {
            CultureInfo ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Resource.Culture = ci;
            Culture = ci.Name;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }

        public static string Culture { get; set; }
        public static string Accept => Resource.Accept;
        public static string ConnectionError => Resource.ConnectionError;
        public static string Error => Resource.Error;
        public static string Title => Resource.Title;
        public static string Book => Resource.Book;
        public static string Books => Resource.Books;
        public static string Year => Resource.Year;
        public static string Quantity => Resource.Quantity;
        public static string Author => Resource.Author;
        public static string Category => Resource.Category;
        public static string Assessment => Resource.Assessment;
        public static string Loading => Resource.Loading;
        public static string SearchBook => Resource.SearchBook;
        public static string Synopsis => Resource.Synopsis;
        public static string Reserve => Resource.Reserve;
        public static string ImageFullPath => Resource.ImageFullPath;
        public static string Reserves => Resource.Reserves;
        public static string ModifyUser => Resource.ModifyUser;
        public static string Login => Resource.Login;
       


    }
}
