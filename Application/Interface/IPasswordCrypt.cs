using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Application.Interface;

public interface IPasswordCrypt
{
    string SecureToPlain(SecureString secureString);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}