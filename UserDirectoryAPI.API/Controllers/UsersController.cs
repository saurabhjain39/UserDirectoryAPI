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

        public UsersController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _repository.GetAllAsync();

            var result = users.Select(u => new UserDto(
                u.Id,
                u.Name,
                u.Age,
                u.City,
                u.State,
                u.Pincode
            ));

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user is null)
                return NotFound();

            return Ok(new UserDto(
                user.Id,
                user.Name,
                user.Age,
                user.City,
                user.State,
                user.Pincode
            ));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Age = dto.Age,
                City = dto.City,
                State = dto.State,
                Pincode = dto.Pincode
            };

            await _repository.AddAsync(user);

            return CreatedAtAction(nameof(GetById),
                new { id = user.Id },
                new UserDto(
                    user.Id,
                    user.Name,
                    user.Age,
                    user.City,
                    user.State,
                    user.Pincode
                ));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, CreateUserDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user is null)
                return NotFound();

            user.Name = dto.Name;
            user.Age = dto.Age;
            user.City = dto.City;
            user.State = dto.State;
            user.Pincode = dto.Pincode;

            await _repository.UpdateAsync(user);

            return NoContent(); 
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user is null)
                return NotFound();

            await _repository.DeleteAsync(user);
            return NoContent();
        }
    }
}
