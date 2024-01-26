using System.Diagnostics.CodeAnalysis;

namespace PlanePlanning.Models
{
    public class Family
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public List<Passenger> Passengers { get; } = new List<Passenger>();

        public int seatsNeeded { get; set; } = 0;

        public int totalCost { get; set; } = 0;

        public List<string> validationErrors { get; set; } = new List<string>();

        [SetsRequiredMembers]
        public Family(string name)
        {
            Name = name;
        }

        public void addMember(Passenger passenger)
        {
            Passengers.Add(passenger);
            totalCost += passenger.calculateSeatCost();
            seatsNeeded += passenger.Needs2Places ? 2 : 1;
        }

        public bool isValid()
        {
            var children = Passengers.FindAll(p => p.Type == PassengerType.Child);
            var adults = Passengers.FindAll(p => p.Type == PassengerType.Adult);
            var childrenUnder12 = Passengers.FindAll(p => p.Age < 12);

            if (children.Count > 3)
            {
                validationErrors.Add("Validation error: Family has more than 3 children");
            }

            if (adults.Count > 2)
            {
                validationErrors.Add("Validation error: Family has more than 2 adults");
            }

            if (childrenUnder12.Count == 3 && adults.Count == 1)
            {
                validationErrors.Add("Validation error: one children under 12 years cannot be placed near his parrents");
            }

            int adultsNeeding2places = 0;
            foreach (Passenger p in adults)
            {
                if (p.Needs2Places) { adultsNeeding2places++; }
            }

            if (childrenUnder12.Count == 3 && adultsNeeding2places > 1)
            {
                validationErrors.Add("Validation error: We can not place falimy in plain. There are two adults needing 2 places in the plane and 3 children under 12 years");
            }

            if (childrenUnder12.Count == 2 && adultsNeeding2places > 0)
            {
                validationErrors.Add("Validation error: We can not place falimy in plain. One adult that needs 2 places and 2 children cannot be placed toghether. They need 4 places.");
            }

            if (childrenUnder12.Any() && !adults.Any())
            {
                validationErrors.Add("Validation error: Children under 12 years cannot travel alone");
            }

            return !validationErrors.Any();
        }


        public string getValidationErrors()
        {
            string errorMessage = string.Format("Family: {0} is not valid because: {1}", Name, string.Join(" \n", validationErrors));
            return errorMessage;
        }

        public Tuple<List<string>, List<string>> getFamilyGroups()
        {
            List<string> firstGroup = new();
            List<string> secondGroup = new();

            var childrenUnder12 = Passengers.FindAll(p => p.Age < 12);
            var adults = Passengers.FindAll(p => p.Age > 18);

            switch (childrenUnder12.Count)
            {
                case 3:
                    var firstAdult = adults[0].Needs2Places ? adults[1] : adults[0];
                    var secondAdult = adults[0].Needs2Places ? adults[0] : adults[1];

                    firstGroup.Add("E_" + Name + "_" + childrenUnder12[0].Id);
                    firstGroup.Add("A_" + Name + "_" + firstAdult.Id);
                    firstGroup.Add("E_" + Name + "_" + childrenUnder12[1].Id);
                    childrenUnder12[0].isAssignedToGroup = true;
                    childrenUnder12[1].isAssignedToGroup = true;
                    firstAdult.isAssignedToGroup = true;

                    secondGroup.Add("E_" + Name + "_" + childrenUnder12[2].Id);
                    secondGroup.Add("A_" + Name + "_" + secondAdult.Id);
                    if (secondAdult.Needs2Places)
                    {
                        secondGroup.Add("A_" + Name + "_" + secondAdult.Id);
                    }
                    childrenUnder12[2].isAssignedToGroup = true;
                    secondAdult.isAssignedToGroup = true;
                    break;

                case 2:
                    var adult = adults[0].Needs2Places ? adults[1] : adults[0];

                    firstGroup.Add("E_" + Name + "_" + childrenUnder12[0].Id);
                    firstGroup.Add("A_" + Name + "_" + adult.Id);
                    firstGroup.Add("E_" + Name + "_" + childrenUnder12[1].Id);
                    childrenUnder12[0].isAssignedToGroup = true;
                    childrenUnder12[1].isAssignedToGroup = true;
                    adult.isAssignedToGroup = true;
                    break;

                case 1:
                    firstGroup.Add("E_" + Name + "_" + childrenUnder12[0].Id);
                    firstGroup.Add("A_" + Name + "_" + adults[0].Id);
                    if (adults[0].Needs2Places)
                    {
                        firstGroup.Add("A_" + Name + "_" + adults[0].Id);
                    }
                    childrenUnder12[0].isAssignedToGroup = true;
                    adults[0].isAssignedToGroup = true;

                    break;
            }

            Tuple<List<string>, List<string>> mandatoryGroups = new(firstGroup, secondGroup);
            return mandatoryGroups;
        }

        private string getPassengerAcronim(Passenger passenger)
        {
            return passenger.Type == PassengerType.Adult
                ? "A_"
                : "E_";
        }
        public List<string> getNotInGroups()
        {
            return Passengers.Where(p => !p.isAssignedToGroup).ToList().ConvertAll(p => getFormattedPassenger(p));
        }

        public string getFormattedPassenger(Passenger p)
        {
            var familyName = p.FamilyName != null ? Name : "-";
            return getPassengerAcronim(p) + familyName + "_" + p.Id;
        }
    }
}
