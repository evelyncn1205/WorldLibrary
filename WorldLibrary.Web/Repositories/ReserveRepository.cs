﻿using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorldLibrary.Web.Models;
using System;
using WorldLibrary.Web.Helper;
using Vereyon.Web;
using WorldLibrary.Web.Enums;

namespace WorldLibrary.Web.Repositories
{
    public class ReserveRepository : GenericRepository<Reserve>, IReserveRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBookRepository _bookRepository;
        private readonly IFlashMessage _flashMessage;
        public ReserveRepository(DataContext context,
            IUserHelper userHelper,
            IBookRepository bookRepository,
            IFlashMessage flashMessage) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _bookRepository= bookRepository;
            _flashMessage = flashMessage;
        }

        public async Task AddItemReserveAsync(AddReserveViewModel model, string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return;
            }
            var reserve = await _context.Books.FindAsync(model.BookId);
            if (reserve == null)
            {
                return;
            }
            var customer = await _context.Customers.FindAsync(model.CustomerId);
            if (customer == null)
            {
                return;
            }
            var reserveDetailTemp = await _context.ReserveDetailsTemp
            .Where(rdt => rdt.User == user && rdt.Book == reserve && rdt.Customer == customer)
            .FirstOrDefaultAsync();
            if (reserveDetailTemp == null)
            {
                reserveDetailTemp = new ReserveDetailTemp
                {
                    Customer = customer,
                    Book = reserve,
                    Quantity = model.Quantity,
                    User = user,
                };
                _context.ReserveDetailsTemp.Add(reserveDetailTemp);
            }
            else
            {
                reserveDetailTemp.Quantity += model.Quantity;
                _context.ReserveDetailsTemp.Update(reserveDetailTemp);
            }
            await _context.SaveChangesAsync();
        }

        public async Task BookReturnAsync(BookReturnViewModel model)
        {
            var reserve = await _context.Reserves.FindAsync(model.Id);
            if (reserve == null)
            {
                return;
            }
            var book = await _context.Books.FindAsync(model.BookId);
            if (book == null)
            {
                return;
            }

            reserve.Book= book;
            reserve.ActualReturnDate = model.ActualReturnDate;
            reserve.Rate = model.Rate;
            reserve.ReturnDate = reserve.ReturnDate;

            if (model.Quantity == reserve.Quantity)
            {
                book.Quantity += model.Quantity;
                reserve.Quantity -= model.Quantity;
                reserve.StatusReserve = StatusReserve.Concluded;

            }
            double sub;
            if (model.Quantity < reserve.Quantity)
            {
                sub = reserve.Quantity - model.Quantity;
                book.Quantity += sub;
                reserve.Quantity -= sub;
                if (reserve.Quantity > 1)
                {
                    reserve.StatusReserve = StatusReserve.Renewed;
                }

            }

            _context.Reserves.Update(reserve);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CancelReserveAsync(BookReturnViewModel model)
        {
            var reserve = await _context.Reserves.FindAsync(model.Id);
            if (reserve == null)
            {
                return false;

            }
            else
            {
                var book = _context.Books.FindAsync(model.Id);
                book.Result.Quantity += reserve.Quantity;
                reserve.StatusReserve = StatusReserve.Cancelled;
                _context.Reserves.Update(reserve);
                _context.Books.Update(book.Result);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> ConfirmReservAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return false;
            }
            var reserveTemp = await _context.ReserveDetailsTemp
                .Include(r => r.Book)
                .Include(r => r.Customer)
                .Where(r => r.User == user)
                .ToListAsync();
            if (reserveTemp == null || reserveTemp.Count == 0)
            {
                return false;
            }

            var details = reserveTemp.Select(r => new ReserveDetail
            {
                Customer = r.Customer,
                Book = r.Book,
                Quantity = r.Quantity,
            }).ToList();
            foreach (ReserveDetail id in details)
            {
                var reserve = new Reserve
                {
                    Customer = id.Customer,
                    Book = id.Book,
                    BookingDate = DateTime.Now,
                    User = user,
                    Quantity = id.Quantity,
                    StatusReserve = StatusReserve.Active


                };
                await CreateAsync(reserve);


            }

            _context.ReserveDetailsTemp.RemoveRange(reserveTemp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var reserveDetailTemp = await _context.ReserveDetailsTemp.FindAsync(id);
            if (reserveDetailTemp == null)
            {
                return;

            }
            _context.ReserveDetailsTemp.Remove(reserveDetailTemp);
            await _context.SaveChangesAsync();
        }
               
        public async Task DeliverReserveAsync(DeliveryViewModel model)
        {
            var reserve = await _context.Reserves.FindAsync(model.Id);

            if (reserve == null)
            {
                return;
            }

            reserve.DeliveryDate = model.DeliveryDate;
            reserve.ReturnDate = model.ReturnDate;
            reserve.StatusReserve = StatusReserve.Active;

            _context.Reserves.Update(reserve);
            await _context.SaveChangesAsync();
        }

        public async Task EditReserveDetailTempAsync(AddReserveViewModel model, string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return;

            }
            var book = await _context.Books.FindAsync(model.BookId);
            if (book == null)
            {
                return;
            }
            var customer = await _context.Customers.FindAsync(model.CustomerId);
            if (customer == null)
            {
                return;
            }
            var reserveDetailTemp = await _context.ReserveDetailsTemp.FindAsync(model.Id);

            reserveDetailTemp.Customer = customer;
            reserveDetailTemp.User= user;
            reserveDetailTemp.Book= book;
            reserveDetailTemp.Quantity= model.Quantity;

            _context.ReserveDetailsTemp.Update(reserveDetailTemp);

            await _context.SaveChangesAsync();
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.PhysicalLibraries.Include(p => p.User);
        }

        public DateTime GetBookingDate()
        {
            return DateTime.Now;
        }

        public DateTime GetDeliveryDate()
        {
            return DateTime.Now;
        }

        public async Task<IQueryable<ReserveDetailTemp>> GetDetailsTempAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }
            return _context.ReserveDetailsTemp
                .Include(r => r.Book)
                .Include(r => r.Customer)
                .Where(r => r.User == user)
                .OrderBy(r => r.Customer.FullName);
        }

        public async Task<IQueryable<Reserve>> GetReserveAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }
            if (await _userHelper.IsUserInRoleAsync(user, "Admin") ||
                await _userHelper.IsUserInRoleAsync(user, "Employee"))

            {
                return _context.Reserves
                    .Include(b => b.User)
                    .Include(c => c.Customer)
                    .Include(i => i.Book)
                    .OrderByDescending(m => m.DeliveryDate);
            }

            return _context.Reserves
                    .Include(i => i.Book)
                    .Where(r => r.User == user)
                    .OrderByDescending(m => m.DeliveryDate);
        }

        public async Task<Reserve> GetReserveAsync(int id)
        {
            return await _context.Reserves.FindAsync(id);
        }

        public async Task<ReserveDetailTemp> GetReserveDetailTempAsync(int id)
        {
            return await _context.ReserveDetailsTemp.FindAsync(id);
        }
               
    }
}
