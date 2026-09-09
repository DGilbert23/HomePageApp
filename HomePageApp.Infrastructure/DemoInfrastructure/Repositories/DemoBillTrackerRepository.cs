using HomePageApp.Core.Interfaces.BillTracker;
using HomePageApp.Core.Models.BillTracker;
using Microsoft.EntityFrameworkCore;

namespace HomePageApp.Infrastructure.DemoInfrastructure.Repositories
{
    internal class DemoBillTrackerRepository : IBillTrackerRepository
    {
        private List<Bill> bills;

        public DemoBillTrackerRepository()
        {
            bills = GenerateDemoBills();
        }

        private List<Bill> GenerateDemoBills()
        {
            List<Bill> demoBills = new List<Bill>();

            demoBills.Add(new Bill
            {
                Name = "Utility"
                                    ,
                Description = "Water / Gas / Electric"
                                    ,
                Reoccurring = true
                                    ,
                Frequency = "MONTHLY"
                                    ,
                StartDue = DateTime.Now.AddMonths(-1)
                                    ,
                NextDue = DateTime.Now
                                    ,
                EstimatedAmountDue = 400.00
                                    ,
                PaymentUrl = "https://google.com"
                                    ,
                AutoDraft = false

            });

            demoBills.Add(new Bill
            {
                Name = "Mortgage"
                                    ,
                Reoccurring = true
                                    ,
                Frequency = "MONTHLY"
                                    ,
                StartDue = DateTime.Now.AddMonths(-1)
                                    ,
                NextDue = DateTime.Now.AddDays(5)
                                    ,
                EstimatedAmountDue = 2500.00
                                    ,
                PaymentUrl = "https://www.jpmorgan.com/global"
                                    ,
                AutoDraft = true

            });

            demoBills.Add(new Bill
            {
                Name = "Amazon Prime"
                                    ,
                Reoccurring = true
                                    ,
                Frequency = "YEARLY"
                                    ,
                StartDue = DateTime.Now.AddMonths(-11)
                                    ,
                NextDue = DateTime.Now.AddDays(1)
                                    ,
                EstimatedAmountDue = 150.00
                                    ,
                PaymentUrl = "https://amazon.com"
                                    ,
                AutoDraft = true

            });

            return demoBills;
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

        public Task AddBillAsync(Bill bill)
        {
            var nextDue = CalculateNextDue(bill.Frequency ?? "MONTHLY", null, bill.StartDue, bill.NextDue);
            bill.NextDue = nextDue;

            bills.Add(bill);
            return Task.CompletedTask;
        }

        public Task DeleteBillAsync(int id)
        {
            var billToRemove = bills.Find(b => b.Id == id);
            if (billToRemove != null)
                bills.Remove(billToRemove);
            else
                throw new InvalidOperationException("No bill with id " + id + " found to remove.");

            return Task.CompletedTask;
        }

        public Task<List<Bill>> GetAllBillsAsync()
        {
            return Task.FromResult(bills.OrderByDescending(b => b.NextDue)
                                        .ThenBy(b => b.Name)
                                        .ToList()
                                   );
        }

        public Task<List<Bill>> GetUpcomingBillsAsync(int daysOut)
        {
            return Task.FromResult(bills.Where(b => b.NextDue < DateTime.Now.AddDays(daysOut))
                                        .OrderByDescending(b => b.NextDue)
                                        .ThenBy(b => b.Name)
                                        .ToList()
                                   );
        }

        public Task MarkPaidOrSeenAsync(int id)
        {
            var billToEdit = bills.Find(b => b.Id == id);

            if (billToEdit != null)
            {
                billToEdit.NextDue = CalculateNextDue(billToEdit.Frequency ?? "MONTHLY", null, billToEdit.StartDue, billToEdit.NextDue);
            }
            else
                throw new InvalidOperationException("No bill with id " + id + " found to mark paid/seen.");

            return Task.CompletedTask;
        }

        public Task SaveBillAsync(Bill bill)
        {
            var editBill = bills.Find(b => b.Id == bill.Id);

            if (editBill != null)
            {
                editBill.Name = bill.Name;
                editBill.Description = bill.Description;
                editBill.Reoccurring = bill.Reoccurring;
                editBill.Frequency = bill.Frequency;
                editBill.StartDue = bill.StartDue;
                editBill.NextDue = bill.NextDue;
                editBill.LastPaidOrSeen = bill.LastPaidOrSeen;
                editBill.EstimatedAmountDue = bill.EstimatedAmountDue;
                editBill.PaymentUrl = bill.PaymentUrl;
                editBill.AutoDraft = bill.AutoDraft;
                editBill.UserId = bill.UserId;                
            }
            else
                throw new InvalidOperationException("No bill with id " + bill.Id + " found to mark paid/seen.");

            return Task.CompletedTask;
        }
    }
}
