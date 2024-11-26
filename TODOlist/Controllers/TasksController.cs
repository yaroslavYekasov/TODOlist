// Controllers/TasksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TODOlist.Data;
using Microsoft.AspNetCore.Http;

namespace TODOlist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AuthContext _context;

        public TasksController(AuthContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId.Value)
                .ToListAsync();

            return tasks;
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTaskItem([FromBody] TaskItemDto taskItemDto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taskItem = new TaskItem
            {
                Date = taskItemDto.Date,
                Subject = taskItemDto.Subject,
                Title = taskItemDto.Title,
                Description = taskItemDto.Description,
                UserId = userId.Value
            };

            _context.Tasks.Add(taskItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
        }


        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var taskItem = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId.Value);

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, TaskItem taskItem)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            if (id != taskItem.Id)
            {
                return BadRequest();
            }

            if (taskItem.UserId != userId.Value)
            {
                return Unauthorized("You cannot modify this task.");
            }

            _context.Entry(taskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id))
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

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var taskItem = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId.Value);

            if (taskItem == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskItemExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
