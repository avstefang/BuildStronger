using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class ReservationException(string message) : System.Exception(message)
{
}