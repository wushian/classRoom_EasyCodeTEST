using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MyProject.Filters
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
    public class ExportColumnAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Äæ¦ìÅã¥Ü¶¶§Ç.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        public ExportColumnAttribute()
        {
        }
    }
}
 
