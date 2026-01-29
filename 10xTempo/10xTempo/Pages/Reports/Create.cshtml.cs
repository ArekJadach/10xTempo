using System.ComponentModel.DataAnnotations;
using _10xTempo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace _10xTempo.Pages.Reports;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public ReportInput Input { get; set; } = new();

    public List<SelectListItem> CompanyOptions { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadCompanies();
        if (!CompanyOptions.Any())
        {
            TempData["Flash"] = "Najpierw dołącz do firmy lub utwórz własną.";
            return RedirectToPage("/Companies/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadCompanies();
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            return Challenge();
        }

        var belongs = await _context.UserCompanies
            .AnyAsync(uc => uc.UserId == userId && uc.CompanyId == Input.CompanyId);
        if (!belongs)
        {
            ModelState.AddModelError(string.Empty, "Nie masz dostępu do wybranej firmy.");
            return Page();
        }

        var now = DateTimeOffset.UtcNow;
        _context.Reports.Add(new Models.Report
        {
            CompanyId = Input.CompanyId,
            UserId = userId,
            Hours = Input.Hours,
            CreatedOn = now,
            UpdatedOn = now
        });
        await _context.SaveChangesAsync();

        TempData["Flash"] = "Raport zapisany.";
        return RedirectToPage("Index");
    }

    private async Task LoadCompanies()
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            CompanyOptions = [];
            return;
        }

        var memberships = await _context.UserCompanies
            .Where(uc => uc.UserId == userId)
            .ToListAsync();

        var companyIds = memberships.Select(m => m.CompanyId).ToList();
        var companies = await _context.Companies
            .Where(c => companyIds.Contains(c.Id))
            .ToListAsync();

        CompanyOptions = memberships
            .Join(companies,
                m => m.CompanyId,
                c => c.Id,
                (m, c) => new SelectListItem { Text = $"{c.Name} ({m.Role})", Value = c.Id.ToString() })
            .OrderBy(x => x.Text)
            .ToList();
    }
}

public class ReportInput
{
    [Required]
    public Guid CompanyId { get; set; }

    [Range(0.25, 24, ErrorMessage = "Podaj liczbę godzin z zakresu 0.25 - 24.")]
    public decimal Hours { get; set; }
}
