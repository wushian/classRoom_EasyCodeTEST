using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    public partial class SystemUser
    {
        public Guid ID { get; set; }
        public String Name { get; set; }
        public String Account { get; set; }
        public String Password { get; set; }
        public String Salt { get; set; }
        public String Email { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
 
