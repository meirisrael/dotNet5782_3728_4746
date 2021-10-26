using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	namespace DO
	{
		public enum Choice
		{
			ADD=1,UPDATE,DISPLAY,VIEW_LIST,EXIT
		}
		public enum Add 
		{ 
			AddBaseStation = 1, AddDrone, AddCustomer, AddParcel 
		}
		public enum Update 
		{ 
			AssignParcelToDrone = 1, ParcelOnDrone, ParcelDelivered, AssignDroneToBaseStation,DroneLeaveChargeStation 
		}
		public enum Display 
		{ 
			BaseStation = 1, Drone, Customer, Parcel 
		}
		public enum DisplayList 
		{ 
			BaseStations = 1, Drones, Customers, Parcels, ParcelsNotAssignedToDrone, BaseStationsCanCharge 
		}
		public enum WeightCategories
		{
			Light=1,Medium,Heavy
		}
		public enum DroneStatuses
		{
			free=1,Maintenance,Shipping
		}
		public enum Priorities
		{
			Normal=1,fast,Emergecey
		}
	}
}
