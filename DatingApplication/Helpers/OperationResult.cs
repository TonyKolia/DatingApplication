using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApplication.Helpers
{
    //helper class used to return a result of success & message from various operations
    public class OperationResult
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }
}