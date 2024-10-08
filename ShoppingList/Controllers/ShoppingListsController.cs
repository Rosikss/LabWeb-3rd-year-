﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LabWeb.Context;
using LabWeb.DTOs;
using LabWeb.DTOs.ShoppingListDTO;
using LabWeb.Models;
using LabWeb.Services.Interfaces;
using LabWeb.Services;

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


        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ShoppingListResponse>>> GetPaginatedItems([FromQuery] int skip = 0, [FromQuery] int limit = 10)
        {
            var paginatedEntities = await _shoppingListService.GetAllPaginatedAsync(skip, limit);

            string? nextLink = String.Empty;
            if (limit <= paginatedEntities.Entities.Count())
            {
                nextLink = Url.Action(nameof(GetPaginatedItems), new { skip = skip + limit, limit });
            }
            paginatedEntities.NextLink = nextLink;


            return paginatedEntities;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingListResponse>> GetShoppingList(Guid id)
        {
            var shoppingList = await _shoppingListService.FindByIdAsync(id);

            if (shoppingList == null)
            {
                return NotFound();
            }

            return shoppingList;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutShoppingList(Guid id, ShoppingListResponse shoppingList)
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

        [HttpPost]
        public async Task<ActionResult<ShoppingListResponse>> PostShoppingList(ShoppingListRequest shoppingList)
        {
            var shoppingListDto = await _shoppingListService.Insert(shoppingList);

            return CreatedAtAction("GetShoppingList", new { id = shoppingListDto.Id }, shoppingListDto);
        }

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
