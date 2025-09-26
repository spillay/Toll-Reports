using System;

namespace Common
{
    public class Enums
    {

        public enum AVCIncident 
        { 
            Axle1_Failure = 1,
            Axle1_Ok = 2,
            Axle2_Failure = 3,
            Axle2_Ok = 4,
            Loop1_Failure = 5,
            Loop1_Ok = 6,
            Loop2_Failure = 7,
            Loop2_Ok = 8,
            Height1_Failure = 9,
            Height1_Ok = 10,
            Height2_Failure = 11,
            Height2_Ok = 12,
            Height3_Failure = 13,
            Height3_Ok = 14,
            AVCTCCComs_Failure = 15,
            AVCTCCComs_Ok = 16,
            AVCIOComs_Failure = 17,
            AVCIOComs_Ok = 18,
            AVCIOData_Failure = 19,
            AVCIOData_Ok = 20
        }

        public enum HeightState : short
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        public enum AVCTCCMessageType : short
        {
            FirstAxleMessage = 0,
            IncidentMesssage = 1,
            VehicleDataMesssage = 2,
            AliveMessage = 3
        }

        public enum CashupShortagePaymentMethod : short
        {
            Cash = 1,
            Salary = 2,
            BankDeposit = 3,
            CashDeclarationOffset = 4
        }

        public enum AllocatedTo : short
        {
            Collector = 1,
            Equipment = 2,
            RoadUser = 3,
            Operator = 4
        }

        public enum ClassCorrectionType : short
        {
            ClassMismatch = 1,
            ViolationPassage = 2,   
            ExemptPassage = 3,
            Timeout = 5,
            DiscountedPassage = 4
        }

        public enum Currency : short
        {
            Metical = 1,
            SouthAfricaRand = 2,
            UnitedStatesDollar = 3
        }

        public enum Role : short
        {
            Collector = 1,
            Supervisor = 2,
            PlazaManager = 3,
            SystemAdministrator = 4,
            Technician = 5,
            Reporter = 6,
            Banking = 7
        }

        public enum ShiftStatus : short
        {
            Open = 1,
            AwaitingCashUp = 2,
            Closed = 3
        }

        public enum LaneDefaultValue : int
        {
            PrePostShiftMinutes = 1,
            Currency = 2,
            UseExchangeRate = 3,
            ChangeInLocalCurrency = 4,
            AllowLaneTopup = 5,
            AVCClassWaitTime = 6,
            ReceiptPrintNumbers = 7,
            ReceiptHeader = 8,
            ReceiptLineFeedBeforeCut = 9,
            PassbackActive = 10,
            PassbackSeconds = 11
        }

        public enum ShiftSelected : short
        {
            Current = 1,
            Previous = 2,
            Next = 3
        }

        public enum DiscountType : short
        {
            Nominal = 1,
            FrequentUser = 2,
            ExemptPassage = 3,
            PublicTransport = 4,
            Tractor = 5,
            MonthlyLocalUser = 6
        }

        public enum CalculationMethod : short
        {
            Percentage = 1,
            MonthTripCountPercentage = 2,
            MonthTripCountAmount = 3,
            CappedAmountPercentage = 4,
            CappedAmountAmount = 5,
            Amount = 6
        }

        public enum TransactionType
        {
            CashPassageTransaction = 1,
            ForeignCashPassageTransaction = 2,
            PrepaidPassageTransaction = 3,
            PostpaidPassageTransaction = 4,
            ExemptPassageTransaction = 5,
            PrepaidTopUp = 6,
            ViolationPassage = 7
        }

        public enum UFDMessage
        {
            ClosedLane = 1,
            OpenLane = 2,
            CashPaid = 3,
            Exempt = 4
        }

        public enum Report
        {
            TrafficCalendarHourlyActual,
            TrafficCalendarHourlyCollector,
            TrafficCalendarHourlyAVC,
            TrafficCalendarDailyActual,
            TrafficCalendarDailyCollector,
            TrafficCalendarDailyAVC,
            TrafficCalendarMonthlyActual,
            TrafficCalendarMonthlyCollector,
            TrafficCalendarMonthlyAVC,
            TrafficShiftHourlyActual,
            TrafficShiftHourlyCollector,
            TrafficShiftHourlyAVC,
            TrafficShiftDailyActual,
            TrafficShiftDailyCollector,
            TrafficShiftDailyAVC,
            TrafficShiftMonthlyActual,
            TrafficShiftMonthlyCollector,
            TrafficShiftMonthlyAVC,
            RevenueCalendarHourlyActual,
            RevenueCalendarHourlyCollector,
            RevenueCalendarHourlyAVC,
            RevenueCalendarDailyActual,
            RevenueCalendarDailyCollector,
            RevenueCalendarDailyAVC,
            RevenueCalendarMonthlyActual,
            RevenueCalendarMonthlyCollector,
            RevenueCalendarMonthlyAVC,
            RevenueShiftHourlyActual,
            RevenueShiftHourlyCollector,
            RevenueShiftHourlyAVC,
            RevenueShiftDailyActual,
            RevenueShiftDailyCollector,
            RevenueShiftDailyAVC,
            RevenueShiftMonthlyActual,
            RevenueShiftMonthlyCollector,
            RevenueShiftMonthlyAVC,
            CompCalendarHourlyActual,
            CompCalendarHourlyCollector,
            CompCalendarHourlyAVC,
            CompCalendarDailyActual,
            CompCalendarDailyCollector,
            CompCalendarDailyAVC,
            CompCalendarMonthlyActual,
            CompCalendarMonthlyCollector,
            CompCalendarMonthlyAVC,
            CompShiftHourlyActual,
            CompShiftHourlyCollector,
            CompShiftHourlyAVC,
            CompShiftDailyActual,
            CompShiftDailyCollector,
            CompShiftDailyAVC,
            CompShiftMonthlyActual,
            CompShiftMonthlyCollector,
            CompShiftMonthlyAVC,
            ExemptTypeCalendarHourly,
            ExemptTypeCalendarDaily,
            ExemptTypeCalendarMonthly,
            ExemptTypeShiftHourly,
            ExemptTypeShiftDaily,
            ExemptTypeShiftMonthly
        }
    }
}
