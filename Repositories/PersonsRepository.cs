using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByID(Guid guid)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(p => p.PersonID == guid));
            int rowDeleted = await _db.SaveChangesAsync();
            return rowDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include(p => p.Country).ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include(p => p.Country).Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonID(Guid guid)
        {
            return await _db.Persons.Include(p => p.Country).FirstOrDefaultAsync(p => p.PersonID == guid);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            await _db.SaveChangesAsync();
            return person;
        }
    }
}
