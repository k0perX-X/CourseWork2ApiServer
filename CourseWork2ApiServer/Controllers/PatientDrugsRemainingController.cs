using System.ComponentModel.DataAnnotations;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Controllers;

public static class PatientDrugsRemainingData
{
    public static IEnumerable<PatientDrugsRemainingController.OutputPatientMedications> Data(MyDbContext db, OAuth oAuth)
    {
        return db.PatientsDrugs
            .Where(pd => pd.PatientId == oAuth.PatientId & pd.Remaining > 0)
            .GroupBy(pd => pd.DrugId)
            .Select(g => new 
                {
                    Remaining = g.Sum(pd => pd.Remaining),
                    DrugId = g.Key,
                    MinimalDateOfManufacture = g.Min(pd => pd.DateOfManufacture)
                })
            .Join(
                db.Drugs,
                n => n.DrugId,
                d => d.Id,
                (n, d) => new PatientDrugsRemainingController.OutputPatientMedications()
                {
                    Drug = d, 
                    MinimalDateOfManufacture = n.MinimalDateOfManufacture,
                    Remaining = n.Remaining,
                }).ToList();
    }
}

[ApiController]
[Route("[controller]")]
public class PatientDrugsRemainingController : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public PatientDrugsRemainingController(ILogger<OAuthController> logger)
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

        return new ActionResult<IEnumerable<OutputPatientMedications>>(PatientDrugsRemainingData.Data(db, oAuth));
    }

    public class DrugResponse
    {
        public int Id { get; set; }
        public DateTime DateOfManufacture { get; set; }
        public int Remaining { get; set; }
    }

    [HttpPost]
    public async Task<ActionResult<OutputPatientMedications>> Post(DrugResponse drug)
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);

        Drug? d = await db.Drugs.FirstOrDefaultAsync(d => d.Id == drug.Id);

        if (d == null)
            return Problem(statusCode: 404);

        db.PatientsDrugs.Add(new PatientsDrug()
        {
            DateOfManufacture = drug.DateOfManufacture,
            DrugId = drug.Id,
            Drug = d,
            PatientId = oAuth.PatientId,
            Patient = oAuth.Patient,
            Remaining = drug.Remaining,
        });
        await db.SaveChangesAsync();

        return PatientDrugsRemainingData.Data(db, oAuth).First(p => p.Drug == d);
    }
}