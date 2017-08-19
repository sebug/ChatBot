using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ChatBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ChatBot.Controllers
{
	/// <summary>
	/// Messages controller.
	/// 
	/// Based on https://carlos.mendible.com/2016/09/11/netcore-and-microsoft-bot-framework/
	/// </summary>
	[Route("api/[controller]")]
    public class MessagesController : Controller
    {
		private readonly ChatBotOptions _chatBotOptions;

		public MessagesController(IOptions<ChatBotOptions> chatBotOptions)
		{
			this._chatBotOptions = chatBotOptions.Value;
		}

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Activity activity)
        {
            string authorizationHeader = this.Request.Headers["Authorization"];
            string conversationID = null;

            string activityID = null;
            string serviceUrl = null;
            if (activity != null)
            {
                if (activity.conversation != null)
                {
                    conversationID = WebUtility.UrlEncode(activity.conversation.id);
                }
                activityID = WebUtility.UrlEncode(activity.id);
                serviceUrl = activity.serviceUrl;
            }

            if (conversationID != null)
            {
				string token = await this.GetBotApiToken();

				using (var client = new HttpClient())
				{
                    Activity message = new Activity();
					message.type = "message";
                    message.from = activity.recipient;
                    message.recipient = activity.from;
                    message.conversation = activity.conversation;
					message.text = "Ohai " + activity.text;
                    message.replyToId = activity.id;


                    if (serviceUrl.StartsWith("https", StringComparison.InvariantCultureIgnoreCase)) {
						client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);   
                    }

                    string postUrl =
                        $"{serviceUrl}/v3/conversations/{conversationID}/activities/{activityID}";
                    
                    var response = await client.PostAsync(postUrl,
						new StringContent(JsonConvert.SerializeObject(message),
										  Encoding.UTF8, "application/json"));
                    var statusCode = response.StatusCode;
                    string content = await response.Content.ReadAsStringAsync();
				}   
            }
            return Created(Url.Content("~/"), string.Empty);
		}

		/// <summary>
		/// Gets and caches a valid token so the bot can send messages.
		/// </summary>
		/// <returns>The token</returns>
		private async Task<string> GetBotApiToken()
		{
            // Check to see if we already have a valid token
            string token = null; //memoryCache.Get("token")?.ToString();
			if (string.IsNullOrEmpty(token))
			{
				// we need to get a token.
				using (var client = new HttpClient())
				{
					// Create the encoded content needed to get a token
					var parameters = new Dictionary<string, string>
					{
                        {"client_id", this._chatBotOptions.AppID },
                        {"client_secret", this._chatBotOptions.Secret },
						{"scope", "https://graph.microsoft.com/.default" },
						{"grant_type", "client_credentials" }
					};
					var content = new FormUrlEncodedContent(parameters);

					// Post
					var response = await client.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token", content);

                    // Get the token response
                    string r = await response.Content.ReadAsStringAsync();

                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(r);

					token = tokenResponse.access_token;

					// Cache the token for 15 minutes.
					//memoryCache.Set(
					//	"token",
					//	token,
					//	new DateTimeOffset(DateTime.Now.AddMinutes(15)));
				}
			}

			return token;
		}
    }
}
