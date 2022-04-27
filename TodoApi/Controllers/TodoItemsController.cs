#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    /// <summary>
    /// Todo Items API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems        
        /// <summary>
        /// Get Todo Items List
        /// </summary>
        /// <returns>Todo Items List</returns>
        /// <response code="200"></response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        /// <summary>
        /// Get Todo Item
        /// </summary>
        /// <param name="id">Todo Identifier</param>
        /// <returns>Todo Item</returns>
        /// <response code="200"></response>
        /// <response code="404">Todo Item Not Found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound("Todo Item Not Found");
            }

            return Ok(todoItem);
            //return todoItem;
        }

        // PUT: api/TodoItems/5
        /// <summary>
        /// Rewrites Todo Item
        /// </summary>
        /// <param name="id">Todo Identifier</param>
        /// <param name="todoItem"></param>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /TodoItems/1
        ///     {
        ///        "id": 1,
        ///        "name": "Item #1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <response code="204">Item Updated</response>
        /// <response code="404">Todo Item Not Found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest("The Todo Id doesn't match with the payload's value");
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound("Todo Item Not Found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
            //return Ok();
        }

        // POST: api/TodoItems
        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// 
        /// <param name="item"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /TodoItems
        ///     {
        ///        "id": 1,
        ///        "name": "Item #1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Todo Item Created</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]        
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        /// <summary>
        /// Deletes a TodoItem
        /// </summary>
        /// <param name="id">Todo Identifier</param>
        /// <response code="204">Item Updated</response>
        /// <response code="404">Todo Item Not Found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound("Todo Item Not Found");
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            //return StatusCode(204, new { message = $"Todo Item {id} Deleted" });
            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
