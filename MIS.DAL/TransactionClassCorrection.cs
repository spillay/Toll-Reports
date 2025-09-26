using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Models;

namespace MIS.DAL
{
    public class TransactionClassCorrection
    {
        public void Save(Models.TransactionClassCorrection TransactionClassCorrection)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.TransactionClassCorrections.Add(TransactionClassCorrection);
                dBContext.SaveChanges();
            }
        }

        public void Update(Models.TransactionClassCorrection TransactionClassCorrection)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.TransactionClassCorrections.Attach(TransactionClassCorrection);
                dBContext.Entry(TransactionClassCorrection).State = EntityState.Modified;
                dBContext.SaveChanges();
            }
        }

        public List<Models.TransactionClassCorrection> Update(List<Models.TransactionClassCorrection> TransactionClassCorrection)
        {
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                dBContext.TransactionClassCorrections.AttachRange(TransactionClassCorrection);
                foreach(var item in TransactionClassCorrection)
                    dBContext.Entry(item).State = EntityState.Modified;
                dBContext.SaveChanges();
            }

            return TransactionClassCorrection;
        }

        public List<Models.TransactionClassCorrection> Get(long SystemUserId, DateTime ShiftDate, byte ShiftId,bool ClassCorrected)
        {
            List<Models.TransactionClassCorrection> classCorrections = new List<Models.TransactionClassCorrection>(); 
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                classCorrections = dBContext.TransactionClassCorrections
                    .Where(z => z.Transaction.ShiftDate == ShiftDate 
                    && z.Transaction.ShiftId == ShiftId
                    && z.ClassCorrected == ClassCorrected 
                    && z.Transaction.SystemUserId == SystemUserId).ToList();
            }
            return classCorrections;
        }

        public List<Models.TransactionClassCorrection> Get(DateTime ShiftDate,  bool ClassCorrected)
        {
            List<Models.TransactionClassCorrection> classCorrections = new List<Models.TransactionClassCorrection>();
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                classCorrections = dBContext.TransactionClassCorrections.Include(x => x.ClassCorrectionType).Include(x => x.ClassCorrected).Include(x => x.AllocatedTo).
                    Include(x => x.Transaction).Include(v => v.Transaction.SystemUser).Include(v => v.Transaction.ManualTollClass).
                    Include(v => v.Transaction.AutomaticTollClass).
                    Include(v => v.Transaction.RegisteredTollClass)
                    .Where(z => z.Transaction.ShiftDate == ShiftDate && z.ClassCorrected == ClassCorrected).ToList();
            }
            return classCorrections;
        }

        public List<Models.TransactionClassCorrection> Get(DateTime ShiftDate, byte? ShiftId, byte? TollPlazaId, byte? VirtualPlazaId, byte? LaneId, byte? ClassCorrectionTypeId, bool ClassCorrected)
        {
            List<Models.TransactionClassCorrection> classCorrections = new List<Models.TransactionClassCorrection>();
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                classCorrections = dBContext.TransactionClassCorrections.Include(x => x.ClassCorrectionType)
                    .Include(x => x.AllocatedTo)
                    .Include(x => x.CorrectedClass)
                    .Include(x => x.AllocatedTo)
                    .Include(x => x.Transaction)
                        .ThenInclude(v => v.SystemUser)
                    .Include(x => x.Transaction)
                        .ThenInclude(v => v.ManualTollClass)
                    .Include(x => x.Transaction)
                        .ThenInclude(v => v.AutomaticTollClass)
                    .Include(x => x.Transaction)
                        .ThenInclude(v => v.RegisteredTollClass)
                    .Include(x => x.Lane)
                        .ThenInclude(x => x.VirtualPlaza)
                        .ThenInclude(x => x.TollPlaza)
                    .Include(x => x.Lane)
                        .ThenInclude(x => x.LaneCameras)
                    .Where(z => z.Transaction.ShiftDate == ShiftDate && z.Transaction.ShiftId == (ShiftId.HasValue ? ShiftId : z.Transaction.ShiftId)
                                && z.LaneId == (LaneId.HasValue ? LaneId : z.Transaction.LaneId)
                                && z.Lane.VirtualPlazaId == (VirtualPlazaId.HasValue ? VirtualPlazaId : z.Lane.VirtualPlazaId)
                                && z.Lane.VirtualPlaza.TollPlazaId == (TollPlazaId.HasValue ? TollPlazaId : z.Lane.VirtualPlaza.TollPlazaId)
                                && z.ClassCorrectionTypeId == (ClassCorrectionTypeId.HasValue ? ClassCorrectionTypeId : z.ClassCorrectionTypeId)
                                && z.ClassCorrected == ClassCorrected
                    ).ToList();
            }
            return classCorrections;
        }

        public List<Models.TransactionClassCorrection> GetForCashup(Models.CollectorCashup CollectorCashup)
        {
            List<Models.TransactionClassCorrection> classCorrections = new List<Models.TransactionClassCorrection>();
            using (Models.MISDBContext dBContext = new Models.MISDBContext())
            {
                classCorrections = dBContext.TransactionClassCorrections
                    .Include(z => z.Transaction)
                    .Where(z => z.Transaction.ShiftDate == CollectorCashup.ShiftDate && z.Transaction.ShiftId == CollectorCashup.ShiftId && 
                            z.Transaction.SystemUserId == CollectorCashup.SystemUserId && z.ClassCorrected && 
                            (!z.AllocatedToCollectorCashupId.HasValue | z.AllocatedToCollectorCashupId == CollectorCashup.CollectorCashupId)).ToList();
            }
            return classCorrections;
        }
    }
}
