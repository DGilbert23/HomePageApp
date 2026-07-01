using HomePageApp.Core.Interfaces;
using HomePageApp.Core.Models.BillTracker;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Infrastructure.Repositories
{
    public class EfBillTrackerRepository : IBillTrackerRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public EfBillTrackerRepository(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        private DateTime? CalculateNextDue(string frequency, DateTime? lastPaid, DateTime startDue)
        {
            DateTime? nextDue = startDue;

            switch (frequency.ToUpper())
            {
                case "MONTHLY":
                    if (lastPaid != null)
                    {
                        while (nextDue <= DateTime.Now)
                        {
                            nextDue = nextDue?.AddMonths(1);
                        }
                    }
                    else
                    {
                        nextDue = startDue;
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

        public async Task AddBillAsync(Bill bill)
        {
            using var context = _dbFactory.CreateDbContext();

            var nextDue = CalculateNextDue(bill.Frequency ?? "MONTHLY", null, bill.StartDue);
            bill.NextDue = nextDue;

            context.Bills.Add(bill);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBillAsync(int id)
        {
            using var context = _dbFactory.CreateDbContext();
            var target = await context.Bills.FindAsync(id);

            if (target != null)
            {
                context.Bills.Remove(target);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Bill>> GetAllBillsAsync()
        {
            using var context = _dbFactory.CreateDbContext();
            return await context.Bills.OrderByDescending(b => b.NextDue)
                                      .ThenBy(b => b.Name)
                                      .ToListAsync<Bill>();
        }

        public async Task<List<Bill>> GetUpcomingBillsAsync(int daysOut)
        {
            using var context = _dbFactory.CreateDbContext();
            return await context.Bills.Where(b => b.NextDue < DateTime.Now.AddDays(daysOut))
                                      .OrderByDescending(b => b.NextDue).ToListAsync<Bill>();
        }

        public async Task MarkPaidAsync(int id)
        {
            using var context = _dbFactory.CreateDbContext();
            var target = await context.Bills.FindAsync(id);
            if (target != null)
            {
                target.LastPaid = DateTime.Now;
                if (target.Reoccurring)
                {
                    var nextDue = CalculateNextDue(target.Frequency ?? "MONTHLY", target.LastPaid, target.StartDue);
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
                currentBill.Name = updatedBill.Name;
                currentBill.Description = updatedBill.Description;
                currentBill.Reoccurring = updatedBill.Reoccurring;
                currentBill.Frequency = updatedBill.Frequency;
                currentBill.StartDue = updatedBill.StartDue;
                currentBill.NextDue = CalculateNextDue(updatedBill.Frequency ?? "", updatedBill.LastPaid, updatedBill.StartDue);
                currentBill.LastPaid = updatedBill.LastPaid;
                currentBill.EstimatedAmountDue = updatedBill.EstimatedAmountDue;
                currentBill.PaymentUrl = updatedBill.PaymentUrl;

                await context.SaveChangesAsync();
            }
        }
    }
}
