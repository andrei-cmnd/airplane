using PlanePlanning.Interfaces;
using PlanePlanning.Models;
using System.ComponentModel.DataAnnotations;

namespace PlanePlanning.Services
{
    public class PassengersDistributionService : IPassengersDistributionService
    {


        // convert passengers as families. single passanger is a family with one member
        private Dictionary<string, Family> groupPassengersByFamilly(IEnumerable<Passenger> passengers)
        {
            Dictionary<string, Family> families = new();

            foreach (Passenger p in passengers)
            {
                string familyName = string.IsNullOrEmpty(p.FamilyName) || p.FamilyName.Equals("-")
                    ? "NFP_" + p.Id
                    : p.FamilyName;

                if (!families.ContainsKey(familyName))
                {
                    families.Add(familyName, new Family(familyName));
                }

                Family f = families[familyName];

                f.addMember(p);
            }

            return families;
        }


        public string getPassangersDistribution(IEnumerable<Passenger> passengers)
        {
            Airplane airplaine = new Airplane();

            Dictionary<string, Family> families = groupPassengersByFamilly(passengers);
            
            var orderedFamilies = families.Values.ToList().OrderByDescending(f => f.totalCost).ToList();
            var invalidFamilies = orderedFamilies.Where(f => !f.isValid()).ToList();
            if (invalidFamilies.Count > 0)
            {
                string message = string.Join(" \n", invalidFamilies.ConvertAll(f => f.getValidationErrors()));
                throw new ValidationException(message);
            }

            var validFamilies = orderedFamilies.Where(f => f.isValid()).ToList();

            // orderedFamilies = families.Values.ToList();
            validFamilies.ForEach(family => placeInPlain(airplaine, family));
            return airplaine.getLayout();
        }

        private void placeInPlain(Airplane airplaine, Family family)
        {
            Console.WriteLine(family.Name + ": " + family.seatsNeeded + ": " + family.totalCost);
            placeFamily(airplaine, family);
        }

        private void placeFamily(Airplane airplane, Family family)
        {
            var famGroups = family.getFamilyGroups();
            var group1 = famGroups.Item1;
            var group2 = famGroups.Item2;
            var others = family.getNotInGroups();


            //Console.WriteLine("LeftGroup: " + string.Join("|", addPadding(group1.ToArray<string>())) + "   RightGroup: " + string.Join("|", addPadding(group2.ToArray<string>()))
            //    + "   Others: " + string.Join("|", addPadding(others.ToArray<string>())));

            for (int i = 0; i < airplane.rows.Length; i++)
            {
                var placed = true;

                if (group1.Count > 0 && group2.Count > 0)
                {
                    placed = placeGroups(airplane.rows[i], airplane.rows[i + 1], group1, group2, others);
                }
                else if (group2.Count == 0)
                {
                    placed = placeGroups(airplane.rows[i], airplane.rows[i + 1], group1, others);
                }
                else
                {
                    placed = placeAnyWhereOnRows(airplane.rows[i], airplane.rows[i + 1], others);
                }

                if (placed) break;
            }
        }

        private List<string> addPadding(string[] values)
        {
            return values.ToList().ConvertAll(v => v == null ? "       " : v.PadRight(7, ' '));
        }

        private bool placeGroups(Row currentRow, Row nextRow, List<string> group1, List<string> group2, List<string> others)
        {
            int requiredPlacesGroup1 = group1.Count;
            int requiredPlacesGroup2 = group2.Count;

            // we do not have enough free seats on this pair of rows
            if (currentRow.getFreeSeats() + nextRow.getFreeSeats() < group1.Count + group2.Count + others.Count)
            {
                return false;
            }

            //we'll try to get largest group on the left side 
            bool canPlaceGroup1Left = currentRow.hasFreeSeats(0, requiredPlacesGroup1);
            bool canPlaceGroup2Right = currentRow.hasFreeSeats(1, requiredPlacesGroup2);
            bool canPlaceGroup2BehindLeft = nextRow.hasFreeSeats(0, requiredPlacesGroup2);

            bool canPlaceGroup1Right = currentRow.hasFreeSeats(1, requiredPlacesGroup1);
            bool canPlaceGroup2BehindRight = nextRow.hasFreeSeats(1, requiredPlacesGroup2);

            bool placed = true;

            //try from left
            if (canPlaceGroup1Left)
            {
                if (canPlaceGroup2Right)
                {
                    // place the family on row
                    currentRow.assignSeats(0, group1);
                    currentRow.assignSeats(1, group2);
                    assignAnyWhereOnRows(currentRow, nextRow, others);
                }
                else
                {
                    if (canPlaceGroup2BehindLeft)
                    {
                        // place the family on the 2 rows
                        currentRow.assignSeats(0, group1);
                        nextRow.assignSeats(0, group2);
                        assignAnyWhereOnRows(currentRow, nextRow, others);
                    }
                    else
                    {
                        placed = false;
                    }
                }

            }
            else
            {
                placed = false;
            }

            if (!placed)
            {
                placed = true;
                if (canPlaceGroup1Right)
                {
                    if (canPlaceGroup2BehindRight)
                    {
                        // place family
                        currentRow.assignSeats(1, group1);
                        nextRow.assignSeats(1, group2);
                        assignAnyWhereOnRows(currentRow, nextRow, others);
                    }
                    else
                    {
                        placed = false;
                    }
                }
                else
                {
                    placed = false;
                }
            }
            return placed;
        }

        private bool placeGroups(Row currentRow, Row nextRow, List<string> group, List<string> others)
        {
            // we do not have enough free seats on this pair of rows
            if (currentRow.getFreeSeats() + nextRow.getFreeSeats() < group.Count + others.Count)
            {
                return false;
            }

            bool canPlaceGroupLeft = currentRow.hasFreeSeats(0, group.Count);
            bool canPlaceGroupRight = currentRow.hasFreeSeats(1, group.Count);
            bool placed = true;
            //try from left
            if (canPlaceGroupLeft)
            {
                currentRow.assignSeats(0, group);
                assignAnyWhereOnRows(currentRow, nextRow, others);
            }
            else if (canPlaceGroupRight)
            {
                currentRow.assignSeats(1, group);
                assignAnyWhereOnRows(currentRow, nextRow, others);
            } else
            {
                placed = false;
            }
            return placed;
        }

        private bool placeAnyWhereOnRows(Row firstRow, Row secondRow, List<string> others)
        {

            if ( firstRow.getFreeSeats() + secondRow.getFreeSeats() >= others.Count)
            {
                // the are no enough free seats on this rows
                return false;
            }

            assignAnyWhereOnRows(firstRow, secondRow, others);
            return true;
        }

        private void assignAnyWhereOnRows(Row firstRow, Row secondRow, List<string> others)
        {
            //we have previosly check that we can place passengers on this two rows
            foreach (string passenger in others)
            {
                if(firstRow.getFreeSeats() > 0)
                {
                    firstRow.assignSeat(passenger);
                } 
                else
                {
                    secondRow.assignSeat(passenger);
                }
            }
   
        }

    }
}
