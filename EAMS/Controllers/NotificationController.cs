using AutoMapper;
using EAMS.ViewModels;
using EAMS.ViewModels.ReportViewModel;
using EAMS_ACore.HelperModels;
using EAMS_ACore.Interfaces;
using EAMS_ACore.NotificationModels;
using EAMS_ACore.ReportModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EAMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public NotificationController(INotificationService notificationService, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _notificationService = notificationService;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        [Route("SendNotification")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendNotification(NotificationViewModel notificationViewModel)
        {
            var mappedData = _mapper.Map<Notification>(notificationViewModel);
            var fcmResult = await SendFCMNotificationAsync(mappedData);
            if (fcmResult.IsSucceed == true)
            {
                var result = await _notificationService.SendNotification(mappedData);
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }

        }
        private async Task<ServiceResponse> SendFCMNotificationAsync(Notification notification)
        {
            var fcmEndpoint = "https://fcm.googleapis.com/fcm/send";
            var fcmServerKey = "AAAAOWLoIE0:APA91bHFgXO3W9a4NnztgobUGF4ov6dBNoma9gU9wqdV0O9A_3b9UtnoW-qBcQKRcMC3I3CQtyxYrLaUWAWvkdgI64IABZwjr4PFenW0JpBfd9GA1O2xqZVEi536WYDgVGTyEK2-XN7j";

            using (var client = _httpClientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", fcmServerKey);

                var payload = new
                {
                    to = "/topics/SOtopic",
                    notification = new
                    {
                        body = notification.Body,
                        title = notification.Title,
                        android_channel_id = "high_importance_channel",
                        sound = true,
                        color = "#FF5733",
                        //icon = "ic_notification", // You can specify the icon for the notification
                        //tag = "unique_tag", // You can specify a tag for the notification

                    },
                    data = new
                    {
                        message = "This is a Firebase Cloud Messaging Topic Message!"
                    }
                };

                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(fcmEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    return new ServiceResponse { IsSucceed = true, Message = "Push Notification Sent Successfully" };

                }
                else
                {
                    return new ServiceResponse { IsSucceed = false, Message = "Push Notification Not Sent" };
                }
            }
        }

        [HttpPost]
        [Route("IsNotificationSeen")]
        [Authorize]
        public async Task<IActionResult> IsNotificationSeen(string notificationId, bool isStatus)
        {
            var result=_notificationService.IsNotificationSeen(notificationId, isStatus);
            return Ok(result);

        }
        
        [Route("GetNotification")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotification()
        {
            var result = await _notificationService.GetNotification();

            return Ok(result);
        }

        [Route("AddSMSTemplate")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddSMSTemplate(SMSTemplateViewModel sMSTemplateViewModel)
        {
            var mappedData = _mapper.Map<SMSTemplate>(sMSTemplateViewModel);
            var result = await _notificationService.AddSMSTemplate(mappedData);

            return Ok(result);
        }

        [Route("GetSMSTemplates")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSMSTemplates()
        {
            var result = await _notificationService.GetSMSTemplate();

            return Ok(result);
        }
        
        [Route("GetSMSTemplateById")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSMSTemplateById(string SMSTemplateById)
        {
            //var mappedData = _mapper.Map<SMSSent>(SMSSentModel);
            var result = await _notificationService.GetSMSTemplateById(SMSTemplateById);
            if (result == null)
            {
                return BadRequest("Record Not Found");
            }
            else
            {
                return Ok(result);
            }
        }
        
        //[Route("SendOtp")]
        //[HttpPost]
        private async Task<IActionResult> SendOtp(string mobile, string otp)
        {
            //not in use
            var result = await _notificationService.SendOtp(mobile, otp);

            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateSMSTemplateById")]
        [Authorize]
        public async Task<IActionResult> UpdateSMSTemplateById(SMSTemplateViewModel sMSTemplateViewModel)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<SMSTemplate>(sMSTemplateViewModel);
                var sms_temp = await _notificationService.UpdateSMSTemplateById(mappedData);

                return Ok(sms_temp);

            }
            else

            {
                return BadRequest(ModelState);
            }
        }

        [Route("SendSmsToSO")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendSMSToSectorOfficers(SendSMSViewModel sendSMSViewModel)
        {
            var mappedData = _mapper.Map<SendSMSModel>(sendSMSViewModel);
            var result = await _notificationService.SendSMSToSectorOfficers(mappedData);

            return Ok(result);
        }
    }
}
