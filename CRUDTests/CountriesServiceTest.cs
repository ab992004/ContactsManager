using Xunit;
using ServiceContract;
using Services;
using ServiceContract.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
namespace CRUDTests
{

    public class CountriesServiceTest
    {
        readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }
        #region Add Country
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            CountryAddRequest? request = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }


        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
                await _countriesService.AddCountry(request1);
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request2);
            });
        }
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
            CountryResponse response =await  _countriesService.AddCountry(request);
            var countries_from_getAllCountries = await _countriesService.GetAllCountries();
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_getAllCountries);
        }
        #endregion
        #region Get All Countries
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            var actual_country_response_list =await  _countriesService.GetAllCountries();
            Assert.Empty(actual_country_response_list);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest(){CountryName = "USA"},
                new CountryAddRequest(){CountryName = "UK"}
            };
            var countries_list_from_add_country = new List<CountryResponse>();
            foreach (var country_request in country_request_list)
            {
                countries_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
            }
            var actualCountryResponseList =await _countriesService.GetAllCountries();
            foreach (var expected_country in countries_list_from_add_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }
        #endregion
        #region Get Country By ID
        [Fact]
        public async Task GetCountryByID_NullCountryID()
        {
            Guid? countryId = null;
            var actual_country_response =await _countriesService.GetCountryById(countryId);
            Assert.Null(actual_country_response);
        }
        [Fact]
        public async Task GetCountryByID_ValidCountryID()
        {
            // Add a country first
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
            CountryResponse addedCountry =await  _countriesService.AddCountry(request);

            // Try to get the country by its ID
            var actual_country_response =await _countriesService.GetCountryById(addedCountry.CountryID);
            Assert.Equal(addedCountry, actual_country_response);
        }

        #endregion
    }
}
