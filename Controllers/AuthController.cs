using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using User_Email_Verification.Dtos;
using User_Email_Verification.Service;

namespace User_Email_Verification.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
            
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<string>>> Register([FromBody]UserRegisterDto userDto) {
            if(!ModelState.IsValid) {
                return BadRequest(userDto);
            }

            return Ok(await _authService.Register(new User{Username=userDto.Username, Email = userDto.Email}, userDto.Password));
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody]UserLogin userDto) {
            if(!ModelState.IsValid) {
                return BadRequest(userDto);
            }

            return Ok(await _authService.Login(userDto.Email, userDto.Password));
        }

    }
}