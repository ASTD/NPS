using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPS.Models
{
    public class NpsModel
    {
        public int? NpsEngagementId { get; set; }

        public string EngagementType { get; set; }

        public string MasterCustomerId { get; set; }

        public string ReferenceId { get; set; }

        public int Score { get; set; }

        public string Comments { get; set; }
    }
}