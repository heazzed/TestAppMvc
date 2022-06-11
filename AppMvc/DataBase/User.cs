using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppMvc.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }

        public string Company { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public List<Order> Orders { get; set; }
    }
}
