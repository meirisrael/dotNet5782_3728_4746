using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
	public interface IDal
	{
		//add method
		public void AddBaseStation(int id,int name, int chargeSlots, double longe, double lati);
		public void AddDrone(int id,string model, DO.WeightCategories weight);
		public void AddCustomer(int id,string name, string phone, double longi, double lati);
		public void AddParcel(int id, int senderId, int targetId, int droneId, DO.WeightCategories weight, DO.Priorities priorities);
		//method - action on drone and parcel
		public void AssignParcelToDrone(int parcelId, int droneId);
		public void ParcelOnDrone(int parcelId);
		public void ParcelDelivered(int parcelId);
		//method- drone to charge,leav charge
		public void AssignDroneToBaseStation(int droneId, int baseId);
		public void DroneLeaveChargeStation(int droneId, int baseId);

		//method- get a specific object by his Id
		public DO.BaseStation GetBaseStation(int baseId);
		public DO.Drone GetDrone(int droneId);
		public DO.Customer GetCustomer(int customer);
		public DO.Parcel GetParcel(int parecel);

		//method - return a IEnumerable list of object
		public IEnumerable<DO.BaseStation> GetListBaseStations(Predicate<DO.BaseStation> f);
		public IEnumerable<DO.Drone> GetListDrones(Predicate<DO.Drone> f); 
		public IEnumerable<DO.Customer> GetListCustomers();
		public IEnumerable<DO.Parcel> GetListParcels(Predicate<DO.Parcel> f);

		//method - update data of an object
		public void UpdateDrone(DO.Drone drone);
		public void UpdateBaseStation(DO.BaseStation baseStation);
		public void UpdateCustomer(DO.Customer customer);
		public void UpdateParcel(DO.Parcel parcel);
		//methot return a data about the charge and the electricity use
		public double[] GetChargingRate();
	}
}
