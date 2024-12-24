using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHM_Survey_App_Backend.Models
{
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "E-posta adresi girilmelidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girilmelidir.")]
        public string UserMail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre girilmelidir.")]
        public string Password { get; set; }

        public List<int> StoreIds { get; set; } = new List<int>(); // Birden fazla StoreId'yi tutmak için
    }
}
