using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    public partial class teacher
    {
        public Guid id { get; set; }
        public String name { get; set; }
        public String gender { get; set; }
        public DateTime createDate { get; set; }
        public DateTime? birthDay { get; set; }
        public String address { get; set; }
        public String officeAddress { get; set; }
        public String telPhone { get; set; }
        public String cellPhone { get; set; }
        public String job { get; set; }
        public String ability { get; set; }
        public String educational { get; set; }

    }
}
 
