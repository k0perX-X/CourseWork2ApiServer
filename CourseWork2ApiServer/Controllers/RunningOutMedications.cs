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

        var doctorsAppointments = await db.DoctorsAppointments
            .Where(da => da.PatientId == oAuth.PatientId)
            .Include(da => da.PrescribedMedications)
            .ThenInclude(pm => pm.Drug)
            .ToListAsync();
        var list = doctorsAppointments
            .SelectMany(da => da.PrescribedMedications
                .Select(pm => new {PM = pm, Date = da.DateTime}))
            .Where(pm => pm.PM.TakeMedicineBeforeTheDate >= DateTime.Now)
            .Select(pm => new {pm.PM, DaysRemaining = (pm.PM.TakeMedicineBeforeTheDate - pm.Date).Days});
        var receptionTimeDuringTheDay = list
            .Where(pm => pm.PM.ReceptionTimeDuringTheDay)
            .GroupBy(pm => pm.PM.Drug.Id)
            .Select(g => new
            {
                DrugId = g.Key,
                LeftToTake = g.Max(pm => pm.DaysRemaining)
            }).ToList();
        var receptionTimeInTheEvening = list
            .Where(pm => pm.PM.ReceptionTimeInTheEvening)
            .GroupBy(pm => pm.PM.Drug.Id)
            .Select(g => new
            {
                DrugId = g.Key,
                LeftToTake = g.Max(pm => pm.DaysRemaining)
            }).ToList();
        var receptionTimeInTheMorning = list
            .Where(pm => pm.PM.ReceptionTimeInTheMorning)
            .GroupBy(pm => pm.PM.Drug.Id)
            .Select(g => new
            {
                DrugId = g.Key,
                LeftToTake = g.Max(pm => pm.DaysRemaining)
            }).ToList();
        var r1 = receptionTimeDuringTheDay
            .Concat(receptionTimeInTheEvening).Concat(receptionTimeInTheMorning);
        var r2 = r1.GroupBy(r => r.DrugId)
            .Select(g => new
            {
                LeftToTake = g.Sum(r => r.LeftToTake),
                DrugId = g.Key
            });

        var r4 =
            from s in r2
            join pmr in PatientDrugsRemainingData.Data(db, oAuth)
                on s.DrugId equals pmr.Drug.Id into gj
            from pmr in gj.DefaultIfEmpty()
            select new OutputRunningOutMedications()
            {
                Drug = pmr?.Drug ?? db.Drugs.First(d => s.DrugId == d.Id),
                Remaining = pmr?.Remaining ?? 0,
                LeftToTake = s.LeftToTake
            };

        var r5 = r4.ToList();

        //var r3 = r2.Join(PatientDrugsRemainingData.Data(db, oAuth),
        //        s => s.DrugId,
        //        pmr => pmr.Drug.Id,
        //        (s, pmr) => new OutputRunningOutMedications()
        //        {
        //            Drug = pmr.Drug,
        //            Remaining = pmr.Remaining,
        //            LeftToTake = s.LeftToTake
        //        })
        //    .Where(pmr => pmr.Remaining < pmr.LeftToTake)
        //    .ToList();
        return new(r5);
    }
}