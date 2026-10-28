using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RepositoryContracts
{
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a new person to the data store asynchronously.
        /// </summary>
        /// <param name="person">The person to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added person.</returns>
        public Task<Person> AddPerson(Person person);
        /// <summary>
        /// Retrieves all persons asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of persons.</returns>
        public Task<List<Person>> GetAllPersons();
        /// <summary>
        /// Retrieves a person by their unique identifier.
        /// </summary>
        /// <param name="guid">The unique identifier of the person.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the person associated with the
        /// specified identifier.</returns>
        public Task<Person?> GetPersonByPersonID(Guid guid);
        /// <summary>
        /// Retrieves a list of persons matching the specified filter expression.
        /// </summary>
        /// <param name="predicate">An expression used to filter the persons.</param>
        /// <returns>A task representing the asynchronous operation, containing the filtered list of persons.</returns>
        public Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);
        /// <summary>
        /// Deletes a person identified by the specified unique identifier.
        /// </summary>
        /// <param name="guid">The unique identifier of the person to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains true if the person was deleted;
        /// otherwise, false.</returns>
        public Task<bool> DeletePersonByID(Guid guid);
        /// <summary>
        /// Updates the specified person entity asynchronously.
        /// </summary>
        /// <param name="person">The person entity with updated information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated person entity.</returns>
        public Task<Person> UpdatePerson(Person person);

    }
}
