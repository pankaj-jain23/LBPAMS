﻿using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace EAMS_ACore.NotificationModels
{

    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string UserType { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        
        public string NotificationTime { get; set; }
        public bool NotificationStatus {  get; set; }
    }

}
