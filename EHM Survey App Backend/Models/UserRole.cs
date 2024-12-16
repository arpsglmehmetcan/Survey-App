using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHM_Survey_App_Backend.Models
{
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı girilmelidir.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre girilmelidir.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Store")]
        public int StoreId { get; set; }
    }
}
