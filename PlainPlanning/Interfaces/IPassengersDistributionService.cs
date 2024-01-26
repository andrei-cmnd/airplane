using PlanePlanning.Models;

namespace PlanePlanning.Interfaces
{
    public interface IPassengersDistributionService
    {
        public string getPassangersDistribution(IEnumerable<Passenger> passengers);
    }
}
