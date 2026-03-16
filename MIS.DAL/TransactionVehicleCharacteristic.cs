using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollReportingSystem.Data;

namespace MIS.DAL
{
    public class TransactionVehicleCharacteristic
    {
        public void Save(Models.TransactionVehicleCharacteristic TransactionVehicleCharacteristic)
        {
            using (ApplicationDbContext dBContext = new ApplicationDbContext())
            {
                dBContext.TransactionVehicleCharacteristics.Add(TransactionVehicleCharacteristic);
                dBContext.SaveChanges();
            }
        }
    }
}
