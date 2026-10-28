using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using Services;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsRepository _personsRepository;
        public PersonsService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {

            if (request is null)
                throw new ArgumentNullException(nameof(request));
           ValidationHelper.ModelValidation(request);
            Person person = request.ToPerson();
            person.PersonID = Guid.NewGuid();

            return (await _personsRepository.AddPerson(person)).ToPersonResponse();

        }
        
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _personsRepository.GetAllPersons();
            return persons.ToList().Select(person => person.ToPersonResponse()).ToList();
            //return _db.sp_GetAllPersons().Select(person => ConvertPersonToPersonResponse(person)).ToList();
        }

        public async Task<PersonResponse?> GetPersonById(Guid? personId)
        {
            if(personId is null || personId == Guid.Empty)
            {
                return null;
            }
            Person? person = await _personsRepository.GetPersonByPersonID(personId.Value);
            return person is null ? null : person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString)
        {
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return await GetAllPersons();
            }
            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) => await _personsRepository.GetFilteredPersons(
           p => p.PersonName != null && p.PersonName.Contains(searchString)),

                nameof(PersonResponse.Email) => await _personsRepository.GetFilteredPersons(
                    p => p.Email != null && p.Email.Contains(searchString)),

                nameof(PersonResponse.Gender) => await _personsRepository.GetFilteredPersons(
                    p => p.Gender != null && p.Gender == searchString),

                nameof(PersonResponse.CountryID) => await _personsRepository.GetFilteredPersons(
                    p => p.Country != null && p.Country.CountryName.Contains(searchString)),

                nameof(PersonResponse.Address) => await _personsRepository.GetFilteredPersons(
                    p => p.Address != null && p.Address.Contains(searchString)),
                nameof(PersonResponse.DateOfBirth) => (await _personsRepository.GetAllPersons())
                    .Where(p => p.DateOfBirth != null &&
                    p.DateOfBirth.Value.ToString("dd MMMM yyyy")
                        .Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        .ToList(),

                _ => await _personsRepository.GetAllPersons()
            };
            return persons.Select(p => p.ToPersonResponse()).ToList();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderEnum sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderEnum.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderEnum.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderEnum.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderEnum.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderEnum.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderEnum.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderEnum.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderEnum.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderEnum.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderEnum.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderEnum.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderEnum.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderEnum.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderEnum.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsletters), SortOrderEnum.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsletters).ToList(),

                (nameof(PersonResponse.ReceiveNewsletters), SortOrderEnum.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsletters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }
        public async Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? request)
        {
            if(request is null)
                throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidation(request);
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(request.PersonID);
            if(matchingPerson is null)
                throw new ArgumentException($"Person with ID {request.PersonID} does not exist.");
            matchingPerson.PersonName = request.PersonName;
            matchingPerson.Email = request.Email;
            matchingPerson.DateOfBirth = request.DateOfBirth;
            matchingPerson.Gender = request.Gender?.ToString();
            matchingPerson.CountryID = request.CountryID;
            matchingPerson.Address = request.Address;
            matchingPerson.ReceiveNewsletters = request.ReceiveNewsletters;

            return (await _personsRepository.UpdatePerson(matchingPerson)).ToPersonResponse();

        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            if(personId is null)
                throw new ArgumentNullException(nameof(personId));
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personId.Value);
            if (matchingPerson is null)
                return false;
            return await _personsRepository.DeletePersonByID(matchingPerson.PersonID);

        }

        public async Task<MemoryStream> GetPersonsCSV(List<PersonResponse> persons)
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter= new StreamWriter(memoryStream);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter,csvConfiguration,leaveOpen:true);
            //csvWriter.WriteHeader<PersonResponse>();
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsletters));
            csvWriter.NextRecord();
            //await csvWriter.WriteRecordsAsync(persons);
            foreach(var person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                csvWriter.WriteField(person.DateOfBirth);
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsletters);
                csvWriter.NextRecord();
            }

            await csvWriter.FlushAsync();
            await streamWriter.FlushAsync();

            memoryStream.Position = 0;
            return memoryStream;

        }

        public async Task<MemoryStream> GetPersonsExcel(List<PersonResponse> persons)
        {
            MemoryStream memoryStream = new MemoryStream();
            using(ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet sheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                sheet.Cells["A1"].Value = "Person Name";
                sheet.Cells["B1"].Value = "Email";
                sheet.Cells["C1"].Value = "Date Of Birth";
                sheet.Cells["D1"].Value = "Age";
                sheet.Cells["E1"].Value = "Gender";
                sheet.Cells["F1"].Value = "Address";
                sheet.Cells["G1"].Value = "Country";
                sheet.Cells["H1"].Value = "Recieve News Letters";
                using(ExcelRange headerCells = sheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }
                int row = 2;
                foreach(var person in persons)
                {
                    sheet.Cells[$"A{row}"].Value = person.PersonName;
                    sheet.Cells[$"B{row}"].Value = person.Email;
                    sheet.Cells[$"C{row}"].Value = person.DateOfBirth;
                    sheet.Cells[$"D{row}"].Value = person.Age;
                    sheet.Cells[$"E{row}"].Value = person.Gender;
                    sheet.Cells[$"F{row}"].Value = person.Address;
                    sheet.Cells[$"G{row}"].Value = person.Country;
                    sheet.Cells[$"H{row}"].Value = person.ReceiveNewsletters;
                    row++;
                }
                sheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
