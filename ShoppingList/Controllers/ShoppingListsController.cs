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

namespace LabWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingListsController : ControllerBase
    {
        private readonly IShoppingListService _shoppingListService;

        public ShoppingListsController(IShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        // GET: api/ShoppingLists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetShoppingLists()
        {
            return await _shoppingListService.GetAllAsync();
        }

        // GET: api/ShoppingLists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingListDto>> GetShoppingList(Guid id)
        {
            var shoppingList = await _shoppingListService.FindByIdAsync(id);

            if (shoppingList == null)
            {
                return NotFound();
            }

            return shoppingList;
        }

        // PUT: api/ShoppingLists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShoppingList(Guid id, ShoppingListDto shoppingList)
        {
            if (id != shoppingList.Id)
            {
                return BadRequest();
            }


            try
            {
                await _shoppingListService.Update(shoppingList);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ShoppingListExists(id))
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

        // POST: api/ShoppingLists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShoppingListDto>> PostShoppingList(ShoppingListDto shoppingList)
        {
            var shoppingListDto = await _shoppingListService.Insert(shoppingList);

            return CreatedAtAction("GetShoppingList", new { id = shoppingListDto.Id }, shoppingListDto);
        }

        // DELETE: api/ShoppingLists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingList(Guid id)
        {
            var shoppingList = await _shoppingListService.FindByIdAsync(id);
            if (shoppingList == null)
            {
                return NotFound();
            }

            await _shoppingListService.DeleteAsync(shoppingList);

            return NoContent();
        }

        private async Task<bool> ShoppingListExists(Guid id)
        {
            return await _shoppingListService.FindByIdAsync(id) != null;
        }
    }
}
