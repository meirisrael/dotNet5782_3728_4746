using System;

namespace ConsoleUI_BL
{
    class Program
    {
        static void Main(string[] args)
        {
            BlApi.IBL ibl = BL.BlFactory.GetBl();
            MenuWindow(ibl);
        }
        static void MenuWindow(BlApi.IBL ibl)
        {
            BO.Choice choice = 0;
            bool res;
            BO.Location loc = new();
            BO.WeightCategories weight;
            BO.Priorities priorities;
            int intA, intB, intC, intD;
            double doubA, doubB;
            string stringA, stringB;
            while (choice != BO.Choice.Exit)//only if the user want to leav the program
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
                    res = Enum.TryParse<BO.Choice>(Console.ReadLine(), out choice);
                    if (!res) Console.WriteLine("Wrong input");
                } while (!res);
                try
                {
                    switch (choice)
                    {
                        case BO.Choice.Add:
                            BO.Add add;
                            do
                            {
                                Console.WriteLine("Enter\n" +
                                    " 1 to add a base station \n" +
                                    " 2 to add a drone \n" +
                                    " 3 to register as a new customer\n" +
                                    " 4 to send a parcel");
                                res = Enum.TryParse<BO.Add>(Console.ReadLine(), out add);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (add)
                            {

                                case BO.Add.AddBaseStation:
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
                                    loc.Longitude = doubA;
                                    loc.Latitude = doubB;
                                    ibl.AddBaseStation(intA, intB, intC, loc);
                                    Console.WriteLine("Successfuly added.");

                                    break;


                                case BO.Add.AddDrone:
                                    Console.WriteLine("Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("Model:");
                                    stringA = Console.ReadLine();
                                    Console.WriteLine("Weight:\n" +
                                        "   1.Light\n" +
                                        "   2.Medium\n" +
                                        "   3.Heavy");
                                    Enum.TryParse<BO.WeightCategories>(Console.ReadLine(), out weight);
                                    Console.WriteLine("Choose first Base Id:");
                                    foreach (BO.BaseToList item in ibl.GetListOfBaseStations(b => b.ChargeSlots > 0))
                                    { Console.WriteLine(item.ToString()); }
                                    int.TryParse(Console.ReadLine(), out intB);
                                    ibl.AddDrone(intA, stringA, weight, intB);
                                    Console.WriteLine("Successfuly added.");

                                    break;
                                case BO.Add.AddCustomer:
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
                                    loc.Longitude = doubA;
                                    loc.Latitude = doubB;
                                    ibl.AddCustomer(intA, stringA, stringB, loc);
                                    Console.WriteLine("Successfuly added.");
                                    break;

                                case BO.Add.AddParcel:
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
                                    Enum.TryParse<BO.WeightCategories>(Console.ReadLine(), out weight);
                                    Console.WriteLine("Priority:\n" +
                                        "   1.Normal\n" +
                                        "   2.Fast\n" +
                                        "   3.Emergecey");
                                    Enum.TryParse<BO.Priorities>(Console.ReadLine(), out priorities);
                                    ibl.AddParcel(intA, intB, intC, weight, priorities);
                                    Console.WriteLine("Successfuly added.");
                                    break;
                                default:
                                    Console.WriteLine("Wrong input");
                                    break;
                            }
                            break;
                        case BO.Choice.Update:
                            BO.Update update;
                            do
                            {
                                Console.WriteLine("Enter\n" +
                                    " 1 to update a drone model \n" +
                                    " 2 to update a base station \n" +
                                    " 3 to update a customer \n" +
                                    " 4 to send a drone reload his battery at a base station \n" +
                                    " 5 to make a drone leave his charge station\n" +
                                    " 6 to make affect a parcel to a drone\n" +
                                    " 7 to make a drone collect his affected parcel\n" +
                                    " 8 to make a drone deliver his affected parcel");

                                res = Enum.TryParse<BO.Update>(Console.ReadLine(), out update);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (update)
                            {
                                case BO.Update.UpdateDrone:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("Model:");
                                    stringA = Console.ReadLine();
                                    ibl.UpdateDrone(intA, stringA);
                                    Console.WriteLine("Successfuly updated.");
                                    break;

                                case BO.Update.UpdateBase:
                                    Console.WriteLine("BaseId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("Name:");
                                    stringA = Console.ReadLine();
                                    Console.WriteLine("Chargeslots:");
                                    stringB = Console.ReadLine();
                                    ibl.UpdateBaseStation(intA, stringA, stringB);
                                    Console.WriteLine("Successfuly updated.");
                                    break;

                                case BO.Update.UpdateCustomer:
                                    Console.WriteLine("CustomerId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine("Name:");
                                    stringA = Console.ReadLine();
                                    Console.WriteLine("Phone:");
                                    stringB = Console.ReadLine();
                                    ibl.UpdateCustomer(intA, stringA, stringB);
                                    Console.WriteLine("Successfuly updated.");
                                    break;

                                case BO.Update.DroneToCharge:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    ibl.DroneToCharge(intA);
                                    Console.WriteLine("Drone was sent to charging.");
                                    break;

                                case BO.Update.DroneLeaveChargeStation:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    ibl.DroneLeaveCharge(intA);
                                    Console.WriteLine("Drone was leav the charging-station.");
                                    break;
                                case BO.Update.AffectParcel:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    ibl.AffectParcelToDrone(intA);
                                    Console.WriteLine("Drone was affected to a parcel successfuly.");
                                    break;
                                case BO.Update.ParcelCollection:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    ibl.ParcelCollection(intA);
                                    Console.WriteLine("Drone was collect the parcel parcel successfuly.");
                                    break;
                                case BO.Update.ParcelDelivery:
                                    Console.WriteLine("DroneId:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    ibl.ParcelDeliverd(intA);
                                    Console.WriteLine("Drone was deliver the parcel parcel successfuly.");
                                    break;
                                default:
                                    Console.WriteLine("Wrong input");
                                    break;
                            }
                            break;
                        case BO.Choice.Display:
                            BO.Display display;
                            do
                            {
                                Console.WriteLine("To show details enter \n" +
                                    " 1 for a base station \n" +
                                    " 2 for a drone \n" +
                                    " 3 for a customer \n" +
                                    " 4 for a parcel");
                                res = Enum.TryParse<BO.Display>(Console.ReadLine(), out display);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (display)
                            {
                                case BO.Display.BaseStation:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine((ibl.GetBaseStation(intA)).ToString());
                                    break;

                                case BO.Display.Drone:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine((ibl.GetDrone(intA)).ToString());
                                    break;

                                case BO.Display.Customer:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine((ibl.GetCustomer(intA)).ToString());
                                    break;

                                case BO.Display.Parcel:
                                    Console.WriteLine("Enter Id:");
                                    int.TryParse(Console.ReadLine(), out intA);
                                    Console.WriteLine((ibl.GetParcel(intA)).ToString());
                                    break;
                                default:
                                    Console.WriteLine("Wrong input");
                                    break;
                            }
                            break;
                        case BO.Choice.View_List:
                            BO.DisplayList displayList;
                            do
                            {
                                Console.WriteLine("To show a list enter \n" +
                                    " 1 for base stations \n" +
                                    " 2 for drones \n" +
                                    " 3 for customers \n" +
                                    " 4 for parcels \n" +
                                    " 5 for parcels not assigned to a drone \n 6 for base stations with free battery reload places");
                                res = Enum.TryParse<BO.DisplayList>(Console.ReadLine(), out displayList);
                                if (!res) Console.WriteLine("Wrong input");
                            } while (!res);
                            switch (displayList)
                            {
                                case BO.DisplayList.BaseStations:
                                    foreach (BO.BaseToList item in ibl.GetListOfBaseStations(b => true))
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case BO.DisplayList.Drones:
                                    foreach (BO.DroneToList item in ibl.GetListOfDrones(d => true))
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case BO.DisplayList.Customers:
                                    foreach (BO.CustomerToList item in ibl.GetListOfCustomers())
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case BO.DisplayList.Parcels:
                                    foreach (BO.ParcelToList item in ibl.GetListOfParcels(p => true))
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case BO.DisplayList.ParcelsNotAssignedToDrone:
                                    foreach (BO.ParcelToList item in ibl.GetListOfParcels(p => p.DroneId == 0))
                                    { Console.WriteLine(item.ToString()); }
                                    break;

                                case BO.DisplayList.BaseStationsCanCharge:
                                    foreach (BO.BaseToList item in ibl.GetListOfBaseStations(b => b.ChargeSlots > 0))
                                    { Console.WriteLine(item.ToString()); }
                                    break;
                                default:
                                    Console.WriteLine("Wrong input");
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
