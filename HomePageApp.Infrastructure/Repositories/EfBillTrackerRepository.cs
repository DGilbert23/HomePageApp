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

        private DateTime? CalculateNextDue(string frequency, DateTime? lastDue, DateTime startDue)
        {
            DateTime? nextDue = DateTime.Now;

            switch (frequency.ToUpper())
            {
                case "MONTHLY":
                    if (lastDue != null)
                        nextDue = lastDue.Value.AddMonths(1);
                    else
                        nextDue = startDue;
                    break;

                case null:
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

            var nextDue = CalculateNextDue(bill.Frequency ?? "MONTHLY", bill.NextDue, bill.StartDue);
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
            return await context.Bills.OrderByDescending(b => b.NextDue).ToListAsync<Bill>();
        }

        public async Task<List<Bill>> GetUpcomingBillsAsync()
        {
            using var context = _dbFactory.CreateDbContext();
            return await context.Bills.Where(b => b.NextDue < DateTime.Now.AddDays(3))
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
                    var nextDue = CalculateNextDue(target.Frequency ?? "MONTHLY", target.NextDue, target.StartDue);
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
                currentBill.NextDue = updatedBill.NextDue;
                currentBill.LastPaid = updatedBill.LastPaid;
                currentBill.EstimatedAmountDue = updatedBill.EstimatedAmountDue;
                currentBill.PaymentUrl = updatedBill.PaymentUrl;

                await context.SaveChangesAsync();
            }
        }
    }
}
