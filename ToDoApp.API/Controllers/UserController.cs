using Microsoft.AspNetCore.Mvc;
using ToDoApp.BL;
using ToDoApp.Model;

namespace ToDoApp.API.Controllers
{
    public class UserController : BaseController<User>
    {
        public UserController(IUserBL bl) : base(bl)
        {
        }

    }
}
