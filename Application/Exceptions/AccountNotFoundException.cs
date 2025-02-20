namespace Application.Exceptions;

public class AccountNotFoundException : Exception
{
    public AccountNotFoundException(int id) : base($"Account with id {id} was not found")
    { }

    public AccountNotFoundException(string login) : base($"Account with login {login} was not found")
    { }
}