using cfs.demo.Models;
using cfs.demo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace cfs.demo.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ICfsDatabase _db;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ICfsDatabase db, ILogger<UsersController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET /api/users
        //[Authorize(Roles = "userdb.read")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            _logger.LogInformation("GET /api/users requested.");
            var users = (await _db.GetAllAsync()).ToList();
            _logger.LogInformation("Returning {Count} users.", users.Count);
            return Ok(users);
        }

        // GET /api/users/{id}
        [Authorize(Roles = "userdb.read")]
        [HttpGet("{id:guid}", Name = "GetUserById")]
        public async Task<ActionResult<User>> GetById(Guid id)
        {
            _logger.LogInformation("GET /api/users/{Id} requested.", id);

            var user = await _db.GetByIdAsync(id);
            if (user is null)
            {
                _logger.LogWarning("User {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("User {Id} found.", id);
            return Ok(user);
        }

        // POST /api/users
        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] UserCreateDto dto)
        {
            _logger.LogInformation("POST /api/users requested. Creating user LastName={LastName}.", dto?.LastName);

            if (dto is null)
            {
                _logger.LogWarning("Create request body was null.");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create request model validation failed: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Age = dto.Age,
                City = dto.City,
                State = dto.State,
                Pincode = dto.Pincode
            };

            await _db.CreateAsync(user);

            _logger.LogInformation("User {Id} created. Age={Age}, City={City}, State={State}.", user.Id, user.Age, user.City, user.State);

            return CreatedAtRoute("GetUserById", new { id = user.Id }, user);
        }


        // PUT /api/users/{id}
        [Authorize(Roles = "userdb.write")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
        {
            _logger.LogInformation("PUT /api/users/{Id} requested.", id);

            if (dto is null)
            {
                _logger.LogWarning("Update request body was null for user {Id}.", id);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Update request model validation failed for user {Id}: {@ModelState}", id, ModelState);
                return BadRequest(ModelState);
            }

            var existing = await _db.GetByIdAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("User {Id} not found for update.", id);
                return NotFound();
            }

            existing.FirstName = dto.FirstName ?? existing.FirstName;
            existing.LastName = dto.LastName ?? existing.LastName;
            existing.Age = dto.Age ?? existing.Age;
            existing.City = dto.City ?? existing.City;
            existing.State = dto.State ?? existing.State;
            existing.Pincode = dto.Pincode ?? existing.Pincode;

            var updated = await _db.UpdateAsync(existing);
            if (!updated)
            {
                _logger.LogWarning("Failed to update user {Id}.", id);
                return NotFound();
            }

            _logger.LogInformation("User {Id} updated. Age={Age}, City={City}, State={State}.", id, existing.Age, existing.City, existing.State);
            return NoContent();
        }

        // DELETE /api/users/{id}
        [Authorize(Roles = "userdb.write")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            _logger.LogInformation("DELETE /api/users/{Id} requested.", id);

            var deleted = await _db.DeleteAsync(id);
            if (deleted)
            {
                _logger.LogInformation("User {Id} deleted.", id);
                return NoContent();
            }

            _logger.LogWarning("User {Id} not found for deletion.", id);
            return NotFound();
        }
    }
}