using ServiceContract;
using ServiceContract.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;
        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if(countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if(string.IsNullOrEmpty(countryAddRequest.CountryName))
            {
                throw new ArgumentException("Country name cannot be null or empty.", nameof(countryAddRequest.CountryName));
            }

            if ((await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName)) != null)
            {
                throw new ArgumentException($"Country with name '{countryAddRequest.CountryName}' already exists.");
            }

            Country country = countryAddRequest.ToCountry();
            
            country.CountryID = Guid.NewGuid();

            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(c => c.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? countryId)
        {
            if(countryId is null)
            {
                return null;
            }
            var country = await _countriesRepository.GetCountryByCountryID(countryId.Value);
            return country?.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile? formFile)
        {
            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            using ExcelPackage excelPackage = new ExcelPackage(memoryStream);
            ExcelWorksheet sheet = excelPackage.Workbook.Worksheets["Countries"];

            if (sheet?.Dimension == null)
                return 0;

            int rowCount = sheet.Dimension.Rows;

            var countries = (await _countriesRepository.GetAllCountries())
                .Select(c => c.CountryName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            List<Country> countriesAdded = new List<Country>();
            for (int row = 2; row <= rowCount; row++)
            {
                string? cellValue = Convert.ToString(sheet.Cells[row, 1].Value)?.Trim();
                if (string.IsNullOrEmpty(cellValue))
                    continue;

                if (!countries.Add(cellValue))
                    continue;

                countriesAdded.Add(new Country { CountryID = Guid.NewGuid(), CountryName = cellValue });
            }
            if (countriesAdded.Count > 0)
                await _countriesRepository.AddCountries(countriesAdded);
            return countriesAdded.Count;
        }
    }
}
