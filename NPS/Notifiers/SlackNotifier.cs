using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using NPS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
namespace NPS.Notifiers
{
    public class SlackNotifier
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public static object ParameterType { get; private set; }

        public enum NotificationMode
        {
            Score,
            Comments
        }

        public class Field
        {
            public string title { get; set; }
            public string value { get; set; }

            [JsonProperty("short")]
            public bool Short { get; set; }
        }

        public class Attachment
        {
            public string fallback { get; set; }
            public string color { get; set; }
            public string pretext { get; set; }
            public string author_name { get; set; }
            public string author_link { get; set; }

            public string title { get; set; }
            public List<Field> fields { get; set; }

            public string footer { get; set; }
        }

        public class SlackMessage
        {
            public List<Attachment> attachments { get; set; }
        }


        public void Notify(Engagement_NetPromoterScore item)
        {

            try
            {

                using (var ctx = new NpsCustomerRepositoryDataContext(ConfigurationManager.ConnectionStrings["NpsCustomerRepository"].ConnectionString))
                {
                    var customer = ctx.CUSTOMERs.FirstOrDefault(c => c.MASTER_CUSTOMER_ID == item.MasterCustomerId);
                    if (customer == null) return;

                    // build message
                    var sb = new StringBuilder();

                    var message = new SlackMessage() { attachments = new List<Attachment>() };
                    var attachment = new Attachment();
                    attachment.fallback = sb.ToString();
                    attachment.color = item.NPSScore > 8 ? "#6c9c2c" : (item.NPSScore > 6 ? "#ff9e01" : "#aa0027");
                    attachment.pretext = "";
                    attachment.author_name = $"{customer.LABEL_NAME} ({customer.MASTER_CUSTOMER_ID})";
                    attachment.author_link = $"mailto:{customer.PRIMARY_EMAIL_ADDRESS}";
                    attachment.title = Enumerations.GetEnumDescription(item.GetEngagementTypeByCode());
                    attachment.footer = $"Reference ID: {item.ReferenceId} | {item.NPSResultDate?.ToString("MM/dd/yyyy hh:MM:ss tt")}";
                    
                    attachment.fallback = $"{customer.LABEL_NAME} ({customer.MASTER_CUSTOMER_ID}), {customer.PRIMARY_EMAIL_ADDRESS}, scored the {Enumerations.GetEnumDescription(item.GetEngagementTypeByCode())} *{item.NPSScore}* out of 10.";
                    attachment.fields = new List<Field>();

                    if (item.NPSScore.HasValue)
                    {
                        var scoreField = new Field();
                        scoreField.title = "Score";
                        scoreField.Short = true;
                        scoreField.value = $"{item.NPSScore} out of 10";
                        attachment.fields.Add(scoreField);
                    }

                    if (!string.IsNullOrEmpty(item.NPSComments))
                    {
                        var commentsField = new Field();
                        commentsField.title = "Comments";
                        commentsField.Short = true;
                        commentsField.value = $"{item.NPSComments}";
                        attachment.fields.Add(commentsField);
                    }

                    message.attachments.Add(attachment);
                    using (var client = new WebClient())
                    {
                        var json = JsonConvert.SerializeObject(message, Formatting.None, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        client.UploadString(new Uri("https://hooks.slack.com/services/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"), "POST", json);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, "Error in Slack Notifier | " + ex.Message + "|" + ex.StackTrace);
            }
        }
    }
}