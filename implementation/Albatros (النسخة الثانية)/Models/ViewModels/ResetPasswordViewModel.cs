using System.ComponentModel.DataAnnotations;

namespace Albatros.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "يجب أن تكون كلمة المرور 6 أحرف على الأقل")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "كلمتا المرور غير متطابقتين")]
        public string ConfirmPassword { get; set; }
    }
}
