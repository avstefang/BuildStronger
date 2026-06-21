using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interface;

public interface ITokenService
{
    string GenerateToken(Domain.Entity.Athlete athlete);
    bool ValidateToken(string token);
}