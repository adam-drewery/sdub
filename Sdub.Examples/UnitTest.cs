using FluentAssertions;
using Xunit;

namespace Sdub.Examples;

public class UnitTest
{
    private StubAccountClient _accountClient = new();

    private readonly Account _account = new()
    {
        CreatedAt = DateTime.UtcNow,
        Name = "John Doe",
        Email = "john.doe@example.com"
    };

    public UnitTest()
    {
        Stub.Setup(() => _accountClient.GetAccountAsync(null))
            .Returns(_account);
        
        Stub.Setup(() => _accountClient.GetToken())
            .Returns("this is my token");
    }

    [Fact]
    public async Task ExampleTest()
    {
        var account = await _accountClient.GetAccountAsync("123");

        account.Should().NotBeNull();
        account.Name.Should().Be("John Doe");
    }

    [Fact]
    public void NonAsyncMethod()
    {
        Stub.Setup(() => _accountClient.GetToken())
            .Returns("yes");
        
        var token = _accountClient.GetToken();

        token.Should().NotBeNull();
        token.Should().Be("yes");
    }

    [Fact]
    public async Task AsyncFunctionReturn()
    {
        Stub.Setup(() => _accountClient.GetAccountAsync(null))
            .Returns(_ => Task.FromResult<Account>(null));

        var account = await _accountClient.GetAccountAsync("yo");

        account.Should().BeNull();
    }

    [Fact]
    public void FunctionReturn()
    {
        Stub.Setup(() => _accountClient.GetToken())
            .Returns(_ => new Random().NextInt64().ToString());
        
        var token = _accountClient.GetToken();

        token.Should().NotBeNull();
        token.Should().HaveLength(19);
    }

    [Fact]
    public void ReturnDefaultWhenReturnValueNotExplicitlySet()
    {
        _accountClient = new StubAccountClient();
        
        var token = _accountClient.GetToken();
        
        token.Should().BeNull();
    }
    
    // [Fact]
    // public async Task ReturnDefaultWhenReturnValueNotExplicitlySetAsync()
    // {
    //     _accountClient = new StubAccountClient();
    //     
    //     var account = await _accountClient.GetAccountAsync("123");
    //     
    //     account.Should().BeNull();
    // }
    
    [Fact] 
    public async Task CheckCallCount()
    {
        Stub.Setup(() => _accountClient.GetAccountAsync(null))
            .Returns(_account);
        
        await _accountClient.GetAccountAsync("123");

        Stub.CallsTo(() => _accountClient.GetAccountAsync(null))
            .Should()
            .HaveCount(1);
        
    }
}