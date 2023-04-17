using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Settings
{
    public class SiteSettings
    {
        public AdminUserSeed AdminUserSeed { get; set; }
        public EmailSetting EmailSetting { get; set; }
        public SiteInfo SiteInfo { get; set; }
        public JwtSettings JwtSettings { get; set; }
    }


    public class AdminUserSeed
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class EmailSetting
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class SiteInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Favicon { get; set; }
        public string MetaDescriptionTag { get; set; }
    }
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string EncrypKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationMinutes { get; set; }

    }

}
