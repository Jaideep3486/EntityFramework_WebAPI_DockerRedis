using Microsoft.EntityFrameworkCore;
using webAPI2.Models;

namespace webAPI2.Data
{
    public class IssueDBContext :  DbContext 
    {
        
        public IssueDBContext(DbContextOptions<IssueDBContext> options) : base(options)
        {
            
        }

        public DbSet<Issue> Issues { get; set; }
    }
}
