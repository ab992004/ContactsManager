using Entities;
using ServiceContract.DTO;
using ServiceContract.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContract.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Double? Age { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is PersonResponse other)
            {
                return this.PersonID == other.PersonID &&
                       this.PersonName == other.PersonName &&
                       this.Email == other.Email &&
                       this.DateOfBirth == other.DateOfBirth &&
                       this.Age == other.Age &&
                       this.Gender == other.Gender &&
                       this.CountryID == other.CountryID &&
                       this.Address == other.Address &&
                       this.ReceiveNewsletters == other.ReceiveNewsletters;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"PersonResponse [PersonID={PersonID}, PersonName={PersonName}, Email={Email}, DateOfBirth={DateOfBirth}, Age={Age}, Gender={Gender}, CountryID={CountryID}, Address={Address}, ReceiveNewsletters={ReceiveNewsletters}]";
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = this.PersonID,
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), this.Gender ?? string.Empty,true),
                CountryID = this.CountryID,
                Address = this.Address,
                ReceiveNewsletters = this.ReceiveNewsletters
            };
        }
    }
    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReceiveNewsletters = person.ReceiveNewsletters,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                Country = person.Country?.CountryName
            };
        }
    }
}


