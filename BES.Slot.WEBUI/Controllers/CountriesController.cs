using BES.Slot.WEBUI.Models;
using Microsoft.AspNetCore.Mvc;
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
        private readonly string CountriesURL="https://restcountries.eu/rest/v2/all";
        public async Task<IActionResult> IndexAsync()
        {
            using var httpClient = new HttpClient();
            var responseMessage = await httpClient.GetAsync(CountriesURL);
            var jsonCountries = await responseMessage.Content.ReadAsStringAsync();

            var countries = JsonConvert.DeserializeObject<List<CountryWithLanguage>>(jsonCountries);

            return View(countries);
        }
    }
}
