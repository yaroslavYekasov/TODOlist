using System.ComponentModel.DataAnnotations;

namespace TODOlist.Data
{
    public class TaskItemUpdateDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        public string Description { get; set; }
    }

}
