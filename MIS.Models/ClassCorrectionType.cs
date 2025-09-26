using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class ClassCorrectionType
    {
        public ClassCorrectionType()
        {
            TransactionClassCorrections = new HashSet<TransactionClassCorrection>();
        }

        public byte ClassCorrectionTypeId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; }
    }
}
