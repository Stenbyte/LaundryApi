using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaundryApi.Controllers{

    [Authorize]
    [Route("api/protected")]
    [ApiController]
    public class ProtectedController : ControllerBase{

        [HttpGet]
        public IActionResult GetProtectedData(){
            return Ok(new {message = "You have accessed a protoected resource!!!"});
        }
    }
}