using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectInternShip.Models
{
    [Table("user_table")]
    public class account
    {
        [Key]
        [StringLength(50)]
        public string Username { get; set; }
        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        public Boolean IsAdmin { get; set; }
    }
}
