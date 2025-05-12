using EAMS_ACore.Models;
using EAMS_ACore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAMS_ACore.SMSModels;

namespace EAMS_DAL.DBContext
{
    public class SMSContext : DbContext
    {
        public SMSContext(DbContextOptions<SMSContext> options)
       : base(options)
        {
            
        }
        public virtual DbSet<SMSNumbers> SMSNumbers { get; set; }
        public virtual DbSet<SMSConfiguration> SMSConfiguration { get; set; }
    }
}
