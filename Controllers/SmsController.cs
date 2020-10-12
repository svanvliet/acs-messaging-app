using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SVV.MessagingApp.Data;
using SVV.MessagingApp.Hubs;

namespace SVV.MessagingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventGridController : Controller
    {
        IHubContext<SmsHub> HubContext { get; set; }

        public EventGridController(IHubContext<SmsHub> context)
        {
            HubContext = context;
        }

        private bool EventTypeSubcriptionValidation
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
               "SubscriptionValidation";

        private bool EventTypeNotification
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
               "Notification";

        [HttpPost("[action]")]
        public async Task<IActionResult> Update()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var json = await reader.ReadToEndAsync();

                if (EventTypeSubcriptionValidation)
                {
                    var validationEvent = JsonSerializer.Deserialize<List<EventGridPayload<Dictionary<string,string>>>>(json, 
                        new JsonSerializerOptions(){ 
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                        }).First();
                    var validationCode = validationEvent.Data["validationCode"];
                    return new JsonResult(new { validationResponse = validationCode});
                } 
                else if (EventTypeNotification)
                {
                    var notificationList = JsonSerializer.Deserialize<List<EventGridPayload<SmsEvent>>>(json, 
                        new JsonSerializerOptions(){ 
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                        });
                    foreach (var notification in notificationList){
                        await HubContext.Clients.All.SendAsync("SmsEvent_Received", notification);
                    }
                }
            }
            return NoContent();
        }

    }
}