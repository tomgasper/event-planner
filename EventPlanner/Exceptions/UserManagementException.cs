public class UserManagementException : Exception
{
    public UserManagementException(string message) : base(message)
    {
    }

    public UserManagementException(string message, Exception inner) : base(message, inner)
    {
    }
}