using System.ComponentModel.DataAnnotations;
using _10xTempo.Data;
using _10xTempo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _10xTempo.Pages.Companies;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public CreateCompanyInput Input { get; set; } = new();

    public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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

        var company = new Company { Name = Input.Name };
        _context.Companies.Add(company);
        _context.UserCompanies.Add(new UserCompany
        {
            CompanyId = company.Id,
            UserId = userId,
            Role = CompanyRole.Admin
        });

        await _context.SaveChangesAsync();
        TempData["Flash"] = "Firma utworzona. Udostępnij GUID członkom zespołu.";
        return RedirectToPage("Index");
    }
}

public class CreateCompanyInput
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}
