using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HelpdeskDAL;
using HelpdeskViewModels;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallController : ControllerBase
    {
        //[HttpGet("{email}")]
        //public async Task<IActionResult> GetByEmail(string email)
        //{
        //    try
        //    {
        //        EmployeeViewModel viewmodel = new() { Email = email };
        //        await viewmodel.GetByEmail();
        //        return Ok(viewmodel);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
        //    }
        //}

        [HttpGet("{problem}")]
        public async Task<IActionResult> GetByProblem(string problem)
        {
            try
            {
                CallViewModel viewmodel = new() { ProblemDescription = problem };
                await viewmodel.GetByProblem();
                return Ok(viewmodel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                CallViewModel viewmodel = new();
                List<CallViewModel> allEmployees = await viewmodel.GetAll();
                return Ok(allEmployees);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(CallViewModel viewmodel)
        {
            try
            {
                int retVal = (int)await viewmodel.Update();
                return retVal switch
                {
                    1 => Ok(new { msg = "Call #" + viewmodel.Id + " updated!" }),
                    -1 => Ok(new { msg = "Call #" + viewmodel.Id + " not updated!" }),
                    -2 => Ok(new { msg = "Data is stale for call #" + viewmodel.Id + " not updated!" }),
                    _ => Ok(new { msg = "Call #" + viewmodel.Id + " not updated!" }),
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                CallViewModel viewmodel = new CallViewModel { Id = id };
                int deletedRows = await viewmodel.Delete();
                if (deletedRows > 0)
                {
                    return Ok(new { msg = $"Call with ID #{id} deleted!" });
                }
                else
                {
                    return NotFound(new { msg = $"Call with ID #{id} not found!" });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(CallViewModel viewmodel)
        {
            try
            {
                await viewmodel.Add();
                return viewmodel.Id > 1
                ? Ok(new { msg = "Call #" + viewmodel.Id + " added!" })
                : Ok(new { msg = "Call #" + viewmodel.Id + " not added!" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }

    }
}
