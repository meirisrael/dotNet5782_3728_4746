using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		/// <summary>
		/// this enum give to the user the option what he can do 
		/// </summary>
		public enum Choice
		{
			ADD = 1, UPDATE, DISPLAY, VIEW_LIST, EXIT
		}
		/// <summary>
		/// this enum give to the user the option which add he want to do 
		/// </summary>
		public enum Add
		{
			AddBaseStation = 1, AddDrone, AddCustomer, AddParcel
		}
		/// <summary>
		/// this enum give to the user the option which update he want to do 
		/// </summary>
		public enum Update
		{
			AssignParcelToDrone = 1, ParcelOnDrone, ParcelDelivered, AssignDroneToBaseStation, DroneLeaveChargeStation
		}
		/// <summary>
		/// this enum give to the user the option which display he want 
		/// </summary>
		public enum Display
		{
			BaseStation = 1, Drone, Customer, Parcel
		}
		/// <summary>
		/// this enum give to the user the option which display he want 
		/// </summary>
		public enum DisplayList
		{
			BaseStations = 1, Drones, Customers, Parcels, ParcelsNotAssignedToDrone, BaseStationsCanCharge
		}
		/// <summary>
		/// this enum give to the user the option to set the Weight Categories of the parcel or the drone can take
		/// </summary>
		public enum WeightCategories
		{
			Light = 1, Medium, Heavy
		}


		/// <summary>
		/// this enum give to the user the option to set what the statue of drone right now
		/// </summary>
		public enum DroneStatuses
		{
			free = 1, Maintenance, Shipping
		}


		/// <summary>
		/// give to the user the option to set what the priority of the packet 
		/// </summary>
		public enum Priorities
		{
			Normal = 1, fast, Emergecey
		}
	}
}