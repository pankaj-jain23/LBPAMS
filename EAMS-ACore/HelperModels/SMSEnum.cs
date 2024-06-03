namespace EAMS_ACore.HelperModels
{
    public enum SMSEnum
    {
        //[StringValue("1301157492438182299")]
        //EntityId,

        //[StringValue("pbgovt.sms")]
        //UserName,

        //[StringValue("wbx3actu")]
        //Password,

        //[StringValue("PBGOVT")]
        //SenderId,

        [StringValue("1101445180000039470")]
        EntityId,

        [StringValue("ceopunjab.otp")]
        UserName,

        [StringValue("hvrc4pVA")]
        Password,

        [StringValue("CEOPUN")]
        SenderId,

        [StringValue("7")]
        OTP,

        [StringValue("Message Accepted")]
        MessageAccepted
    }

    public class StringValueAttribute : Attribute
    {
        public string Value { get; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }

    public static class SMSEnumExtensions
    {
        public static string GetStringValue(this SMSEnum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = (StringValueAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(StringValueAttribute));
            return attribute?.Value;
        }
    }
}
