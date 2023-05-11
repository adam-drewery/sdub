namespace Sdub.Examples;

public interface IAccountClient
{
    public Task<Account> GetAccountAsync(string accountCode);
}