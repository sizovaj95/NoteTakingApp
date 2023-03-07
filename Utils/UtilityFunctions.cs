using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class UtilityFunctions
    {
        public static string GetConnectionString()
        {
            var root = Path.GetPathRoot(Directory.GetCurrentDirectory());
            var path = Path.Combine(root, "Julia/Documents/Learning/connectionString.txt");
            string connString = File.ReadAllText(path);
            return connString;
        }
    }
}
