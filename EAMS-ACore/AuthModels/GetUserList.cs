namespace EAMS_ACore.AuthModels
{
    public class GetUserList
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }

    }

    public class UpdateLockoutUserViewModel
    {
        public string UserId { get; set; }
        public bool LockoutEnabled { get; set; }

    }
    public class UpdateLockoutUserInBulkViewModel
    {
        public int? StateMasterId { get; set; }
        public int? DistrictMasterId { get; set; }
        public int? ElectionTypeMasterId { get; set; }
        public int? AssemblyMasterId { get; set; }
        public int? FourthLevelHMasterId { get; set; }
        public bool LockoutEnabled { get; set; }

    }
}
