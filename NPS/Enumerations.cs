using NPS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NPS
{
    public class Enumerations
    {

        public static NpsEngagementType GetEngagementTypeByCode(string code)
        {
            switch (code)
            {
                case "ORDER": return NpsEngagementType.CheckoutExperience;
                case "WEB_ARTICLE": return NpsEngagementType.WebArticle;

                default: return NpsEngagementType.None;
            }
        }


        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

     
    }
}