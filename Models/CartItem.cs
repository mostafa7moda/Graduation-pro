using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
	public class CartItem
	{
		public book Book { get; set; }
		public int Quantity { get; set; }

		public decimal Price => Book.price * Quantity;


	}
}
