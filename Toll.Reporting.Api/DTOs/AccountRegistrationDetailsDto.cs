namespace Toll.Reporting.Api.DTOs
{
    public class AccountRegistrationDetailsDto
    {
        public int? SystemUserId { get; set; }
        public string? SystemUserFirstName { get; set; }
        public string? SystemUserLastName { get; set; }

        public string? AccNr { get; set; }
        public string? Status { get; set; }

        public string? UserFirstName { get; set; }
        public string? UserLastName { get; set; }
        public string? CompanyName { get; set; }
        public string? Address { get; set; }

        public string? PrimaryContact { get; set; }
        public string? PrimaryEmail { get; set; }

        public DateTime? ActivationDate { get; set; }
        public decimal? Balance { get; set; }

        public string? IdentifierType { get; set; }
        public string? RegisteredIdentifier { get; set; }
        public string? NumberPlateDetails { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int? RegisterUserId { get; set; }
    }
}
