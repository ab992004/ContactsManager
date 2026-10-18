using Entities;
using ServiceContract.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace ServiceContract.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "PersonName is required.")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage ="Please select gender of the person")]
        public GenderOptions? Gender { get; set; }
        [Required(ErrorMessage ="Please select a country")]
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }
        public override string ToString()
        {
            return $"PersonAddRequest [PersonName={PersonName}, Email={Email}, DateOfBirth={DateOfBirth}, Gender={Gender}, CountryID={CountryID}, Address={Address}, ReceiveNewsletters={ReceiveNewsletters}]";
        }

        public Person ToPerson()
        {
            return new Person
            {
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
