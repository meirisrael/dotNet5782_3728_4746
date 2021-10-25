using System;

namespace ConsoleUI
{
	public enum Menu { Add=1,Update,Display,DisplayList}
	public enum Add { AddBaseStation=1,AddDrone,AddCustomer,AddParcel}
	public enum Update { AssignParcelToDrone=1,ParcelOnDrone,ParcelDelivered,AssignDroneToBaseStation}
	public enum Display { BaseStation=1,Drone,Customer,Parcel}
	public enum DisplayList { BaseStations=1, Drones, Customers, Parcels,ParcelsNotAssignedToDrone,BaseStationsCanCharge }
    class Program
    {
        static void Main(string[] args)
        {
            Menu menu;
            bool res;
            do
            {
                Console.WriteLine("Enter\n 1 for add options \n 2 for update options \n 3 for display options \n 4 for list display options");
                res = Enum.TryParse<Menu>(Console.ReadLine(), out menu);
                if (!res) Console.WriteLine("Wrong input");
            } while (!res);
            switch (menu)
            {
                case Menu.Add:
                    Add add;
                    do
                    {
                        Console.WriteLine("Enter\n 1 to add a base station \n 2 to add a drone \n 3 to register as a new customer" +
                            " \n 4 to send a parcel");
                        res = Enum.TryParse<Add>(Console.ReadLine(), out add);
                        if (!res) Console.WriteLine("Wrong input");
                    } while (!res);
                    switch (add)
                    {
                        case Add.AddBaseStation:
                            break;
                        case Add.AddDrone:
                            break;
                        case Add.AddCustomer:
                            break;
                        case Add.AddParcel:
                            break;
                        default:
                            break;
                    }
                    break;
                case Menu.Update:
                    Update update;
                    do
                    {
                        Console.WriteLine("Enter\n 1 to assign a parcel to a drone \n 2 to update the collection of a parcel by a drone" +
                            " \n 3 to update a delivered parcel \n 4 to send a drone reload his battery at a base station");
                        res = Enum.TryParse<Update>(Console.ReadLine(), out update);
                        if (!res) Console.WriteLine("Wrong input");
                    } while (!res);
                    switch (update)
                    {
                        case Update.AssignParcelToDrone:
                            break;
                        case Update.ParcelOnDrone:
                            break;
                        case Update.ParcelDelivered:
                            break;
                        case Update.AssignDroneToBaseStation:
                            break;
                        default:
                            break;
                    }
                    break;
                case Menu.Display:
                    Display display;
                    do
                    {
                        Console.WriteLine("To show details enter \n 1 for a base station \n 2 for a drone \n 3 for a customer " +
                            "\n 4 for a parcel");
                        res = Enum.TryParse<Display>(Console.ReadLine(), out display);
                        if (!res) Console.WriteLine("Wrong input");
                    } while (!res);
                    switch (display)
                    {
                        case Display.BaseStation:
                            break;
                        case Display.Drone:
                            break;
                        case Display.Customer:
                            break;
                        case Display.Parcel:
                            break;
                        default:
                            break;
                    }
                    break;
                case Menu.DisplayList:
                    DisplayList displayList;
                    do
                    {
                        Console.WriteLine("To show a list enter \n 1 for base stations \n 2 for drones \n 3 for customers" +
                            " \n 4 for parcels \n 5 for parcels not assigned to a drone \n 6 for base stations with free battery reload places");
                        res = Enum.TryParse<DisplayList>(Console.ReadLine(), out displayList);
                        if (!res) Console.WriteLine("Wrong input");
                    } while (!res);
                    switch (displayList)
                    {
                        case DisplayList.BaseStations:
                            break;
                        case DisplayList.Drones:
                            break;
                        case DisplayList.Customers:
                            break;
                        case DisplayList.Parcels:
                            break;
                        case DisplayList.ParcelsNotAssignedToDrone:
                            break;
                        case DisplayList.BaseStationsCanCharge:
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
