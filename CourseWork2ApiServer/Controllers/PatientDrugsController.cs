using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientDrugsController : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public PatientDrugsController(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    public class OutputPatientDrugs
    {
        [Required] public Drug Drug { get; set; }

        [Required] public int Remaining { get; set; }

        [Required] public DateTime DateOfManufacture { get; set; }

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutputPatientDrugs>>> Get()
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);

        return new(db.PatientsDrugs
            .Where(pd => pd.PatientId == oAuth.PatientId & pd.Remaining > 0)
            .Include(pd => pd.Drug)
            .Select(pd => new OutputPatientDrugs()
            {
                Drug = pd.Drug,
                Remaining = pd.Remaining,
                DateOfManufacture = pd.DateOfManufacture
            }));
    }
}