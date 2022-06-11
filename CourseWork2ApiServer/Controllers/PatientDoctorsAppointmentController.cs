using System.ComponentModel.DataAnnotations;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientDoctorsAppointmentController : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public PatientDoctorsAppointmentController(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    public class OutputPatientDoctorsAppointment
    {
        [Required] public DateTime DateTime { get; set; }

        [Required] public Doctor Doctor { get; set; }

        public string? Note { get; set; }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutputPatientDoctorsAppointment>>> Get(DateTime beforeDate)
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);

        return new ActionResult<IEnumerable<OutputPatientDoctorsAppointment>>(db.DoctorsAppointments
            .Include(pp => pp.Doctor).Where(pp => pp.PatientId == oAuth.PatientId)
            .Where(p => !p.Visited & p.DateTime < beforeDate & p.DateTime > DateTime.Today)
            .Select(p => new OutputPatientDoctorsAppointment()
            {
                DateTime = p.DateTime,
                Note = p.Note,
                Doctor = p.Doctor
            }));
    }
}