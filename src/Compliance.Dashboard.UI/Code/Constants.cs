using System.Configuration;
using System.Web;

namespace Compliance.Dashboard.UI.Code
{
    public static class Constants
    {

        #region Email
        internal static readonly string EmailHost = ConfigurationManager.AppSettings["EmailHost"] ?? string.Empty;
        internal static readonly int EmailPort = "EmailPort".GetInt(80);
        internal static readonly string EmailFrom = ConfigurationManager.AppSettings["EmailFrom"] ?? string.Empty;
        internal static readonly string EmailTo = ConfigurationManager.AppSettings["EmailTo"] ?? string.Empty;
        internal static readonly string QAEmail = ConfigurationManager.AppSettings["QAEmail"] ?? "samrina.aimviz@gmail.com";
        internal static readonly string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"] ?? string.Empty;
        internal static readonly bool EmailEnableSsl = "EmailEnableSsl".GetBool(false);
        internal static readonly int EmailTimeout = "EmailTimeout".GetInt(1000);
        #endregion

        #region General

        public static readonly string PostCardFilePath = HttpContext.Current.Server.MapPath("~/") + "Upload/PostCards";
        public static readonly string SiteMembershipFilePath = HttpContext.Current.Server.MapPath("~/") + "Upload/MembshipAgreements";
        public static readonly string DateFormat = ConfigurationManager.AppSettings["DateFormat"] ?? "MMMM dd, yyyy";
        public static readonly string SocialReturnUrl = ConfigurationManager.AppSettings["SocialReturnUrl"] ?? string.Empty;
        public static readonly string MasterPassword = ".netgo";
        //public static readonly string PasswordPattern = @"/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]/";
        public static readonly string PasswordPattern = @"/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d$@$!%*?&]/";
        public static readonly string EmailPattern = @"/^[-a-z0-9~!$%^&*_=+}{\'?]+(\.[-a-z0-9~!$%^&*_=+}{\'?]+)*@([a-z0-9_][-a-z0-9_]*(\.[-a-z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-z][a-z])|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$/";
        public static int MinPostCardWidth = 550;
        public static int MinPostCardHeight = 325;

        #endregion

        #region Social Media
        public static readonly string FbAppId = ConfigurationManager.AppSettings["FbAppId"] ?? string.Empty;
        internal static readonly string FbAppSecret = ConfigurationManager.AppSettings["FbAppSecret"] ?? string.Empty;
        public static readonly string GoogleeClientId = ConfigurationManager.AppSettings["GoogleeClientId"] ?? string.Empty;
        internal static readonly string GoogleeClientSecret = ConfigurationManager.AppSettings["GoogleeClientSecret"] ?? string.Empty;
        #endregion
    }
}
