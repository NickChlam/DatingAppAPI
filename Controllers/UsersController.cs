using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Data
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;

        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var users = await _repo.GetUsers(userParams);
            //map UserForListDto to users that we brought in from GetUsers()
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            // we have access to the response 

            Response.AddPagination(users.CurrentPage, userParams.PageSize, users.TotalCount, users.TotalPages);


            return Ok(usersToReturn);
        }

        
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int Id)
        {
            var user = await _repo.GetUser(Id);
            //map UserForDetailedDto to User 
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }

        // api/users/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForUpdateDto)
        {
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            // get value of user that sent request from the claimTypes
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            // get the user 
            var userFromRepo = await _repo.GetUser(id);
            // check to see if user is there 
            if(userFromRepo == null)
                return NotFound($"User not found with ID of {id}");
            // check if it is the same user 
            if(currentUserId != userFromRepo.Id)
                return Unauthorized();

            // take userforupdatedto map it into userfromrepo 
            _mapper.Map(userForUpdateDto, userFromRepo);

            if(await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed to save");
        }
    }
}