using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
//using Microsoft.Web.Infrastructure;

namespace SocialMonitorCloud.Models
{
    public class Account
    {
        public Account()
        {
        }
        public Account(Guid id)
        {
            this.LocalAccountId = id;
        }
        
        public int AccountId { get; set; }
        public Guid LocalAccountId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<KeywordModel> keywords { get; set; }
        //public string Password { get; set; }
        public bool IsActive { get; set; }
        public string companyName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string apartmentNum { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName + ", " + LocalAccountId.ToString() + ", " + IsActive;
        }
        public void getKeywords()
        {
            keywords = AccountManager.GetUserKeywords(LocalAccountId);
        }
        public string printAccountDataHTML()
        {
            string s = "";

            s += "Company: " + companyName;
            s += "\nPhone Number: " + phoneNumber;
            s += "\nEmail: " + email;
            s += "\nAddress 1: " + address1;

            return s;
        }

        public void FixNullParams()
        {
            EmptyNull(FirstName);
            EmptyNull(LastName);
            EmptyNull(companyName);
            EmptyNull(phoneNumber);
            EmptyNull(email);
            EmptyNull(address1);
            EmptyNull(address2);
            EmptyNull(address3);
            EmptyNull(city);
            EmptyNull(state);
            EmptyNull(zip);
            EmptyNull(country);
            EmptyNull(apartmentNum);
        }
        private void EmptyNull(String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                str = String.Empty;
            }
        }
    }
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base(ServerConfiguration.GetConnectionString())
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
