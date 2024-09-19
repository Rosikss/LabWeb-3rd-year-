﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LabWeb.Context;
using LabWeb.DTOs;
using LabWeb.Models;
using LabWeb.Services.Interfaces;
using LabWeb.Services;

namespace LabWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        //{
        //    return await _userService.GetAllAsync();
        //}

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<UserDto>>> GetPaginatedItems([FromQuery] int skip = 0, [FromQuery] int limit = 10)
        {
            var paginatedEntities = await _userService.GetAllPaginatedAsync(skip, limit);

            string? nextLink = String.Empty;
            if (limit <= paginatedEntities.MappedEntities.Count())
            {
                nextLink = Url.Action(nameof(GetPaginatedItems), new { skip = skip + limit, limit });
            }
            paginatedEntities.NextLink = nextLink;


            return paginatedEntities;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _userService.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, UserDto user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }


            try
            {
                await _userService.Update(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDto user)
        {
            var userDto = await _userService.Insert(user);

            return CreatedAtAction("GetUser", new { id = userDto.Id }, userDto);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userService.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(user);

            return NoContent();
        }

        private async Task<bool> UserExists(Guid id)
        {
            return await _userService.FindByIdAsync(id) != null;
        }
    }
}
