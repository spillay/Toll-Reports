using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class ExemptType
    {
        public ExemptType()
        {
            TransactionClassCorrections = new HashSet<TransactionClassCorrection>();
        }

        public byte ExemptTypeId { get; set; }
        public string ExemptTypeDescription { get; set; }
        public byte DisplayOrder { get; set; }

        public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; }
    }
}
