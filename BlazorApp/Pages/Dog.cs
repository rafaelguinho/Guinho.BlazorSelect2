using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Pages
{
    public class Dog
    {
        [Required]
        public string Race { get; set; }
    }
}
