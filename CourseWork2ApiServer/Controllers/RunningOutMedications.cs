using System.ComponentModel.DataAnnotations;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class RunningOutMedications : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public RunningOutMedications(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    public class OutputRunningOutMedications
    {
        public int Remaining { get; set; }
        public Drug Drug { get; set; }
        public int LeftToTake { get; set; }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutputRunningOutMedications>>> Get()
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);

        var list = await db.DoctorsAppointments
            .Where(da => da.PatientId == oAuth.PatientId)
            .Include(da => da.PrescribedMedications)
            .SelectMany(da => da.PrescribedMedications
                .Select(pm => new {PM = pm, Date = da.DateTime}))
            .Where(pm => pm.PM.TakeMedicineBeforeTheDate >= DateTime.Now)
            .Select(pm => new {pm.PM, DaysRemaining = (pm.PM.TakeMedicineBeforeTheDate - pm.Date).Days})
            .ToListAsync();
        var receptionTimeDuringTheDay = list
            .Where(pm => pm.PM.ReceptionTimeDuringTheDay)
            .GroupBy(pm => pm.PM.Drug)
            .Select(g => new
            {
                Drug = g.Key,
                LeftToTake = list.Max(pm => pm.DaysRemaining)
            });
        var receptionTimeInTheEvening = list
            .Where(pm => pm.PM.ReceptionTimeInTheEvening)
            .GroupBy(pm => pm.PM.Drug)
            .Select(g => new
            {
                Drug = g.Key,
                LeftToTake = list.Max(pm => pm.DaysRemaining)
            });
        var receptionTimeInTheMorning = list
            .Where(pm => pm.PM.ReceptionTimeInTheMorning)
            .GroupBy(pm => pm.PM.Drug)
            .Select(g => new
            {
                Drug = g.Key,
                LeftToTake = list.Max(pm => pm.DaysRemaining)
            });
        return new ActionResult<IEnumerable<OutputRunningOutMedications>>(receptionTimeDuringTheDay
            .Concat(receptionTimeInTheEvening).Concat(receptionTimeInTheMorning)
            .GroupBy(r => r.Drug)
            .Select(g => new
            {
                LeftToTake = g.Sum(r => r.LeftToTake),
                Drug = g.Key
            })
            .Join(PatientMedicationsRemainingData.Data(db, oAuth),
                s => s.Drug,
                pmr => pmr.Drug,
                (s, pmr) => new OutputRunningOutMedications()
                {
                    Drug = pmr.Drug,
                    Remaining = pmr.Remaining,
                    LeftToTake = s.LeftToTake
                })
            .Where(pmr => pmr.Remaining < pmr.LeftToTake));
    }
}