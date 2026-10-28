using Xunit;
using ServiceContract;
using Services;
using ServiceContract.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using System.Diagnostics.Metrics;
namespace CRUDTests
{

    public class CountriesServiceTest
    {
        #region private fields
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;
        #endregion
        public CountriesServiceTest()
        {
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository);
            _fixture = new Fixture();
        }


        #region Add Country
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            CountryAddRequest? request = null;
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    await _countriesService.AddCountry(request);
            //});
            var action = async () => await _countriesService.AddCountry(request);
            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    await _countriesService.AddCountry(request);
            //});
            var action = async () => await _countriesService.AddCountry(request);
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            CountryAddRequest? request1 = _fixture.Create<CountryAddRequest>();
            Country country = request1.ToCountry();
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(country);

            var action = async () => await _countriesService.AddCountry(request1);
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task AddCountry_ProperCountryDetails_ToBeSuccessful()
        {
            CountryAddRequest? request = _fixture.Create<CountryAddRequest>();
            Country country = request.ToCountry();
            CountryResponse expected_country = country.ToCountryResponse();


            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync((Country c) => c);

            CountryResponse actual_country = await _countriesService.AddCountry(request);
            expected_country.CountryID = actual_country.CountryID;
            //Assert.True(response.CountryID != Guid.Empty);
            actual_country.Should().BeEquivalentTo(expected_country);
            //Assert.Contains(response, countries_from_getAllCountries);

        }
        #endregion
        #region Get All Countries
        [Fact]
        public async Task GetAllCountries_EmptyList_ToBeEmpty()
        {
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(new List<Country>());
            

            var actual_country_response_list =await  _countriesService.GetAllCountries();
            //Assert.Empty(actual_country_response_list);
            actual_country_response_list.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries_ToBeSuccessful()
        {
            List<Country> countries = new List<Country>()
            {
                _fixture.Build<Country>().Without(c => c.Persons).Create(),
                _fixture.Build<Country>().Without(c => c.Persons).Create()
            };
            List<CountryResponse> expected_countries = countries.Select(c => c.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            List<CountryResponse> actual_countries =await _countriesService.GetAllCountries();
            //foreach (var expected_country in countries_list_from_add_country)
            //{
            //    Assert.Contains(expected_country, actualCountryResponseList);
            //}
            actual_countries.Should().BeEquivalentTo(expected_countries);
        }
        #endregion
        #region Get Country By ID
        [Fact]
        public async Task GetCountryByID_NullCountryID_ToBeNull()
        {
            Guid? countryId = null;
            var actual_country_response =await _countriesService.GetCountryById(countryId);
            //Assert.Null(actual_country_response);
            actual_country_response.Should().BeNull();
        }
        [Fact]
        public async Task GetCountryByID_ValidCountryID_ToBeSuccessful()
        {
            // Add a country first
            Country? country = _fixture.Build<Country>().Without(c => c.Persons).Create();
            CountryResponse expected_country = country.ToCountryResponse();
            
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
            .ReturnsAsync(country);
            // Try to get the country by its ID
            CountryResponse? actual_country =await _countriesService.GetCountryById(country.CountryID);
            //Assert.Equal(addedCountry, actual_country_response);
            actual_country.Should().Be(expected_country);
        }

        #endregion
    }
}
