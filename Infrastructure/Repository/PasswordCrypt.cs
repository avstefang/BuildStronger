using Application.Interface;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Infrastructure.Auth;

public class PasswordCrypt : IPasswordCrypt
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public string SecureToPlain(SecureString secureString)
    {
        throw new NotImplementedException();
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}