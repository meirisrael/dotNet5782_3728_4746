using System;

namespace ConsoleUI
{
	
    class Program
    {
        static void Main(string[] args)
        {
            IDAL.DO.Choice choice=0;
            DalObject.DalObject p = new DalObject.DalObject();
            bool res;
            while (choice != IDAL.DO.Choice.EXIT)
            {
                do
                {
                    Console.WriteLine("Enter\n 1 for add options \n 2 for update options \n 3 for display options \n 4 for list display options");
                    res = Enum.TryParse<IDAL.DO.Choice>(Console.ReadLine(), out choice);
                    if (!res) Console.WriteLine("Wrong input");
                } while (!res);
                switch (choice)
                {
                    case IDAL.DO.Choice.ADD:
                        IDAL.DO.Add add;
                        do
                        {
                            Console.WriteLine("Enter\n 1 to add a base station \n 2 to add a drone \n 3 to register as a new customer" +
                                " \n 4 to send a parcel");
                            res = Enum.TryParse<IDAL.DO.Add>(Console.ReadLine(), out add);
                            if (!res) Console.WriteLine("Wrong input");
                        } while (!res);
                        switch (add)
                        {
                            case IDAL.DO.Add.AddBaseStation:
                                DalObject.DalObject.AddBaseStation();
                                break;
                            case IDAL.DO.Add.AddDrone:
                                DalObject.DalObject.AddDrone();
                                break;
                            case IDAL.DO.Add.AddCustomer:
                                DalObject.DalObject.AddCustomer();
                                break;
                            case IDAL.DO.Add.AddParcel:
                                DalObject.DalObject.AddParcel();
                                break;
                            default:
                                break;
                        }
                        break;
                    case IDAL.DO.Choice.UPDATE:
                        IDAL.DO.Update update;
                        do
                        {
                            Console.WriteLine("Enter\n 1 to assign a parcel to a drone \n 2 to update the collection of a parcel by a drone" +
                                " \n 3 to update a delivered parcel \n 4 to send a drone reload his battery at a base station");
                            res = Enum.TryParse<IDAL.DO.Update>(Console.ReadLine(), out update);
                            if (!res) Console.WriteLine("Wrong input");
                        } while (!res);
                        switch (update)
                        {
                            case IDAL.DO.Update.AssignParcelToDrone:
                                break;
                            case IDAL.DO.Update.ParcelOnDrone:
                                break;
                            case IDAL.DO.Update.ParcelDelivered:
                                break;
                            case IDAL.DO.Update.AssignDroneToBaseStation:
                                break;
                            default:
                                break;
                        }
                        break;
                    case IDAL.DO.Choice.DISPLAY:
                        IDAL.DO.Display display;
                        do
                        {
                            Console.WriteLine("To show details enter \n 1 for a base station \n 2 for a drone \n 3 for a customer " +
                                "\n 4 for a parcel");
                            res = Enum.TryParse<IDAL.DO.Display>(Console.ReadLine(), out display);
                            if (!res) Console.WriteLine("Wrong input");
                        } while (!res);
                        switch (display)
                        {
                            case IDAL.DO.Display.BaseStation:
                                DalObject.DalObject.DisplayBaseStation();
                                break;
                            case IDAL.DO.Display.Drone:
                                break;
                            case IDAL.DO.Display.Customer:
                                break;
                            case IDAL.DO.Display.Parcel:
                                break;
                            default:
                                break;
                        }
                        break;
                    case IDAL.DO.Choice.VIEW_LIST:
                        IDAL.DO.DisplayList displayList;
                        do
                        {
                            Console.WriteLine("To show a list enter \n 1 for base stations \n 2 for drones \n 3 for customers" +
                                " \n 4 for parcels \n 5 for parcels not assigned to a drone \n 6 for base stations with free battery reload places");
                            res = Enum.TryParse<IDAL.DO.DisplayList>(Console.ReadLine(), out displayList);
                            if (!res) Console.WriteLine("Wrong input");
                        } while (!res);
                        switch (displayList)
                        {
                            case IDAL.DO.DisplayList.BaseStations:
                                break;
                            case IDAL.DO.DisplayList.Drones:
                                break;
                            case IDAL.DO.DisplayList.Customers:
                                break;
                            case IDAL.DO.DisplayList.Parcels:
                                break;
                            case IDAL.DO.DisplayList.ParcelsNotAssignedToDrone:
                                break;
                            case IDAL.DO.DisplayList.BaseStationsCanCharge:
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
