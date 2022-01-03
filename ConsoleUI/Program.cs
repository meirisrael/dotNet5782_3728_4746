using System;

namespace ConsoleUI
{
    class Program
    {
        /// <summary>
        /// the main of the project 
        /// it give the option for the user the posibility to change add or display the information 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DO.Choice choice=0;
            DalApi.IDal dal = DAL.DalFactory.GetDal("XML");
            bool res;
            int intA,intB,intC,intD;
            double doubA,doubB;
            string stringA, stringB;
            DO.WeightCategories weight;
            DO.Priorities priorities;

            while (choice != DO.Choice.Exit)//only if the user want to leav the program
            {
                do
                {

                    Console.WriteLine
                        ("Enter\n" +
                        " 1 for add options \n" +
                        " 2 for update options \n" +
                        " 3 for display options \n" +
                        " 4 for list display options\n" +
                        " 5 to Exit");
                    res = Enum.TryParse<DO.Choice>(Console.ReadLine(), out choice);
                    if (!res) Console.WriteLine("Wrong input");
                } while (!res);
                try
                {
                    switch (choice)
                    {
                        case DO.Choice.Add:
                            DO.Add add;
                            do
                            {
                                Console.WriteLine("Enter\n" +
                                    " 1 to add a base station \n" +
                                    " 2 to add a drone \n" +
                                    " 3 to register as a new customer\n" +
                                    " 4 to send a parcel");
                                res = Enum.TryParse<DO.Add>(Console.ReadLine(), out add);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (add)
                            {

                                case DO.Add.AddBaseStation:
                                    Console.WriteLine("Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("Name:");
                                    int.TryParse(Console.ReadLine(), out intB);
                                    Console.WriteLine("ChargeSlots:");
                                    int.TryParse(Console.ReadLine(), out intC);
                                    Console.WriteLine("Longitude:");
                                    double.TryParse(Console.ReadLine(), out doubA);
                                    Console.WriteLine("Latitude:");
                                    double.TryParse(Console.ReadLine(), out doubB);
                                    dal.AddBaseStation(intA, intB, intC, doubA, doubB);
                                    break;


                                case DO.Add.AddDrone:
                                    Console.WriteLine("Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("Model:");
                                    stringA = Console.ReadLine();
                                    Console.WriteLine("Weight:\n" +
                                        "   1.Light\n" +
                                        "   2.Medium\n" +
                                        "   3.Heavy");
                                    Enum.TryParse<DO.WeightCategories>(Console.ReadLine(), out weight);
                                    dal.AddDrone(intA, stringA, weight);
                                    break;
                                case DO.Add.AddCustomer:
                                    Console.WriteLine("Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("Name:");
                                    stringA = Console.ReadLine();
                                    Console.WriteLine("Phone:");
                                    stringB = Console.ReadLine();
                                    Console.WriteLine("Longitude:");
                                    double.TryParse(Console.ReadLine(), out doubA);
                                    Console.WriteLine("Latitude:");
                                    double.TryParse(Console.ReadLine(), out doubB);
                                    dal.AddCustomer(intA, stringA, stringB, doubA, doubB);
                                    break;

                                case DO.Add.AddParcel:
                                    Console.WriteLine("Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("SenderId:");
                                    int.TryParse(Console.ReadLine(), out intB);
                                    Console.WriteLine("TargetId:");
                                    int.TryParse(Console.ReadLine(), out intC);
                                    Console.WriteLine("Weigh:\n" +
                                        "   1.Light\n" +
                                        "   2.Medium\n" +
                                        "   3.Heavy\n");
                                    Enum.TryParse<DO.WeightCategories>(Console.ReadLine(), out weight);
                                    Console.WriteLine("Priority:\n" +
                                        "   1.Normal\n" +
                                        "   2.Fast\n" +
                                        "   3.Emergecey");
                                    Enum.TryParse<DO.Priorities>(Console.ReadLine(), out priorities);
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intD);

                                    dal.AddParcel(intA, intB, intC, intD, weight, priorities);
                                    break;
                                default:
                                    break;
                            }
                                break;
                        case DO.Choice.Update:
                            DO.Update update;
                            do
                            {
                                Console.WriteLine("Enter\n" +
                                    " 1 to assign a parcel to a drone \n" +
                                    " 2 to update the collection of a parcel by a drone \n" +
                                    " 3 to update a delivered parcel \n" +
                                    " 4 to send a drone reload his battery at a base station \n" +
                                    " 5 to make a drone leave his charge station");
                                res = Enum.TryParse<DO.Update>(Console.ReadLine(), out update);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (update)
                            {
                                case DO.Update.AssignParcelToDrone:
                                    Console.WriteLine("ParcelId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intB);
                                    dal.AssignParcelToDrone(intA, intB);
                                    break;

                                case DO.Update.ParcelOnDrone:
                                    Console.WriteLine("ParcelId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    dal.ParcelOnDrone(intA);
                                    break;

                                case DO.Update.ParcelDelivered:
                                    Console.WriteLine("ParcelId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    dal.ParcelDelivered(intA);
                                    break;

                                case DO.Update.AssignDroneToBaseStation:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("BaseId:");
                                    int.TryParse(Console.ReadLine(), out intB);
                                    dal.AssignDroneToBaseStation(intA, intB);
                                    break;

                                case DO.Update.DroneLeaveChargeStation:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("BaseId:");
                                    int.TryParse(Console.ReadLine(), out intB);
                                    dal.DroneLeaveChargeStation(intA, intB);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case DO.Choice.Display:
                            DO.Display display;
                            do
                            {
                                Console.WriteLine("To show details enter \n" +
                                    " 1 for a base station \n" +
                                    " 2 for a drone \n" +
                                    " 3 for a customer \n" +
                                    " 4 for a parcel");
                                res = Enum.TryParse<DO.Display>(Console.ReadLine(), out display);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (display)
                            {
                                case DO.Display.BaseStation:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine((dal.GetBaseStation(intA)).ToString());
                                    break;

                                case DO.Display.Drone:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine((dal.GetDrone(intA)).ToString());
                                    break;

                                case DO.Display.Customer:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine((dal.GetCustomer(intA)).ToString());
                                    break;

                                case DO.Display.Parcel:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine((dal.GetParcel(intA)).ToString());
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case DO.Choice.View_List:
                            DO.DisplayList displayList;
                            do
                            {
                                Console.WriteLine("To show a list enter \n" +
                                    " 1 for base stations \n" +
                                    " 2 for drones \n" +
                                    " 3 for customers \n" +
                                    " 4 for parcels \n" +
                                    " 5 for parcels not assigned to a drone \n " +
                                    " 6 for base stations with free battery reload places");
                                res = Enum.TryParse<DO.DisplayList>(Console.ReadLine(), out displayList);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (displayList)
                            {
                                case DO.DisplayList.BaseStations:
                                    foreach (DO.BaseStation item in dal.GetListBaseStations(b => true))
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case DO.DisplayList.Drones:
                                    foreach (DO.Drone item in dal.GetListDrones(d=>true))
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case DO.DisplayList.Customers:
                                    foreach (DO.Customer item in dal.GetListCustomers())
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case DO.DisplayList.Parcels:
                                    foreach (DO.Parcel item in dal.GetListParcels(p => true))
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case DO.DisplayList.ParcelsNotAssignedToDrone:
                                    foreach (DO.Parcel item in dal.GetListParcels(p => p.DroneId == 0))
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case DO.DisplayList.BaseStationsCanCharge:
                                    foreach (DO.BaseStation item in dal.GetListBaseStations(b => b.ChargeSlots > 0))
                                    { Console.WriteLine(item.ToString()); }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
