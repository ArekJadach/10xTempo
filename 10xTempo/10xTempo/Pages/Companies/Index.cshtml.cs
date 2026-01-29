using System.ComponentModel.DataAnnotations;
using _10xTempo.Data;
using _10xTempo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _10xTempo.Pages.Companies;

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

    public IReadOnlyList<CompanyListItem> Companies { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            Companies = [];
            return;
        }

        var memberships = await _context.UserCompanies
            .Where(uc => uc.UserId == userId)
            .ToListAsync();

        var companyIds = memberships.Select(m => m.CompanyId).ToList();
        var companies = await _context.Companies
            .Where(c => companyIds.Contains(c.Id))
            .ToListAsync();

        Companies = memberships
            .Join(companies,
                m => m.CompanyId,
                c => c.Id,
                (m, c) => new CompanyListItem(c.Id, c.Name, m.Role))
            .OrderBy(c => c.Name)
            .ToList();
    }
}

public record CompanyListItem(Guid Id, string Name, CompanyRole Role);
