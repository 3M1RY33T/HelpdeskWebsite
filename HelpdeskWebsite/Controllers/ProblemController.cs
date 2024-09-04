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
    public class ProblemController : ControllerBase
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

        [HttpGet("{description}")]
        public async Task<IActionResult> GetByDescription(string desc)
        {
            try
            {
                ProblemViewModel viewmodel = new() { Description = desc };
                await viewmodel.GetByDescription();
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
                ProblemViewModel viewmodel = new();
                List<ProblemViewModel> allEmployees = await viewmodel.GetAll();
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
        public async Task<ActionResult> Put(ProblemViewModel viewmodel)
        {
            try
            {
                int retVal = (int)await viewmodel.Update();
                return retVal switch
                {
                    1 => Ok(new { msg = "Problem " + viewmodel.Id + " updated!" }),
                    -1 => Ok(new { msg = "Problem " + viewmodel.Id + " not updated!" }),
                    -2 => Ok(new { msg = "Data is stale for problem #" + viewmodel.Id + " not updated!" }),
                    _ => Ok(new { msg = "Problem " + viewmodel.Id + " not updated!" }),
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
                ProblemViewModel viewmodel = new ProblemViewModel { Id = id };
                int deletedRows = await viewmodel.Delete();
                if (deletedRows > 0)
                {
                    return Ok(new { msg = $"Problem with ID #{id} deleted!" });
                }
                else
                {
                    return NotFound(new { msg = $"Problem with ID #{id} not found!" });
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
        public async Task<ActionResult> Post(ProblemViewModel viewmodel)
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
