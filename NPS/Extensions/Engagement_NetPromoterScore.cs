using NPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPS
{
    public partial class Engagement_NetPromoterScore
    {
        public NpsEngagementType GetEngagementTypeByCode()
        {
            return Enumerations.GetEngagementTypeByCode(this.EngagementType);
        }
    }
}