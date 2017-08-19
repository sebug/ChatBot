using System;
namespace ChatBot.Models
{
    public class Activity
    {
        public string type { get; set; }
        public string id { get; set; }
        public string channelId { get; set; }
        public string serviceUrl { get; set; }
        public Contact from { get; set; }
        public Conversation conversation { get; set; }
        public Contact recipient { get; set; }
        public string text { get; set; }
		public string replyToId { get; set; }
    }
}
