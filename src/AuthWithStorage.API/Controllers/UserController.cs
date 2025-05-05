using System.Text.Json;
using AuthWithStorage.Application.DTOs;
using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Domain.Queries;
using AuthWithStorage.Infrastructure.Cache;
using AuthWithStorage.Infrastructure.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthWithStorage.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User, int, UserSearchQuery> _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<UserDto> userValidator;
        private readonly CachingService cachingService;
        private readonly QueryHasher<UserSearchQuery> _queryHasher;

        public UserController(
            IRepository<User, int, UserSearchQuery> userRepository,
            IMapper mapper,
            IValidator<UserDto> userValidator,
            CachingService cachingService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            this.userValidator = userValidator;
            this.cachingService = cachingService;
            _queryHasher = new QueryHasher<UserSearchQuery>();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserSearchQuery query = null)
        {
            var queryHash = _queryHasher.GetHashKey(query);
            var key = $"_users_{queryHash}";

            var users = await cachingService.GetOrSetAsync(
                key,
                async () =>
                {
                    var dbUsers = await _userRepository.GetAllAsync(query);
                    return _mapper.Map<List<UserResponse>>(dbUsers);
                }
            );
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<UserResponse>(user));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] UserDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validationResult = await userValidator.ValidateAsync(user);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToDictionary());

            await _userRepository.AddAsync(_mapper.Map<User>(user));
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validationResult = await userValidator.ValidateAsync(user);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToDictionary());

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound();

            user.Id = id;
            await _userRepository.UpdateAsync(_mapper.Map<User>(user));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}