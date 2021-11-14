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
            IDAL.DO.Choice choice=0;
            IDAL.IDal p = new DalObject.DalObject();
            bool res;
            int intA,intB,intC,intD;
            double doubA,doubB;
            string stringA, stringB;
            DateTime dateA, dateB, dateC, dateD;
            IDAL.DO.WeightCategories weight;
            IDAL.DO.Priorities priorities;

            while (choice != IDAL.DO.Choice.Exit)//only if the user want to leav the program
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
                    res = Enum.TryParse<IDAL.DO.Choice>(Console.ReadLine(), out choice);
                    if (!res) Console.WriteLine("Wrong input");
                } while (!res);
                try
                {
                    switch (choice)
                    {
                        case IDAL.DO.Choice.Add:
                            IDAL.DO.Add add;
                            do
                            {
                                Console.WriteLine("Enter\n" +
                                    " 1 to add a base station \n" +
                                    " 2 to add a drone \n" +
                                    " 3 to register as a new customer\n" +
                                    " 4 to send a parcel");
                                res = Enum.TryParse<IDAL.DO.Add>(Console.ReadLine(), out add);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (add)
                            {

                                case IDAL.DO.Add.AddBaseStation:
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
                                    p.AddBaseStation(intA, intB, intC, doubA, doubB);
                                    break;


                                case IDAL.DO.Add.AddDrone:
                                    Console.WriteLine("Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("Model:");
                                    stringA = Console.ReadLine();
                                    Console.WriteLine("Weight:\n" +
                                        "   1.Light\n" +
                                        "   2.Medium\n" +
                                        "   3.Heavy");
                                    Enum.TryParse<IDAL.DO.WeightCategories>(Console.ReadLine(), out weight);
                                    p.AddDrone(intA, stringA, weight);
                                    break;
                                case IDAL.DO.Add.AddCustomer:
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
                                    p.AddCustomer(intA, stringA, stringB, doubA, doubB);
                                    break;

                                case IDAL.DO.Add.AddParcel:
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
                                    Enum.TryParse<IDAL.DO.WeightCategories>(Console.ReadLine(), out weight);
                                    Console.WriteLine("Priority:");
                                    Enum.TryParse<IDAL.DO.Priorities>(Console.ReadLine(), out priorities);
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intD);

                                    p.AddParcel(intA, intB, intC, intD, weight, priorities);
                                    break;
                                default:
                                    break;
                            }
                                break;
                        case IDAL.DO.Choice.Update:
                            IDAL.DO.Update update;
                            do
                            {
                                Console.WriteLine("Enter\n" +
                                    " 1 to assign a parcel to a drone \n" +
                                    " 2 to update the collection of a parcel by a drone \n" +
                                    " 3 to update a delivered parcel \n" +
                                    " 4 to send a drone reload his battery at a base station \n" +
                                    " 5 to make a drone leave his charge station");
                                res = Enum.TryParse<IDAL.DO.Update>(Console.ReadLine(), out update);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (update)
                            {
                                case IDAL.DO.Update.AssignParcelToDrone:
                                    Console.WriteLine("ParcelId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intB);
                                    p.AssignParcelToDrone(intA, intB);
                                    break;

                                case IDAL.DO.Update.ParcelOnDrone:
                                    Console.WriteLine("ParcelId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    p.ParcelOnDrone(intA);
                                    break;

                                case IDAL.DO.Update.ParcelDelivered:
                                    Console.WriteLine("ParcelId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    p.ParcelDelivered(intA);
                                    break;

                                case IDAL.DO.Update.AssignDroneToBaseStation:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("BaseId:");
                                    int.TryParse(Console.ReadLine(), out intB);
                                    p.AssignDroneToBaseStation(intA, intB);
                                    break;

                                case IDAL.DO.Update.DroneLeaveChargeStation:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("BaseId:");
                                    int.TryParse(Console.ReadLine(), out intB);
                                    p.DroneLeaveChargeStation(intA, intB);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case IDAL.DO.Choice.Display:
                            IDAL.DO.Display display;
                            do
                            {
                                Console.WriteLine("To show details enter \n" +
                                    " 1 for a base station \n" +
                                    " 2 for a drone \n" +
                                    " 3 for a customer \n" +
                                    " 4 for a parcel");
                                res = Enum.TryParse<IDAL.DO.Display>(Console.ReadLine(), out display);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (display)
                            {
                                case IDAL.DO.Display.BaseStation:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    (p.GetBaseStation(intA)).ToString();
                                    break;

                                case IDAL.DO.Display.Drone:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    (p.GetDrone(intA)).ToString();
                                    break;

                                case IDAL.DO.Display.Customer:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    (p.GetCustomer(intA)).ToString();
                                    break;

                                case IDAL.DO.Display.Parcel:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    (p.GetParcel(intA)).ToString();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case IDAL.DO.Choice.View_List:
                            IDAL.DO.DisplayList displayList;
                            do
                            {
                                Console.WriteLine("To show a list enter \n" +
                                    " 1 for base stations \n" +
                                    " 2 for drones \n" +
                                    " 3 for customers \n" +
                                    " 4 for parcels \n" +
                                    " 5 for parcels not assigned to a drone \n 6 for base stations with free battery reload places");
                                res = Enum.TryParse<IDAL.DO.DisplayList>(Console.ReadLine(), out displayList);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (displayList)
                            {
                                case IDAL.DO.DisplayList.BaseStations:
                                    foreach (IDAL.DO.BaseStation item in p.GetListBaseStations())
                                    { item.ToString(); }
                                    break;

                                case IDAL.DO.DisplayList.Drones:
                                    foreach (IDAL.DO.Drone item in p.GetListDrones())
                                    { item.ToString(); }
                                    break;

                                case IDAL.DO.DisplayList.Customers:
                                    foreach (IDAL.DO.Customer item in p.GetListCustomers())
                                    { item.ToString(); }
                                    break;

                                case IDAL.DO.DisplayList.Parcels:
                                    foreach (IDAL.DO.Parcel item in p.GetListParcels())
                                    { item.ToString(); }
                                    break;

                                case IDAL.DO.DisplayList.ParcelsNotAssignedToDrone:
                                    foreach (IDAL.DO.Parcel item in p.GetListOfParcelsNotAssignedToDrone())
                                    { item.ToString(); }
                                    break;

                                case IDAL.DO.DisplayList.BaseStationsCanCharge:
                                    foreach (IDAL.DO.BaseStation item in p.GetListBaseStationsCanCharge())
                                    { item.ToString(); }
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
