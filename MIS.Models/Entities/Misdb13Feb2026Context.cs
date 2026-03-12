using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MIS.Models.Entities;

public partial class Misdb13Feb2026Context : DbContext
{
    public Misdb13Feb2026Context(DbContextOptions<Misdb13Feb2026Context> options)
        : base(options)
    {
    }

    public virtual DbSet<AllocatedTo> AllocatedTos { get; set; }

    public virtual DbSet<CalculationMethod> CalculationMethods { get; set; }

    public virtual DbSet<Camera> Cameras { get; set; }

    public virtual DbSet<CashupShortagePaymentMethod> CashupShortagePaymentMethods { get; set; }

    public virtual DbSet<ClassCorrectionType> ClassCorrectionTypes { get; set; }

    public virtual DbSet<CollectorCashDeclaration> CollectorCashDeclarations { get; set; }

    public virtual DbSet<CollectorCashDeclarationDenomination> CollectorCashDeclarationDenominations { get; set; }

    public virtual DbSet<CollectorCashup> CollectorCashups { get; set; }

    public virtual DbSet<CollectorCashupCashSurplusAllocatedToDiscrepancy> CollectorCashupCashSurplusAllocatedToDiscrepancies { get; set; }

    public virtual DbSet<CollectorCashupReprocess> CollectorCashupReprocesses { get; set; }

    public virtual DbSet<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; }

    public virtual DbSet<CollectorShiftAssignment> CollectorShiftAssignments { get; set; }

    public virtual DbSet<ControlCentre> ControlCentres { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Denomination> Denominations { get; set; }

    public virtual DbSet<DeploymentCleanupList> DeploymentCleanupLists { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountStructure> DiscountStructures { get; set; }

    public virtual DbSet<DiscountStructureDetail> DiscountStructureDetails { get; set; }

    public virtual DbSet<DiscountType> DiscountTypes { get; set; }

    public virtual DbSet<Exempt> Exempts { get; set; }

    public virtual DbSet<ExemptType> ExemptTypes { get; set; }

    public virtual DbSet<IdentifierStat> IdentifierStats { get; set; }

    public virtual DbSet<IdentifierType> IdentifierTypes { get; set; }

    public virtual DbSet<IdentifierTypeStat> IdentifierTypeStats { get; set; }

    public virtual DbSet<ImageQueueMessage> ImageQueueMessages { get; set; }

    public virtual DbSet<Incident> Incidents { get; set; }

    public virtual DbSet<Lane> Lanes { get; set; }

    public virtual DbSet<LaneCamera> LaneCameras { get; set; }

    public virtual DbSet<LaneDefaultValue> LaneDefaultValues { get; set; }

    public virtual DbSet<LaneDisplayMessage> LaneDisplayMessages { get; set; }

    public virtual DbSet<LaneHourlyAudit> LaneHourlyAudits { get; set; }

    public virtual DbSet<LaneIncident> LaneIncidents { get; set; }

    public virtual DbSet<LaneLastNo> LaneLastNos { get; set; }

    public virtual DbSet<LaneLastTransactionImage> LaneLastTransactionImages { get; set; }

    public virtual DbSet<LaneLoginLogout> LaneLoginLogouts { get; set; }

    public virtual DbSet<LaneScadaStatus> LaneScadaStatuses { get; set; }

    public virtual DbSet<ListAccountHolder> ListAccountHolders { get; set; }

    public virtual DbSet<ListIdentifier> ListIdentifiers { get; set; }

    public virtual DbSet<OtherIncome> OtherIncomes { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Reconciliation> Reconciliations { get; set; }

    public virtual DbSet<RegisterUserAccountMovement> RegisterUserAccountMovements { get; set; }

    public virtual DbSet<RegisteredUser> RegisteredUsers { get; set; }

    public virtual DbSet<RegisteredUserFee> RegisteredUserFees { get; set; }

    public virtual DbSet<RegisteredUserIdentifier> RegisteredUserIdentifiers { get; set; }

    public virtual DbSet<RegisteredUserTopUp> RegisteredUserTopUps { get; set; }

    public virtual DbSet<ReguserStat> ReguserStats { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<ShiftDaySummary> ShiftDaySummaries { get; set; }

    public virtual DbSet<ShiftStatus> ShiftStatuses { get; set; }

    public virtual DbSet<SupervisorCashup> SupervisorCashups { get; set; }

    public virtual DbSet<SystemUser> SystemUsers { get; set; }

    public virtual DbSet<SystemUserRole> SystemUserRoles { get; set; }

    public virtual DbSet<TariffPlan> TariffPlans { get; set; }

    public virtual DbSet<TariffPlanDetail> TariffPlanDetails { get; set; }

    public virtual DbSet<TheoreticalIncome> TheoreticalIncomes { get; set; }

    public virtual DbSet<TollClass> TollClasses { get; set; }

    public virtual DbSet<TollClass1> TollClasses1 { get; set; }

    public virtual DbSet<TollClassSpecification> TollClassSpecifications { get; set; }

    public virtual DbSet<TollPlaza> TollPlazas { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Transaction1> Transactions1 { get; set; }

    public virtual DbSet<TransactionClassCorrection> TransactionClassCorrections { get; set; }

    public virtual DbSet<TransactionCreditNote> TransactionCreditNotes { get; set; }

    public virtual DbSet<TransactionImage> TransactionImages { get; set; }

    public virtual DbSet<TransactionMissing> TransactionMissings { get; set; }

    public virtual DbSet<TransactionMissingDetail> TransactionMissingDetails { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<TransactionVehicleCharacteristic> TransactionVehicleCharacteristics { get; set; }

    public virtual DbSet<Ufdmessage> Ufdmessages { get; set; }

    public virtual DbSet<VirtualPlaza> VirtualPlazas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AllocatedTo>(entity =>
        {
            entity.ToTable("AllocatedTo");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CalculationMethod>(entity =>
        {
            entity.ToTable("CalculationMethod");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Camera>(entity =>
        {
            entity.ToTable("Camera");

            entity.Property(e => e.CameraId).ValueGeneratedOnAdd();
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CashupShortagePaymentMethod>(entity =>
        {
            entity.ToTable("CashupShortagePaymentMethod");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ClassCorrectionType>(entity =>
        {
            entity.ToTable("ClassCorrectionType");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CollectorCashDeclaration>(entity =>
        {
            entity.ToTable("CollectorCashDeclaration");

            entity.Property(e => e.DeclaredAt).HasColumnType("datetime");

            entity.HasOne(d => d.AllocatedToCollectorCashup).WithMany(p => p.CollectorCashDeclarations)
                .HasForeignKey(d => d.AllocatedToCollectorCashupId)
                .HasConstraintName("FK_CollectorCashDeclaration_CollectorCashup");

            entity.HasOne(d => d.Shift).WithMany(p => p.CollectorCashDeclarations)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashDeclaration_Shift");

            entity.HasOne(d => d.SystemUser).WithMany(p => p.CollectorCashDeclarationSystemUsers)
                .HasForeignKey(d => d.SystemUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashDeclaration_SystemUser");

            entity.HasOne(d => d.VerifiedBy).WithMany(p => p.CollectorCashDeclarationVerifiedBies)
                .HasForeignKey(d => d.VerifiedById)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashDeclaration_SystemUser1");
        });

        modelBuilder.Entity<CollectorCashDeclarationDenomination>(entity =>
        {
            entity.HasKey(e => new { e.CollectorCashDeclarationId, e.DenominationId });

            entity.ToTable("CollectorCashDeclarationDenomination");

            entity.HasOne(d => d.CollectorCashDeclaration).WithMany(p => p.CollectorCashDeclarationDenominations)
                .HasForeignKey(d => d.CollectorCashDeclarationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashDeclarationDenomination_CollectorCashDeclaration");

            entity.HasOne(d => d.Denomination).WithMany(p => p.CollectorCashDeclarationDenominations)
                .HasForeignKey(d => d.DenominationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashDeclarationDenomination_Denomination");
        });

        modelBuilder.Entity<CollectorCashup>(entity =>
        {
            entity.ToTable("CollectorCashup");

            entity.Property(e => e.CashedUpAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalUsddeclared).HasColumnName("TotalUSDDeclared");
            entity.Property(e => e.TotalUsdreceived).HasColumnName("TotalUSDReceived");
            entity.Property(e => e.TotalUsdshortages).HasColumnName("TotalUSDShortages");
            entity.Property(e => e.TotalUsdsurplus).HasColumnName("TotalUSDSurplus");
            entity.Property(e => e.TotalZardeclared).HasColumnName("TotalZARDeclared");
            entity.Property(e => e.TotalZarreceived).HasColumnName("TotalZARReceived");
            entity.Property(e => e.TotalZarshortages).HasColumnName("TotalZARShortages");
            entity.Property(e => e.TotalZarsuplus).HasColumnName("TotalZARSuplus");

            entity.HasOne(d => d.Shift).WithMany(p => p.CollectorCashups)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashup_Shift");

            entity.HasOne(d => d.SystemUser).WithMany(p => p.CollectorCashups)
                .HasForeignKey(d => d.SystemUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashup_SystemUser");
        });

        modelBuilder.Entity<CollectorCashupCashSurplusAllocatedToDiscrepancy>(entity =>
        {
            entity.ToTable("CollectorCashupCashSurplusAllocatedToDiscrepancy");

            entity.HasOne(d => d.CollectorCashUp).WithMany(p => p.CollectorCashupCashSurplusAllocatedToDiscrepancies)
                .HasForeignKey(d => d.CollectorCashUpId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashupCashSurplusAllocatedToDiscrepancy_CollectorCashup");
        });

        modelBuilder.Entity<CollectorCashupReprocess>(entity =>
        {
            entity.ToTable("CollectorCashupReprocess");

            entity.Property(e => e.CashedUpAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalUsddeclared).HasColumnName("TotalUSDDeclared");
            entity.Property(e => e.TotalUsdreceived).HasColumnName("TotalUSDReceived");
            entity.Property(e => e.TotalUsdshortages).HasColumnName("TotalUSDShortages");
            entity.Property(e => e.TotalUsdsurplus).HasColumnName("TotalUSDSurplus");
            entity.Property(e => e.TotalZardeclared).HasColumnName("TotalZARDeclared");
            entity.Property(e => e.TotalZarreceived).HasColumnName("TotalZARReceived");
            entity.Property(e => e.TotalZarshortages).HasColumnName("TotalZARShortages");
            entity.Property(e => e.TotalZarsuplus).HasColumnName("TotalZARSuplus");

            entity.HasOne(d => d.CollectorCashup).WithMany(p => p.CollectorCashupReprocesses)
                .HasForeignKey(d => d.CollectorCashupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashupReprocess_CollectorCashup");
        });

        modelBuilder.Entity<CollectorCashupShortagePayment>(entity =>
        {
            entity.ToTable("CollectorCashupShortagePayment");

            entity.Property(e => e.ReceivedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CashupShortagePaymentMethod).WithMany(p => p.CollectorCashupShortagePayments)
                .HasForeignKey(d => d.CashupShortagePaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashupShortagePayment_CashupShortagePaymentMethod");

            entity.HasOne(d => d.CollectorCashupCashSurplusAllocatedToDiscrepancy).WithMany(p => p.CollectorCashupShortagePayments)
                .HasForeignKey(d => d.CollectorCashupCashSurplusAllocatedToDiscrepancyId)
                .HasConstraintName("FK_CollectorCashupShortagePayment_CollectorCashupCashSurplusAllocatedToDiscrepancy");

            entity.HasOne(d => d.CollectorCashup).WithMany(p => p.CollectorCashupShortagePayments)
                .HasForeignKey(d => d.CollectorCashupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashupShortagePayment_CollectorCashup");

            entity.HasOne(d => d.ReceivedBy).WithMany(p => p.CollectorCashupShortagePayments)
                .HasForeignKey(d => d.ReceivedById)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CollectorCashupShortagePayment_SystemUser");
        });

        modelBuilder.Entity<CollectorShiftAssignment>(entity =>
        {
            entity.HasKey(e => new { e.SystemUserId, e.ShiftDate, e.ShiftId }).HasName("PK_SystemUserShift");

            entity.ToTable("CollectorShiftAssignment");

            entity.HasOne(d => d.Shift).WithMany(p => p.CollectorShiftAssignments)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemUserShift_Shift");

            entity.HasOne(d => d.SystemUser).WithMany(p => p.CollectorShiftAssignments)
                .HasForeignKey(d => d.SystemUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemUserShift_SystemUser");
        });

        modelBuilder.Entity<ControlCentre>(entity =>
        {
            entity.ToTable("ControlCentre");

            entity.Property(e => e.ControlCentreCode)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ControlCentreName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currency");

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Denomination>(entity =>
        {
            entity.ToTable("Denomination");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Currency).WithMany(p => p.Denominations)
                .HasForeignKey(d => d.CurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Denomination_Currency");
        });

        modelBuilder.Entity<DeploymentCleanupList>(entity =>
        {
            entity.HasKey(e => new { e.SchemaName, e.TableName });

            entity.ToTable("DeploymentCleanupList");

            entity.Property(e => e.SchemaName).HasMaxLength(128);
            entity.Property(e => e.TableName).HasMaxLength(128);
            entity.Property(e => e.CleanupMethod)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ResetIdentity).HasDefaultValue(true);
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.ReportDate).HasName("PK__Discount__826382E9ECE94695");

            entity.ToTable("Discounts", "star");

            entity.Property(e => e.ClassIAnonymousAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_I_AnonymousAmount");
            entity.Property(e => e.ClassIAnonymousCount).HasColumnName("Class_I_AnonymousCount");
            entity.Property(e => e.ClassICorporateAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_I_CorporateAmount");
            entity.Property(e => e.ClassIIndividualAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_I_IndividualAmount");
            entity.Property(e => e.ClassIStaffAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_I_StaffAmount");
            entity.Property(e => e.ClassIStaffCount).HasColumnName("Class_I_StaffCount");
            entity.Property(e => e.ClassIiAnonymousAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_II_AnonymousAmount");
            entity.Property(e => e.ClassIiAnonymousCount).HasColumnName("Class_II_AnonymousCount");
            entity.Property(e => e.ClassIiCorporateAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_II_CorporateAmount");
            entity.Property(e => e.ClassIiIndividualAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_II_IndividualAmount");
            entity.Property(e => e.ClassIiStaffAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_II_StaffAmount");
            entity.Property(e => e.ClassIiStaffCount).HasColumnName("Class_II_StaffCount");
            entity.Property(e => e.ClassIiiAnonymousAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_III_AnonymousAmount");
            entity.Property(e => e.ClassIiiAnonymousCount).HasColumnName("Class_III_AnonymousCount");
            entity.Property(e => e.ClassIiiCorporateAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_III_CorporateAmount");
            entity.Property(e => e.ClassIiiIndividualAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_III_IndividualAmount");
            entity.Property(e => e.ClassIiiStaffAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_III_StaffAmount");
            entity.Property(e => e.ClassIiiStaffCount).HasColumnName("Class_III_StaffCount");
            entity.Property(e => e.ClassMAnonymousAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_M_AnonymousAmount");
            entity.Property(e => e.ClassMAnonymousCount).HasColumnName("Class_M_AnonymousCount");
            entity.Property(e => e.ClassMCorporateAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_M_CorporateAmount");
            entity.Property(e => e.ClassMIndividualAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_M_IndividualAmount");
            entity.Property(e => e.ClassMStaffAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_M_StaffAmount");
            entity.Property(e => e.ClassMStaffCount).HasColumnName("Class_M_StaffCount");
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.TotalDiscountAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<DiscountStructure>(entity =>
        {
            entity.ToTable("DiscountStructure");

            entity.Property(e => e.DiscountStructureId).ValueGeneratedNever();

            entity.HasOne(d => d.DiscountType).WithMany(p => p.DiscountStructures)
                .HasForeignKey(d => d.DiscountTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountStructure_DiscountType");
        });

        modelBuilder.Entity<DiscountStructureDetail>(entity =>
        {
            entity.ToTable("DiscountStructureDetail");

            entity.Property(e => e.DiscountStructureDetailId).ValueGeneratedNever();

            entity.HasOne(d => d.DiscountStructure).WithMany(p => p.DiscountStructureDetails)
                .HasForeignKey(d => d.DiscountStructureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountStructureDetail_DiscountStructure");

            entity.HasOne(d => d.TollClass).WithMany(p => p.DiscountStructureDetails)
                .HasForeignKey(d => d.TollClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountStructureDetail_DiscountStructureDetail");
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.ToTable("DiscountType");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.CalculationMethod).WithMany(p => p.DiscountTypes)
                .HasForeignKey(d => d.CalculationMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountType_CalculationMethod");
        });

        modelBuilder.Entity<Exempt>(entity =>
        {
            entity.HasKey(e => e.ReportDate).HasName("PK__Exempts__826382E93213C014");

            entity.ToTable("Exempts", "star");

            entity.Property(e => e.ClassIExemptAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_I_ExemptAmount");
            entity.Property(e => e.ClassIExemptCount).HasColumnName("Class_I_ExemptCount");
            entity.Property(e => e.ClassIiExemptAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_II_ExemptAmount");
            entity.Property(e => e.ClassIiExemptCount).HasColumnName("Class_II_ExemptCount");
            entity.Property(e => e.ClassIiiExemptAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_III_ExemptAmount");
            entity.Property(e => e.ClassIiiExemptCount).HasColumnName("Class_III_ExemptCount");
            entity.Property(e => e.ClassMExemptAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_M_ExemptAmount");
            entity.Property(e => e.ClassMExemptCount).HasColumnName("Class_M_ExemptCount");
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.TotalExemptAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ExemptType>(entity =>
        {
            entity.ToTable("ExemptType");

            entity.Property(e => e.ExemptTypeDescription)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<IdentifierStat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("IdentifierStats");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(15)
                .IsFixedLength();
        });

        modelBuilder.Entity<IdentifierType>(entity =>
        {
            entity.ToTable("IdentifierType");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<IdentifierTypeStat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("IdentifierTypeStats");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ImageQueueMessage>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.LaneCode).HasMaxLength(50);
        });

        modelBuilder.Entity<Incident>(entity =>
        {
            entity.ToTable("Incident");

            entity.Property(e => e.IncidentId).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IncidentCode)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Lane>(entity =>
        {
            entity.ToTable("Lane", tb => tb.HasComment("Transactions concluded in teh lanes and transmitted to teh Back office"));

            entity.Property(e => e.AnprcameraIp)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ANPRCameraIP");
            entity.Property(e => e.Avccomms)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AVCComms");
            entity.Property(e => e.FrontCameraIp)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("FrontCameraIP");
            entity.Property(e => e.IodigitalPort)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IODigitalPort");
            entity.Property(e => e.LaneCode)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LaneName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PrinterPort)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Rfidport)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RFIDPort");
            entity.Property(e => e.SideCameraIp)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("SideCameraIP");
            entity.Property(e => e.SmartCardComPort)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ufdport)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UFDPort");

            entity.HasOne(d => d.VirtualPlaza).WithMany(p => p.Lanes)
                .HasForeignKey(d => d.VirtualPlazaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lane_VirtualPlaza");
        });

        modelBuilder.Entity<LaneCamera>(entity =>
        {
            entity.HasKey(e => new { e.LaneId, e.CameraId });

            entity.ToTable("LaneCamera");

            entity.Property(e => e.DefaultCamera).HasDefaultValue(false);

            entity.HasOne(d => d.Camera).WithMany(p => p.LaneCameras)
                .HasForeignKey(d => d.CameraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LaneCamera_Camera");

            entity.HasOne(d => d.Lane).WithMany(p => p.LaneCameras)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LaneCamera_Lane");
        });

        modelBuilder.Entity<LaneDefaultValue>(entity =>
        {
            entity.ToTable("LaneDefaultValue");

            entity.Property(e => e.LaneDefaultValueId).ValueGeneratedNever();
            entity.Property(e => e.Bvalue).HasColumnName("BValue");
            entity.Property(e => e.Cvalue)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CValue");
            entity.Property(e => e.DefaultValueDescriptions)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Dvalue).HasColumnName("DValue");
            entity.Property(e => e.Ivalue).HasColumnName("IValue");
            entity.Property(e => e.Svalue)
                .IsUnicode(false)
                .HasColumnName("SValue");
        });

        modelBuilder.Entity<LaneDisplayMessage>(entity =>
        {
            entity.ToTable("LaneDisplayMessage");

            entity.Property(e => e.LaneDisplayMessageId).ValueGeneratedNever();
            entity.Property(e => e.English)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ToDisplay)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LaneHourlyAudit>(entity =>
        {
            entity.HasKey(e => new { e.LaneId, e.CalendarDate, e.Hour }).HasName("PK_TransactionHourlyAudit");

            entity.ToTable("LaneHourlyAudit");
        });

        modelBuilder.Entity<LaneIncident>(entity =>
        {
            entity.HasKey(e => new { e.LaneIncidentId, e.LaneId });

            entity.ToTable("LaneIncident");

            entity.Property(e => e.LaneId).HasDefaultValue((byte)1);
            entity.Property(e => e.OccurredAt).HasColumnType("datetime");

            entity.HasOne(d => d.Incident).WithMany(p => p.LaneIncidents)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LaneIncident_Incident");
        });

        modelBuilder.Entity<LaneLastNo>(entity =>
        {
            entity.HasKey(e => e.LaneId);

            entity.ToTable("LaneLastNo");

            entity.HasOne(d => d.Lane).WithOne(p => p.LaneLastNo)
                .HasForeignKey<LaneLastNo>(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LaneLastNo_Lane");
        });

        modelBuilder.Entity<LaneLastTransactionImage>(entity =>
        {
            entity.HasKey(e => e.LaneId).HasName("PK_LaneLastTransactionImageId");

            entity.ToTable("LaneLastTransactionImage");

            entity.HasOne(d => d.Lane).WithOne(p => p.LaneLastTransactionImage)
                .HasForeignKey<LaneLastTransactionImage>(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LaneLastTransactionImage_Lane");
        });

        modelBuilder.Entity<LaneLoginLogout>(entity =>
        {
            entity.HasKey(e => new { e.LaneLoginLogoutId, e.LaneId });

            entity.ToTable("LaneLoginLogout");

            entity.Property(e => e.LogOutAt).HasColumnType("datetime");
            entity.Property(e => e.LoginAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<LaneScadaStatus>(entity =>
        {
            entity.HasKey(e => e.LaneId);

            entity.ToTable("LaneScadaStatus");

            entity.Property(e => e.Anprcamera).HasColumnName("ANPRCamera");
            entity.Property(e => e.Avcloop).HasColumnName("AVCLoop");
            entity.Property(e => e.Collector)
                .IsRequired()
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Igcamera).HasColumnName("IGCamera");
            entity.Property(e => e.Ohls).HasColumnName("OHLS");
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<ListAccountHolder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ListAccountHolders");

            entity.Property(e => e.AccNr).HasMaxLength(50);
            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.BalanceChangedOn).HasColumnType("datetime");
            entity.Property(e => e.BalanceVisibilityUfd).HasColumnName("BalanceVisibilityUFD");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PrimaryContact)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PrimaryEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SecondaryContact)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SecondaryEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(15)
                .IsFixedLength();
        });

        modelBuilder.Entity<ListIdentifier>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ListIdentifiers");

            entity.Property(e => e.ClassDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NumberPlateDetails)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RegisteredIdentifier)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(15)
                .IsFixedLength();
        });

        modelBuilder.Entity<OtherIncome>(entity =>
        {
            entity.HasKey(e => e.ReportDate).HasName("PK__OtherInc__826382E99D15CEF5");

            entity.ToTable("OtherIncome", "star");

            entity.Property(e => e.BankDepositTopupAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CashSurplusShortage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CashTopupAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DigitalTopupAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.ExpectedAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NfctopupAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("NFCTopupAmount");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.SwitchTopupAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalActualAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDeclaredAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalNettAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalOtherIncome).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTopupAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.ToTable("PaymentMethod");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Reconciliation>(entity =>
        {
            entity.HasKey(e => e.ReportDate).HasName("PK__Reconcil__826382E965E77C5E");

            entity.ToTable("Reconciliation", "star");

            entity.Property(e => e.CashBanked).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CashDeclared).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CashShortages).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CashSurplusShortage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CollectorDebt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DigitalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Discrepancy).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.EtctagAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("ETCTagAmount");
            entity.Property(e => e.ExemptionsAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OtherLaneTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrePaidTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SmartCardAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.SwitchAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAccounted).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ViolationAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<RegisterUserAccountMovement>(entity =>
        {
            entity.ToTable("RegisterUserAccountMovement", tb => tb.HasTrigger("trg_InsertAccountMovement"));

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TransactionDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.RegisterUser).WithMany(p => p.RegisterUserAccountMovements)
                .HasForeignKey(d => d.RegisterUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegisterUserAccountMovement_RegisteredUser");
        });

        modelBuilder.Entity<RegisteredUser>(entity =>
        {
            entity.HasKey(e => e.RegisterUserId);

            entity.ToTable("RegisteredUser", tb => tb.HasTrigger("TR_RegisteredUser_AfterInsert"));

            entity.Property(e => e.AccNr).HasMaxLength(50);
            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.BalanceChangedOn).HasColumnType("datetime");
            entity.Property(e => e.BalanceVisibilityUfd).HasColumnName("BalanceVisibilityUFD");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPrepaid).HasDefaultValue(true);
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PrimaryContact)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PrimaryEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SecondaryContact)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SecondaryEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.SystemUserId).HasDefaultValue(-1L);
        });

        modelBuilder.Entity<RegisteredUserFee>(entity =>
        {
            entity.HasKey(e => e.RegisteredUserFeesId);

            entity.Property(e => e.RegisteredUserFeesType)
                .IsRequired()
                .HasMaxLength(25);
            entity.Property(e => e.RegisteredUserFeesValue).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<RegisteredUserIdentifier>(entity =>
        {
            entity.ToTable("RegisteredUserIdentifier", tb => tb.HasTrigger("TR_RegisteredUserIdentifier_AfterInsert"));

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NumberPlateDetails)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RegisteredIdentifier)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.SystemUserId).HasDefaultValue(-1L);

            entity.HasOne(d => d.IdentifierType).WithMany(p => p.RegisteredUserIdentifiers)
                .HasForeignKey(d => d.IdentifierTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegisteredUserIdentifier_IdentifierType");

            entity.HasOne(d => d.RegisteredUser).WithMany(p => p.RegisteredUserIdentifiers)
                .HasForeignKey(d => d.RegisteredUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegisteredUserIdentifier_RegisteredUser");
        });

        modelBuilder.Entity<RegisteredUserTopUp>(entity =>
        {
            entity.HasKey(e => new { e.RegisteredUserTopUpId, e.RechargeStation });

            entity.ToTable("RegisteredUserTopUp", tb => tb.HasTrigger("TR_RegisteredUserTopUp_AfterInsert"));

            entity.Property(e => e.RegisteredUserTopUpId).ValueGeneratedOnAdd();
            entity.Property(e => e.RechargeStation)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RechargedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.RegisteredUserTopUps)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegisteredUserTopUp_PaymentMethod");

            entity.HasOne(d => d.RegisterUser).WithMany(p => p.RegisteredUserTopUps)
                .HasForeignKey(d => d.RegisterUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegisteredUserTopUp_RegisteredUser");
        });

        modelBuilder.Entity<ReguserStat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ReguserStats");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(15)
                .IsFixedLength();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.ToTable("Shift");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ShiftDaySummary>(entity =>
        {
            entity.HasKey(e => new { e.ShiftDate, e.ShiftId, e.SystemUserId, e.CollectorId });

            entity.ToTable("ShiftDaySummary");

            entity.Property(e => e.VehiclesXnominalTariff).HasColumnName("VehiclesXNominalTariff");
        });

        modelBuilder.Entity<ShiftStatus>(entity =>
        {
            entity.ToTable("ShiftStatus");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SupervisorCashup>(entity =>
        {
            entity.ToTable("SupervisorCashup");

            entity.Property(e => e.CashedUpAt).HasColumnType("datetime");
            entity.Property(e => e.TotalUsd).HasColumnName("TotalUSD");
            entity.Property(e => e.TotalZar).HasColumnName("TotalZAR");
            entity.Property(e => e.VarianceTotalUsd).HasColumnName("VarianceTotalUSD");
            entity.Property(e => e.VarianceTotalZar).HasColumnName("VarianceTotalZAR");
            entity.Property(e => e.VerifiedAt).HasColumnType("datetime");
            entity.Property(e => e.VerifiedTotalUsd).HasColumnName("VerifiedTotalUSD");
            entity.Property(e => e.VerifiedTotalZar).HasColumnName("VerifiedTotalZAR");

            entity.HasOne(d => d.Shift).WithMany(p => p.SupervisorCashups)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupervisorCashup_Shift");

            entity.HasOne(d => d.SystemUser).WithMany(p => p.SupervisorCashupSystemUsers)
                .HasForeignKey(d => d.SystemUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupervisorCashup_SystemUser");

            entity.HasOne(d => d.VerifiedBy).WithMany(p => p.SupervisorCashupVerifiedBies)
                .HasForeignKey(d => d.VerifiedById)
                .HasConstraintName("FK_SupervisorCashup_SystemUser1");
        });

        modelBuilder.Entity<SystemUser>(entity =>
        {
            entity.ToTable("SystemUser");

            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SystemUserRole>(entity =>
        {
            entity.HasKey(e => new { e.SystemUserId, e.RoleId });

            entity.ToTable("SystemUserRole");

            entity.HasOne(d => d.Role).WithMany(p => p.SystemUserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemUserRole_Role");

            entity.HasOne(d => d.SystemUser).WithMany(p => p.SystemUserRoles)
                .HasForeignKey(d => d.SystemUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemUserRole_SystemUser");
        });

        modelBuilder.Entity<TariffPlan>(entity =>
        {
            entity.HasKey(e => e.TariffPlanId).HasName("PK_TariffPlan_1");

            entity.ToTable("TariffPlan");

            entity.Property(e => e.TariffPlanId).ValueGeneratedNever();

            entity.HasOne(d => d.Currency).WithMany(p => p.TariffPlans)
                .HasForeignKey(d => d.CurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TariffPlan_Currency");
        });

        modelBuilder.Entity<TariffPlanDetail>(entity =>
        {
            entity.HasKey(e => new { e.TariffPlanId, e.TollClassId, e.TransactionTypeId });

            entity.ToTable("TariffPlanDetail");

            entity.Property(e => e.TransactionTypeId).HasDefaultValue((byte)1);
            entity.Property(e => e.Vat).HasColumnName("VAT");

            entity.HasOne(d => d.TariffPlan).WithMany(p => p.TariffPlanDetails)
                .HasForeignKey(d => d.TariffPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TariffPlanDetail_TariffPlan");

            entity.HasOne(d => d.TollClass).WithMany(p => p.TariffPlanDetails)
                .HasForeignKey(d => d.TollClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TariffPlanDetail_Class");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.TariffPlanDetails)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TariffPlanDetail_TransactionType");
        });

        modelBuilder.Entity<TheoreticalIncome>(entity =>
        {
            entity.HasKey(e => new { e.ReportDate, e.Metric });

            entity.ToTable("TheoreticalIncome", "star");

            entity.Property(e => e.Metric)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClassI)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_I");
            entity.Property(e => e.ClassIi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_II");
            entity.Property(e => e.ClassIii)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_III");
            entity.Property(e => e.ClassM)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Class_M");
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<TollClass>(entity =>
        {
            entity.HasKey(e => e.TollClassId).HasName("PK_Class");

            entity.ToTable("TollClass");

            entity.Property(e => e.ClassDescription)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TollClass1>(entity =>
        {
            entity.HasKey(e => e.TollClassId).HasName("PK_Class");

            entity.ToTable("TollClass", "star");

            entity.Property(e => e.ClassDescription)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TollClassSpecification>(entity =>
        {
            entity.ToTable("TollClassSpecification");

            entity.Property(e => e.TollClassSpecificationId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<TollPlaza>(entity =>
        {
            entity.ToTable("TollPlaza");

            entity.Property(e => e.PlazaCode)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PlazaName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.ControlCentre).WithMany(p => p.TollPlazas)
                .HasForeignKey(d => d.ControlCentreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TollPlaza_ControlCentre");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => new { e.LaneId, e.TransactionNumber });

            entity.ToTable("Transaction", tb => tb.HasTrigger("trgTransaction_Insert"));

            entity.Property(e => e.ActualAmount).HasDefaultValue(0.0);
            entity.Property(e => e.AutomaticAmount).HasDefaultValue(0.0);
            entity.Property(e => e.CardNumber)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceReceiptPrefix)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RegisteredIdentifier)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TransactionDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.ActualTollClass).WithMany(p => p.TransactionActualTollClasses)
                .HasForeignKey(d => d.ActualTollClassId)
                .HasConstraintName("FK_Transaction_TollClass1");

            entity.HasOne(d => d.AllocatedToCollectorCashup).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AllocatedToCollectorCashupId)
                .HasConstraintName("FK_Transaction_CollectorCashup");

            entity.HasOne(d => d.AutomaticTollClass).WithMany(p => p.TransactionAutomaticTollClasses)
                .HasForeignKey(d => d.AutomaticTollClassId)
                .HasConstraintName("FK_Transaction_TollClass2");

            entity.HasOne(d => d.Currency).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Currency");

            entity.HasOne(d => d.DiscountType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.DiscountTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_DiscountType");

            entity.HasOne(d => d.Lane).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Lane");

            entity.HasOne(d => d.ManualTollClass).WithMany(p => p.TransactionManualTollClasses)
                .HasForeignKey(d => d.ManualTollClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_TollClass");

            entity.HasOne(d => d.RegisteredTollClass).WithMany(p => p.TransactionRegisteredTollClasses)
                .HasForeignKey(d => d.RegisteredTollClassId)
                .HasConstraintName("FK_Transaction_TollClass3");

            entity.HasOne(d => d.RegisteredUser).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.RegisteredUserId)
                .HasConstraintName("FK_Transaction_RegisteredUser");

            entity.HasOne(d => d.Shift).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Shift");

            entity.HasOne(d => d.TariffPlan).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TariffPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_TariffPlan");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_TransactionType");
        });

        modelBuilder.Entity<Transaction1>(entity =>
        {
            entity.HasKey(e => new { e.LaneId, e.TransactionNumber });

            entity.ToTable("Transaction", "star");

            entity.Property(e => e.ActualAmount).HasDefaultValue(0.0);
            entity.Property(e => e.AutomaticAmount).HasDefaultValue(0.0);
            entity.Property(e => e.CardNumber)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceReceiptPrefix)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RegisteredIdentifier)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TransactionDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.ActualTollClass).WithMany(p => p.Transaction1ActualTollClasses)
                .HasForeignKey(d => d.ActualTollClassId)
                .HasConstraintName("FK_Transaction_TollClass1");

            entity.HasOne(d => d.AllocatedToCollectorCashup).WithMany(p => p.Transaction1s)
                .HasForeignKey(d => d.AllocatedToCollectorCashupId)
                .HasConstraintName("FK_Transaction_CollectorCashup");

            entity.HasOne(d => d.AutomaticTollClass).WithMany(p => p.Transaction1AutomaticTollClasses)
                .HasForeignKey(d => d.AutomaticTollClassId)
                .HasConstraintName("FK_Transaction_TollClass2");

            entity.HasOne(d => d.Currency).WithMany(p => p.Transaction1s)
                .HasForeignKey(d => d.CurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Currency");

            entity.HasOne(d => d.DiscountType).WithMany(p => p.Transaction1s)
                .HasForeignKey(d => d.DiscountTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_DiscountType");

            entity.HasOne(d => d.Lane).WithMany(p => p.Transaction1s)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Lane");

            entity.HasOne(d => d.ManualTollClass).WithMany(p => p.Transaction1ManualTollClasses)
                .HasForeignKey(d => d.ManualTollClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_TollClass");

            entity.HasOne(d => d.RegisteredTollClass).WithMany(p => p.Transaction1RegisteredTollClasses)
                .HasForeignKey(d => d.RegisteredTollClassId)
                .HasConstraintName("FK_Transaction_TollClass3");

            entity.HasOne(d => d.RegisteredUser).WithMany(p => p.Transaction1s)
                .HasForeignKey(d => d.RegisteredUserId)
                .HasConstraintName("FK_Transaction_RegisteredUser");

            entity.HasOne(d => d.Shift).WithMany(p => p.Transaction1s)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Shift");

            entity.HasOne(d => d.SystemUser).WithMany(p => p.Transaction1s)
                .HasForeignKey(d => d.SystemUserId)
                .HasConstraintName("FK_Transaction_SystemUser");

            entity.HasOne(d => d.TariffPlan).WithMany(p => p.Transaction1s)
                .HasForeignKey(d => d.TariffPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_TariffPlan");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Transaction1s)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_TransactionType");
        });

        modelBuilder.Entity<TransactionClassCorrection>(entity =>
        {
            entity.HasKey(e => new { e.TransactionClassCorrectionId, e.LaneId });

            entity.ToTable("TransactionClassCorrection", tb => tb.HasTrigger("trg_TransactionClassCorrection"));

            entity.HasOne(d => d.AllocatedToCollectorCashup).WithMany(p => p.TransactionClassCorrections)
                .HasForeignKey(d => d.AllocatedToCollectorCashupId)
                .HasConstraintName("FK_TransactionClassCorrection_CollectorCashup");

            entity.HasOne(d => d.AllocatedTo).WithMany(p => p.TransactionClassCorrections)
                .HasForeignKey(d => d.AllocatedToId)
                .HasConstraintName("FK_TransactionClassCorrection_AllocatedTo");

            entity.HasOne(d => d.ClassCorrectionType).WithMany(p => p.TransactionClassCorrections)
                .HasForeignKey(d => d.ClassCorrectionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionClassCorrection_ClassCorrectionType");

            entity.HasOne(d => d.CorrectedClass).WithMany(p => p.TransactionClassCorrections)
                .HasForeignKey(d => d.CorrectedClassId)
                .HasConstraintName("FK_TransactionClassCorrection_TollClass");

            entity.HasOne(d => d.ExemptType).WithMany(p => p.TransactionClassCorrections)
                .HasForeignKey(d => d.ExemptTypeId)
                .HasConstraintName("FK_TransactionClassCorrection_ExemptType");

            entity.HasOne(d => d.Lane).WithMany(p => p.TransactionClassCorrections)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionClassCorrection_Lane");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionClassCorrections)
                .HasForeignKey(d => new { d.LaneId, d.TransactionNumber })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionClassCorrection_Transaction");
        });

        modelBuilder.Entity<TransactionCreditNote>(entity =>
        {
            entity.HasKey(e => e.CreditNoteId);

            entity.ToTable("TransactionCreditNote");

            entity.Property(e => e.CreditNoteId).ValueGeneratedNever();

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionCreditNotes)
                .HasForeignKey(d => new { d.LaneId, d.TransactionNumber })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionCreditNote_Transaction");
        });

        modelBuilder.Entity<TransactionImage>(entity =>
        {
            entity.ToTable("TransactionImage", tb => tb.HasTrigger("trgTransactionImageInsert"));

            entity.Property(e => e.SnapShot)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionImages)
                .HasForeignKey(d => new { d.LaneId, d.TransactionNumber })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionImage_Transaction");
        });

        modelBuilder.Entity<TransactionMissing>(entity =>
        {
            entity.HasKey(e => e.TransactionMissing1);

            entity.ToTable("TransactionMissing");

            entity.Property(e => e.TransactionMissing1).HasColumnName("TransactionMissing");
        });

        modelBuilder.Entity<TransactionMissingDetail>(entity =>
        {
            entity.HasKey(e => new { e.TransactionNumber, e.LaneId });

            entity.ToTable("TransactionMissingDetail");

            entity.Property(e => e.ReceivedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.ToTable("TransactionType");

            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TransactionVehicleCharacteristic>(entity =>
        {
            entity.HasKey(e => new { e.LaneId, e.TransactionNumber });

            entity.ToTable("TransactionVehicleCharacteristic");
        });

        modelBuilder.Entity<Ufdmessage>(entity =>
        {
            entity.ToTable("UFDMessage");

            entity.Property(e => e.UfdmessageId).HasColumnName("UFDMessageId");
            entity.Property(e => e.Page1Line1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Page1Line2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Page2Line1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Page2Line2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ufdmessage1)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("UFDMessage");
        });

        modelBuilder.Entity<VirtualPlaza>(entity =>
        {
            entity.ToTable("VirtualPlaza");

            entity.Property(e => e.VirtualPlazaCode)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VirtualPlazaName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.TollPlaza).WithMany(p => p.VirtualPlazas)
                .HasForeignKey(d => d.TollPlazaId)
                .HasConstraintName("FK_VirtualPlaza_TollPlaza");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
