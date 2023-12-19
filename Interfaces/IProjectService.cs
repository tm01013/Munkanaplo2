using Munkanaplo2.Data;
using Munkanaplo2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Munkanaplo2.Services
{
    public interface IProjectService
    {
        List<ProjectModel> GetProjectsAsync();
        List<string> GetProjectMembers(int id);
    }
}
