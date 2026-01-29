namespace _10xTempo.Models;

public enum CompanyRole
{
    Employee = 0,
    Admin = 1
}

public class Company
{
    public Guid Id { get; set; } = Guid.NewGuid(); // unikalny klucz do dołączania
    public string Name { get; set; } = string.Empty;
}

public class UserCompany
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public CompanyRole Role { get; set; } = CompanyRole.Employee;
}

public class Report
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }

    public decimal Hours { get; set; } // decimal(18,2)

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.UtcNow;

    public bool IsApproved { get; set; } = false;
}
