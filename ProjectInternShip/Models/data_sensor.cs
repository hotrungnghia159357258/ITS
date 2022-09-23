using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProjectInternShip.Models
{
    [Table("data_table")]
    public class data_sensor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int data_sensor_ID { get; set; }

               
        public double sensor_value { get; set; }
        public int sensor_value_1 { get; set; } 
        
        public DateTime time { get; set; }

    }
}
