﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly DataContext _context;
        public CustomerRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Customers.Include(p => p.User);
        }
        public IEnumerable<SelectListItem> GetComboCustomers()
        {
            var list = _context.Customers.Select(c => new SelectListItem
            {
                Text = c.FullName,
                Value = c.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select the Customer...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboCustomersEmail()
        {
            var list = _context.Customers.Select(p => new SelectListItem
            {
                Text = p.FullName,
                Value = p.Email,

            }).ToList();
            var allEmails = "";
            foreach (var email in list)
            {
                if (allEmails != "")
                {
                    allEmails = allEmails + ",";
                }
                allEmails = allEmails + email.Value.ToString();

            }
            list.Insert(0, new SelectListItem
            {
                Text = "Empty",
                Value = null
            });
            list.Insert(1, new SelectListItem
            {
                Text = "All Emails",
                Value = allEmails
            });

            return list;
        }
    }
}
