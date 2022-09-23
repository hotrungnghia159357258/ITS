using ProjectInternShip.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectInternShip.DBContexts
{
    public class data_sensor_DBcontexts : DbContext
    {
        //public Db
        public data_sensor_DBcontexts():base("con_string")
        {

        }
        public DbSet<data_sensor> data { get; set; }
        public DbSet<account> accounts { get; set; }
        public DbSet<param> parameters { get; set; }
    
    }

}
