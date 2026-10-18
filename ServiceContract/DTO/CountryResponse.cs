using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContract.DTO
{
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is not CountryResponse other || obj is null)
            {
            return false;
            }
            return this.CountryID == other.CountryID && this.CountryName == other.CountryName;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName
            };
        }
    }
}
