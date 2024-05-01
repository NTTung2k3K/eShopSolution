using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.Catalog.Users
{
    public static class UserRole
    {
        public static string ADMIN = "Admin";
        public static string MANAGER = "Manager";
        public static string EMPLOYEE = "Employee";
        public static string CUSTOMER = "Customer";

        public static async Task<List<string>> getListRoleAsync()
        {
            var listRole = new List<string>();
            listRole.Add(ADMIN);
            listRole.Add(MANAGER);
            listRole.Add(CUSTOMER);
            listRole.Add(EMPLOYEE);
            return listRole;
        }
    }
}
