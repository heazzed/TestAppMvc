using System;
using System.ComponentModel.DataAnnotations;

namespace AppMvc.Models
{
    public class RegisterModel
    {
        public string Name { get; set; }

        public string Age { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }

        public string Company { get; set; }

    }
}
