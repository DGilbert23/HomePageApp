using HomePageApp.Core.Models.BillTracker;

namespace HomePageApp.Core.Interfaces
{
    public interface IBillTrackerRepository
    {
        Task<List<Bill>> GetAllBillsAsync();
        Task<List<Bill>> GetUpcomingBillsAsync(int daysOut);

        Task AddBillAsync(Bill bill);
        Task DeleteBillAsync(int id);
        Task SaveBillAsync(Bill bill);
        Task MarkPaidOrSeenAsync(int id);
    }
}
