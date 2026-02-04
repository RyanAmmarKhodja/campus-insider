using campus_insider.DTOs;
using campus_insider.Models;
using campus_insider.Services;
using Microsoft.AspNetCore.Mvc;


namespace campus_insider.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(long id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
        }


        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            var result = users.Select(e => new UserResponseDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Role = e.Role,
                CreatedAt = e.CreatedAt
            }).ToList();
            return Ok(result);
        }


        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create(
       [FromBody] UserCreateDto userDto)
        {
            //  normally OwnerId comes from auth (JWT)
            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Password = userDto.Password,
                Email = userDto.Email
            };

            var created = await _userService.CreateAsync(user);

            return CreatedAtAction(nameof(GetUserById), new { id = created.Id },
                new UserResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                });
        }



        [HttpPut]
        public async Task<ActionResult<UserUpdateDto>> Update([FromBody] UserUpdateDto userDto)
        {

            try
            {
                var user = new User
                {
                    Id = userDto.Id,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Password = userDto.Password,
                    Email = userDto.Email
                };
                
                
                await _userService.UpdateAsync(user);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
