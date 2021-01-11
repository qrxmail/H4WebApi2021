using H4WebApi.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace H4WebApi.Services
{
    public class CommonService
    {
        public static User GetCurrentUser(HttpContext HttpContext)
        {
            byte[] currentUserByte = HttpContext.Session.Get("currentUser");
            string currentUserStr = currentUserByte == null ? null : Encoding.UTF8.GetString(currentUserByte);
            if (string.IsNullOrEmpty(currentUserStr) == false)
            {
                User currentUser = JsonConvert.DeserializeObject<User>(currentUserStr);
                return currentUser;
            }
            else
            {
                User currentUser = new User();
                currentUser.Status = "error";
                currentUser.CurrentAuthority = "guest";
                return currentUser;
            }
        }
    }
}
