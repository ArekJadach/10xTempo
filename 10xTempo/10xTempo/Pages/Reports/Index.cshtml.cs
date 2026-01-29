using _10xTempo.Data;
using _10xTempo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _10xTempo.Pages.Reports;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IReadOnlyList<ReportListItem> Reports { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            Reports = [];
            return;
        }

        var userReports = await _context.Reports
            .Where(r => r.UserId == userId)
            .ToListAsync();

        var companyIds = userReports.Select(r => r.CompanyId).Distinct().ToList();
        var companies = await _context.Companies
            .Where(c => companyIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        Reports = userReports
            .Select(r => new ReportListItem(
                r.Id,
                companies.TryGetValue(r.CompanyId, out var name) ? name : "Unknown",
                r.Hours,
                r.CreatedOn,
                r.IsApproved))
            .OrderByDescending(r => r.CreatedOn)
            .ToList();
    }
}

public record ReportListItem(int Id, string CompanyName, decimal Hours, DateTimeOffset CreatedOn, bool IsApproved);
