using System.ComponentModel.DataAnnotations;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientMedicineWillGoBadController : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public PatientMedicineWillGoBadController(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDrugsRemainingController.OutputPatientMedications>>> Get(
        DateTime beforeDate)
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);

        var list = await db.PatientsDrugs
            .Where(pd => pd.PatientId == oAuth.PatientId)
            .Include(pd => pd.Drug)
            .ToListAsync();
        var list2 = list
            .Where(pd => pd.Remaining > 0)
            .Where(pd =>
                    pd.DateOfManufacture.Date +
                    TimeSpan.FromDays(
                        (pd.Drug.ExplorationDate.Date.Month - 1 + (pd.Drug.ExplorationDate.Date.Year - 1) * 12) * 30.5 +
                        pd.Drug.ExplorationDate.Date.Day - 1) < beforeDate
                // & pd.DateOfManufacture.Date +
                // TimeSpan.FromDays(
                //     (pd.Drug.ExplorationDate.Date.Month - 1 + (pd.Drug.ExplorationDate.Date.Year - 1) * 12) * 30.5 +
                //     pd.Drug.ExplorationDate.Date.Day) > DateTime.Today
            );
        return new(list2.GroupBy(pd => pd.DrugId)
            .Select(g => new PatientDrugsRemainingController.OutputPatientMedications()
            {
                MinimalDateOfManufacture = g.Min(pd => pd.DateOfManufacture),
                Drug = g.First(pd => pd.DrugId == g.Key).Drug,
                Remaining = g.Sum(pd => pd.Remaining)
            })
            .ToList());
    }
}