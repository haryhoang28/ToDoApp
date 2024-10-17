using Microsoft.AspNetCore.Mvc;
using ToDoApp.BL;
using ToDoApp.Model;

namespace ToDoApp.API.Controllers
{
    public class GroupsController : BaseController<Group>
    {
        // Sort, 
        public GroupsController(IGroupBL bl) : base(bl)
        {
           
        }
    }
}
