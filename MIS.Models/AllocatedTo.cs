using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class AllocatedTo
    {
        public AllocatedTo()
        {
            TransactionClassCorrections = new HashSet<TransactionClassCorrection>();
        }

        public byte AllocatedToId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; }
    }
}
