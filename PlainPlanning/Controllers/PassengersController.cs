using Microsoft.AspNetCore.Mvc;
using PlanePlanning.Interfaces;
using PlanePlanning.Models;
using System.ComponentModel.DataAnnotations;

namespace PlanePlanning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengersController : ControllerBase
    {
        private readonly IPassengersDistributionService _passengersDistributionService;

        public PassengersController(IPassengersDistributionService passengersDistributionService)
        {
            _passengersDistributionService = passengersDistributionService;
        }

      // POST api/passengers
        [HttpPost]
        public ActionResult<string> Post([FromBody] IEnumerable<Passenger> passengers)
        {
            try
            {
                string passangersDistribution = _passengersDistributionService.getPassangersDistribution(passengers);
                return passangersDistribution;
            }
            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
            
        }
    }
}
