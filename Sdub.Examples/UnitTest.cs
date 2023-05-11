using FluentAssertions;
using Xunit;

namespace Sdub.Examples;

public class UnitTest
{
    private readonly StubAccountClient _accountClient = new();

    private readonly Account _account = new()
    {
        CreatedAt = DateTime.UtcNow,
        Name = "John Doe",
        Email = "john.doe@example.com"
    };

    public UnitTest()
    {
        Stub
            .Setup(() => _accountClient.GetAccountAsync(null))
            .Returns(_account);
    }

    [Fact]
    public async Task ExampleTest()
    {
        var account = await _accountClient.GetAccountAsync("123");

        account.Should().NotBeNull();
        account.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task NonAsyncMethod()
    {
        Stub
            .Setup(() => _accountClient.GetToken())
            .Returns("yes");
        
        var token = _accountClient.GetToken();

        token.Should().NotBeNull();
    }

    [Fact] 
    public async Task ReturnNull()
    {
        Stub
            .Setup(() => _accountClient.GetAccountAsync(null))
            .Returns(null);
        
        var account = await _accountClient.GetAccountAsync("123");
        
        account.Should().BeNull();
    }
    
    [Fact] 
    public async Task CheckCallCount()
    {
        Stub.Setup(() => _accountClient.GetAccountAsync(null))
            .Returns(null);
        
        await _accountClient.GetAccountAsync("123");

        Stub.CallsTo(() => _accountClient.GetAccountAsync(null))
            .Should()
            .HaveCount(1);
        
    }
}