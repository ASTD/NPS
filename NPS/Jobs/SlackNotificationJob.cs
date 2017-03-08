using NPS.Notifiers;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPS.Jobs
{
    public class SlackNotificationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var slackNotifider = new SlackNotifier();
            using (var ctx = new DataClassesDataContext())
            {
                var toBeNotified = ctx.Engagement_NetPromoterScores.Where(r => r.CreateDateTime < DateTime.Now.AddMinutes(-15) && !r.SentToSlack && r.NPSScore.HasValue).OrderBy(r=>r.CreateDateTime).ToList();

                foreach (var item in toBeNotified)
                {
                    item.SentToSlack = true;
                    slackNotifider.Notify(item);
                }

                ctx.SubmitChanges();
            }
        }
    }
}