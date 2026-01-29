using _10xTempo.Data;
using _10xTempo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _10xTempo.Pages.Reports;

[Authorize]
public class ApproveModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ApproveModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IReadOnlyList<PendingReport> PendingReports { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadPending();
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            return Challenge();
        }

        var adminCompanyIds = await _context.UserCompanies
            .Where(uc => uc.UserId == userId && uc.Role == CompanyRole.Admin)
            .Select(uc => uc.CompanyId)
            .ToListAsync();

        var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == id && adminCompanyIds.Contains(r.CompanyId));
        if (report is null)
        {
            return Forbid();
        }

        report.IsApproved = true;
        report.UpdatedOn = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Flash"] = "Raport zatwierdzony.";
        return RedirectToPage();
    }

    private async Task LoadPending()
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            PendingReports = [];
            return;
        }

        var adminCompanyIds = await _context.UserCompanies
            .Where(uc => uc.UserId == userId && uc.Role == CompanyRole.Admin)
            .Select(uc => uc.CompanyId)
            .ToListAsync();

        var pendingReports = await _context.Reports
            .Where(r => !r.IsApproved && adminCompanyIds.Contains(r.CompanyId))
            .ToListAsync();

        var companyIds = pendingReports.Select(r => r.CompanyId).Distinct().ToList();
        var companies = await _context.Companies
            .Where(c => companyIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        var userIds = pendingReports.Select(r => r.UserId).Distinct().ToList();
        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? "unknown");

        PendingReports = pendingReports
            .Select(r => new PendingReport(
                r.Id,
                companies.TryGetValue(r.CompanyId, out var name) ? name : "Unknown",
                users.TryGetValue(r.UserId, out var email) ? email : "unknown",
                r.Hours,
                r.CreatedOn))
            .OrderByDescending(r => r.CreatedOn)
            .ToList();
    }
}

public record PendingReport(int Id, string CompanyName, string UserEmail, decimal Hours, DateTimeOffset CreatedOn);
