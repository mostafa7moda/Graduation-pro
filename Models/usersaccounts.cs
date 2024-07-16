using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class usersaccounts
    {
        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string pass { get; set; }
        [Required]
        public string? role { get; set; } 
        [Required]
        public string email { get; set; }
        
 
    }
}
