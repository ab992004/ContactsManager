using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //seed to countries

            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
            
            foreach(var country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            //seed to Persons

            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);
            
            foreach(var person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }
            //fluent api
            modelBuilder.Entity<Person>().Property(temp => temp.TIN).HasColumnName("TaxIdentificationNumber").HasColumnType("varchar(8)").HasDefaultValue("ABC123");
            //modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN).IsUnique();
            modelBuilder.Entity<Person>().ToTable(t => t.HasCheckConstraint("Check_TIN", "LEN([TaxIdentificationNumber]) = 6"));
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne(c => c.Country).WithMany(p => p.Persons).HasForeignKey(p => p.CountryID);
            //});
        }
        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }
    }
}
