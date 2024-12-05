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
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return tasks;
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTaskItem([FromBody] TaskItemCreateDto taskItemDto)
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

            var taskItem = await _context.Tasks.FindAsync(id);

            if (taskItem == null || taskItem.UserId != userId.Value)
            {
                return NotFound();
            }

            return taskItem;
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

            var taskItem = await _context.Tasks.FindAsync(id);

            if (taskItem == null || taskItem.UserId != userId.Value)
            {
                return NotFound();
            }

            _context.Tasks.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, [FromBody] TaskItemUpdateDto taskItemDto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var taskItem = await _context.Tasks.FindAsync(id);

            if (taskItem == null || taskItem.UserId != userId.Value)
            {
                return NotFound();
            }

            // Update task item with the new values from the DTO
            taskItem.Date = taskItemDto.Date;
            taskItem.Subject = taskItemDto.Subject;
            taskItem.Title = taskItemDto.Title;
            taskItem.Description = taskItemDto.Description;

            _context.Entry(taskItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();  // No content as the task was successfully updated
        }
    }
}
