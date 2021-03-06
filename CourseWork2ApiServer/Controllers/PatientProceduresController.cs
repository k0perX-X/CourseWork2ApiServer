using System.ComponentModel.DataAnnotations;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public string? Note { get; set; }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutputPatientProcedure>>> Get(DateTime beforeDate)
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);

        return new ActionResult<IEnumerable<OutputPatientProcedure>>(db.PatientProcedures
            .Include(pp => pp.Procedure).Where(pp => pp.PatientId == oAuth.PatientId)
            .Where(p => !p.Visited & p.DateTime < beforeDate & p.DateTime > DateTime.Today)
            .Select(p => new OutputPatientProcedure()
            {
                DateTime = p.DateTime,
                Note = p.Note,
                Procedure = p.Procedure
            }));
    }
}