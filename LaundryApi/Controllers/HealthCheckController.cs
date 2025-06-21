using System.Text.Json;
using LaundryApi.Exceptions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using LaundryApi.Models;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    public class IsAliveController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        private static readonly string InfoFile = "build-info.json";

        public IsAliveController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult isAlive()
        {
            var path = Path.Combine(_env.ContentRootPath, InfoFile);

            if (!System.IO.File.Exists(path))
            {
                throw new CustomException("Build info file is not found", null, 404);
            }

            var json = System.IO.File.ReadAllText(path);

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return Ok(
                new IsAliveResponse {
                    Status = "Alive",
                    Commit = data?["commit"] ?? "something went wrong",
                    CommitDate = data?["commitDate"] ?? "",
                    BuildTime = data?["buildTime"] ?? "",
                }
            );
        }
    }
}