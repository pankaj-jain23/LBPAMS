﻿namespace EAMS_ACore.AuthModels
{
    public class GetUserList
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }

    }
}
