using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.DAL
{
    public class TransactionVehicleCharacteristic
    {
        public void Save(Models.TransactionVehicleCharacteristic TransactionVehicleCharacteristic)
        {
            using (Models.ApplicationDbContext dBContext = new Models.ApplicationDbContext())
            {
                dBContext.TransactionVehicleCharacteristics.Add(TransactionVehicleCharacteristic);
                dBContext.SaveChanges();
            }
        }
    }
}
