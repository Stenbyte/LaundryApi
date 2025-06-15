using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LaundryApi.Controllers;

namespace LaundryApi.Controllers
{

    [Authorize]
    [Route("api/protected")]
    public class ProtectedController : ControllerBase
    {

        private readonly AuthController _auth;

        public ProtectedController(AuthController auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult GetProtectedData()
        {

            var userData = _auth.GetUserInfo();
            return Ok(new { message = "You have accessed a protoected resource!!!", data = userData });
        }
    }
}