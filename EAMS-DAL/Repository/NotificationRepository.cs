using EAMS_ACore.HelperModels;
using EAMS_ACore.IRepository;
using EAMS_ACore.Models;
using EAMS_ACore.NotificationModels;
using EAMS_ACore.ReportModels;
using EAMS_DAL.DBContext;
using Microsoft.EntityFrameworkCore;

namespace EAMS_DAL.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly EamsContext _context;
        public NotificationRepository(EamsContext context)
        {
            _context = context;
        }
        private DateTime? BharatDateTime()
        {
            DateTime dateTime = DateTime.Now;
            DateTime utcDateTime = DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
            TimeSpan istOffset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
            TimeZoneInfo istTimeZone = TimeZoneInfo.CreateCustomTimeZone("IST", istOffset, "IST", "IST");
            DateTime hiINDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);

            return DateTime.SpecifyKind(hiINDateTime, DateTimeKind.Utc);
        }
        private string BharatDateTimeNow()
        {
            DateTime dateTime = DateTime.Now;
            DateTime utcDateTime = DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
            TimeSpan istOffset = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
            TimeZoneInfo istTimeZone = TimeZoneInfo.CreateCustomTimeZone("IST", istOffset, "IST", "IST");
            DateTime hiINDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);

            // Format the date and time
            string formattedDateTime = hiINDateTime.ToString("ddd, dd MMM yyyy, HH:mm");

            return formattedDateTime;
        }
        public async Task<ServiceResponse> SendNotification(Notification notificationModel)
        {

            notificationModel.NotificationTime = BharatDateTimeNow();


            _context.Notification.Add(notificationModel);
            _context.SaveChanges();

            return new ServiceResponse() { IsSucceed = true };
        }
        public async Task<ServiceResponse> IsNotificationSeen(int notificationId, bool isStatus)
        {
            var isExist = _context.Notification.FirstOrDefault(d => d.NotificationId == notificationId);
            if (isExist != null)
            {
                isExist.NotificationStatus = isStatus;

                _context.Notification.Update(isExist);
                _context.SaveChanges();
                return new ServiceResponse { IsSucceed = true ,Message="Message Seen SuccessFully"};
            }
            else
            {
                return new ServiceResponse { IsSucceed = false, Message = "Message not exist " };

            }
        }
        public async Task<ServiceResponse> AddSMSTemplate(SMSTemplate smsTemplateModel)
        {

            _context.SMSTemplate.Add(smsTemplateModel);
            _context.SaveChanges();

            return new ServiceResponse() { IsSucceed = true };
        }

        public async Task<SMSTemplate> GetSMSTemplateById(string smsTemplateMasterId)
        {
            //return await _context.SMSTemplate.Where(d => d.Status== true && d.SMSTemplateMasterId == Convert.ToInt32(smsTemplateMasterId)).FirstOrDefaultAsync();
            return await _context.SMSTemplate.Where(d => d.SMSTemplateMasterId == Convert.ToInt32(smsTemplateMasterId)).FirstOrDefaultAsync();

        }
        public async Task<List<Notification>> GetNotification()
        {
            return await _context.Notification.OrderByDescending(d => d.NotificationId).ToListAsync();

        }


        public async Task<List<SMSTemplateModel>> GetSMSTemplate()
        {
            var templates = await _context.SMSTemplate.OrderByDescending(d => d.SMSTemplateMasterId).ToListAsync();

            return templates.Select(template => new SMSTemplateModel
            {
                SMSTemplateMasterId = template.SMSTemplateMasterId,
                SMSName = template.SMSName,
                Message = template.Message,
                EntityId = template.EntityId,
                TemplateId = template.TemplateId,
                IsStatus = template.Status,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt
            }).ToList();


        }
        public async Task<ServiceResponse> UpdateSMSTemplateById(SMSTemplate sMSTemplate)
        {
            var smsTemplateMasterRecord = _context.SMSTemplate.Where(d => d.SMSTemplateMasterId == sMSTemplate.SMSTemplateMasterId).FirstOrDefault();

            if (smsTemplateMasterRecord != null)
            {
                smsTemplateMasterRecord.SMSName = sMSTemplate.SMSName;
                smsTemplateMasterRecord.Message = sMSTemplate.Message;
                smsTemplateMasterRecord.EntityId = sMSTemplate.EntityId;
                smsTemplateMasterRecord.TemplateId = sMSTemplate.TemplateId;
                smsTemplateMasterRecord.UpdatedAt = BharatDateTime();
                smsTemplateMasterRecord.Status = sMSTemplate.Status;
                _context.SMSTemplate.Update(smsTemplateMasterRecord);
                _context.SaveChanges();
                return new ServiceResponse() { IsSucceed = true, Message = "SMS template Updated Successfully" + sMSTemplate.SMSName };

            }
            else
            {
                return new ServiceResponse() { IsSucceed = true, Message = "SMS template Not Found" + sMSTemplate.SMSName };
            }
        }



        public async Task<ServiceResponse> SaveSMS(SMSSentModel sMSSentModel)
        {

            var smssent = ConvertToSMSSent(sMSSentModel);

            _context.SMSSent.Add(smssent);
            _context.SaveChanges();

            return new ServiceResponse() { IsSucceed = true };

        }

        private SMSSent ConvertToSMSSent(SMSSentModel sMSSentModel)
        {
            return new SMSSent
            {
                SMSTemplateMasterId = sMSSentModel.SMSTemplateMasterId,
                Message = sMSSentModel.Message,
                Mobile = sMSSentModel.Mobile,
                RemarksFromGW = sMSSentModel.RemarksFromGW,
                SentToUserType = sMSSentModel.SentToUserType,
                Status = sMSSentModel.Status,
                CreatedAt = sMSSentModel.CreatedAt
            };
        }

        #region Send SMS to SectorOfficers
        public async Task<List<SOList>> GetSectorOfficerstoSendSMS(SendSMSModel sendSMSModel)
        {
            List<SOList> soEvents = new List<SOList>();
            var soList = _context.SectorOfficerMaster.Where(p => p.StateMasterId == sendSMSModel.StateMasterId && p.SoStatus).Select(d => new { d.SOMasterId, d.SoName, d.SoMobile }).ToList();
            if (soList.Count > 0)
            {
                if (sendSMSModel.EventId > 0)
                {
                    foreach (var so in soList)
                    {

                        List<int> boothIdPending = new List<int>(); List<int> boothIdTotal = new List<int>();
                        var boothList = await _context.BoothMaster.Where(d => d.StateMasterId == sendSMSModel.StateMasterId && d.BoothStatus == true && d.AssignedTo == so.SOMasterId.ToString()).Select(p => p.BoothMasterId).ToListAsync();
                        boothIdTotal.Add(boothList.Count);
                        foreach (var boothId in boothList)
                        {

                            switch (sendSMSModel.EventId)
                            {
                                case 1:
                                    var res = await _context.ElectionInfoMaster.Where(e => e.StateMasterId == sendSMSModel.StateMasterId && e.IsPartyDispatched == true && e.BoothMasterId == boothId).FirstOrDefaultAsync();
                                    if (res == null)
                                    {
                                        boothIdPending.Add(boothId);
                                    }
                                    break;

                                case 2:

                                    break;


                                default:

                                    break;
                            }




                        }


                        var soRecord = new SOList
                        {
                            SOMasterId = so.SOMasterId,
                            SoName = so.SoName,
                            SoMobile = so.SoMobile,
                            PendingBooths = boothIdPending.Count,
                            TotalBooths = boothIdTotal.Count
                        };
                        soEvents.Add(soRecord);

                    }
                    return soEvents;
                }
                else
                {
                    foreach (var so in soList)
                    {


                        var soRecord = new SOList
                        {
                            SOMasterId = so.SOMasterId,
                            SoName = so.SoName,
                            SoMobile = so.SoMobile

                        };
                        soEvents.Add(soRecord);

                    }
                    return soEvents;
                }
            }
            return soEvents;
        }



        //Task<List<SectorOfficerMaster>> INotificationRepository.GetSectorOfficerstoSendSMS(SendSMSModel sendSMSModel)
        //{
        //    throw new NotImplementedException();
        //}





        #endregion
    }
}


