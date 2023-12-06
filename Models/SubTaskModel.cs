using System;
using System.ComponentModel.DataAnnotations;

namespace Munkanaplo2.Models
{
	public class SubTaskModel
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string TaskTitle { get; set; }

		[Required]
		public string TaskDetails { get; set; }

		[Required]
		public int JobId { get; set; }

		[Required]
		public string TaskCreator { get; set; }

		[Required]
		public string TaskCreationDate { get; set; }
	}
}