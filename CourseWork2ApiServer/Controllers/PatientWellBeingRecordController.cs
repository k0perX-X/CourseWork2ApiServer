using System.ComponentModel.DataAnnotations;
using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientWellBeingRecordController : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public PatientWellBeingRecordController(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    public class WellBeingRecordRequest
    {
        public double Temperature { get; set; }

        public string Note { get; set; }
    }

    public class WellBeingRecordResponse
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }
        public double? Temperature { get; set; }

        public string? Note { get; set; }

        public int PatientId { get; set; }
    }

    [HttpPost]
    public async Task<ActionResult<WellBeingRecordResponse>> Post(WellBeingRecordRequest wellBeingRecord)
    {
        var db = new MyDbContext();
        string token = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(token))
            return Problem(statusCode: 401);
        var oAuth = await db.OAuths.FirstOrDefaultAsync(t => t.Token == token);
        if (oAuth == null)
            return Problem(statusCode: 403);

        WellBeingRecord newWellBeingRecord = new WellBeingRecord()
        {
            DateTime = DateTime.Now,
            Note = wellBeingRecord.Note,
            PatientId = oAuth.PatientId,
            Patient = db.Patients.First(p => p.Id == oAuth.PatientId),
            Temperature = wellBeingRecord.Temperature,
        };
        db.WellBeingRecords.Add(newWellBeingRecord);
        new Task(() => db.SaveChanges()).Start();
        return new(new WellBeingRecordResponse()
        {
            DateTime = newWellBeingRecord.DateTime, 
            Id = newWellBeingRecord.Id,
            PatientId = newWellBeingRecord.PatientId,
            Temperature = newWellBeingRecord.Temperature,
            Note = newWellBeingRecord.Note
        });
    }
}