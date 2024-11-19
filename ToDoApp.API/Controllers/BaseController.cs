using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.BL;
using ToDoApp.Model;

namespace ToDoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase where T : IBaseModel, ICreationInfo, IModificationInfo
    {
        protected IBaseBL _bl;

        public BaseController(IBaseBL bl)
        {
            _bl = bl;
        }


        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] T model)
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] T model, int id)
        {
            try
            {
                model.SetPrimaryKey(id);
                var res = await _bl.UpdateOne(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var res = await _bl.DeleteOne<T>(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
