namespace Sdub.Examples;

public class StubAccountClient : Stub, IAccountClient
{
    public Task<Account> GetAccountAsync(string accountCode) => Invoke(new [] { accountCode });
    
    public string GetToken() => Invoke();
}