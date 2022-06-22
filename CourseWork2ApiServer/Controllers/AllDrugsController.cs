using System.ComponentModel.DataAnnotations;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AllDrugsController: ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public AllDrugsController(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Drug>>> Get()
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);

        return db.Drugs;
    }
}