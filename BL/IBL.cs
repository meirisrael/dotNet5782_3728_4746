using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	public interface IBL
	{
		//add - option
		public void AddBaseStation(int id, int name, int chargeSlots, BO.Location location);
		public void AddDrone(int id, string model, IDAL.DO.WeightCategories weight, int firstBaseStation);
		public void AddCustomer(int id, string name, string phone, BO.Location location);
		public void AddParcel(int id, int senderId, int targetId, IDAL.DO.WeightCategories weight, IDAL.DO.Priorities priorities);

		//update data - option
		public void UpdateDrone(int droneId, string model);
		public void UpdateBaseStation(int baseId, string name = null, string chargeslots = null);
		public void UpdateCustomer(int customerId, string name = null, string phone = null);
		public void DroneToCharge(int droneId);
		public void DroneLeaveCharge(int droneId, int time);
		public bool AffectParcelToDrone(int droneId);
		public void ParcelCollection(int droneId);
		public void ParcelDeliverd(int droneId);
		
		//get an object - option
		public BO.BaseStation GetBaseStation(int baseId);
		public BO.Drone GetDrone(int droneId);
		public BO.Customer GetCustomer(int customerId);
		public BO.Parcel GetParcel(int parcelId);
		
		//get all list - option 
		public IEnumerable<BO.BaseToList> GetListOfBaseStations(Predicate<IDAL.DO.BaseStation> f);
		public IEnumerable<BO.DroneToList> GetListOfDrone();
		public IEnumerable<BO.CustomerToList> GetListOfCustomer();
		public IEnumerable<BO.ParcelToList> GetListOfParcel(Predicate<IDAL.DO.Parcel> f);
	}
}