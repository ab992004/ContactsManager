using System;
using System.Collections.Generic;
using System.Text;
using Entities;
namespace ServiceContract.DTO
{
    public class CountryAddRequest
    {
        public string? CountryName { get; set; }
        public Country ToCountry()
        {
            return new Country
            {
                CountryName = CountryName
            };
        }
    }
    
}

