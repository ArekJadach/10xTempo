using System.ComponentModel.DataAnnotations;
using _10xTempo.Data;
using _10xTempo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _10xTempo.Pages.Companies;

[Authorize]
public class JoinModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public JoinInput Input { get; set; } = new();

    public JoinModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            return Challenge();
        }

        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == Input.CompanyId);
        if (company is null)
        {
            ModelState.AddModelError(string.Empty, "Firma o podanym GUID nie istnieje.");
            return Page();
        }

        var exists = await _context.UserCompanies.AnyAsync(uc => uc.CompanyId == company.Id && uc.UserId == userId);
        if (exists)
        {
            TempData["Flash"] = "Już należysz do tej firmy.";
            return RedirectToPage("Index");
        }

        _context.UserCompanies.Add(new UserCompany
        {
            CompanyId = company.Id,
            UserId = userId,
            Role = CompanyRole.Employee
        });
        await _context.SaveChangesAsync();

        TempData["Flash"] = "Dołączono do firmy.";
        return RedirectToPage("Index");
    }
}

public class JoinInput
{
    [Required]
    public Guid CompanyId { get; set; }
}
