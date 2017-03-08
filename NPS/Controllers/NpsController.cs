using NLog;
using NPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NPS.Controllers
{
    public class NpsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPut]
        public NpsUpdateResponse Update(NpsModel model)
        {
            var updateResponse = new NpsUpdateResponse();
            try
            {
                using (var ctx = new DataClassesDataContext())
                {

                    var nps = ctx.Engagement_NetPromoterScores.FirstOrDefault(r => r.NPSEngagementId == model.NpsEngagementId && r.MasterCustomerId == model.MasterCustomerId);

                    if (nps != null)
                    {
                        nps.NPSResultDate = DateTime.Now;
                        nps.NPSScore = Convert.ToInt32(model.Score);
                        nps.NPSComments = model.Comments;
                        ctx.SubmitChanges();
                    }

                    updateResponse.Success = true;
                    return updateResponse;
                }
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e, "Error in Controller");
                updateResponse.Success = false;
                updateResponse.ErrorMessage = e.Message;
                return updateResponse;
            }
        }
    }
}
