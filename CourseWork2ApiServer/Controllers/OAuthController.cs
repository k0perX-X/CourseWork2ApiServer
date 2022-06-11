using CourseWork2ApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql.Replication;

namespace CourseWork2ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class OAuthController : ControllerBase
{
    private readonly ILogger<OAuthController> _logger;

    public OAuthController(ILogger<OAuthController> logger)
    {
        _logger = logger;
    }

    public static async Task<OAuth?> GenerateOAuth(string eMail, string password, string deviceInformation)
    {
        var db = new MyDbContext();
        Patient? patient = db.Patients.FirstOrDefault(p => p.Email == eMail);
        if (patient == null)
            return null;
        if (!BCrypt.Net.BCrypt.Verify(password, patient.Password))
            return null;
        if (patient.Tokens == null)
            patient.Tokens = new();
        OAuth? oAuth = patient.Tokens.FirstOrDefault(t => t.DeviceInformation == deviceInformation);
        if (oAuth == null)
        {
            oAuth = new OAuth()
            {
                CreateTime = DateTime.Now,
                DeviceInformation = deviceInformation,
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                Patient = patient,
                PatientId = patient.Id,
            };
            // while (Program.Db.OAuths.Count(o => oAuth.Token == o.Token) != 0)
            // {
            //     oAuth = new OAuth()
            //     {
            //         CreateTime = DateTime.Now,
            //         DeviceInformation = deviceInformation,
            //         Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            //         Patient = patient,
            //         PatientId = patient.Id,
            //     };
            // }
            patient.Tokens.Add(oAuth);
            // Program.Db.SaveChangesTask.Start();
            await db.SaveChangesAsync();
        }
        return oAuth;
    }

    public class TokenClass
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? MiddleName { get; set; }
    }

    public class PostClass
    {
        public string EMail { get; set; }
        public string Password { get; set; }
        public string DeviceInformation { get; set; }
    }

    [HttpPost]
    public async Task<ActionResult<TokenClass?>> Post(PostClass postClass)
    {
        // _logger.Log(LogLevel.Information, $"{postClass.EMail} {postClass.Password} {postClass.DeviceInformation}");
        OAuth? oAuth = await GenerateOAuth(postClass.EMail, postClass.Password, postClass.DeviceInformation);
        Response.Headers.Add("access-control-allow-credentials", "true");
        Response.Headers.Add("access-control-allow-headers", Request.Headers.Origin);
        Response.Headers.Add("cache-control", "private, no-cache, no-store, must-revalidate, max-age=0");
        Response.Headers.Add("strict-transport-security", "max-age=31536000");
        if (oAuth == null)
        {
            return Problem(statusCode:403);
        }
        return new TokenClass()
        {
            Token = oAuth.Token,
            Name = oAuth.Patient.Name,
            Surname = oAuth.Patient.Surname,
            MiddleName = oAuth.Patient.MiddleName,
        };
    }

    // [HttpGet]
    // public TokenClass? Get()
    // {
    // //     access - control - allow - credentials: true
    // //     access - control - allow - origin: https://yandex.ru
    // //     cache - control: private, no-cache, no-store, must-revalidate, max-age=0
    // // content-length: 351
    // // content-type: application/json; charset=utf-8
    // // date: Sun, 05 Jun 2022 11:23:06 GMT
    // //     expires: Sun, 05-Jun-2022 11:23:06 GMT
    // //     last-modified: Sun, 05-Jun-2022 11:23:06 GMT
    // //     pragma: no-cache
    // //     strict-transport-security: max-age=31536000
    // // x-content-type-options: nosniff
    // //     x-xss-protection: 1; mode=block
    //     Response.Headers.Add("X-Developed-By", "Your Name");
    //     Response.Headers.Add("access-control-allow-credentials", "true");
    //     Response.Headers.Add("access-control-allow-headers", Request.Headers.Origin);
    //     Response.Headers.Add("cache-control", "private, no-cache, no-store, must-revalidate, max-age=0");
    //     Response.Headers.Add("strict-transport-security", "max-age=31536000");
    //     return new TokenClass()
    //     {
    //         Token = "123",
    //         Name = "Name",
    //         Surname = "Surname"
    //     };
    // }
}