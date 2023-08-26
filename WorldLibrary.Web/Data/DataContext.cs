﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<PhysicalLibrary> PhysicalLibraries { get; set; }
        public DbSet<ReserveDetail> ReserveDetails { get; set; }
        public DbSet<ReserveDetailTemp> ReserveDetailsTemp { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
