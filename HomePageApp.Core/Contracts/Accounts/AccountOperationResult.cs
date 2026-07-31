namespace HomePageApp.Core.Contracts.Accounts
{
    public class AccountOperationResult
    {
        public bool Succeeded { get; init; }
        public IReadOnlyList<string> Errors { get; init; } = [];
        public static AccountOperationResult Success() => new() { Succeeded = true };
        public static AccountOperationResult Failure(params string[] errors) =>
            new() { Succeeded = false, Errors = errors };
    }
}
