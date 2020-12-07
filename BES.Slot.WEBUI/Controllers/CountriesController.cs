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
        private readonly string CountriesURL = "https://restcountries.eu/rest/v2/all?fields=name;nativeName;flag;languages";
        private readonly string CountriesSearchingURL = "https://restcountries.eu/rest/v2/name/{name}?fields=name;nativeName;flag;languages";
        private IMemoryCache _cache;
        public CountriesController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public async Task<IActionResult> IndexAsync(int curPage = 0, string search = "")
        {
            List<CountryWithLanguage> currentCountries;
            string jsonCountries;
            List<CountryWithLanguage> countries= new List<CountryWithLanguage>();

            if (string.IsNullOrEmpty(search))
            {
                jsonCountries = await GetJsonCountriesFromCacheAsync();
            }
            else
            {
                jsonCountries = await GetJsonSCountriesFromCacheAsync(search);
            }            
            if (!string.IsNullOrEmpty(jsonCountries))
            {
                countries = JsonConvert.DeserializeObject<List<CountryWithLanguage>>(jsonCountries);
            }


            int totalPages = (int)Math.Ceiling((double)countries.Count / 8);
            ViewBag.TotalPages = totalPages;

            currentCountries = countries.Skip(curPage * 8).Take(8).ToList();

            ViewBag.CurrentPage = curPage;
            ViewBag.SearchingText = search;

            return View(currentCountries);
        }

        private async Task<string> GetJsonSCountriesFromCacheAsync(string search)
        {
            var cacheSCountries = await _cache.GetOrCreateAsync(search, async entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                return await GetSearchingCountriesAsJsonAsync(search);
            });

            return cacheSCountries;
        }

        private async Task<string> GetSearchingCountriesAsJsonAsync(string search)
        {
            using var httpClient = new HttpClient();
            var responseMessage = await httpClient.GetAsync(CountriesSearchingURL.Replace("{name}", search.ToUpper()));
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadAsStringAsync();
            }
            else
                return "";

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
