using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaineSocietyDataContext db;

        static Query()
        {
            db = new HumaineSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "read":
                    employee = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).SingleOrDefault();
                    break;
                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "update":
                    Employee employeeFromDb = null;

                    try
                    {
                        employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine("No employees have a EmployeeNumber that matches the Employee passed in.");
                        Console.WriteLine("No update have been made.");
                        return;
                    }
                    employeeFromDb.FirstName = employee.FirstName;
                    employeeFromDb.LastName = employee.LastName;
                    employeeFromDb.UserName = employee.UserName;
                    employeeFromDb.Password = employee.Password;
                    employeeFromDb.Email = employee.Email;

                    db.SubmitChanges();
                    break;
                    
                default:
                    throw new NotImplementedException();
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            if(animal != null)
            {
                db.Animals.InsertOnSubmit(animal);
                db.SubmitChanges();
            }
            else
            {
                throw new NotImplementedException();
            }
            
        }

        internal static Animal GetAnimalByID(int id)
        {
            var animal = from Animal in db.Animals
                         where id == Animal.AnimalId
                         select Animal;
            return animal.SingleOrDefault();
        }
        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates) 
        {
            var animal = db.Animals.Where(x => x.AnimalId == animalId).SingleOrDefault();

            foreach (KeyValuePair<int, string> element in updates)
            {
                switch (element.Key)
                {
                    case 1:
                        animal.Category.Name = element.Value;
                        break;
                    case 2:
                        animal.Name = element.Value;
                        break;
                    case 3:
                        animal.Age = Convert.ToInt32(element.Value);
                        break;
                    case 4:
                        animal.Demeanor = element.Value;
                        break;
                    case 5:
                        animal.KidFriendly = Convert.ToBoolean(element.Value) == (true || false);
                        break;
                    case 6: 
                        animal.PetFriendly = Convert.ToBoolean(element.Value) ==  (true || false);
                        break;
                    case 7:
                        animal.Weight = Convert.ToInt32(element.Value);
                        break;
                    case 8:
                        animal.AnimalId = Convert.ToInt32(element.Value);
                        break;
                }

            }
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            if (animal != null)
            {
                db.Animals.DeleteOnSubmit(animal);
                db.SubmitChanges();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            throw new NotImplementedException();
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            if (categoryName != null)
            {
                var category = from categories in db.Categories
                               where categoryName == categories.Name
                               select categories.CategoryId;
                return category.SingleOrDefault();
            }
            else
            {
               throw new NotImplementedException();
            }
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room room = null;
            if (animalId != 0)
            {
                room = db.Rooms.Where(r => r.AnimalId == animalId).SingleOrDefault();
                return room;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            if(dietPlanName != null)
            {
                var dietPlan = db.DietPlans.Where(d => d.Name == dietPlanName).SingleOrDefault();
                return dietPlan.DietPlanId;
            }
            throw new NotImplementedException();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}