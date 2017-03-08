using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPS.Models
{
    public class NpsCreateResponse
    {
        public bool Success { get; set; }
        public int Id { get; set; }

        public string ErrorMessage { get; set; }
    }
}