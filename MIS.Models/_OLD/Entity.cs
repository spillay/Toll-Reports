using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Entity
    {
        public int EntityId { get; set; }
        public string EntityCode { get; set; }
        public string Apikey { get; set; }
        public string Description { get; set; }
    }
}
