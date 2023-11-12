using System;
using System.ComponentModel.DataAnnotations;

namespace Munkanaplo2.Models
{
    public class ProjectModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProjectTitle { get; set; }
        public List<ProjectMembership> ProjectMembers { get; set; }
        [Required]
        public string ProjectCreator { get; set; }
    }
}
