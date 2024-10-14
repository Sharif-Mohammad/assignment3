using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJTPWebserver
{
    public static class Status
    {
        public const string ReadSuccess = "1 Ok";
        public const string UpdateSuccess = "3 Updated";
        public const string BadRequest = "4 Bad Request";
        public const string CreateSuccess = "2 Created";
        public const string NotFound = "5 Not Found";
        public const string Error = "5 Error";
    }
}
