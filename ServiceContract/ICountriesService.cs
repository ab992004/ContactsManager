using Microsoft.AspNetCore.Http;
using ServiceContract.DTO;

namespace ServiceContract
{
    public interface ICountriesService
    {
        public Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list 
        /// </summary> 
        /// <returns>all countries from the list as List of CountryResponse</CountryResponse>></returns>
        public Task<List<CountryResponse>> GetAllCountries();
        public Task<CountryResponse?> GetCountryById(Guid? countryId);
        public Task<int> UploadCountriesFromExcelFile(IFormFile? formFile);
    }
}
