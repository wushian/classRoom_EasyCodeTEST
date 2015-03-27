using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    public partial class student
    {
        public Guid id { get; set; }
        public String name { get; set; }
        public DateTime? birthDay { get; set; }
        public String gender { get; set; }
        public DateTime? createDate { get; set; }
        public String education { get; set; }
        public String address { get; set; }
        public String company { get; set; }
        public String telno { get; set; }
        public String cellno { get; set; }

    }
}
 
