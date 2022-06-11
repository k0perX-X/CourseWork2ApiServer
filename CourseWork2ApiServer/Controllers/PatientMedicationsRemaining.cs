using System.ComponentModel.DataAnnotations;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Controllers;

public static class PatientMedicationsRemainingData
{
    public static IEnumerable<PatientMedicationsRemaining.OutputPatientMedications> Data(MyDbContext db, OAuth oAuth)
    {
        return db.PatientsDrugs
            .Include(pd => pd.Drug)
            .Where(pd => pd.PatientId == oAuth.PatientId & pd.Remaining > 0)
            .GroupBy(pd => pd.Drug)
            .Select(g => new PatientMedicationsRemaining.OutputPatientMedications()
                {
                    Remaining = g.Sum(pd => pd.Remaining),
                    Drug = g.Key,
                    MinimalDateOfManufacture = g.Min(pd => pd.DateOfManufacture)
                }
            );
    }
}

[ApiController]
[Route("[controller]")]
public class PatientMedicationsRemaining : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public PatientMedicationsRemaining(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    public class OutputPatientMedications
    {
        public int Remaining { get; set; }
        public Drug Drug { get; set; }
        public DateTime? MinimalDateOfManufacture { get; set; }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutputPatientMedications>>> Get()
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);
        //Select[Drug ID] as DrugID, SUM(Remaining) as Remaining, 
        //MIN([Date of manufacture]) as DateOfManufacture
        //    from(Select * from[Patient's drugs]
        //where[Patient ID] = @SelectedPatient and Remaining > 0) as pd
        //    group by[Drug ID]

        return new ActionResult<IEnumerable<OutputPatientMedications>>(PatientMedicationsRemainingData.Data(db, oAuth));
    }
}