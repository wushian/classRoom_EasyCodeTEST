using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    public partial class TaiwanZipCode
    {
        public Int32 ID { get; set; }
        public Int32 Zip { get; set; }
        public String CityName { get; set; }
        public String Town { get; set; }
        public Int32 Sequence { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
 
