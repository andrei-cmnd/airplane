using PlanePlanning.Models;

namespace PlanePlanning.Tests
{
    [TestClass]
    public class FamilyTest
    {
        [TestMethod]
        public void validate_WhenChildrenUnder12AreAlone_ReturnValidationError()
        {
            var familyName = "fam1";
            var family = new Family(familyName);
            family.addMember(new Passenger { Age = 10, FamilyName = familyName, Type = PassengerType.Child, Id = 1 });
            family.addMember(new Passenger { Age = 11, FamilyName = familyName, Type = PassengerType.Child, Id = 2 });
            family.addMember(new Passenger { Age = 11, FamilyName = familyName, Type = PassengerType.Child, Id = 3 });

            var isValid = family.isValid();

            Assert.IsFalse(isValid);
            Assert.AreEqual(1, family.validationErrors.Count());
            Assert.AreEqual("Validation error: Children under 12 years cannot travel alone", family.validationErrors.First());
        }

        [TestMethod]
        public void validate_WhenMoreChildrenThan3_ReturnValidationError()
        {
            var familyName = "fam1";
            var family = new Family(familyName);
            family.addMember(new Passenger { Age = 10, FamilyName = familyName, Type = PassengerType.Child, Id = 1 });
            family.addMember(new Passenger { Age = 11, FamilyName = familyName, Type = PassengerType.Child, Id = 2 });
            family.addMember(new Passenger { Age = 11, FamilyName = familyName, Type = PassengerType.Child, Id = 3 });
            family.addMember(new Passenger { Age = 12, FamilyName = familyName, Type = PassengerType.Child, Id = 4 });

            var isValid = family.isValid();

            Assert.IsFalse(isValid);
            Assert.AreEqual(2, family.validationErrors.Count());
            Assert.AreEqual("Validation error: Family has more than 3 children", family.validationErrors.First());
            Assert.AreEqual("Validation error: Children under 12 years cannot travel alone", family.validationErrors.Last());
        }

        [TestMethod]
        public void validate_WhenMoreAdultsThan3_ReturnValidationError()
        {
            var familyName = "fam1";
            var family = new Family(familyName);
            family.addMember(new Passenger { Age = 22, FamilyName = familyName, Type = PassengerType.Adult, Id = 1 });
            family.addMember(new Passenger { Age = 23, FamilyName = familyName, Type = PassengerType.Adult, Id = 2 });
            family.addMember(new Passenger { Age = 24, FamilyName = familyName, Type = PassengerType.Adult, Id = 3 });

            var isValid = family.isValid();

            Assert.IsFalse(isValid);
            Assert.AreEqual(1, family.validationErrors.Count());
            Assert.AreEqual("Validation error: Family has more than 2 adults", family.validationErrors.First());
        }

        [TestMethod]
        public void validate_WhenEachAdultsNeeds2PlacesAndFamilyHas3Children_ReturnValidationError()
        {
            var familyName = "fam1";
            var family = new Family(familyName);
            family.addMember(new Passenger { Age = 42, FamilyName = familyName, Type = PassengerType.Adult, Id = 1, Needs2Places = true });
            family.addMember(new Passenger { Age = 43, FamilyName = familyName, Type = PassengerType.Adult, Id = 2, Needs2Places = true });
            family.addMember(new Passenger { Age = 10, FamilyName = familyName, Type = PassengerType.Child, Id = 3 });
            family.addMember(new Passenger { Age = 11, FamilyName = familyName, Type = PassengerType.Child, Id = 4 });
            family.addMember(new Passenger { Age = 11, FamilyName = familyName, Type = PassengerType.Child, Id = 5 });

            var isValid = family.isValid();

            Assert.IsFalse(isValid);
            Assert.AreEqual(1, family.validationErrors.Count());
            Assert.AreEqual("Validation error: We can not place falimy in plain. There are two adults needing 2 places in the plane and 3 children under 12 years", family.validationErrors.First());
        }

        [TestMethod]
        public void validate_WhenSinleAdultHas3Children_ReturnValidationError()
        {
            var familyName = "fam1";
            var family = new Family(familyName);
            family.addMember(new Passenger { Age = 42, FamilyName = familyName, Type = PassengerType.Adult, Id = 1 });
            family.addMember(new Passenger { Age = 10, FamilyName = familyName, Type = PassengerType.Child, Id = 2 });
            family.addMember(new Passenger { Age = 11, FamilyName = familyName, Type = PassengerType.Child, Id = 3 });
            family.addMember(new Passenger { Age = 11, FamilyName = familyName, Type = PassengerType.Child, Id = 4 });

            var isValid = family.isValid();

            Assert.IsFalse(isValid);
            Assert.AreEqual(1, family.validationErrors.Count());
            Assert.AreEqual("Validation error: one children under 12 years cannot be placed near his parrents", family.validationErrors.First());
        }
    }
}