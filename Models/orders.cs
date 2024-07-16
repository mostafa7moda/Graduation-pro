using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication5.Models
{
    public class orders
    {
        public int Id { get; set; }
        public int bookId { get; set; }
        public int userid { get; set; }
        public int quantity { get; set; }
        
        public string? CustomerName { get; set; }
        
        public string? BookTitel { get; set; }

        public DateTime orderdate { get; set; }

    }
}
