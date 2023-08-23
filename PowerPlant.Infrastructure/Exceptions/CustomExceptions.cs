using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlant.Infrastructure.Exceptions
{
    public class CustomExceptions : Exception
    {
        public int StatusCode { get; set; }
        public CustomExceptions(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
