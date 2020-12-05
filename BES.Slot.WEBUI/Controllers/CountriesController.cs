using BES.Slot.WEBUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BES.Slot.WEBUI.Controllers
{
    public class CountriesController : Controller
    {
        private readonly string CountriesURL = "https://restcountries.eu/rest/v2/all";
        private IMemoryCache _cache;
        public CountriesController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public async Task<IActionResult> IndexAsync(int curPage = 0)
        {
            string jsonCountries = await GetJsonCountriesFromCacheAsync();

            List<CountryWithLanguage> countries = JsonConvert.DeserializeObject<List<CountryWithLanguage>>(jsonCountries);

            int totalPages = (int)Math.Ceiling((double)countries.Count / 8);
            ViewBag.TotalPages = totalPages;

            var currentCountries = countries.Skip(curPage * 8).Take(8).ToList();

            ViewBag.CurrentPage = curPage;

            return View(currentCountries);
        }

        private async Task<string> GetJsonCountriesFromCacheAsync()
        {
            var cacheCountries = await _cache.GetOrCreateAsync("Countries", async entry =>
             {
                 entry.SetAbsoluteExpiration(TimeSpan.FromDays(30));
                 return await GetCountriesAsJsonAsync();
             });


            return cacheCountries;
        }


        private async Task<string> GetCountriesAsJsonAsync()
        {
            using var httpClient = new HttpClient();
            var responseMessage = await httpClient.GetAsync(CountriesURL);
            return await responseMessage.Content.ReadAsStringAsync();
        }
    }
}
