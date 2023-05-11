using FluentAssertions;
using Xunit;

namespace Sdub.Examples;

public class UnitTest
{
    private readonly StubAccountClient _accountClient = new();

    [Fact]
    public async Task ExampleTest()
    {
        var account = await _accountClient.GetAccountAsync("123");

        account.Should().NotBeNull();
        account.Name.Should().Be("John Doe");
    }

    [Fact] 
    public async Task ReturnNull()
    {
        _accountClient
            .Setup(x => x.GetAccountAsync(""))
            .Returns(Task.FromResult((Account)null));
        
        var account = await _accountClient.GetAccountAsync("123");
        
        account.Should().BeNull();
    }
}