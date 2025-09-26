using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RegisteredUserAddress
    {
        public long RegisteredUserId { get; set; }
        public byte AddressTypeId { get; set; }
        public string Address { get; set; }
        public int? CountryId { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? SuburbId { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual AddressType AddressType { get; set; }
        public virtual Country Country { get; set; }
        public virtual Province Province { get; set; }
        public virtual RegisteredUser RegisteredUser { get; set; }
        public virtual Suburb Suburb { get; set; }
    }
}
