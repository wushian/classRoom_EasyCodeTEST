using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    public partial class calendar_detail
    {
        public Guid id { get; set; }
        public String subject { get; set; }
        public DateTime? startDateTime { get; set; }
        public DateTime? endDateTime { get; set; }
        public Int32? feeStaff { get; set; }
        public Int32? feeNonStaff { get; set; }
        public String contact { get; set; }
        public String contactTel { get; set; }
        public String describe { get; set; }

    }
}
 
