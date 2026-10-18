using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }
        [StringLength(40)]
        public string? PersonName { get; set; }
        [StringLength(40)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(10)]
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        [StringLength(200)]
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }
        public string? TIN { get; set; }
        [ForeignKey(nameof(CountryID))]
        public virtual Country? Country { get; set; }

    }
}
