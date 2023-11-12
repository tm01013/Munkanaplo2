using System;
using System.ComponentModel.DataAnnotations;

namespace Munkanaplo2.Models
{
    public class JobModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public string JobDescription { get; set; }

        [Required]
        public string JobOwner { get; set; }
        [Required]
        public string JobCreator { get; set; }

        [Required]
        public string CreationDate { get; set; }
        [Required]
        public string JobStatus { get; set; }
        public string FinishDate { get; set; }

        public int ProjectId { get; set; }

    }
}

