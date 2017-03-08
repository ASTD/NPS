using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NPS.Models
{

    public enum NpsEngagementType
    {
        None,
        [Description("Checkout Experience")]
        CheckoutExperience,

        [Description("Web Article")]
        WebArticle
    }



}