using System.ComponentModel.DataAnnotations;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientProceduresController : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public PatientProceduresController(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    public class OutputPatientProcedure
    {
        [Required] public DateTime DateTime { get; set; }

        [Required] public Procedure Procedure { get; set; }
        [Required] public int ProcedureId { get; set; }

        public string? Note { get; set; }
    }

    [HttpGet]
    public IEnumerable<OutputPatientProcedure>? Get(string token, DateTime beforeDate)
    {
        var patient = Program.Db.OAuths.FirstOrDefault(t => t.Token == token)?.Patient;
        if (patient == null)
        {
            Response.StatusCode = 403;
            return null;
        }

        return patient.Procedures.Where(p => !p.Visited & p.DateTime < beforeDate & p.DateTime > DateTime.Today).Select(
            p => new OutputPatientProcedure()
            {
                DateTime = p.DateTime,
                ProcedureId = p.ProcedureId,
                Note = p.Note,
                Procedure = p.Procedure
            });
    }
}