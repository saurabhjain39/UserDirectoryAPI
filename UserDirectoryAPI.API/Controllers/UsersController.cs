using Microsoft.AspNetCore.Mvc;
using UserDirectoryAPI.Application.DTOs;
using UserDirectoryAPI.Application.Interfaces;
using UserDirectoryAPI.Domain.Entities;

namespace UserDirectoryAPI.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository repository, ILogger<UsersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET /api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching all users from database");
            var users = await _repository.GetAllAsync();
            return Ok(users);
        }

        // GET /api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("Fetching user with ID {UserId}", id);
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound(new { Message = $"User with ID {id} not found" });
            }
            return Ok(user);
        }

        // POST /api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = createUserDto.Name,
                Age = createUserDto.Age,
                City = createUserDto.City,
                State = createUserDto.State,
                Pincode = createUserDto.Pincode
            };

            await _repository.AddAsync(user);

            var dto = new UserDto(user.Id, user.Name, user.Age, user.City, user.State, user.Pincode);
            _logger.LogInformation("Created user {UserId}", user.Id);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, dto);
        }

        // PUT /api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateUserDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _repository.GetByIdAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning("User {UserId} not found for update", id);
                return NotFound(new { Message = $"User {id} not found" });
            }

            existingUser.Name = updateDto.Name;
            existingUser.Age = updateDto.Age;
            existingUser.City = updateDto.City;
            existingUser.State = updateDto.State;
            existingUser.Pincode = updateDto.Pincode;

            await _repository.UpdateAsync(existingUser);
            _logger.LogInformation("Updated user {UserId}", id);

            return NoContent(); 
        }

        // DELETE /api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingUser = await _repository.GetByIdAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning("User {UserId} not found for deletion", id);
                return NotFound(new { Message = $"User {id} not found" });
            }

            await _repository.DeleteAsync(existingUser);
            _logger.LogInformation("Deleted user {UserId}", id);

            return NoContent();
        }
    }
}
