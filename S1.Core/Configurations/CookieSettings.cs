namespace S1.Core.Configurations
{
    public class CookieSettings
    {
        public string LoginPath { get; set; } = "/Account/Login";
        public string LogoutPath { get; set; } = "/Account/Logout";
        public int DefaultExpireSeconds { get; set; } = 14400;
        public int RememberMeExpireDays { get; set; } = 14;
        public bool SlidingExpiration { get; set; } = true;
    }
}
