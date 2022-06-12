using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientPrescribedMedicationsController : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public PatientPrescribedMedicationsController(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    public class OutputPatientPrescribedMedication
    {
        [Required] public Drug Drug { get; set; }
        [Required] public bool ReceptionTimeInTheMorning { get; set; }
        [Required] public bool ReceptionTimeDuringTheDay { get; set; }
        [Required] public bool ReceptionTimeInTheEvening { get; set; }
        [Required] public bool TakeBeforeMeals { get; set; }
        [Required] public bool TakeAfterMeals { get; set; }
        [Required] public bool TakeWithMeals { get; set; }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutputPatientPrescribedMedication>>> Get()
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
            .ThenInclude(pm => pm.Drug)
            .SelectMany(da => da.PrescribedMedications)
            .ToListAsync();
        return new(list
            .Where(pm => pm.TakeMedicineBeforeTheDate.Date >= DateTime.Today.Date)
            .GroupBy(pm => pm.Drug)
            .Select(g => new OutputPatientPrescribedMedication()
            {
                Drug = g.Key,
                ReceptionTimeDuringTheDay = g.Any(pm => pm.ReceptionTimeDuringTheDay),
                ReceptionTimeInTheEvening = g.Any(pm => pm.ReceptionTimeInTheEvening),
                ReceptionTimeInTheMorning = g.Any(pm => pm.ReceptionTimeInTheMorning),
                TakeAfterMeals = g.Any(pm => pm.TakeAfterMeals),
                TakeBeforeMeals = g.Any(pm => pm.TakeBeforeMeals),
                TakeWithMeals = g.Any(pm => pm.TakeWithMeals)
            }));
    }
}