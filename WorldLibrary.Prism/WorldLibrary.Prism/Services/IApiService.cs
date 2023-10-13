using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorldLibrary.Prism.Models;

namespace WorldLibrary.Prism.Services
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(string urlBase, string servicePrefix, string controller);
    }
}
