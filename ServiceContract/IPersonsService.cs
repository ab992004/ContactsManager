using ServiceContract.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using ServiceContract.Enums; 
namespace ServiceContract
{
    public interface IPersonsService
    {
        public Task<PersonResponse> AddPerson(PersonAddRequest? request);
        public Task<List<PersonResponse>> GetAllPersons();
        public Task<PersonResponse?> GetPersonById(Guid? personId);
        public Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString);
        public Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons,string sortBy,SortOrderEnum sortOrder);
        public Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? request);
        public Task<bool> DeletePerson(Guid? personId);
        public Task<MemoryStream> GetPersonsCSV(List<PersonResponse> persons);
        public Task<MemoryStream> GetPersonsExcel(List<PersonResponse> persons);
        
    }
}
