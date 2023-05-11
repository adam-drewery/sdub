﻿namespace Sdub;

public class AsyncStubMember<TMember>
{
    private readonly Stub _stub;
    private readonly string _memberName;

    public AsyncStubMember(Stub stub, string memberName)
    {
        _stub = stub;
        _memberName = memberName;
    }
    
    public void Returns(TMember value)
    {
        _stub.ReturnValues[_memberName] = Task.FromResult(value);
    }
}