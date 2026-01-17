using ASP_NET_06._Student_MVC___Pagination_Filtering_Ordering.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_06._Student_MVC___Pagination_Filtering_Ordering.Data
{
    public class StudentsDbContext : DbContext
    {
        public StudentsDbContext(DbContextOptions options) 
            : base(options)
        {}

        public DbSet<Student> Students => Set<Student>();
    }
}
