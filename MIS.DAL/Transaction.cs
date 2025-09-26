using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL   
{
    public class Transaction
    {
        public void Save(Models.Transaction Transaction)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.Transactions.Add(Transaction);
                dBContext.SaveChanges();
            }
        }

        public List<Models.Transaction> Update(List<Models.Transaction> Transactions)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.Transactions.AttachRange(Transactions);
                foreach (var item in Transactions)
                    dBContext.Entry(item).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
            return Transactions;
        }

        public void Save(List<Models.Transaction> Transaction)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.Transactions.AddRange(Transaction);
                dBContext.SaveChanges();
            }
        }

        public List<Models.Transaction> GetTransactionDetails(DateTime StartDate, DateTime EndDate, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate.AddDays(1).AddSeconds(-1) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (TollClasses == null || TollClasses.Contains(x.ActualTollClass)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                        .Include(x => x.TransactionType)
                        .Include(x => x.ActualTollClass)
                        .Include(x => x.AutomaticTollClass)
                        .Include(x => x.ManualTollClass)
                        .Include(x => x.Shift)
                        .Include(x => x.Currency)
                        .Include(x => x.DiscountType)
                        .Include(x => x.RegisteredUser)
                        .Include(x => x.RegisteredIdentifier)
                        .Include(x => x.RegisteredTollClass)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetTransactionDetails(DateTime ShiftDate, DateTime EndShiftDate, List<Models.Shift> Shifts, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => (x.ShiftDate >= ShiftDate && x.ShiftDate <= EndShiftDate) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (TollClasses == null || TollClasses.Contains(x.ActualTollClass)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                        .Include(x => x.TransactionType)
                        .Include(x => x.ActualTollClass)
                        .Include(x => x.AutomaticTollClass)
                        .Include(x => x.ManualTollClass)
                        .Include(x => x.Shift)
                        .Include(x => x.Currency)
                        .Include(x => x.DiscountType)
                        .Include(x => x.RegisteredUser)
                        .Include(x => x.RegisteredIdentifier)
                        .Include(x => x.RegisteredTollClass)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetForCashup(Models.CollectorCashup CollectorCashup)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.ShiftDate == CollectorCashup.ShiftDate && x.ShiftId == CollectorCashup.ShiftId && 
                                                    x.SystemUserId == CollectorCashup.SystemUserId &&
                                                    (!x.AllocatedToCollectorCashupId.HasValue | x.AllocatedToCollectorCashupId == CollectorCashup.CollectorCashupId)).ToList();
            }
        }

        public List<Models.Transaction> GetUnallocatedToCashup(DateTime ShiftDate, byte ShiftId)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.ShiftDate == ShiftDate 
                                                    && x.ShiftId == ShiftId 
                                                    && !x.AllocatedToCollectorCashupId.HasValue).ToList();
            }
        }

        public List<Models.Transaction> GetManualTollClass(DateTime StartDate, DateTime EndDate, List<Models.Lane> Lanes,
                List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate.AddDays(1).AddSeconds(-1) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (TollClasses == null || TollClasses.Contains(x.ManualTollClass)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                        .Include(x => x.TransactionType)
                        .Include(x => x.ManualTollClass)
                        .Include(x => x.Shift)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetExemptType(DateTime StartDate, DateTime EndDate, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers, List<Models.ExemptType> ExemptTypes)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate.AddDays(1).AddSeconds(-1) &&
                    (x.TransactionType.TransactionTypeId == (byte)Common.Enums.TransactionType.ExemptPassageTransaction) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))                    
                        .Include(x => x.TransactionType)
                        .Include(x => x.TransactionClassCorrections.Where(z => z.ClassCorrectionTypeId == (byte)Common.Enums.ClassCorrectionType.ExemptPassage 
                                        && (ExemptTypes == null || ExemptTypes.Contains(z.ExemptType))))
                            .ThenInclude(w => w.ExemptType)
                        .Include(x => x.Shift)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetExemptType(DateTime ShiftDate, DateTime EndShiftDate, List<Models.Shift> Shifts, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers, List<Models.ExemptType> ExemptTypes)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => (x.ShiftDate >= ShiftDate && x.ShiftDate <= EndShiftDate) &&
                    (x.TransactionType.TransactionTypeId == (byte)Common.Enums.TransactionType.ExemptPassageTransaction) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                        .Include(x => x.TransactionType)
                        .Include(x => x.TransactionClassCorrections.Where(z => z.ClassCorrectionTypeId == (byte)Common.Enums.ClassCorrectionType.ExemptPassage
                                        && (ExemptTypes == null || ExemptTypes.Contains(z.ExemptType))))
                            .ThenInclude(w => w.ExemptType)
                        .Include(x => x.Shift)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetManualTollClass(DateTime ShiftDate, DateTime EndShiftDate, List<Models.Shift> Shifts, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => (x.ShiftDate >= ShiftDate && x.ShiftDate <= EndShiftDate) &&
                (Shifts == null || Shifts.Contains(x.Shift)) &&
                (Lanes == null|| Lanes.Contains(x.Lane)) &&
                (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                (TollClasses == null|| TollClasses.Contains(x.ManualTollClass)) &&
                (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                    .Include(x => x.TransactionType)
                    .Include(x => x.ManualTollClass)
                    .Include(x => x.Shift)
                    .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetActualTollClass(DateTime StartDate, DateTime EndDate, List<Models.Lane> Lanes,
                List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate.AddDays(1).AddSeconds(-1) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (TollClasses == null || TollClasses.Contains(x.ActualTollClass)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                        .Include(x => x.TransactionType)
                        .Include(x => x.ActualTollClass)
                        .Include(x => x.Shift)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetActualTollClass(DateTime ShiftDate, DateTime EndShiftDate, List<Models.Shift> Shifts, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => (x.ShiftDate >= ShiftDate && x.ShiftDate <= EndShiftDate) &&
                                (Shifts == null || Shifts.Contains(x.Shift)) &&
                                (Lanes == null || Lanes.Contains(x.Lane)) &&
                                (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                                (TollClasses == null || TollClasses.Contains(x.ActualTollClass)) &&
                                (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                                    .Include(x => x.TransactionType)
                                    .Include(x => x.ActualTollClass)
                                    .Include(x => x.Shift)
                                    .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetAutomaticTollClass(DateTime StartDate, DateTime EndDate, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate.AddDays(1).AddSeconds(-1) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (TollClasses == null || TollClasses.Contains(x.AutomaticTollClass)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                        .Include(x => x.TransactionType)
                        .Include(x => x.AutomaticTollClass)
                        .Include(x => x.Shift)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetAutomaticTollClass(DateTime ShiftDate, DateTime EndShiftDate, List<Models.Shift> Shifts, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => (x.ShiftDate >= ShiftDate && x.ShiftDate <= EndShiftDate) &&
                                (Shifts == null || Shifts.Contains(x.Shift)) &&
                                (Lanes == null || Lanes.Contains(x.Lane)) &&
                                (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                                (TollClasses == null || TollClasses.Contains(x.AutomaticTollClass)) &&
                                (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                                    .Include(x => x.TransactionType)
                                    .Include(x => x.AutomaticTollClass)
                                    .Include(x => x.Shift)
                                    .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetManualRevenue(DateTime StartDate, DateTime EndDate, List<Models.Lane> Lanes,
                List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate.AddDays(1).AddSeconds(-1) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (TollClasses == null || TollClasses.Contains(x.ManualTollClass)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                        .Include(x => x.TransactionType)
                        .Include(x => x.ManualTollClass)
                        .Include(x => x.Shift)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetManualRevenue(DateTime ShiftDate, DateTime EndShiftDate, List<Models.Shift> Shifts, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => (x.ShiftDate >= ShiftDate && x.ShiftDate <= EndShiftDate) &&
                (Shifts == null || Shifts.Contains(x.Shift)) &&
                (Lanes == null || Lanes.Contains(x.Lane)) &&
                (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                (TollClasses == null || TollClasses.Contains(x.ManualTollClass)) &&
                (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                    .Include(x => x.TransactionType)
                    .Include(x => x.ManualTollClass)
                    .Include(x => x.Shift)
                    .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetActualRevenue(DateTime StartDate, DateTime EndDate, List<Models.Lane> Lanes,
                List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate.AddDays(1).AddSeconds(-1) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (TollClasses == null || TollClasses.Contains(x.ActualTollClass)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                        .Include(x => x.TransactionType)
                        .Include(x => x.ActualTollClass)
                            
                        .Include(x => x.Shift)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetActualRevenue(DateTime ShiftDate, DateTime EndShiftDate, List<Models.Shift> Shifts, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => (x.ShiftDate >= ShiftDate && x.ShiftDate <= EndShiftDate) &&
                                (Shifts == null || Shifts.Contains(x.Shift)) &&
                                (Lanes == null || Lanes.Contains(x.Lane)) &&
                                (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                                (TollClasses == null || TollClasses.Contains(x.ActualTollClass)) &&
                                (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                                    .Include(x => x.TransactionType)
                                    .Include(x => x.ActualTollClass)
                                    .Include(x => x.Shift)
                                    .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetAutomaticRevenue(DateTime StartDate, DateTime EndDate, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate.AddDays(1).AddSeconds(-1) &&
                    (Lanes == null || Lanes.Contains(x.Lane)) &&
                    (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                    (TollClasses == null || TollClasses.Contains(x.AutomaticTollClass)) &&
                    (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                        .Include(x => x.TransactionType)
                        .Include(x => x.AutomaticTollClass)
                        .Include(x => x.Shift)
                        .Include(x => x.SystemUser).ToList();
            }
        }

        public List<Models.Transaction> GetAutomaticRevenue(DateTime ShiftDate, DateTime EndShiftDate, List<Models.Shift> Shifts, List<Models.Lane> Lanes,
        List<Models.TransactionType> TransactionTypes, List<Models.TollClass> TollClasses, List<Models.SystemUser> SystemUsers)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                return dBContext.Transactions.Where(x => (x.ShiftDate >= ShiftDate && x.ShiftDate <= EndShiftDate) &&
                                (Shifts == null || Shifts.Contains(x.Shift)) &&
                                (Lanes == null || Lanes.Contains(x.Lane)) &&
                                (TransactionTypes == null || TransactionTypes.Contains(x.TransactionType)) &&
                                (TollClasses == null || TollClasses.Contains(x.AutomaticTollClass)) &&
                                (SystemUsers == null || SystemUsers.Contains(x.SystemUser)))
                                    .Include(x => x.TransactionType)
                                    .Include(x => x.AutomaticTollClass)
                                    .Include(x => x.Shift)
                                    .Include(x => x.SystemUser).ToList();
            }
        }
    }
}
