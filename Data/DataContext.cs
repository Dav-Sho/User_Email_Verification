using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace User_Email_Verification.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
            
        }

        public DbSet<User> Users => Set<User>();
        
        
    }
}