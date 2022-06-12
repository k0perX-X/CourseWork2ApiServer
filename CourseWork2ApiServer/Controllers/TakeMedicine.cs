using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class TakeMedicine : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public TakeMedicine(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    [HttpPut]
    public async Task<ActionResult<bool>> Get(int drugId, int numberOfDoses)
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);

        var patientsDrugs = await db.PatientsDrugs
            .Include(pd => pd.Drug)
            .Where(pd => pd.Drug.Id == drugId & pd.Remaining > 0).ToListAsync();
        if (patientsDrugs.Count == 0 | patientsDrugs.Sum(pd => pd.Remaining) < numberOfDoses)
            return new ActionResult<bool>(false);
        patientsDrugs.Sort((x, y) => x.DateOfManufacture.CompareTo(y.DateOfManufacture));
        foreach (PatientsDrug patientsDrug in patientsDrugs)
        {
            if (patientsDrug.Remaining <= numberOfDoses)
            {
                numberOfDoses -= patientsDrug.Remaining;
                db.PatientsDrugs.Remove(patientsDrug);
                if (numberOfDoses == 0)
                    break;
            }
            else
            {
                patientsDrug.Remaining -= numberOfDoses;
                db.Entry(patientsDrug).State = EntityState.Modified;
                break;
            }
        }
        await db.SaveChangesAsync();
        return new ActionResult<bool>(true);
    }
}