using System.ComponentModel.DataAnnotations;

namespace Munkanaplo2.Models
{
    public class ProjectMembership
    {
        [Key]
        public int ProjectMembershipId { get; set; }
        public int ProjectId { get; set; }
        public string Member { get; set; }
        public ProjectModel Project { get; set; }
    }
}