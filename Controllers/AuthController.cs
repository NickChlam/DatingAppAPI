using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;

        }

        // [Authorize]
        // [HttpGet("users")]
        // public async Task<IActionResult> GetUsers()
        // {
        //    var values = await _repo.GetUsers();
        //    return Ok(values);

        // }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegister UserForRegister)
        {
            //convert username to lowercase string if there is one
            if (!string.IsNullOrEmpty(UserForRegister.UserName))
                UserForRegister.UserName = UserForRegister.UserName.ToLower();

            if (await _repo.UserExists(UserForRegister.UserName))
                ModelState.AddModelError("UserName", "Username Already Exists");

            // validate request 
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userToCreate = _mapper.Map<User>(UserForRegister);

            var createdUser = await _repo.Register(userToCreate, UserForRegister.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

            return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.Id},userToReturn );

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
        {



            var userFromRepo = await _repo.Login(userForLoginDto.UserName.ToLower(), userForLoginDto.Password);

            // if (userFromRepo == null)
            // ModelState.AddModelError("UserName", "Username doesnt exist"); 

            if (userFromRepo == null)
            {
                // throw new Exception( "login Invalid"); 
                return Unauthorized();
            }

            //generate JSON web Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                        new Claim(ClaimTypes.Name, userFromRepo.UserName)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };
            // create token 
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var user = _mapper.Map<UserForListDto>(userFromRepo);


                return Ok(new { tokenString, user });




        }
    }
}