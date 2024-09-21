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
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: api/Items
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        //{
        //    return await _itemService.GetAllAsync();
        //}

        [HttpPost("create-index")]
        public async Task<IActionResult> CreateIndex(string indexName)
        {
            await _itemService.CreateIndexIfNotExistsAsync(indexName);
            return Ok($"{indexName} was created");
        }


        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ItemDto>>> GetPaginatedItems([FromQuery] int skip = 0, [FromQuery] int limit = 10)
        {
            var paginatedEntities = await _itemService.GetAllPaginatedAsync(skip, limit);

            string? nextLink = String.Empty;
            if (limit <= paginatedEntities.MappedEntities.Count())
            {
                nextLink = Url.Action(nameof(GetPaginatedItems), new { skip = skip + limit, limit });
            }
            paginatedEntities.NextLink = nextLink;
            

            return paginatedEntities;
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(Guid id)
        {
            var item = await _itemService.FindByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        // PUT: api/Items/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(Guid id, ItemDto item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            try
            {
                await _itemService.Update(item);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ItemExists(id))
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

        // POST: api/Items
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(ItemDto item)
        {
            var itemDto = await _itemService.Insert(item);

            return CreatedAtAction("GetItem", new { id = itemDto.Id }, itemDto);
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _itemService.FindByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await  _itemService.DeleteAsync(item);

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ItemDto>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var items = await _itemService.SearchAsync(query);
            return Ok(items);
        }

        private async Task<bool> ItemExists(Guid id)
        {
            return await _itemService.FindByIdAsync(id) != null;
        }
    }
}
