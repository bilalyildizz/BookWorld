using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookWorld.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string  Surname{ get; set; }
        public DateTime? BirthDay { get; set; }
        [NotMapped]
        public string NameSurname { get { return Name + " " + Surname; } }

    }
}
