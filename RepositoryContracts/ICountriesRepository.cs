using Entities;

namespace RepositoryContracts
{
    public interface ICountriesRepository
    {
        public Task<Country> AddCountry(Country country);
        public Task<List<Country>> AddCountries(List<Country> countries);
        public Task<List<Country>> GetAllCountries();
        public Task<Country?> GetCountryByCountryID(Guid guid);
        public Task<Country?> GetCountryByCountryName(string countryName);
    }
}
