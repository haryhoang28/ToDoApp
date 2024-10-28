using Microsoft.AspNetCore.Mvc;
using ToDoApp.BL;
using ToDoApp.Model;

namespace ToDoApp.API.Controllers
{
    public class ToDoController : BaseController<ToDo>
    {
        public ToDoController(IToDoBL bl) : base(bl)
        {
        }
    }
}
