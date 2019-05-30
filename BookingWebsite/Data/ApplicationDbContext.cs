using System;
using System.Collections.Generic;
using System.Text;
using BookingWebsite.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingWebsite.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //add model to database - passing ProductTypes to DBSet object 
        public DbSet<ProductTypes> ProductTypes { get; set; }

        // Add model for tags to database - passing tags to DBSet object
        public DbSet<Tags> Tags { get; set; }

        // Add model for Product to database - passing Product to DBSet object
        public DbSet<Products> Products { get; set; }

        // Appointments
        public DbSet<Appointments> Appointments { get; set; }

        // Products Selected for appointment
        public DbSet<ProductsSelectedForAppointment> ProductsSelectedForAppointments { get; set; }

        // Application User
        public DbSet<ApplicationUser> ApplicationUser { get; set; }


        // Employee
        public DbSet<Employee> Employees { get; set; }


        //
        public DbSet<Branch> Branches { get; set; }


        // Customer







    }
}
