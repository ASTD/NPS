using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NPS.Controllers
{
    public class WidgetController : Controller
    {
        // GET: Widget
        public ActionResult Index(string engagementType, string masterCustomerId, string referenceId)
        {
            var shouldView = ShouldView(engagementType, referenceId, masterCustomerId);

            if (!shouldView)
            {
                return new ContentResult() { Content = "", ContentEncoding = Encoding.UTF8, ContentType = "text/html" };
            }

            // Create the NPS Survey Entry
            using (var ctx = new DataClassesDataContext())
            {
                // check if entry exists beforehand
                var exists = ctx.Engagement_NetPromoterScores.FirstOrDefault(r => r.MasterCustomerId == masterCustomerId && r.ReferenceId == referenceId && r.EngagementType == engagementType);

                if (exists != null)
                {
                    ViewBag.NpsEngagementId = exists.NPSEngagementId;
                    return PartialView();

                }

                var nps = new Engagement_NetPromoterScore();
                var customerId = ctx.Customers.FirstOrDefault(c => c.MasterCustomerId == masterCustomerId);
                nps.CreateDateTime = DateTime.Now;
                nps.CustomerId = customerId?.CustomerId;
                nps.CreatedBy = "WEB";
                nps.EngagementType = engagementType;
                nps.NPSSendDate = DateTime.Today;
                nps.NPSScore = null;
                nps.MasterCustomerId = masterCustomerId;
                nps.ReferenceId = referenceId;
                ctx.Engagement_NetPromoterScores.InsertOnSubmit(nps);
                ctx.SubmitChanges();
                ViewBag.NpsEngagementId = nps.NPSEngagementId;
            }

            return PartialView();

        }

        public bool ShouldView(string engagementType, string referenceId, string masterCustomerId)
        {

            var shouldView = false;
            var engagementTypeEnum = Enumerations.GetEngagementTypeByCode(engagementType);

            using (var ctx = new DataClassesDataContext())
            {
                var viewedToday = ctx.Engagement_NetPromoterScores.Any(r => r.MasterCustomerId == masterCustomerId && r.NPSSendDate >= DateTime.Today);
                var viewedMoreThan2In60Days = ctx.Engagement_NetPromoterScores.Count(r => r.MasterCustomerId == masterCustomerId && r.NPSSendDate >= DateTime.Today.AddDays(-60)) > 2;
                var respondedLast150Days = ctx.Engagement_NetPromoterScores.Any(r => r.MasterCustomerId == masterCustomerId && r.NPSResultDate >= DateTime.Today.AddDays(-150) && r.NPSScore.HasValue);

                shouldView = !(viewedToday || viewedMoreThan2In60Days || respondedLast150Days);
                switch (engagementTypeEnum)
                {
                    case Models.NpsEngagementType.CheckoutExperience:
                        // Already populated NPS for this order.
                        var ratedPurchaseAlready = ctx.Engagement_NetPromoterScores.Any(r => r.MasterCustomerId == masterCustomerId && r.ReferenceId == referenceId && r.EngagementType == engagementType && r.NPSScore.HasValue);
                        shouldView = shouldView && !ratedPurchaseAlready;
                        break;

                }
            }

            return shouldView;

        }
    }
}