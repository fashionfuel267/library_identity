using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class ROleVm
    {
        [Required(ErrorMessage = "Role name is required.")]
        public string? Name { get; set; }
        public string? ID { get; set; }
    }
}
