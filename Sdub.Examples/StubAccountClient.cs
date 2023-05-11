namespace Sdub.Examples;

public class StubAccountClient : Stub<IAccountClient>, IAccountClient
{
    public async Task<Account> GetAccountAsync(string accountCode) => Invoke();
    // {
    //     await Task.Delay(100);
    //     return new Account
    //     {
    //         CreatedAt = DateTime.UtcNow,
    //         Name = "John Doe",
    //         Email = "john.doe@example.com"
    //     };
    // }
}