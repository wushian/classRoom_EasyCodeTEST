using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProject.Helpers
{
    public class ExportColumnObject
    {
        public Guid ID { get; set; }

        /// <summary>
        /// ����X�W��.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// ��춶��.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// ���]�ݩʡ^�W��.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName { get; set; }

        public ExportColumnObject()
        {
            this.ID = Guid.NewGuid();
        }

    }
}
 
