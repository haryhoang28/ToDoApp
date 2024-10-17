using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.BL;
using ToDoApp.Model;

namespace ToDoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase where T : IBaseModel, ICreationInfo
    {
        protected IBaseBL _bl;

        public BaseController(IBaseBL bl)
        {
            _bl = bl;
        }


        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] T model)
        {
            try
            {
                var res = await _bl.InsertOne(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }
    }
}
