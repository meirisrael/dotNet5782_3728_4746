using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
	public interface IBL
	{
		#region add - option
		//add - option
		public void AddBaseStation(int id, int name, int chargeSlots, BO.Location location);
		public void AddDrone(int id, string model, BO.WeightCategories weight, int firstBaseStation);
		public void AddCustomer(int id, string name, string phone, BO.Location location);
		public void AddParcel(int id, int senderId, int targetId, BO.WeightCategories weight, BO.Priorities priorities);
		#endregion

		#region update data - option
		//update data - option
		public void UpdateDrone(int droneId, string model);
		public void UpdateBaseStation(int baseId, string name = null, string chargeslots = null);
		public void UpdateCustomer(int customerId, string name = null, string phone = null);
		public void DroneToCharge(int droneId);
		public void DroneLeaveCharge(int droneId);
		public void DeleteParcel(int parcelId);
		public bool AffectParcelToDrone(int droneId);
		public void ParcelCollection(int droneId);
		public void ParcelDeliverd(int droneId);
		public void Fullycharged_simulator(int droneId);
		public void Simulator(int droneId, Action<BO.Drone> ReportProgressSimulator, Func<bool> Cancellation);
		#endregion

		#region get an object - option
		//get an object - option
		public BO.BaseStation GetBaseStation(int baseId);
		public BO.Drone GetDrone(int droneId);
		public BO.Customer GetCustomer(int customerId);
		public BO.Parcel GetParcel(int parcelId);
		#endregion

		#region get all list - option 
		//get all list - option 
		public IEnumerable<BO.BaseToList> GetListOfBaseStations(Predicate<DO.BaseStation> f);
		public IEnumerable<BO.DroneToList> GetListOfDrones(Predicate<BO.DroneToList> f);
		public IEnumerable<BO.CustomerToList> GetListOfCustomers();
		public IEnumerable<BO.ParcelToList> GetListOfParcels(Predicate<DO.Parcel> f);
		#endregion
	}
}