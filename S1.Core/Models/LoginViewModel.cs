using MES.Shared.Attribute;
using MES.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace S1.Core.Models
{
    public class LoginViewModel
    {
        [Display(ResourceType = typeof(SharedResources), Name = "Email")]
        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(SharedResources))]
        [CustomEmailAddress(
            ErrorMessageResourceName = "Email_Invalid",
            ErrorMessageResourceType = typeof(SharedResources))]
        public string Email { get; set; } = string.Empty;

        [Display(ResourceType = typeof(SharedResources), Name = "Password")]
        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(SharedResources))]
        [StringLength(12, MinimumLength = 6,
            ErrorMessageResourceName = "Password_Length_Invalid",
            ErrorMessageResourceType = typeof(SharedResources))]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(ResourceType = typeof(SharedResources), Name = "Remember_Me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
