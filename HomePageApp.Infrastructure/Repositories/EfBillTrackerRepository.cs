using HomePageApp.Core.Interfaces.BillTracker;
using HomePageApp.Core.Interfaces.Identity;
using HomePageApp.Core.Models.BillTracker;
using HomePageApp.Infrastructure.Services.Google.GoogleCalendar;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Infrastructure.Repositories
{
    public class EfBillTrackerRepository : IBillTrackerRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly IUserAccountService _userAccountService;

        public EfBillTrackerRepository(IDbContextFactory<AppDbContext> dbFactory, IUserAccountService userAccountService)
        {
            _dbFactory = dbFactory;
            _userAccountService = userAccountService;
        }

        private DateTime? CalculateNextDue(string frequency, DateTime? lastMarked, DateTime startDue, DateTime? lastDue)
        {
            DateTime? nextDue = startDue;

            switch (frequency.ToUpper())
            {
                case "MONTHLY":
                    if (lastMarked != null)
                    {
                        do
                            nextDue = nextDue?.AddMonths(1);
                        while (nextDue < lastMarked?.AddMonths(1));
                    }

                    break;

                case "ANNUALLY":
                    if (lastMarked != null)
                    {
                        do
                            nextDue = nextDue?.AddYears(1);
                        while (nextDue < lastMarked?.AddYears(1));
                    }

                    break;

                case "":
                    nextDue = null;
                    break;

                default:
                    nextDue = DateTime.Now;
                    break;
            }

            return nextDue;
        }

        private async Task<int> GetCurrentUserId()
        {
            return await _userAccountService.GetCurrentUserProfileIdAsync();
        }

        private async Task<bool> AllowedToEdit(int billOwnerId)
        {
            using var context = _dbFactory.CreateDbContext();
            var currentUserId = await GetCurrentUserId();

            return billOwnerId == currentUserId
                || await context.BillShareDefinitions.AnyAsync(s => s.OwnerId == billOwnerId && s.ShareWithId == currentUserId);
        }

        public async Task AddBillAsync(Bill bill)
        {
            using var context = _dbFactory.CreateDbContext();

            var nextDue = CalculateNextDue(bill.Frequency ?? "MONTHLY", null, bill.StartDue, bill.NextDue);
            bill.NextDue = nextDue;

            bill.UserId = await GetCurrentUserId();

            context.Bills.Add(bill);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBillAsync(int id)
        {
            using var context = _dbFactory.CreateDbContext();
            var target = await context.Bills.FindAsync(id);

            if (target != null)
            {
                if (!await AllowedToEdit(target.UserId))
                    throw new InvalidOperationException("Authenticated UserId does not match record to delete and has not been granted delete permissions.");

                context.Bills.Remove(target);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Delete target ID does not exist.");
            }
        }

        public async Task<List<Bill>> GetAllBillsAsync()
        {
            using var context = _dbFactory.CreateDbContext();
            var userId = await GetCurrentUserId();

            return await context.Bills.Where(b => b.UserId == userId
                                               || context.BillShareDefinitions.Any(s => s.OwnerId == b.UserId && s.ShareWithId == userId)
                                            )
                                      .OrderByDescending(b => b.NextDue)
                                      .ThenBy(b => b.Name)
                                      .ToListAsync<Bill>();
        }

        public async Task<List<Bill>> GetUpcomingBillsAsync(int daysOut)
        {
            using var context = _dbFactory.CreateDbContext();
            var userId = await GetCurrentUserId();

            return await context.Bills.Where(b => b.NextDue < DateTime.Now.AddDays(daysOut) && (b.UserId == userId
                                                                                                || context.BillShareDefinitions.Any(s => s.OwnerId == b.UserId && s.ShareWithId == userId)
                                                                                               )
                                            )
                                      .OrderByDescending(b => b.NextDue).ToListAsync<Bill>();
        }

        public async Task MarkPaidOrSeenAsync(int id)
        {
            using var context = _dbFactory.CreateDbContext();
            var target = await context.Bills.FindAsync(id);

            if (target != null)
            {
                if (!await AllowedToEdit(target.UserId))
                    throw new InvalidOperationException("Authenticated UserId does not match record to update and has not been granted update permissions.");

                target.LastPaidOrSeen = DateTime.Now;
                if (target.Reoccurring)
                {
                    var nextDue = CalculateNextDue(target.Frequency ?? "MONTHLY", target.LastPaidOrSeen, target.StartDue, target.NextDue);
                    target.NextDue = nextDue;
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task SaveBillAsync(Bill updatedBill)
        {
            using var context = _dbFactory.CreateDbContext();
            var currentBill = await context.Bills.FindAsync(updatedBill.Id);

            if (currentBill != null)
            {
                if (!await AllowedToEdit(currentBill.UserId))
                    throw new InvalidOperationException("Authenticated UserId does not match record to update and has not been granted update permissions.");

                currentBill.Name = updatedBill.Name;
                currentBill.Description = updatedBill.Description;
                currentBill.Reoccurring = updatedBill.Reoccurring;
                currentBill.Frequency = updatedBill.Frequency;
                currentBill.StartDue = updatedBill.StartDue;
                currentBill.NextDue = CalculateNextDue(updatedBill.Frequency ?? "", updatedBill.LastPaidOrSeen, updatedBill.StartDue, updatedBill.NextDue);
                currentBill.LastPaidOrSeen = updatedBill.LastPaidOrSeen;
                currentBill.EstimatedAmountDue = updatedBill.EstimatedAmountDue;
                currentBill.PaymentUrl = updatedBill.PaymentUrl;
                currentBill.AutoDraft = updatedBill.AutoDraft;

                await context.SaveChangesAsync();
            }
        }
    }
}
