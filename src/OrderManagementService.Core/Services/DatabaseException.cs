using System;

namespace OrderManagementService.Core.Services;

public class DatabaseException : Exception
{
    public DatabaseExceptionType ExceptionType { get; }

    public DatabaseException(DatabaseExceptionType exceptionType, string message) : base(message)
    {
        ExceptionType = exceptionType;
    }
}