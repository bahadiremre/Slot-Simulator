using BES.Slot.WEBUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BES.Slot.WEBUI.Controllers
{
    public class MyCountryController : Controller
    {
        private readonly ILogger<MyCountryController> _logger;
        private readonly string CountryByIpURL= "http://demo.ip-api.com/json/?fields=66842623&lang=en";
        private readonly string CountryLangURL = "https://restcountries.eu/rest/v2/name/{name}?fullText=true";
        public MyCountryController(ILogger<MyCountryController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            using var httpClient = new HttpClient();
            var responseMessage = await httpClient.GetAsync(CountryByIpURL);

            var jsonCountry = await responseMessage.Content.ReadAsStringAsync();

            var countryInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonCountry);

            responseMessage = await httpClient.GetAsync(GetCountryLanguageURL(countryInfo["country"]));

            var jsonLanguage = await responseMessage.Content.ReadAsStringAsync();

            var languages = JsonConvert.DeserializeObject<List<CountryWithLanguage>>(jsonLanguage);

            return View(languages?[0]);
        }

        private string GetCountryLanguageURL(string countryName)
        {
            return CountryLangURL.Replace("{name}", countryName);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
