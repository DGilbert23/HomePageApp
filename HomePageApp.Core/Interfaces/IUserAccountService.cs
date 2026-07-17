namespace HomePageApp.Core.Interfaces;

public interface IUserAccountService
{
    Task<Guid> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string lastName);

    Task<bool> CheckPasswordAsync(
        string email,
        string password);
}