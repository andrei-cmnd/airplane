using System.Xml.Linq;

namespace PlanePlanning.Models
{
    public class Passenger
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string? FamilyName { get; set; }
        public PassengerType Type { get; set; }
        public bool Needs2Places { get; set; } = false;
        public bool hasPlaceInPlane { get; set; } = false;
        public bool isAssignedToGroup { get; set; } = false;

        public int calculateSeatCost()
        {
            int cost = 0;
            switch (Type)
            {
                case PassengerType.Child:
                    cost = Age > 12 ? 250 : 150;
                    break;
                case PassengerType.Adult:
                    cost = 250;
                    if (Needs2Places)
                    {
                        cost += 250;
                    }
                    break;
            }
            return cost;
        }

    }
}
