using ServiceContract;
using ServiceContract.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;
        public CountriesService(PersonsDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest countryAddRequest)
        {
            if(countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if(string.IsNullOrEmpty(countryAddRequest.CountryName))
            {
                throw new ArgumentException("Country name cannot be null or empty.", nameof(countryAddRequest.CountryName));
            }

            if (await _db.Countries.AnyAsync(c => c.CountryName == countryAddRequest.CountryName))
            {
                throw new ArgumentException($"Country with name '{countryAddRequest.CountryName}' already exists.");
            }

            Country country = countryAddRequest.ToCountry();
            
            country.CountryID = Guid.NewGuid();

            _db.Countries.Add(country);

            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? countryId)
        {
            if(countryId is null)
            {
                return null;
            }
            var country =await _db.Countries.FirstOrDefaultAsync(c => c.CountryID == countryId);
            return country?.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            using var memoryStream= new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            using ExcelPackage excelPackage = new ExcelPackage(memoryStream);
            ExcelWorksheet sheet = excelPackage.Workbook.Worksheets["Countries"];

            if (sheet?.Dimension == null)
                return 0;

            int rowCount = sheet.Dimension.Rows;

            var countries = (await _db.Countries.Select(c => c.CountryName).ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);
            int countriesInserted = 0;

            for(int row = 2;row <= rowCount;row++)
            {
                string? cellValue = Convert.ToString(sheet.Cells[row, 1].Value)?.Trim();
                if (string.IsNullOrEmpty(cellValue)) 
                    continue;

                if (!countries.Add(cellValue)) 
                    continue;

                _db.Countries.Add(new Country { CountryID = Guid.NewGuid(), CountryName = cellValue });
                countriesInserted++;
            }

            await _db.SaveChangesAsync();

            return countriesInserted;
        }
    }
}
