using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace MIS.Models
{
    public partial class MISDBContext : DbContext
    {
        public MISDBContext()
        {
        }

        public MISDBContext(DbContextOptions<MISDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<AllocatedTo> AllocatedTos { get; set; }
        public virtual DbSet<CalculationMethod> CalculationMethods { get; set; }
        public virtual DbSet<Camera> Cameras { get; set; }
        public virtual DbSet<CashupShortagePaymentMethod> CashupShortagePaymentMethods { get; set; }
        public virtual DbSet<ClassCorrectionType> ClassCorrectionTypes { get; set; }
        public virtual DbSet<CollectorCashDeclaration> CollectorCashDeclarations { get; set; }
        public virtual DbSet<CollectorCashDeclarationDenomination> CollectorCashDeclarationDenominations { get; set; }
        public virtual DbSet<CollectorCashDeclarationForeignCurrency> CollectorCashDeclarationForeignCurrencies { get; set; }
        public virtual DbSet<CollectorCashup> CollectorCashups { get; set; }
        public virtual DbSet<CollectorCashupCashSurplusAllocatedToDiscrepancy> CollectorCashupCashSurplusAllocatedToDiscrepancies { get; set; }
        public virtual DbSet<CollectorCashupReprocess> CollectorCashupReprocesses { get; set; }
        public virtual DbSet<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; }
        public virtual DbSet<CollectorShiftAssignment> CollectorShiftAssignments { get; set; }
        public virtual DbSet<ContactType> ContactTypes { get; set; }
        public virtual DbSet<ControlCentre> ControlCentres { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Denomination> Denominations { get; set; }
        public virtual DbSet<DiscountStructure> DiscountStructures { get; set; }
        public virtual DbSet<DiscountStructureDetail> DiscountStructureDetails { get; set; }
        public virtual DbSet<DiscountType> DiscountTypes { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<EmailType> EmailTypes { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<EntityType> EntityTypes { get; set; }
        public virtual DbSet<ExchangeRate> ExchangeRates { get; set; }
        public virtual DbSet<ExemptType> ExemptTypes { get; set; }
        public virtual DbSet<IdentifierType> IdentifierTypes { get; set; }
        public virtual DbSet<Incident> Incidents { get; set; }
        public virtual DbSet<Lane> Lanes { get; set; }
        public virtual DbSet<LaneCamera> LaneCameras { get; set; }
        public virtual DbSet<LaneDefaultValue> LaneDefaultValues { get; set; }
        public virtual DbSet<LaneDisplayMessage> LaneDisplayMessages { get; set; }
        public virtual DbSet<LaneIncident> LaneIncidents { get; set; }
        public virtual DbSet<LaneLastNo> LaneLastNos { get; set; }
        public virtual DbSet<LaneLastTransactionImage> LaneLastTransactionImages { get; set; }
        public virtual DbSet<LaneLoginLogout> LaneLoginLogouts { get; set; }
        public virtual DbSet<LaneScadaStatus> LaneScadaStatuses { get; set; }
        public virtual DbSet<PaymentIdentifier> PaymentIdentifiers { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<RechargePoint> RechargePoints { get; set; }
        public virtual DbSet<RegisterUserAccountMovement> RegisterUserAccountMovements { get; set; }
        public virtual DbSet<RegisteredUser> RegisteredUsers { get; set; }
        public virtual DbSet<RegisteredUserAddress> RegisteredUserAddresses { get; set; }
        public virtual DbSet<RegisteredUserContact> RegisteredUserContacts { get; set; }
        public virtual DbSet<RegisteredUserEmail> RegisteredUserEmails { get; set; }
        public virtual DbSet<RegisteredUserIdentifier> RegisteredUserIdentifiers { get; set; }
        public virtual DbSet<RegisteredUserIdentifierTollPlazaApplicableDiscount> RegisteredUserIdentifierTollPlazaApplicableDiscounts { get; set; }
        public virtual DbSet<RegisteredUserPlazaTransactionSetting> RegisteredUserPlazaTransactionSettings { get; set; }
        public virtual DbSet<RegisteredUserTopUp> RegisteredUserTopUps { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<ShiftStatus> ShiftStatuses { get; set; }
        public virtual DbSet<Suburb> Suburbs { get; set; }
        public virtual DbSet<SupervisorCashup> SupervisorCashups { get; set; }
        public virtual DbSet<SystemUser> SystemUsers { get; set; }
        public virtual DbSet<SystemUserRole> SystemUserRoles { get; set; }
        public virtual DbSet<TariffPlan> TariffPlans { get; set; }
        public virtual DbSet<TariffPlanDetail> TariffPlanDetails { get; set; }
        public virtual DbSet<TollClass> TollClasses { get; set; }
        public virtual DbSet<TollClassSpecification> TollClassSpecifications { get; set; }
        public virtual DbSet<TollPlaza> TollPlazas { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<TransactionClassCorrection> TransactionClassCorrections { get; set; }
        public virtual DbSet<TransactionCreditNote> TransactionCreditNotes { get; set; }
        public virtual DbSet<TransactionImage> TransactionImages { get; set; }
        public virtual DbSet<TransactionMissing> TransactionMissings { get; set; }
        public virtual DbSet<TransactionMissingDetail> TransactionMissingDetails { get; set; }
        public virtual DbSet<TransactionType> TransactionTypes { get; set; }
        public virtual DbSet<TransactionVehicleCharacteristic> TransactionVehicleCharacteristics { get; set; }
        public virtual DbSet<Ufdmessage> Ufdmessages { get; set; }
        public virtual DbSet<VehicleType> VehicleTypes { get; set; }
        public virtual DbSet<VirtualPlaza> VirtualPlazas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var configSection = configBuilder.GetSection("ConnectionStrings");
                var connectionString = configSection["SQLServerConnection"] ?? null;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AddressType>(entity =>
            {
                entity.ToTable("AddressType");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

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

                entity.Property(e => e.ShiftDate).HasColumnType("date");

                entity.Property(e => e.TotalUsd).HasColumnName("TotalUSD");

                entity.Property(e => e.TotalZar).HasColumnName("TotalZAR");

                entity.HasOne(d => d.AllocatedToCollectorCashup)
                    .WithMany(p => p.CollectorCashDeclarations)
                    .HasForeignKey(d => d.AllocatedToCollectorCashupId)
                    .HasConstraintName("FK_CollectorCashDeclaration_CollectorCashup");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.CollectorCashDeclarations)
                    .HasForeignKey(d => d.ShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashDeclaration_Shift");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.CollectorCashDeclarationSystemUsers)
                    .HasForeignKey(d => d.SystemUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashDeclaration_SystemUser");

                entity.HasOne(d => d.VerifiedBy)
                    .WithMany(p => p.CollectorCashDeclarationVerifiedBies)
                    .HasForeignKey(d => d.VerifiedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashDeclaration_SystemUser1");
            });

            modelBuilder.Entity<CollectorCashDeclarationDenomination>(entity =>
            {
                entity.HasKey(e => new { e.CollectorCashDeclarationId, e.DenominationId });

                entity.ToTable("CollectorCashDeclarationDenomination");

                entity.HasOne(d => d.CollectorCashDeclaration)
                    .WithMany(p => p.CollectorCashDeclarationDenominations)
                    .HasForeignKey(d => d.CollectorCashDeclarationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashDeclarationDenomination_CollectorCashDeclaration");

                entity.HasOne(d => d.Denomination)
                    .WithMany(p => p.CollectorCashDeclarationDenominations)
                    .HasForeignKey(d => d.DenominationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashDeclarationDenomination_Denomination");
            });

            modelBuilder.Entity<CollectorCashDeclarationForeignCurrency>(entity =>
            {
                entity.HasKey(e => new { e.CollectorCashDeclarationId, e.CurrencyId });

                entity.ToTable("CollectorCashDeclarationForeignCurrency");
            });

            modelBuilder.Entity<CollectorCashup>(entity =>
            {
                entity.ToTable("CollectorCashup");

                entity.Property(e => e.CashedUpAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ShiftDate).HasColumnType("date");

                entity.Property(e => e.TotalUsddeclared).HasColumnName("TotalUSDDeclared");

                entity.Property(e => e.TotalUsdreceived).HasColumnName("TotalUSDReceived");

                entity.Property(e => e.TotalUsdshortages).HasColumnName("TotalUSDShortages");

                entity.Property(e => e.TotalUsdsurplus).HasColumnName("TotalUSDSurplus");

                entity.Property(e => e.TotalZardeclared).HasColumnName("TotalZARDeclared");

                entity.Property(e => e.TotalZarreceived).HasColumnName("TotalZARReceived");

                entity.Property(e => e.TotalZarshortages).HasColumnName("TotalZARShortages");

                entity.Property(e => e.TotalZarsuplus).HasColumnName("TotalZARSuplus");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.CollectorCashups)
                    .HasForeignKey(d => d.ShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashup_Shift");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.CollectorCashups)
                    .HasForeignKey(d => d.SystemUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashup_SystemUser");
            });

            modelBuilder.Entity<CollectorCashupCashSurplusAllocatedToDiscrepancy>(entity =>
            {
                entity.ToTable("CollectorCashupCashSurplusAllocatedToDiscrepancy");

                entity.HasOne(d => d.CollectorCashUp)
                    .WithMany(p => p.CollectorCashupCashSurplusAllocatedToDiscrepancies)
                    .HasForeignKey(d => d.CollectorCashUpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashupCashSurplusAllocatedToDiscrepancy_CollectorCashup");
            });

            modelBuilder.Entity<CollectorCashupReprocess>(entity =>
            {
                entity.ToTable("CollectorCashupReprocess");

                entity.Property(e => e.CashedUpAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ShiftDate).HasColumnType("date");

                entity.Property(e => e.TotalUsddeclared).HasColumnName("TotalUSDDeclared");

                entity.Property(e => e.TotalUsdreceived).HasColumnName("TotalUSDReceived");

                entity.Property(e => e.TotalUsdshortages).HasColumnName("TotalUSDShortages");

                entity.Property(e => e.TotalUsdsurplus).HasColumnName("TotalUSDSurplus");

                entity.Property(e => e.TotalZardeclared).HasColumnName("TotalZARDeclared");

                entity.Property(e => e.TotalZarreceived).HasColumnName("TotalZARReceived");

                entity.Property(e => e.TotalZarshortages).HasColumnName("TotalZARShortages");

                entity.Property(e => e.TotalZarsuplus).HasColumnName("TotalZARSuplus");

                entity.HasOne(d => d.CollectorCashup)
                    .WithMany(p => p.CollectorCashupReprocesses)
                    .HasForeignKey(d => d.CollectorCashupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashupReprocess_CollectorCashup");
            });

            modelBuilder.Entity<CollectorCashupShortagePayment>(entity =>
            {
                entity.ToTable("CollectorCashupShortagePayment");

                entity.Property(e => e.ReceivedAt).HasColumnType("datetime");

                entity.HasOne(d => d.CashupShortagePaymentMethod)
                    .WithMany(p => p.CollectorCashupShortagePayments)
                    .HasForeignKey(d => d.CashupShortagePaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashupShortagePayment_CashupShortagePaymentMethod");

                entity.HasOne(d => d.CollectorCashupCashSurplusAllocatedToDiscrepancy)
                    .WithMany(p => p.CollectorCashupShortagePayments)
                    .HasForeignKey(d => d.CollectorCashupCashSurplusAllocatedToDiscrepancyId)
                    .HasConstraintName("FK_CollectorCashupShortagePayment_CollectorCashupCashSurplusAllocatedToDiscrepancy");

                entity.HasOne(d => d.CollectorCashup)
                    .WithMany(p => p.CollectorCashupShortagePayments)
                    .HasForeignKey(d => d.CollectorCashupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashupShortagePayment_CollectorCashup");

                entity.HasOne(d => d.ReceivedBy)
                    .WithMany(p => p.CollectorCashupShortagePayments)
                    .HasForeignKey(d => d.ReceivedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectorCashupShortagePayment_SystemUser");
            });

            modelBuilder.Entity<CollectorShiftAssignment>(entity =>
            {
                entity.HasKey(e => new { e.SystemUserId, e.ShiftDate, e.ShiftId })
                    .HasName("PK_SystemUserShift");

                entity.ToTable("CollectorShiftAssignment");

                entity.Property(e => e.ShiftDate).HasColumnType("date");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.CollectorShiftAssignments)
                    .HasForeignKey(d => d.ShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SystemUserShift_Shift");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.CollectorShiftAssignments)
                    .HasForeignKey(d => d.SystemUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SystemUserShift_SystemUser");
            });

            modelBuilder.Entity<ContactType>(entity =>
            {
                entity.ToTable("ContactType");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
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

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.Property(e => e.CountryId).ValueGeneratedNever();

                entity.Property(e => e.Description)
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

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Denominations)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Denomination_Currency");
            });

            modelBuilder.Entity<DiscountStructure>(entity =>
            {
                entity.ToTable("DiscountStructure");

                entity.Property(e => e.DiscountStructureId).ValueGeneratedNever();

                entity.Property(e => e.EffectiveDate).HasColumnType("date");

                entity.HasOne(d => d.DiscountType)
                    .WithMany(p => p.DiscountStructures)
                    .HasForeignKey(d => d.DiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiscountStructure_DiscountType");
            });

            modelBuilder.Entity<DiscountStructureDetail>(entity =>
            {
                entity.ToTable("DiscountStructureDetail");

                entity.Property(e => e.DiscountStructureDetailId).ValueGeneratedNever();

                entity.HasOne(d => d.DiscountStructure)
                    .WithMany(p => p.DiscountStructureDetails)
                    .HasForeignKey(d => d.DiscountStructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiscountStructureDetail_DiscountStructure");

                entity.HasOne(d => d.TollClass)
                    .WithMany(p => p.DiscountStructureDetails)
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

                entity.HasOne(d => d.CalculationMethod)
                    .WithMany(p => p.DiscountTypes)
                    .HasForeignKey(d => d.CalculationMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiscountType_CalculationMethod");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("District");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EmailType>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EmailType");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Entity");

                entity.Property(e => e.Apikey)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("APIKey");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.EntityCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EntityType>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EntityType");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.HasKey(e => new { e.FromCurrencyId, e.ToCurrencyId, e.EffectiveDate });

                entity.ToTable("ExchangeRate");

                entity.Property(e => e.EffectiveDate).HasColumnType("date");

                entity.HasOne(d => d.FromCurrency)
                    .WithMany(p => p.ExchangeRateFromCurrencies)
                    .HasForeignKey(d => d.FromCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExchangeRate_Currency");

                entity.HasOne(d => d.ToCurrency)
                    .WithMany(p => p.ExchangeRateToCurrencies)
                    .HasForeignKey(d => d.ToCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExchangeRate_Currency1");
            });

            modelBuilder.Entity<ExemptType>(entity =>
            {
                entity.ToTable("ExemptType");

                entity.Property(e => e.ExemptTypeDescription)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<IdentifierType>(entity =>
            {
                entity.ToTable("IdentifierType");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
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
                entity.ToTable("Lane");

                entity.Property(e => e.AnprcameraIp)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ANPRCameraIP");

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

                entity.Property(e => e.Ufdport)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UFDPort");

                entity.HasOne(d => d.VirtualPlaza)
                    .WithMany(p => p.Lanes)
                    .HasForeignKey(d => d.VirtualPlazaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lane_VirtualPlaza");
            });

            modelBuilder.Entity<LaneCamera>(entity =>
            {
                entity.HasKey(e => new { e.LaneId, e.CameraId });

                entity.ToTable("LaneCamera");

                entity.Property(e => e.DefaultCamera).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Camera)
                    .WithMany(p => p.LaneCameras)
                    .HasForeignKey(d => d.CameraId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LaneCamera_Camera");

                entity.HasOne(d => d.Lane)
                    .WithMany(p => p.LaneCameras)
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
                    .HasColumnName("CValue")
                    .IsFixedLength(true);

                entity.Property(e => e.DefaultValueDescriptions)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Dvalue)
                    .HasColumnType("date")
                    .HasColumnName("DValue");

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

            modelBuilder.Entity<LaneIncident>(entity =>
            {
                entity.ToTable("LaneIncident");

                entity.Property(e => e.LaneIncidentId).ValueGeneratedNever();

                entity.Property(e => e.OccurredAt).HasColumnType("datetime");

                entity.HasOne(d => d.Incident)
                    .WithMany(p => p.LaneIncidents)
                    .HasForeignKey(d => d.IncidentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LaneIncident_Incident");
            });

            modelBuilder.Entity<LaneLastNo>(entity =>
            {
                entity.HasKey(e => e.LaneId);

                entity.ToTable("LaneLastNo");

                entity.HasOne(d => d.Lane)
                    .WithOne(p => p.LaneLastNo)
                    .HasForeignKey<LaneLastNo>(d => d.LaneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LaneLastNo_Lane");
            });

            modelBuilder.Entity<LaneLastTransactionImage>(entity =>
            {
                entity.HasKey(e => e.LaneId)
                    .HasName("PK_LaneLastTransactionImageId");

                entity.ToTable("LaneLastTransactionImage");

                entity.HasOne(d => d.Lane)
                    .WithOne(p => p.LaneLastTransactionImage)
                    .HasForeignKey<LaneLastTransactionImage>(d => d.LaneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LaneLastTransactionImage_Lane");
            });

            modelBuilder.Entity<LaneLoginLogout>(entity =>
            {
                entity.ToTable("LaneLoginLogout");

                entity.Property(e => e.LaneLoginLogoutId).ValueGeneratedNever();

                entity.Property(e => e.LogOutAt).HasColumnType("datetime");

                entity.Property(e => e.LoginAt).HasColumnType("datetime");

                entity.Property(e => e.ShiftDate).HasColumnType("date");
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

            modelBuilder.Entity<PaymentIdentifier>(entity =>
            {
                entity.ToTable("PaymentIdentifier");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethod");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.ToTable("Province");

                entity.Property(e => e.ProvinceId).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RechargePoint>(entity =>
            {
                entity.ToTable("RechargePoint");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RegisterUserAccountMovement>(entity =>
            {
                entity.ToTable("RegisterUserAccountMovement");

                entity.Property(e => e.RegisterUserAccountMovementId).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.RegisterUser)
                    .WithMany(p => p.RegisterUserAccountMovements)
                    .HasForeignKey(d => d.RegisterUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisterUserAccountMovement_RegisteredUser");

                entity.HasOne(d => d.RegisteredUserTopUp)
                    .WithMany(p => p.RegisterUserAccountMovements)
                    .HasForeignKey(d => d.RegisteredUserTopUpId)
                    .HasConstraintName("FK_RegisterUserAccountMovement_RegisteredUserTopUp");
            });

            modelBuilder.Entity<RegisteredUser>(entity =>
            {
                entity.HasKey(e => e.RegisteredUserId);

                entity.ToTable("RegisteredUser");

                entity.Property(e => e.ActivationDate).HasColumnType("date");

                entity.Property(e => e.BalanceChangedOn).HasColumnType("datetime");

                entity.Property(e => e.BalanceVisibilityUfd).HasColumnName("BalanceVisibilityUFD");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ExpiryDate).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.IdNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPrepaid)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Reference)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.RegistrationNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<RegisteredUserAddress>(entity =>
            {
                entity.HasKey(e => new { e.RegisteredUserId, e.AddressTypeId });

                entity.ToTable("RegisteredUserAddress");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.AddressType)
                    .WithMany(p => p.RegisteredUserAddresses)
                    .HasForeignKey(d => d.AddressTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserAddress_AddressType");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.RegisteredUserAddresses)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_RegisteredUserAddress_Country");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.RegisteredUserAddresses)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_RegisteredUserAddress_Province");

                entity.HasOne(d => d.RegisteredUser)
                    .WithMany(p => p.RegisteredUserAddresses)
                    .HasForeignKey(d => d.RegisteredUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserAddress_RegisteredUser");

                entity.HasOne(d => d.Suburb)
                    .WithMany(p => p.RegisteredUserAddresses)
                    .HasForeignKey(d => d.SuburbId)
                    .HasConstraintName("FK_RegisteredUserAddress_Suburb");
            });

            modelBuilder.Entity<RegisteredUserContact>(entity =>
            {
                entity.HasKey(e => new { e.RegisteredUserId, e.ContactTypeId });

                entity.ToTable("RegisteredUserContact");

                entity.Property(e => e.ContactDetails)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.RegisteredUser)
                    .WithMany(p => p.RegisteredUserContacts)
                    .HasForeignKey(d => d.RegisteredUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserContact_RegisteredUser");
            });

            modelBuilder.Entity<RegisteredUserEmail>(entity =>
            {
                entity.HasKey(e => new { e.RegisteredUserId, e.EmailTypeId });

                entity.ToTable("RegisteredUserEmail");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.RegisteredUser)
                    .WithMany(p => p.RegisteredUserEmails)
                    .HasForeignKey(d => d.RegisteredUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserEmail_RegisteredUser");
            });

            modelBuilder.Entity<RegisteredUserIdentifier>(entity =>
            {
                entity.ToTable("RegisteredUserIdentifier");

                entity.Property(e => e.ActivationDate).HasColumnType("date");

                entity.Property(e => e.ExpiryDate).HasColumnType("date");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NumberPlateDetails)
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

                entity.HasOne(d => d.IdentifierType)
                    .WithMany(p => p.RegisteredUserIdentifiers)
                    .HasForeignKey(d => d.IdentifierTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserIdentifier_IdentifierType");

                entity.HasOne(d => d.RegisteredUser)
                    .WithMany(p => p.RegisteredUserIdentifiers)
                    .HasForeignKey(d => d.RegisteredUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserIdentifier_RegisteredUser");

                entity.HasOne(d => d.VehicleType)
                    .WithMany(p => p.RegisteredUserIdentifiers)
                    .HasForeignKey(d => d.VehicleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserIdentifier_VehicleType");
            });

            modelBuilder.Entity<RegisteredUserIdentifierTollPlazaApplicableDiscount>(entity =>
            {
                entity.HasKey(e => new { e.RegisteredUserIdentifierId, e.TollPlazaId });

                entity.ToTable("RegisteredUserIdentifierTollPlazaApplicableDiscount");

                entity.HasOne(d => d.RegisteredUserIdentifier)
                    .WithMany(p => p.RegisteredUserIdentifierTollPlazaApplicableDiscounts)
                    .HasForeignKey(d => d.RegisteredUserIdentifierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserIdentifierTollPlazaApplicableDiscount_RegisteredUserIdentifier");

                entity.HasOne(d => d.TollPlaza)
                    .WithMany(p => p.RegisteredUserIdentifierTollPlazaApplicableDiscounts)
                    .HasForeignKey(d => d.TollPlazaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserIdentifierTollPlazaApplicableDiscount_TollPlaza");
            });

            modelBuilder.Entity<RegisteredUserPlazaTransactionSetting>(entity =>
            {
                entity.HasKey(e => new { e.RegisteredUserId, e.TollPlazaId });

                entity.ToTable("RegisteredUserPlazaTransactionSetting");

                entity.HasOne(d => d.RegisteredUser)
                    .WithMany(p => p.RegisteredUserPlazaTransactionSettings)
                    .HasForeignKey(d => d.RegisteredUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserPlazaTransactionSetting_RegisteredUser");

                entity.HasOne(d => d.TollPlaza)
                    .WithMany(p => p.RegisteredUserPlazaTransactionSettings)
                    .HasForeignKey(d => d.TollPlazaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserPlazaTransactionSetting_TollPlaza");
            });

            modelBuilder.Entity<RegisteredUserTopUp>(entity =>
            {
                entity.ToTable("RegisteredUserTopUp");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RechargedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.PaymentIdentifier)
                    .WithMany(p => p.RegisteredUserTopUps)
                    .HasForeignKey(d => d.PaymentIdentifierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserTopUp_PaymentIdentifier");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.RegisteredUserTopUps)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserTopUp_PaymentMethod");

                entity.HasOne(d => d.RechargePoint)
                    .WithMany(p => p.RegisteredUserTopUps)
                    .HasForeignKey(d => d.RechargePointId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserTopUp_RechargePoint");

                entity.HasOne(d => d.RegisterUser)
                    .WithMany(p => p.RegisteredUserTopUps)
                    .HasForeignKey(d => d.RegisterUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisteredUserTopUp_RegisteredUser");
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

            modelBuilder.Entity<ShiftStatus>(entity =>
            {
                entity.ToTable("ShiftStatus");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Suburb>(entity =>
            {
                entity.ToTable("Suburb");

                entity.Property(e => e.SuburbId).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SupervisorCashup>(entity =>
            {
                entity.ToTable("SupervisorCashup");

                entity.Property(e => e.CashedUpAt).HasColumnType("datetime");

                entity.Property(e => e.ShiftDate).HasColumnType("date");

                entity.Property(e => e.TotalUsd).HasColumnName("TotalUSD");

                entity.Property(e => e.TotalZar).HasColumnName("TotalZAR");

                entity.Property(e => e.VarianceTotalUsd).HasColumnName("VarianceTotalUSD");

                entity.Property(e => e.VarianceTotalZar).HasColumnName("VarianceTotalZAR");

                entity.Property(e => e.VerifiedAt).HasColumnType("datetime");

                entity.Property(e => e.VerifiedTotalUsd).HasColumnName("VerifiedTotalUSD");

                entity.Property(e => e.VerifiedTotalZar).HasColumnName("VerifiedTotalZAR");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.SupervisorCashups)
                    .HasForeignKey(d => d.ShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupervisorCashup_Shift");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.SupervisorCashupSystemUsers)
                    .HasForeignKey(d => d.SystemUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupervisorCashup_SystemUser");

                entity.HasOne(d => d.VerifiedBy)
                    .WithMany(p => p.SupervisorCashupVerifiedBies)
                    .HasForeignKey(d => d.VerifiedById)
                    .HasConstraintName("FK_SupervisorCashup_SystemUser1");
            });

            modelBuilder.Entity<SystemUser>(entity =>
            {
                entity.ToTable("SystemUser");

                entity.Property(e => e.ActivationDate).HasColumnType("date");

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

                entity.Property(e => e.PasswordExpiryDate).HasColumnType("date");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SystemUserRole>(entity =>
            {
                entity.HasKey(e => new { e.SystemUserId, e.RoleId });

                entity.ToTable("SystemUserRole");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.SystemUserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SystemUserRole_Role");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.SystemUserRoles)
                    .HasForeignKey(d => d.SystemUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SystemUserRole_SystemUser");
            });

            modelBuilder.Entity<TariffPlan>(entity =>
            {
                entity.ToTable("TariffPlan");

                entity.Property(e => e.TariffPlanId).ValueGeneratedNever();

                entity.Property(e => e.EffectiveDate).HasColumnType("date");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.TariffPlans)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TariffPlan_Currency");
            });

            modelBuilder.Entity<TariffPlanDetail>(entity =>
            {
                entity.HasKey(e => new { e.TariffPlanId, e.TollClassId });

                entity.ToTable("TariffPlanDetail");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.HasOne(d => d.TariffPlan)
                    .WithMany(p => p.TariffPlanDetails)
                    .HasForeignKey(d => d.TariffPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TariffPlanDetail_TariffPlan");

                entity.HasOne(d => d.TollClass)
                    .WithMany(p => p.TariffPlanDetails)
                    .HasForeignKey(d => d.TollClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TariffPlanDetail_Class");
            });

            modelBuilder.Entity<TollClass>(entity =>
            {
                entity.ToTable("TollClass");

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

                entity.HasOne(d => d.ControlCentre)
                    .WithMany(p => p.TollPlazas)
                    .HasForeignKey(d => d.ControlCentreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TollPlaza_ControlCentre");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => new { e.LaneId, e.TransactionNumber });

                entity.ToTable("Transaction");

                entity.Property(e => e.ActualAmount).HasDefaultValueSql("((0))");

                entity.Property(e => e.AutomaticAmount).HasDefaultValueSql("((0))");

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

                entity.Property(e => e.ShiftDate).HasColumnType("date");

                entity.Property(e => e.TransactionDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.ActualTollClass)
                    .WithMany(p => p.TransactionActualTollClasses)
                    .HasForeignKey(d => d.ActualTollClassId)
                    .HasConstraintName("FK_Transaction_TollClass1");

                entity.HasOne(d => d.AllocatedToCollectorCashup)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AllocatedToCollectorCashupId)
                    .HasConstraintName("FK_Transaction_CollectorCashup");

                entity.HasOne(d => d.AutomaticTollClass)
                    .WithMany(p => p.TransactionAutomaticTollClasses)
                    .HasForeignKey(d => d.AutomaticTollClassId)
                    .HasConstraintName("FK_Transaction_TollClass2");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Currency");

                entity.HasOne(d => d.DiscountType)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.DiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_DiscountType");

                entity.HasOne(d => d.Lane)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.LaneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Lane");

                entity.HasOne(d => d.ManualTollClass)
                    .WithMany(p => p.TransactionManualTollClasses)
                    .HasForeignKey(d => d.ManualTollClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_TollClass");

                entity.HasOne(d => d.RegisteredTollClass)
                    .WithMany(p => p.TransactionRegisteredTollClasses)
                    .HasForeignKey(d => d.RegisteredTollClassId)
                    .HasConstraintName("FK_Transaction_TollClass3");

                entity.HasOne(d => d.RegisteredUser)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.RegisteredUserId)
                    .HasConstraintName("FK_Transaction_RegisteredUser");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.ShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Shift");

                entity.HasOne(d => d.SystemUser)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.SystemUserId)
                    .HasConstraintName("FK_Transaction_SystemUser");

                entity.HasOne(d => d.TariffPlan)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.TariffPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_TariffPlan");

                entity.HasOne(d => d.TransactionType)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.TransactionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_TransactionType");

               
            });

            modelBuilder.Entity<TransactionClassCorrection>(entity =>
            {
                entity.HasKey(e => new { e.TransactionClassCorrectionId, e.LaneId });

                entity.ToTable("TransactionClassCorrection");

                entity.HasOne(d => d.AllocatedToCollectorCashup)
                    .WithMany(p => p.TransactionClassCorrections)
                    .HasForeignKey(d => d.AllocatedToCollectorCashupId)
                    .HasConstraintName("FK_TransactionClassCorrection_CollectorCashup");

                entity.HasOne(d => d.AllocatedTo)
                    .WithMany(p => p.TransactionClassCorrections)
                    .HasForeignKey(d => d.AllocatedToId)
                    .HasConstraintName("FK_TransactionClassCorrection_AllocatedTo");

                entity.HasOne(d => d.ClassCorrectionType)
                    .WithMany(p => p.TransactionClassCorrections)
                    .HasForeignKey(d => d.ClassCorrectionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TransactionClassCorrection_ClassCorrectionType");

                entity.HasOne(d => d.CorrectedClass)
                    .WithMany(p => p.TransactionClassCorrections)
                    .HasForeignKey(d => d.CorrectedClassId)
                    .HasConstraintName("FK_TransactionClassCorrection_TollClass");

                entity.HasOne(d => d.ExemptType)
                    .WithMany(p => p.TransactionClassCorrections)
                    .HasForeignKey(d => d.ExemptTypeId)
                    .HasConstraintName("FK_TransactionClassCorrection_ExemptType");

                entity.HasOne(d => d.Lane)
                    .WithMany(p => p.TransactionClassCorrections)
                    .HasForeignKey(d => d.LaneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TransactionClassCorrection_Lane");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionClassCorrections)
                    .HasForeignKey(d => new { d.LaneId, d.TransactionNumber })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TransactionClassCorrection_Transaction");
            });

            modelBuilder.Entity<TransactionCreditNote>(entity =>
            {
                entity.HasKey(e => e.CreditNoteId);

                entity.ToTable("TransactionCreditNote");

                entity.Property(e => e.CreditNoteId).ValueGeneratedNever();

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionCreditNotes)
                    .HasForeignKey(d => new { d.LaneId, d.TransactionNumber })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TransactionCreditNote_Transaction");
            });

            modelBuilder.Entity<TransactionImage>(entity =>
            {
                entity.ToTable("TransactionImage");

                entity.Property(e => e.SnapShot)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionImages)
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

            modelBuilder.Entity<VehicleType>(entity =>
            {
                entity.ToTable("VehicleType");

                entity.Property(e => e.VehicleTypeId).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
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

                entity.HasOne(d => d.TollPlaza)
                    .WithMany(p => p.VirtualPlazas)
                    .HasForeignKey(d => d.TollPlazaId)
                    .HasConstraintName("FK_VirtualPlaza_TollPlaza");
            });

            //modelBuilder.Entity<Transaction>(entity =>
            //{
            //    entity.HasKey(e => new { e.LaneId, e.TransactionNumber });

            //    entity.ToTable("Transaction", tb => tb.HasTrigger("trgTransaction_Insert"));

            //    // Other configurations...
            //});

            //modelBuilder.Entity<Transaction>(entity =>
            //{
            //    entity.HasKey(e => new { e.LaneId, e.TransactionNumber });

            //    entity.ToTable(tb => tb.HasTrigger("trgTransaction_Insert"));

            //    // other configurations...
            //});
            // Optional trigger creation (run only once or check existence)
    //        Database.ExecuteSqlRaw(@"
    //    IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'trgTransaction_Insert')
    //    BEGIN
    //        EXEC('
    //            CREATE TRIGGER trgTransaction_Insert
    //            ON [Transaction]
    //            AFTER INSERT
    //            AS
    //            BEGIN
    //                -- Trigger logic here
    //            END
    //        ')
    //    END
    //");
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
