using System;

namespace ConsoleUI_BL
{
    class Program
    {
        static void Main(string[] args)
        {
            IBL.BO.Choice choice = 0;
            IBL.IBL ibl = new BL.BL();
            bool res;
            IBL.BO.Location loc = new();
            IDAL.DO.WeightCategories weight;
            IDAL.DO.Priorities priorities;
            int intA, intB, intC, intD;
            double doubA, doubB;
            string stringA, stringB;
            while (choice != IBL.BO.Choice.Exit)//only if the user want to leav the program
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
                switch (choice)
                {
                    case IBL.BO.Choice.Add:
                        IBL.BO.Add add;
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

                            case IBL.BO.Add.AddBaseStation:
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
                                try
                                {
                                    ibl.AddBaseStation(intA, intB, intC, loc);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;


                            case IBL.BO.Add.AddDrone:
                                Console.WriteLine("Id:");
                                int.TryParse(Console.ReadLine(), out intA);
                                Console.WriteLine("Model:");
                                stringA = Console.ReadLine();
                                Console.WriteLine("Weight:\n" +
                                    "   1.Light\n" +
                                    "   2.Medium\n" +
                                    "   3.Heavy");
                                Enum.TryParse<IDAL.DO.WeightCategories>(Console.ReadLine(), out weight);
                                Console.WriteLine("First Base Id:");
                                int.TryParse(Console.ReadLine(), out intB);
                                try
                                {
                                    ibl.AddDrone(intA, stringA, weight,intB);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case IBL.BO.Add.AddCustomer:
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
                                try
                                {
                                    ibl.AddCustomer(intA, stringA, stringB, loc);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.Add.AddParcel:
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
                                Console.WriteLine("Priority:\n" +
                                    "   1.Normal\n" +
                                    "   2.Fast\n" +
                                    "   3.Emergecey");
                                Enum.TryParse<IDAL.DO.Priorities>(Console.ReadLine(), out priorities);
                                try
                                {
                                    ibl.AddParcel(intA, intB, intC, weight, priorities);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case IBL.BO.Choice.Update:
                        IBL.BO.Update update;
                        do
                        {
                            Console.WriteLine("Enter\n" +
                                " 1 to update a drone model \n" +
                                " 2 to update a base station \n" +
                                " 3 to update a customer \n" +
                                " 4 to send a drone reload his battery at a base station \n" +
                                " 5 to make a drone leave his charge station"+
                                " 6 to make affect a parcel to a drone" +
                                " 7 to make a drone collect his affected parcel" +
                                " 8 to make a drone deliver his affected parcel");

                            res = Enum.TryParse<IDAL.DO.Update>(Console.ReadLine(), out update);
                            if (!res) Console.WriteLine("Wrong input");
                        } while (!res);
                        switch (update)
                        {
                            case IBL.BO.Update.UpdateDrone:
                                Console.WriteLine("DroneId:");
                                int.TryParse(Console.ReadLine(), out intA);
                                Console.WriteLine("Model:");
                                stringA = Console.ReadLine();
                                try
                                {
                                    ibl.UpdateDrone(intA, stringA);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.Update.UpdateBase:
                                Console.WriteLine("BaseId:");
                                int.TryParse(Console.ReadLine(), out intA);
                                Console.WriteLine("Name:");
                                stringA = Console.ReadLine();
                                Console.WriteLine("Chargeslots:");
                                stringB = Console.ReadLine();
                                try
                                {
                                    ibl.UpdateBaseStation(intA, stringA, stringB);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.Update.UpdateCustomer:
                                Console.WriteLine("CustomerId:");
                                int.TryParse(Console.ReadLine(), out intA);
                                Console.WriteLine("Name:");
                                stringA = Console.ReadLine();
                                Console.WriteLine("Phone:");
                                stringB = Console.ReadLine();
                                try                          
                                {
                                    ibl.UpdateCustomer(intA, stringA, stringB);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.Update.DroneToCharge:
                                Console.WriteLine("DroneId:");
                                int.TryParse(Console.ReadLine(), out intA);
                                try
                                {
                                    ibl.DroneToCharge(intA);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.Update.DroneLeaveChargeStation:
                                Console.WriteLine("DroneId:");
                                int.TryParse(Console.ReadLine(), out intA);
                                Console.WriteLine("Time(in minutes):");
                                int.TryParse(Console.ReadLine(), out intB);
                                try
                                {
                                    ibl.DroneLeaveCharge(intA, intB);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case IBL.BO.Update.AffectParcel:
                                Console.WriteLine("DroneId:");
                                int.TryParse(Console.ReadLine(), out intA);
                                try
                                {
                                    ibl.AffectParcelToDrone(intA);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case IBL.BO.Update.ParcelCollection:
                                Console.WriteLine("DroneId:");
                                int.TryParse(Console.ReadLine(), out intA);
                                try
                                {
                                    ibl.ParcelCollection(intA);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case IBL.BO.Update.ParcelDelivery:
                                Console.WriteLine("DroneId:");
                                int.TryParse(Console.ReadLine(), out intA);
                                try
                                {
                                    ibl.ParcelDeliverd(intA);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case IBL.BO.Choice.Display:
                        IBL.BO.Display display;
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
                            case IBL.BO.Display.BaseStation:
                                Console.WriteLine("Enter Id:");
                                int.TryParse(Console.ReadLine(), out intA);
                                try
                                {
                                    Console.WriteLine((ibl.GetBaseStation(intA)).ToString());
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.Display.Drone:
                                Console.WriteLine("Enter Id:");
                                int.TryParse(Console.ReadLine(), out intA);
                                try
                                {
                                    Console.WriteLine((ibl.GetDrone(intA)).ToString());
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.Display.Customer:
                                Console.WriteLine("Enter Id:");
                                int.TryParse(Console.ReadLine(), out intA);
                                try
                                {
                                    Console.WriteLine((ibl.GetCustomer(intA)).ToString());
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.Display.Parcel:
                                Console.WriteLine("Enter Id:");
                                int.TryParse(Console.ReadLine(), out intA);
                                try
                                {
                                    Console.WriteLine((ibl.GetParcel(intA)).ToString());
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case IBL.BO.Choice.View_List:
                        IBL.BO.DisplayList displayList;
                        do
                        {
                            Console.WriteLine("To show a list enter \n" +
                                " 1 for base stations \n" +
                                " 2 for drones \n" +
                                " 3 for customers \n" +
                                " 4 for parcels \n" +
                                " 5 for parcels not assigned to a drone \n 6 for base stations with free battery reload places");
                            res = Enum.TryParse<IBL.BO.DisplayList>(Console.ReadLine(), out displayList);
                            if (!res) Console.WriteLine("Wrong input");
                        } while (!res);
                        switch (displayList)
                        {
                            case IBL.BO.DisplayList.BaseStations:
                                try
                                {
                                  foreach (IDAL.DO.BaseStation item in ibl.GetListOfBaseStations())
                                { Console.WriteLine(item.ToString()); }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.DisplayList.Drones:
                                try
                                {
                                    foreach (IBL.BO.DroneToList item in ibl.GetListOfDrone())
                                     { Console.WriteLine(item.ToString()); }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.DisplayList.Customers:
                                try
                                {
                                 foreach (IBL.BO.Customer item in ibl.GetListOfCustomer())
                                    { Console.WriteLine(item.ToString()); }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.DisplayList.Parcels:
                                try
                                {
                                  foreach (IDAL.DO.Parcel item in ibl.GetListOfParcel())
                                     { Console.WriteLine(item.ToString()); }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.DisplayList.ParcelsNotAssignedToDrone:
                                try
                                {
                                    foreach (IDAL.DO.Parcel item in ibl.GetListParcelNotAssignToDrone())
                                         { Console.WriteLine(item.ToString()); }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;

                            case IBL.BO.DisplayList.BaseStationsCanCharge:
                                try
                                {
                                    foreach (IDAL.DO.BaseStation item in ibl.GetListBaseWithChargeSlot())
                                      { Console.WriteLine(item.ToString()); }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
