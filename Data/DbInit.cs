using CMS.Models;
using System.Linq;

namespace CMS.Data
{
    public static class DbInit
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Sites.Count() == 2)
            {
                return;   // DB has been seeded
            }

            // var site = new Site { Name="Site1", Url="www.home/site.com", UserID="a67b88b9-520f-4c3d-bd04-1c26ec959a4f" };
            // context.Sites.Add(site);
            


            context.SaveChanges();
        }
    }
}