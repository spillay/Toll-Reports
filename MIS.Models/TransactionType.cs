using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class TransactionType
    {
        public TransactionType()
        {
            Transactions = new HashSet<Transaction>();
        }

        public byte TransactionTypeId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
