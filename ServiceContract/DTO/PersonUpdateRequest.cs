using Entities;
using ServiceContract.Enums;
using System.ComponentModel.DataAnnotations;


namespace ServiceContract.DTO
{
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "PersonID is required.")]
        public Guid PersonID { get; set; }
        [Required(ErrorMessage = "PersonName is required.")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }
        public override string ToString()
        {
            return $"PersonAddRequest [PersonID={PersonID}, PersonName={PersonName}, Email={Email}, DateOfBirth={DateOfBirth}, Gender={Gender}, CountryID={CountryID}, Address={Address}, ReceiveNewsletters={ReceiveNewsletters}]";
        }

        public Person ToPerson()
        {
            return new Person
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender?.ToString(),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsletters = ReceiveNewsletters
            };
        }
    }
}
