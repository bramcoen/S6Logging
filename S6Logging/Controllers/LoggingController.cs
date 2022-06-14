using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace S6Logging.Controllers
{
    public class LoggingController : Controller
    {

        private readonly ValidationSettings _validationSettings;
        private readonly ILoggingRepository _loggingRepository;
        private readonly IUserRepository _userRepository;
        public LoggingController(ValidationSettings validationSettings, ILoggingRepository loggingRepository,IUserRepository userRepository)
        {
            _validationSettings = validationSettings;
            _loggingRepository = loggingRepository;
            _userRepository = userRepository;
        }

        // GET: LoggingController
        [HttpGet()]
        public async Task<IActionResult> GetLoggingActions([FromHeader] string token)
        {
            Payload? payload = await GoogleJsonWebSignature.ValidateAsync(token, _validationSettings);

            if (token == null) throw new Exception("Call can't be done while the user is not logged in.");
            var user = await _userRepository.GetByEmail(payload.Email);
            
            var loggingActions = await _loggingRepository.GetForUser(user.Id);
            return Ok(loggingActions);
        }
    }
}
