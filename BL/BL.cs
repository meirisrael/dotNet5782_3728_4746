using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BL
{
	public class BL : BlApi.IBL
	{
		private DalApi.IDal dal;
		/// <summary>
		/// lazy initialization 
		/// </summary>
		internal static readonly Lazy<BlApi.IBL> _instance = new Lazy<BlApi.IBL>(() => new BL());
		/// <summary>
		/// return instance value 
		/// </summary>
		public static BlApi.IBL GetInstance{ get { return _instance.Value; } }

		//internal static BL _instance = null;
		//public static BL GetInstance() => _instance ?? (_instance = new BL());

		private double _useWhenFree;
		private double _useWhenLightly;
		private double _useWhenMedium;
		private double _useWhenHeavily;
		private double _chargingRate;
		List<BO.DroneToList> droneToList = new List<BO.DroneToList>();
		//ctor
		private BL()
		{
			dal = DAL.DalFactory.GetDal("XML");
			getDataCharge();
			reqListOfDrone();
			settingDroneByParcel();
			settingDrone();
		}

		#region METHOD FOR THE CTOR
		/// <summary>
		/// the func get the data of charge and use battery for each situation(free,light packege...)
		/// </summary>
		private void getDataCharge()
		{
			lock (dal)
			{
				double[] arr = new double[5];
				arr = dal.GetChargingRate();
				_useWhenFree = arr[0];
				_useWhenLightly = arr[1];
				_useWhenMedium = arr[2];
				_useWhenHeavily = arr[3];
				_chargingRate = arr[4];
			}
		}
		/// <summary>
		/// the func req from the data base all data about the drone
		/// </summary>
		private void reqListOfDrone()
		{
			lock (dal)
			{
				foreach (DO.Drone d in dal.GetListDrones(d => true))
				{
					DateTime? isDeliverd = dal.GetListParcels(b => true).ToList().Find(item => item.DroneId == d.Id).Delivered;
					int idParcel = dal.GetListParcels(b => true).ToList().Find(item => item.DroneId == d.Id).Id;//return id of parcel that the drone is associated;
					if (isDeliverd != null)//check if the parcel is not in transit
						idParcel = 0;
					BO.Location l = new BO.Location();
					l.Latitude = 0; l.Longitude = 0;
					droneToList.Add(new BO.DroneToList
					{
						Id = d.Id,
						Model = d.Model,
						MaxWeight = (BO.WeightCategories)d.MaxWeight,
						Battery = 0,
						Status = BO.DroneStatuses.free,
						Loc = l,
						IdOfParcel = idParcel
					});
				}
			}
		}
		/// <summary>
		/// the func set the loc and the satus of the drone if is assigne to an parcel
		/// </summary>
		private void settingDroneByParcel()
		{
			List<BO.ParcelToList> parcelToList = new List<BO.ParcelToList>();
			//add the data
			lock (dal)
			{
				foreach (DO.Parcel item in dal.GetListParcels(p => p.Id != 0))
				{
					parcelToList.Add(new BO.ParcelToList
					{
						Id = item.Id,
						NameSender = getCustomerInIBL(item.SenderId).Name,
						NameTarget = getCustomerInIBL(item.TargetId).Name,
						Weight = (BO.WeightCategories)item.Weight,
						Priority = (BO.Priorities)item.Priority,
						Status = getStatusOfParcel(item)
					});
				}
				for (int i = 0; i < parcelToList.Count(); i++)
				{
					int droneIdAssigneToParcel = dal.GetListParcels(b => true).ToList().Find(item => item.Id == parcelToList[i].Id).DroneId;//search drone that assigne to this parcel
					if (parcelToList[i].Status < BO.ParcelStatues.Delivered && droneIdAssigneToParcel > 0)//if the parcel was not deliver and the parcel he has a drone ,need to the shipping
					{
						droneToList[indexDrone(droneIdAssigneToParcel)].Status = BO.DroneStatuses.Shipping;

						locOfDroneByParcel(parcelToList[i], droneIdAssigneToParcel);

						int percent = (int)Math.Ceiling(calcBatteryToShipping(droneToList[indexDrone(droneIdAssigneToParcel)], parcelToList[i]));//calculate the percent of battery the drone need to do the shipping 
						int finalPercent = new Random().Next(percent, 101);//choose random the percent between "percent" to 100
						droneToList[indexDrone(droneIdAssigneToParcel)].Battery = finalPercent;
					}
				}
			}
		}
		/// <summary>
		/// set for all drone that is not in shipping his battery by his status
		/// </summary>
		private void settingDrone()
		{
			List<BO.BaseStation> baseS = new List<BO.BaseStation>();
			//add the data location of base station in the list
			lock (dal)
			{
				foreach (var item in dal.GetListDroneCharge(d => true))
				{ dal.DroneLeaveChargeStation(item.DroneId, item.StationId); }
				foreach (DO.BaseStation b in dal.GetListBaseStations(b => true))
				{
					BO.Location l = new BO.Location();
					l.Latitude = b.Latitude; l.Longitude = b.Longitude;
					baseS.Add(new BO.BaseStation { Id = b.Id, Loc = l, ChargeSlots = b.ChargeSlots });
				}
				
				for (int i = 0; i < droneToList.Count(); i++)
				{
					if (droneToList[i].Status != BO.DroneStatuses.Shipping)//if the drone is not in shipping choose random status
					{
						int r = new Random().Next(1, 3);
						if (r == 1)
							droneToList[i].Status = BO.DroneStatuses.free;
						else
							droneToList[i].Status = BO.DroneStatuses.Maintenance;
					}
					if (droneToList[i].Status == BO.DroneStatuses.Maintenance)//according to the previous "if" now-if the random choose that the drone is in Maintenance
					{
						int r = new Random().Next(0, baseS.Count());//choose randome location 
						if (baseS[r].ChargeSlots > 0)
						{
							droneToList[i].Loc.Longitude = baseS[r].Loc.Longitude;
							droneToList[i].Loc.Latitude = baseS[r].Loc.Latitude;
							dal.AssignDroneToBaseStation(droneToList[i].Id, baseS[r].Id);
							baseS[r].ChargeSlots--;

							r = new Random().Next(0, 21);//choose random percent of battery between 0to20%
							droneToList[i].Battery = r;
							droneToList[i].WhenInCharge = DateTime.Now;
							
						}
						else
						{
							BO.BaseStation b = baseS.FirstOrDefault(b => b.ChargeSlots > 0);
							droneToList[i].Loc.Longitude = b.Loc.Longitude;
							droneToList[i].Loc.Latitude = b.Loc.Latitude;
							dal.AssignDroneToBaseStation(droneToList[i].Id, b.Id);
							baseS[r].ChargeSlots--;

							r = new Random().Next(0, 21);//choose random percent of battery between 0to20%
							droneToList[i].Battery = r;
							droneToList[i].WhenInCharge = DateTime.Now;							
						}
					}
					else if (droneToList[i].Status == BO.DroneStatuses.free)//according to the previous "if" now-if the random choose that the drone is free
					{
						List<BO.Customer> customerList = new List<BO.Customer>();
						foreach (DO.Parcel item in dal.GetListParcels(p => p.Id != 0))//add data of parcel that was deliver
						{
							if (item.Delivered != null)
								customerList.Add(getCustomerInIBL(item.TargetId));
						}
						int r = new Random().Next(0, customerList.Count());//choose a random loc of parcel 
						droneToList[i].Loc.Longitude = customerList[r].Loc.Longitude;
						droneToList[i].Loc.Latitude = customerList[r].Loc.Latitude;

						int percent = calcBatteryToCloserBase(droneToList[i].Loc.Longitude, droneToList[i].Loc.Latitude);//calculate the min percent battery to go back to the base station
						int finalPercent = new Random().Next(percent, 101);//random choose percent between "percent" to 100 
						droneToList[i].Battery = finalPercent;
					}
				}
			}
		}
		//---------------------------------------------------------------------------------------FUNC TO HELP THE METHOD OF THE CTOR-------------------------------------------------------------------------------------
		#region FUNC TO HELP THE METHOD OF THE CTOR
		/// <summary>
		/// the func calculate the distance between two points for examaple: the distance between the target and the closer base station 
		/// </summary>
		/// <param name="latA"></param>
		/// <param name="lonA"></param>
		/// <param name="latB"></param>
		/// <param name="lonB"></param>
		/// <returns> the distance </returns>
		private double distanceBetweenTwoPoints(double latA, double lonA, double latB, double lonB)
		{
			if ((latA == latB) && (lonA == lonB))
			{
				return 0;
			}
			else
			{
				var radlat1 = Math.PI * latA / 180;
				var radlat2 = Math.PI * latB / 180;
				var theta = lonA - lonB;
				var radtheta = Math.PI * theta / 180;
				var dist = Math.Sin(radlat1) * Math.Sin(radlat2) + Math.Cos(radlat1) * Math.Cos(radlat2) * Math.Cos(radtheta);
				if (dist > 1)
				{
					dist = 1;
				}
				dist = Math.Acos(dist);
				dist = dist * 180 / Math.PI;
				dist = dist * 60 * 1.1515;
				dist = dist * 1.609344;
				return dist;
			}
		}
		/// <summary>
		/// the func calculate the percent of battery that the drone need to to the shipping and to return to the base station that the must closer
		/// </summary>
		/// <param name="d"></param>
		/// <param name="p"></param>
		/// <returns> the percent of battery </returns>
		private double calcBatteryToShipping(BO.DroneToList d, BO.ParcelToList p)
		{
			BO.Location locSender = getLocCustomer(0,p.NameSender);
			BO.Location locTarget = getLocCustomer(0,p.NameTarget);

			double disToSenderFromBase = distanceBetweenTwoPoints(d.Loc.Latitude, d.Loc.Longitude, locSender.Latitude, locSender.Longitude); //one way to the sender from the closer base station possible is equal to "0"
			double disToTarget = distanceBetweenTwoPoints(d.Loc.Latitude, d.Loc.Longitude, locTarget.Latitude, locTarget.Longitude);// + one way
			double disToBaseFromTarget = disToCloserBase(locTarget.Latitude, locTarget.Longitude);// + second way

			double batteryToBaseFromTarget = _useWhenFree * disToBaseFromTarget;//percent of battery the drone need to go back to the base station
			double batteryToSenderFromBase = _useWhenFree * disToSenderFromBase;//percent of battery the drone need to arrive at sender
			double batteryToTargetFromSender;// percent of battery need to go from the sender to the target
			if (p.Weight == BO.WeightCategories.Light)
				batteryToTargetFromSender = _useWhenLightly * disToTarget;
			else if (p.Weight == BO.WeightCategories.Medium)
				batteryToTargetFromSender = _useWhenMedium * disToTarget;
			else
				batteryToTargetFromSender = _useWhenHeavily * disToTarget;
			double finalBattery = batteryToBaseFromTarget + batteryToTargetFromSender + batteryToSenderFromBase;
			if (finalBattery > 100) throw new BO.DistanceTooBigger();
			return finalBattery;
		}
		/// <summary>
		/// the func calculate the percent of battery that the drone need to go back to the closer base station 
		/// </summary>
		/// <returns></returns>
		private int calcBatteryToCloserBase(double longi, double lati)
		{
			double disToBase = disToCloserBase(longi, lati);
			double batteryToBase = _useWhenFree * disToBase;
			return ((int)(Math.Ceiling(batteryToBase)));
		}
		/// <summary>
		/// the func search a drone by her id in the "DroneToList" and return the index of the drone in drone list
		/// </summary>
		/// <param name="id"></param>
		/// <returns> index </returns>
		private int indexDrone(int id)
		{
			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Id == id)
					return i;
			}
			throw new BO.IdNotExist("DRONE");
		}
		/// <summary>
		/// the func locate the drone according to his status 
		/// </summary>
		/// <param name="p"></param>
		/// <param name="droneId"></param>
		private void locOfDroneByParcel(BO.ParcelToList p, int droneId)
		{
			BO.Location locSender = getLocCustomer(0,p.NameSender);
			if (p.Status == BO.ParcelStatues.Associated)//if the parcel was Associated so the loc of the drone is in the close base station
			{
				double finalDis = -1;
				BO.Location loc = new BO.Location();
				lock (dal)
				{
					foreach (DO.BaseStation b in dal.GetListBaseStations(b => b.Id != 0))
					{
						double dis = distanceBetweenTwoPoints(b.Latitude, b.Longitude, locSender.Latitude, locSender.Longitude);
						if (finalDis == -1 || dis < finalDis)
						{
							droneToList[indexDrone(droneId)].Loc.Longitude = b.Longitude;
							droneToList[indexDrone(droneId)].Loc.Latitude = b.Latitude;
							finalDis = dis;
						}

					}
				}
			}
			else if (p.Status == BO.ParcelStatues.Collected)//if the parcel was collected by the drone so the loc of drone is where the sender is 
			{
				droneToList[indexDrone(droneId)].Loc.Latitude = locSender.Latitude;
				droneToList[indexDrone(droneId)].Loc.Longitude = locSender.Longitude;
			}
		}
		/// <summary>
		/// the func search who is send the parcel according to the "id"
		/// </summary>
		/// <param name="id"></param>
		/// <returns> an customer </returns>
		private BO.Customer getCustomerInIBL(int id)
		{
			lock (dal)
			{
				try
				{
					BO.Customer customer = new BO.Customer();
					DO.Customer c = new DO.Customer();
					c = dal.GetCustomer(id);
					customer.Id = c.Id;
					customer.Name = c.Name;
					customer.Phone = c.Phone;
					customer.Loc.Latitude = c.Latitude;
					customer.Loc.Longitude = c.Longitude;
					return customer;
				}
				catch (DO.IdNotExist ex)//customer
				{
					throw new BO.IdNotExist(ex.Message, ex._type);
				}
			}
		}
		/// <summary>
		/// the func calculate the distance of the closer base station 
		/// </summary>
		/// <returns> the distance </returns>
		private double disToCloserBase(double longi, double lati)
		{
			double disToBase = -1;
			lock (dal)
			{
				foreach (DO.BaseStation b in dal.GetListBaseStations(b => b.Id != 0))
				{
					double dis = distanceBetweenTwoPoints(b.Latitude, b.Longitude, lati, longi);
					if ((disToBase == -1 || dis < disToBase) && b.ChargeSlots > 0)//if "dis" that is calculate in the previous line is smaller than the "disToBase"= the smaller distanse right now so "disToBase = dis"
						disToBase = dis;

				}
				return disToBase;
			}
		}
		/// <summary>
		/// the func search and return the location of customer by his name
		/// </summary>
		/// <param name="name"></param>
		/// <returns> an location </returns>
		private BO.Location getLocCustomer(int customerId = 0, string name = null)
		{
			lock (dal)
			{
				if (name == null)
				{
					BO.Location location = new BO.Location();
					foreach (DO.Customer item in dal.GetListCustomers())
					{
						if (item.Id == customerId)
						{
							location.Longitude = item.Longitude;
							location.Latitude = item.Latitude;
							return location;
						}
					}
					throw new BO.IdNotExist("CUSTOMER");
				}
				else//if customer id = 0
				{
					BO.Location loc = new BO.Location();
					foreach (DO.Customer item in dal.GetListCustomers())
					{
						if (item.Name == name)
						{
							loc.Latitude = item.Latitude;
							loc.Longitude = item.Longitude;
						}
					}
					if (loc != null)
						return loc;
					else
						throw new BO.NameNotExist("CUSTOMER");
				}
			}
		}
		#endregion
		#endregion

		#region ADD - OPTION
		/// <summary>
		/// the func add an new base station to the data base
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="chargeSlots"></param>
		/// <param name="location"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddBaseStation(int id, int name, int chargeSlots, BO.Location location)
		{
			lock (dal)
			{
				try
				{ dal.AddBaseStation(id, name, chargeSlots, location.Longitude, location.Latitude); }
				catch (DO.InvalidId ex)//base
				{ throw new BO.InvalidId(ex.Message, ex._type); }

				catch (DO.InvalidChargeSlot ex)
				{ throw new BO.InvalidChargeSlot(ex.Message); }

				catch (DO.InvalidLoc ex)
				{ throw new BO.InvalidLoc(ex.Message, ex._type, ex.range); }

				catch (DO.IdExist ex)
				{ throw new BO.IdExist(ex.Message, ex._type); }
			}
		}
		/// <summary>
		/// the func add a new drone to the data base
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <param name="weight"></param>
		/// <param name="firstBaseStation"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddDrone(int id, string model, BO.WeightCategories weight, int firstBaseStation)
		{
			lock (dal)
			{
				try
				{ dal.GetBaseStation(firstBaseStation); }
				catch (DO.IdNotExist ex)//base or drone
				{ throw new BO.IdNotExist(ex.Message, ex._type); }
				try
				{
					dal.AddDrone(id, model, (DO.WeightCategories)weight);
					dal.AssignDroneToBaseStation(id, firstBaseStation);
				}
				catch (DO.InvalidId ex)//drone id
				{ throw new BO.InvalidId(ex.Message, ex._type); }

				catch (DO.InvalidCategory ex)
				{ throw new BO.InvalidCategory(ex.Message, ex._type); }

				catch (DO.IdExist ex)//drone
				{ throw new BO.IdExist(ex.Message, ex._type); }

				int r = new Random().Next(20, 41);
				double battery = r;
				BO.Location l = new() { Latitude = dal.GetBaseStation(firstBaseStation).Latitude, Longitude = dal.GetBaseStation(firstBaseStation).Longitude };
				droneToList.Add(new BO.DroneToList
				{
					Id = id,
					Model = model,
					MaxWeight = (BO.WeightCategories)weight,
					Battery = battery,
					Status = BO.DroneStatuses.Maintenance,
					Loc = l,
					IdOfParcel = 0,
					WhenInCharge = DateTime.Now
				});
			}
		}
		/// <summary>
		/// the func add a new customer to the data base
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="phone"></param>
		/// <param name="location"></param>ff
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddCustomer(int id, string name, string phone, BO.Location location)
		{
			lock (dal)
			{
				try
				{
					dal.AddCustomer(id, name, phone, location.Longitude, location.Latitude);
				}
				catch (DO.InvalidId ex)//customer
				{ throw new BO.InvalidId(ex.Message, ex._type); }

				catch (DO.InvalidLoc ex)
				{ throw new BO.InvalidLoc(ex.Message, ex._type, ex.range); }

				catch (DO.IdExist ex)//customer
				{ throw new BO.IdExist(ex.Message, ex._type); }
			}
		}
		/// <summary>
		/// the fun add an new parcel to the data base
		/// </summary>
		/// <param name="id"></param>
		/// <param name="senderId"></param>
		/// <param name="targetId"></param>
		/// <param name="weight"></param>
		/// <param name="priorities"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddParcel(int id, int senderId, int targetId, BO.WeightCategories weight, BO.Priorities priorities)
		{
			lock (dal)
			{
				try
				{ dal.AddParcel(id, senderId, targetId, 0, (DO.WeightCategories)weight, (DO.Priorities)priorities); }
				//ב-BL כל הזמנים יאותחלו לזמן אפס למעט תאריך יצירה שיאותחל ל-DateTime.Now
				catch (DO.InvalidId ex)//parcel-id or sender or target 
				{ throw new BO.InvalidId(ex.Message, ex._type); }

				catch (DO.NegativeDroneId ex)//for drone id
				{ throw new BO.InvalidId(ex.Message, "DRONE"); }

				catch (DO.InvalidCategory ex)
				{ throw new BO.InvalidCategory(ex.Message, ex._type); }

				catch (DO.IdExist ex)//parcel
				{ throw new BO.IdExist(ex.Message, ex._type); }

				catch (DO.IdNotExist ex)//customer-target or sender
				{ throw new BO.IdNotExist(ex.Message, ex._type); }

				catch (DO.SenderTargetIdEqual ex)
				{ throw new BO.SenderTargetIdEqual(); }
			}
		}
		#endregion

		#region UPDATE - OPTION
		/// <summary>
		/// the func update the name of model drone
		/// </summary>
		/// <param name="droneId"></param>
		/// <param name="model"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateDrone(int droneId, string model)
		{
			lock (dal)
			{
				try
				{
					DO.Drone drone = new DO.Drone();
					drone = dal.GetDrone(droneId);
					drone.Model = model;
					dal.UpdateDrone(drone);
					for (int i = 0; i < droneToList.Count(); i++)
					{ if (droneToList[i].Id == droneId) droneToList[i].Model = model; }
				}
				catch (DO.IdNotExist ex)//drone
				{ throw new BO.IdNotExist(ex.Message, ex._type); }
			}
		}
		/// <summary>
		/// update the data of data base
		/// </summary>
		/// <param name="baseId"></param>
		/// <param name="name"></param>
		/// <param name="chargeslots"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateBaseStation(int baseId, string name = null, string chargeslots = null)
		{
			DO.BaseStation baseStation = new DO.BaseStation();
			lock (dal)
			{
				try
				{ baseStation = dal.GetBaseStation(baseId); }
				catch (DO.IdNotExist ex)//base
				{ throw new BO.IdNotExist(ex.Message, ex._type); }

				int name_int, charge_slots;
				if (name != "")
				{
					name_int = int.Parse(name);
					baseStation = dal.GetBaseStation(baseId);
					baseStation.Name = name_int;
					dal.UpdateBaseStation(baseStation);
				}
				if (chargeslots != "")
				{
					charge_slots = int.Parse(chargeslots);
					baseStation.ChargeSlots = charge_slots;
					dal.UpdateBaseStation(baseStation);
				}
			}
		}
		/// <summary>
		/// the func update the data of customer
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="name"></param>
		/// <param name="phone"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateCustomer(int customerId, string name = null, string phone = null)
		{
			DO.Customer customer = new DO.Customer();
			lock (dal)
			{
				try
				{ customer = dal.GetCustomer(customerId); }
				catch (DO.IdNotExist ex)//customer
				{ throw new BO.IdExist(ex.Message, ex._type); }
				if (name != "")
				{
					customer = dal.GetCustomer(customerId);
					customer.Name = name;
					dal.UpdateCustomer(customer);
				}
				if (phone != "")
				{
					customer = dal.GetCustomer(customerId);
					customer.Phone = phone;
					dal.UpdateCustomer(customer);
				}
			}
		}
		/// <summary>
		/// the func send the drone to charge
		/// </summary>
		/// <param name="droneId"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DroneToCharge(int droneId)
		{
			BO.DroneToList drone = new BO.DroneToList();
			drone = droneToList[indexDrone(droneId)];
			if (drone.Status != BO.DroneStatuses.free)//the drone is not free so he cant go to charge
				throw new BO.DroneNotFree();
			if (drone.Battery < calcBatteryToCloserBase(drone.Loc.Longitude, drone.Loc.Latitude))//no enough battery to go to charge station
				throw new BO.NotEnoughBattery();
			drone.Battery -= calcBatteryToCloserBase(drone.Loc.Longitude, drone.Loc.Latitude);
			drone.WhenInCharge = DateTime.Now;

			DO.BaseStation baseStation = new DO.BaseStation();
			baseStation = CloserBase(drone.Loc.Longitude, drone.Loc.Latitude);//search the closer base 
			drone.Loc.Longitude = baseStation.Longitude;
			drone.Loc.Latitude = baseStation.Latitude;

			drone.Status = BO.DroneStatuses.Maintenance;

			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (drone.Id == droneToList[i].Id)
					droneToList[i] = drone;
			}
			lock (dal)
			{
				dal.AssignDroneToBaseStation(droneId, baseStation.Id);//update the data base
			}
		}
		/// <summary>
		/// the func release the drone from base station charge
		/// </summary>
		/// <param name="droneId"></param>
		/// <param name="time"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DroneLeaveCharge(int droneId)
		{
			BO.DroneToList drone = new BO.DroneToList();
			drone = droneToList[indexDrone(droneId)];
			if (drone.Status != BO.DroneStatuses.Maintenance)//if the drone id of drone that the user gave is not in Maintenance so need to throw that
				throw new BO.DroneNotInCharge();
			drone.Status = BO.DroneStatuses.free;
			double timeCharging = (DateTime.Now.Subtract(drone.WhenInCharge).TotalSeconds) / 3600;
			drone.Battery += (timeCharging) * _chargingRate;
			drone.Battery = Math.Round(drone.Battery, 3);
			if (drone.Battery > 100)
				drone.Battery = 100;
			DO.BaseStation baseStation = new DO.BaseStation();
			baseStation = currentBase(drone.Loc.Longitude, drone.Loc.Latitude);
			lock (dal)
			{
				dal.DroneLeaveChargeStation(droneId, baseStation.Id);//base station- charge slot++ and remove the drone from the list "drone charge"
			}
		}
		/// <summary>
		/// if the user add new parcel and yet is not assigne to a drone and want to delete him
		/// </summary>
		/// <param name="parcelId"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DeleteParcel(int parcelId)
		{
			lock (dal)
			{
				try
				{
					dal.DeleteParcel(parcelId);
				}
				catch (DO.InvalidId ex) { throw new BO.InvalidId(ex.Message); }
				catch (DO.IdNotExist ex) { throw new BO.IdNotExist(ex.Message); }
				catch (DO.CantRemove ex) { throw new BO.CantRemove(ex.Message); }
			}
		}
		/// <summary>
		/// the func associte the parcel to a drone
		/// </summary>
		/// <param name="droneId"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool AffectParcelToDrone(int droneId)
		{
			int parcelId;
			BO.DroneToList drone = new BO.DroneToList();
			drone = droneToList[indexDrone(droneId)];//O(n)
			if (drone.Status != BO.DroneStatuses.free)
				throw new BO.DroneNotFree();

			DO.Parcel p = new DO.Parcel();
			lock (dal)
			{
				List<DO.Parcel> parcel_ = (List<DO.Parcel>)dal.GetListParcels(p => p.DroneId == 0);//new List<DO.Parcel>();
				if (parcel_.Count() == 0)
					throw new BO.AllParcelAssoc();
				List<DO.Parcel> parcelEmrgency = (List<DO.Parcel>)dal.GetListParcels(p => p.DroneId == 0);//removeByPriority(parcel, DO.Priorities.Emergecey);//O(n)
				parcelEmrgency.RemoveAll(item => (int)item.Priority != 3);
				parcelEmrgency.RemoveAll(item => (int)item.Weight > (int)drone.MaxWeight);//(parcelEmrgency, (DO.WeightCategories)drone.MaxWeight);
				parcelEmrgency.OrderByDescending(item => (int)item.Weight).ToList();
				parcelId = chooseParcel(parcelEmrgency, drone);
				if (parcelId != -1)
				{
					drone.Status = BO.DroneStatuses.Shipping;//change status of drone 
					drone.IdOfParcel = parcelId;
					updateDroneList(drone);//update drone in data base
					p = parcel_.Find(item => (item.Id == parcelId));//find parcel from the list of parcel by id
					p.DroneId = drone.Id;
					p.Scheduled = DateTime.Now;//update time of 
					dal.UpdateParcel(p);
					return true;//no need to continue
				}
				//no found a parcel in priority emergency so go to the next priority
				List<DO.Parcel> parcelFast = (List<DO.Parcel>)dal.GetListParcels(p => p.DroneId == 0);//removeByPriority(parcel, DO.Priorities.Emergecey);//O(n)
				parcelFast.RemoveAll(item => (int)item.Priority != 2);
				parcelFast.RemoveAll(item => (int)item.Weight > (int)drone.MaxWeight);//(parcelEmrgency, (DO.WeightCategories)drone.MaxWeight);
				parcelFast.OrderByDescending(item => (int)item.Weight).ToList();
				parcelId = chooseParcel(parcelFast, drone);
				if (parcelId != -1)
				{
					drone.Status = BO.DroneStatuses.Shipping;//change status of drone 
					drone.IdOfParcel = parcelId;
					updateDroneList(drone);//update drone in data base
					p = parcel_.Find(item => (item.Id == parcelId));//find parcel from the list of parcel by id
					p.DroneId = drone.Id;
					p.Scheduled = DateTime.Now;//update time of 
					dal.UpdateParcel(p);
					return true;//no need to continue
				}
				//no found a parcel in priority fast so go to the next priority
				List<DO.Parcel> parcelNormal = (List<DO.Parcel>)dal.GetListParcels(p => p.DroneId == 0);//removeByPriority(parcel, DO.Priorities.Emergecey);//O(n)
				parcelNormal.RemoveAll(item => (int)item.Priority != 1);
				parcelNormal.RemoveAll(item => (int)item.Weight > (int)drone.MaxWeight);//(parcelEmrgency, (DO.WeightCategories)drone.MaxWeight);
				parcelNormal.OrderByDescending(item => (int)item.Weight).ToList();
				parcelId = chooseParcel(parcelNormal, drone);
				if (parcelId != -1)
				{
					drone.Status = BO.DroneStatuses.Shipping;//change status of drone 
					drone.IdOfParcel = parcelId;
					updateDroneList(drone);//update drone in data base
					p = parcel_.Find(item => (item.Id == parcelId));//find parcel from the list of parcel by id
					p.DroneId = drone.Id;
					p.Scheduled = DateTime.Now;//update time of 
					dal.UpdateParcel(p);
					return true;//no need to continue
				}
				throw new BO.NoDroneCanParcel();//the drone can't take any parcel
			}
		}
		/// <summary>
		/// collect a parcel to deliver
		/// </summary>
		/// <param name="droneId"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ParcelCollection(int droneId)
		{
			BO.DroneToList drone = new BO.DroneToList();
			drone = droneToList[indexDrone(droneId)];
			DO.Parcel parcel = new DO.Parcel();
			lock (dal)
			{
				parcel = (from item in dal.GetListParcels(p => true)
						  let id =item.DroneId
						  where (id == drone.Id && item.Id==drone.IdOfParcel)
						  select item).FirstOrDefault();
				if (parcel.DroneId != droneId)
					throw new BO.NoParcelId();
				if (parcel.PickedUp != null)//if parcel pick-up
					throw new BO.AlreadyPickedUp();
				if (parcel.Scheduled == null)// if parcel not associate to a drone 
					throw new BO.NotScheduledYet();
				parcel.PickedUp = DateTime.Now;
				dal.UpdateParcel(parcel);
				drone.Battery -= (int)(Math.Ceiling(_useWhenFree * distanceBetweenTwoPoints(drone.Loc.Latitude, drone.Loc.Longitude, getLocCustomer(parcel.SenderId).Latitude, getLocCustomer(parcel.SenderId).Longitude)));
				drone.Loc.Longitude = getLocCustomer(parcel.SenderId).Longitude;//update the location to the sender
				drone.Loc.Latitude = getLocCustomer(parcel.SenderId).Latitude;
				updateDroneList(drone);
			}
		}
		/// <summary>
		/// the func updaste that the drone was deliver the parcel
		/// </summary>
		/// <param name="droneId"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ParcelDeliverd(int droneId)
		{
			BO.DroneToList drone = new BO.DroneToList();
			drone = droneToList[indexDrone(droneId)];
			DO.Parcel parcel = new DO.Parcel();
			lock (dal)
			{
				parcel = (from item in dal.GetListParcels(p => true)
						  let id=item.DroneId
						  where (id == drone.Id && item.Id == drone.IdOfParcel)
						  select item).FirstOrDefault();
				if (parcel.DroneId != droneId)
					throw new BO.NoParcelId();
				if (parcel.PickedUp == null) //if parcel not pick-up
					throw new BO.NotPickedUpYet();
				if (parcel.Delivered != null) //if parcel was deliver
					throw new BO.AlreadyDelivered();
				parcel.Delivered = DateTime.Now;
				dal.UpdateParcel(parcel);
				drone.Battery -= calcBatteryUsedWhenShipping(parcel.Weight, distanceBetweenTwoPoints(drone.Loc.Latitude, drone.Loc.Longitude, getLocCustomer(parcel.TargetId).Latitude, getLocCustomer(parcel.TargetId).Longitude));
				drone.Loc.Longitude = getLocCustomer(parcel.TargetId).Longitude;//update the location to the target
				drone.Loc.Latitude = getLocCustomer(parcel.TargetId).Latitude;
				drone.Status = BO.DroneStatuses.free;
				drone.IdOfParcel = null;
				updateDroneList(drone);
			}
		}
		/// <summary>
		/// function for charging drone in the simulator
		/// </summary>
		/// <param name="droneId"></param>
		public void Fullycharged_simulator(int droneId)
        {
			BO.DroneToList drone = new BO.DroneToList();
			drone = droneToList[indexDrone(droneId)];
			if (drone.Status != BO.DroneStatuses.Maintenance)//if the drone id of drone that the user gave is not in Maintenance so need to throw that
				throw new BO.DroneNotInCharge();
			drone.Battery += 2;
			if (drone.Battery >= 100)
			{
				DO.BaseStation baseStation = new DO.BaseStation();
				baseStation = currentBase(drone.Loc.Longitude, drone.Loc.Latitude);
				drone.Battery = 100;
				drone.Status = BO.DroneStatuses.free;
				lock (dal)
				{
					dal.DroneLeaveChargeStation(droneId, baseStation.Id);//base station- charge slot++ and remove the drone from the list "drone charge"
				}
			}
		}
		/// <summary>
		/// function to soustract battery for the simulator
		/// </summary>
		/// <param name="droneId"></param>
		/// <param name="num"></param>
		public void Minus_Battery(int droneId,int num)
        {
			BO.DroneToList drone = new BO.DroneToList();
			drone = droneToList[indexDrone(droneId)];
			drone.Battery +=num;
		}
		public void Simulator(int droneId, Action<BO.Drone> ReportProgressSimulator, Func<bool> Cancellation)
        {
			new Background_Worker(this, droneId, ReportProgressSimulator, Cancellation);
        }
		#endregion

		#region DISPLAY SPECIFIC OBJECT - OPTION
		/// <summary>
		/// the func search a base station in the data base 
		/// </summary>
		/// <param name="baseId"></param>
		/// <returns> an base station </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public BO.BaseStation GetBaseStation(int baseId)
		{
			BO.BaseStation baseS = new BO.BaseStation();
			DO.BaseStation b = new DO.BaseStation();
			lock (dal)
			{
				try
				{ b = dal.GetBaseStation(baseId); }
				catch (DO.IdNotExist ex)//base
				{ throw new BO.IdNotExist(ex.Message, ex._type); }

				baseS.Id = b.Id;
				baseS.Name = b.Name;
				baseS.ChargeSlots = b.ChargeSlots;
				baseS.Loc.Latitude = b.Latitude;
				baseS.Loc.Longitude = b.Longitude;

				for (int i = 0; i < droneToList.Count(); i++)
				{
					if (droneToList[i].Status == BO.DroneStatuses.Maintenance && (droneToList[i].Loc.Latitude == baseS.Loc.Latitude && droneToList[i].Loc.Longitude == baseS.Loc.Longitude))
						baseS.DroneInCharge.Add(new BO.DroneCharge { DroneId = droneToList[i].Id, Battery = droneToList[i].Battery });
				}
				return baseS;

			}
		}
		/// <summary>
		/// the func search an drone in the data base
		/// </summary>
		/// <param name="droneId"></param>
		/// <returns> an drone </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public BO.Drone GetDrone(int droneId)
		{
			BO.Drone drone = new BO.Drone();
			lock (dal)
			{
				try
				{ dal.GetDrone(droneId); }
				catch (DO.IdNotExist ex)//drone
				{ throw new BO.IdNotExist(ex.Message, ex._type); }
				for (int i = 0; i < droneToList.Count(); i++)//get all drone
				{
					if (droneToList[i].Id == droneId)
					{
						drone.Id = droneToList[i].Id;
						drone.Model = droneToList[i].Model;
						drone.MaxWeight = droneToList[i].MaxWeight;
						drone.Status = droneToList[i].Status;
						drone.Battery = droneToList[i].Battery;
						drone.Loc.Longitude = droneToList[i].Loc.Longitude;
						drone.Loc.Latitude = droneToList[i].Loc.Latitude;
						break;
					}
				}
				if (drone.Status == BO.DroneStatuses.Shipping)//if drone is in shipping searche who is the parcel
				{
					BO.DroneToList d = droneToList.Find(d => d.Id == drone.Id);
					foreach (DO.Parcel item in dal.GetListParcels(p => true))
					{
						if (item.DroneId == d.Id && (item.Delivered == null))
						{ drone.InTransit = convertParcel(item, droneId,drone.Loc); break; }
					}
				}
				return drone;
			}
		}
		/// <summary>
		/// the func return a customer
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public BO.Customer GetCustomer(int customerId)
		{
			BO.Customer customer = new BO.Customer();
			BO.CustomerInParcel target = new BO.CustomerInParcel();
			BO.CustomerInParcel sender = new BO.CustomerInParcel();

			customer = getCustomerInIBL(customerId);
			lock (dal)
			{
				foreach (DO.Parcel item in dal.GetListParcels(p => p.Id != 0))//add to the list parcel that the customer is the sender
				{
					sender.Id = item.SenderId;//
					sender.Name = getCustomerInIBL(item.SenderId).Name;
					if (item.SenderId == customerId)//if the customer is the sender
					{
						customer.ParcelFromCustomer.Add(new BO.ParcelAtCustomer
						{
							Id = item.Id,
							Weight = (BO.WeightCategories)item.Weight,
							Priority = (BO.Priorities)item.Priority,
							status = getStatusOfParcel(item),
							SenderOrTraget = sender,
						});
					}
				}
				foreach (DO.Parcel item in dal.GetListParcels(p => p.Id != 0))//add to the list parcel that the customer is the target
				{
					target.Id = item.TargetId;
					target.Name = getCustomerInIBL(item.TargetId).Name;
					if (item.TargetId == customerId)//if the customer is the target
					{
						customer.ParcelToCustomer.Add(new BO.ParcelAtCustomer
						{
							Id = item.Id,
							Weight = (BO.WeightCategories)item.Weight,
							Priority = (BO.Priorities)item.Priority,
							status = getStatusOfParcel(item),
							SenderOrTraget = target,
						});
					}
				}
				return customer;
			}
		}
		/// <summary>
		/// the func search and return a parcel
		/// </summary>
		/// <param name="parcelId"></param>
		/// <returns> an parcel </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public BO.Parcel GetParcel(int parcelId)
		{
			BO.Parcel parcel = new BO.Parcel();
			BO.CustomerInParcel sender = new BO.CustomerInParcel();
			BO.CustomerInParcel target = new BO.CustomerInParcel();
			BO.DroneToList drone = new BO.DroneToList();
			DO.Parcel p = new DO.Parcel();
			lock (dal)
			{
				try
				{ p = dal.GetParcel(parcelId); }
				catch (DO.IdNotExist ex)
				{ throw new BO.IdNotExist(ex.Message, ex._type); }
			}
			parcel.Id = p.Id;
			parcel.Weight = (BO.WeightCategories)p.Weight;
			parcel.Priority = (BO.Priorities)p.Priority;
			parcel.Requested = p.Requested;
			parcel.Scheduled = p.Scheduled;
			parcel.PickedUp = p.PickedUp;
			parcel.Delivered = p.Delivered;

			sender.Id = p.SenderId;
			sender.Name = getCustomerInIBL(sender.Id).Name;
			parcel.Sender = sender;

			target.Id = p.TargetId;
			target.Name = getCustomerInIBL(target.Id).Name;
			parcel.Target = target;

			drone = droneToList.Find(item => item.IdOfParcel == parcelId);
			if (drone != null)
			{
				parcel.Drone.Id = drone.Id;
				parcel.Drone.Battery = drone.Battery;
				parcel.Drone.Loc = drone.Loc;
			}

			return parcel;
		}
		#endregion

		#region DISPLAY LIST OF AN OBJECT - OPTION
		/// <summary>
		/// the func return a list of base station
		/// </summary>
		/// <returns> a list of base station </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<BO.BaseToList> GetListOfBaseStations(Predicate<DO.BaseStation> f)
		{
			List<BO.BaseToList> baseS = new List<BO.BaseToList>();
			lock (dal)
			{
				foreach (DO.BaseStation item in dal.GetListBaseStations(f))
				{
					BO.Location l = new() { Latitude = item.Latitude, Longitude = item.Longitude };
					baseS.Add(new BO.BaseToList
					{
						Id = item.Id,
						Name = item.Name,
						ChargeSlots = item.ChargeSlots,
						ChargeBusy = howManyCharge(l)
					});
				}
				return (IEnumerable<BO.BaseToList>)baseS;
			}
		}
		/// <summary>
		/// the func return an list of drone
		/// </summary>
		/// <returns> list of drone </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<BO.DroneToList> GetListOfDrones(Predicate<BO.DroneToList> f)
		{
			List<BO.DroneToList> drones = new();
			lock (dal)
			{
				foreach (DO.Drone item in dal.GetListDrones(d => true))
				{
					BO.Location l = new() { Latitude = droneToList.Find(d => d.Id == item.Id).Loc.Latitude, Longitude = droneToList.Find(d => d.Id == item.Id).Loc.Longitude };
					drones.Add(new()
					{
						Id = item.Id,
						Model = item.Model,
						MaxWeight = (BO.WeightCategories)item.MaxWeight,
						Battery = droneToList.Find(d => d.Id == item.Id).Battery,
						Status = droneToList.Find(d => d.Id == item.Id).Status,
						Loc = l,
						IdOfParcel = droneToList.Find(d => d.Id == item.Id).IdOfParcel
					});
				}
				return (IEnumerable<BO.DroneToList>)drones.FindAll(f);
			}
		}
		/// <summary>
		/// the func return an list of customer
		/// </summary>
		/// <returns> list of customer </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<BO.CustomerToList> GetListOfCustomers()
		{
			List<BO.CustomerToList> customer = new List<BO.CustomerToList>();
			lock (dal)
			{
				foreach (DO.Customer item in dal.GetListCustomers())
				{
					customer.Add(new BO.CustomerToList
					{
						Id = item.Id,
						Name = item.Name,
						Phone = item.Phone,
						ParcelDelivred = parcelWasDliver(item.Id),
						ParcelSentNotDelivred = parcelSentNotDeliver(item.Id),
						ParcelRecived = parcelRecive(item.Id),
						ParcelInTransit = parcelInTransit(item.Id)
					});
				}
				return (IEnumerable<BO.CustomerToList>)customer;
			}
		}
		/// <summary>
		/// the func return a list of parcel 
		/// </summary>
		/// <returns> list of parcel </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<BO.ParcelToList> GetListOfParcels(Predicate<DO.Parcel> f)
		{
			List<BO.ParcelToList> parcel = new List<BO.ParcelToList>();
			lock (dal)
			{
				foreach (DO.Parcel item in dal.GetListParcels(f))
				{
					parcel.Add(new BO.ParcelToList
					{
						Id = item.Id,
						NameSender = dal.GetCustomer(item.SenderId).Name,
						NameTarget = dal.GetCustomer(item.TargetId).Name,
						Weight = (BO.WeightCategories)item.Weight,
						Priority = (BO.Priorities)item.Priority,
						Status = getStatusOfParcel(item)
					});
				}
				return (IEnumerable<BO.ParcelToList>)parcel;
			}
		}
		#endregion

		#region HELP FUNC
		/// <summary>
		/// the func calculate the battery use for each parcel and their weight
		/// </summary>
		/// <param name="weight"></param>
		/// <param name="dist"></param>
		/// <returns> battery in percent </returns>
		private int calcBatteryUsedWhenShipping(DO.WeightCategories weight, double dist)
		{
			double batteryUsed = 0;
			if (weight == DO.WeightCategories.Light) batteryUsed = _useWhenLightly * dist;
			if (weight == DO.WeightCategories.Medium) batteryUsed = _useWhenMedium * dist;
			if (weight == DO.WeightCategories.Heavy) batteryUsed = _useWhenHeavily * dist;
			return ((int)(Math.Ceiling(batteryUsed)));
		}
		/// <summary>
		/// the func update the list of drone "drone to list" for the maintenance the project
		/// </summary>
		/// <param name="drone"></param>
		private void updateDroneList(BO.DroneToList drone)
		{
			for (int i = 0; i < droneToList.Count(); i++)
				if (droneToList[i].Id == drone.Id) droneToList[i] = drone;
		}
		/// <summary>
		/// the func search who is the current base by his location 
		/// </summary>
		/// <param name="longi"></param>
		/// <param name="lati"></param>
		/// <returns> the current base station </returns>
		private DO.BaseStation currentBase(double longi, double lati)
		{
			DO.BaseStation b;
			lock (dal)
			{
				b = (from item in dal.GetListBaseStations(b => true)
					 where (item.Longitude == longi && item.Latitude == lati)
					 select item).FirstOrDefault();
				if (b.Id == 0)
					throw new BO.DroneNotInBase();
				else
					return b;
			}
			
		}
		/// <summary>
		/// the func calculate who is the closer base by his location
		/// </summary>
		/// <param name="longi"></param>
		/// <param name="lati"></param>
		/// <returns> a base station </returns>
		private DO.BaseStation CloserBase(double longi, double lati)
		{
			double disToBase = -1;
			DO.BaseStation baseStation = new DO.BaseStation();
			lock (dal)
			{
				foreach (DO.BaseStation b in dal.GetListBaseStations(b => b.Id != 0))
				{
					double dis = distanceBetweenTwoPoints(b.Latitude, b.Longitude, lati, longi);
					if ((disToBase == -1 || dis < disToBase) && b.ChargeSlots > 0)//if "dis" that is calculate in the previous line is smaller than the "disToBase"= the smaller distanse right now so "disToBase = dis"
					{
						disToBase = dis;
						baseStation = b;
					}
				}
				return baseStation;
			}
		}
		/// <summary>
		/// the func convert a parcel from type idal to ibl include add data for the type ibl
		/// </summary>
		/// <param name="p"></param>
		/// <returns> an parcel </returns>
		private BO.ParcelInTransit convertParcel(DO.Parcel p, int droneId,BO.Location loc)
		{
			BO.ParcelInTransit parcel = new BO.ParcelInTransit();
			BO.CustomerInParcel customerS = new BO.CustomerInParcel();
			BO.CustomerInParcel customerT = new BO.CustomerInParcel();

			parcel.Id = p.Id;
			parcel.Weight = (BO.WeightCategories)p.Weight;
			parcel.Priority = (BO.Priorities)p.Priority;
			parcel.LocPickedUp.Longitude = getCustomerInIBL(p.SenderId).Loc.Longitude;
			parcel.LocPickedUp.Latitude = getCustomerInIBL(p.SenderId).Loc.Latitude;
			parcel.LocDelivered.Longitude = getCustomerInIBL(p.TargetId).Loc.Longitude;
			parcel.LocDelivered.Latitude = getCustomerInIBL(p.TargetId).Loc.Latitude;
			if (GetParcel(p.Id).PickedUp == null)
			{
				parcel.Status = false;
				parcel.DistanceDelivery = distanceBetweenTwoPoints(loc.Latitude, loc.Longitude, parcel.LocPickedUp.Latitude, parcel.LocPickedUp.Longitude);
			}
			else
			{
				parcel.Status = true;
				parcel.DistanceDelivery = distanceBetweenTwoPoints(parcel.LocPickedUp.Latitude, parcel.LocPickedUp.Longitude, parcel.LocDelivered.Latitude, parcel.LocDelivered.Longitude);
			}
			//parcel.DistanceDelivery = distanceBetweenTwoPoints(parcel.LocPickedUp.Latitude, parcel.LocPickedUp.Longitude, parcel.LocDelivered.Latitude, parcel.LocDelivered.Longitude);
			lock (dal)
			{
				foreach (DO.Customer item in dal.GetListCustomers())//search who is the sender and who is the target
				{
					if (p.SenderId == item.Id)
					{ customerS.Id = item.Id; customerS.Name = item.Name; }
					if (p.TargetId == item.Id)
					{ customerT.Id = item.Id; customerT.Name = item.Name; }
				}
			}
			parcel.Sender = customerS;
			parcel.Target = customerT;
			return parcel;
		}
		/// <summary>
		/// the func return the status of the parcel
		/// </summary>
		/// <param name="p"></param>
		/// <returns> status </returns>
		private BO.ParcelStatues getStatusOfParcel(DO.Parcel p)
		{
			BO.ParcelStatues statues = BO.ParcelStatues.Defined;
			if (p.Scheduled != null)
				statues = BO.ParcelStatues.Associated;
			if (p.PickedUp != null)
				statues = BO.ParcelStatues.Collected;
			if (p.Delivered != null)
				statues = BO.ParcelStatues.Delivered;

			return statues;
		}
		/// <summary>
		/// the func serche how many parcel the customer sent and their was deliver
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns> number of parcel </returns>
		private int parcelWasDliver(int customerId)
		{
			lock (dal)
			{
				int counter = 0;
				foreach (DO.Parcel item in dal.GetListParcels(p => true))
				{
					if (item.SenderId == customerId && item.Delivered != null)
						counter++;
				}
				return counter;
			}
		}
		/// <summary>
		/// the func serche how many parcel the customer sent but yet is not delivered
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns> number of parcel </returns>
		private int parcelSentNotDeliver(int customerId)
		{
			lock (dal)
			{
				int counter = 0;
				foreach (DO.Parcel item in dal.GetListParcels(p => true))
				{
					if (item.SenderId == customerId && item.Delivered == null && item.PickedUp != null)
						counter++;
				}
				return counter;
			}
		}
		/// <summary>
		/// the func serche how many parcel the customer recive
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns> number of parcel </returns>
		private int parcelRecive(int customerId)
		{
			lock (dal)
			{
				int counter = 0;
				foreach (DO.Parcel item in dal.GetListParcels(p => true))
				{
					if (item.TargetId == customerId && item.Delivered != null)
						counter++;
				}
				return counter;
			}
		}
		/// <summary>
		/// the func serche how many parcel the customer need to recive in future(parcel is ion tarnsit)
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		private int parcelInTransit(int customerId)
		{
			lock (dal)
			{
				int counter = 0;
				foreach (DO.Parcel item in dal.GetListParcels(p => true))
				{
					if (item.TargetId == customerId && item.Delivered == null && item.PickedUp != null)
						counter++;
				}
				return counter;
			}
		}
		/// <summary>
		/// the func calculate how many drone charge in the base station by his location
		/// </summary>
		/// <param name="loc"></param>
		/// <returns> return a number of drone </returns>
		private int howManyCharge(BO.Location loc)
		{
			lock (dal)
			{
				int counter = 0;
				foreach (BO.DroneToList d in droneToList)
				{
					if (d.Status == BO.DroneStatuses.Maintenance && loc.Longitude == d.Loc.Longitude && loc.Latitude == d.Loc.Latitude)
						counter++;
				}
				return counter;
			}
		}
		/// <summary>
		/// the func calculate by priorities and battery whiche parcel the drone can take and if it is possible return the id of parcel
		/// </summary>
		/// <param name="p"></param>
		/// <param name="drone"></param>
		/// <returns> id of parcel </returns>
		private int chooseParcel(List<DO.Parcel> p, BO.DroneToList drone)
		{
			List<BO.ParcelToList> parcelA = new List<BO.ParcelToList>();//A,B,C IS PRIORITIES
			List<BO.ParcelToList> parcelB = new List<BO.ParcelToList>();
			List<BO.ParcelToList> parcelC = new List<BO.ParcelToList>();
			for (int i = 0; i < p.Count(); i++)
			{
				//if the next parcel is equal to maxWeight
				if (p[i].Weight == (DO.WeightCategories)drone.MaxWeight)
				{
					parcelA.Add(new BO.ParcelToList
					{
						Id = p[i].Id,
						NameSender = GetCustomer(p[i].SenderId).Name,
						NameTarget = GetCustomer(p[i].TargetId).Name,
						Weight = (BO.WeightCategories)p[i].Weight
					});
				}
				//if the next parcel is equal to maxWeight -1 but not 0 (according the enum)
				else if ((p[i].Weight == (DO.WeightCategories)((int)drone.MaxWeight) - 1) && ((int)(drone.MaxWeight - 1) != 0))
				{
					parcelB.Add(new BO.ParcelToList
					{
						Id = p[i].Id,
						NameSender = GetCustomer(p[i].SenderId).Name,
						NameTarget = GetCustomer(p[i].TargetId).Name,
						Weight = (BO.WeightCategories)p[i].Weight
					});
				}
				// if the next parcel is equal to maxWeight - 2 but not 0(according the enum)
				else if ((p[i].Weight == (DO.WeightCategories)((int)drone.MaxWeight) - 2) && ((int)(drone.MaxWeight - 2) != 0))
				{
					parcelC.Add(new BO.ParcelToList
					{
						Id = p[i].Id,
						NameSender = GetCustomer(p[i].SenderId).Name,
						NameTarget = GetCustomer(p[i].TargetId).Name,
						Weight = (BO.WeightCategories)p[i].Weight
					});
				}
			}
			if (parcelA.Count() != 0)
			{
				int idA = closerParcel(parcelA, drone);
				if (calcBatteryToShipping(drone, parcelA.Find(item => (item.Id == idA))) <= drone.Battery)//if the closer parcel in list A the drone can do the shipping
					return idA;
			}
			if (parcelB.Count() != 0)
			{
				int idB = closerParcel(parcelB, drone);
				if (calcBatteryToShipping(drone, parcelB.Find(item => (item.Id == idB))) <= drone.Battery)//if the closer parcel in list B the drone can do the shipping
					return idB;
			}
			if (parcelC.Count() != 0)
			{
				int idC = closerParcel(parcelC, drone);
				if (calcBatteryToShipping(drone, parcelC.Find(item => (item.Id == idC))) <= drone.Battery)//if the closer parcel in list C the drone can do the shipping
					return idC;
			}
			return -1;//if the drone can take any parcel return -1
		}
		/// <summary>
		/// calculate the closer parcel from drone
		/// </summary>
		/// <param name="parcel"></param>
		/// <returns> the id of parcel </returns>
		private int closerParcel(List<BO.ParcelToList> parcel, BO.DroneToList drone)
		{
			double disToParcel = -1;
			BO.ParcelToList p = new BO.ParcelToList();
			foreach (BO.ParcelToList item in parcel)
			{
				BO.Location locParcel = new BO.Location { Longitude = getLocCustomer(0,item.NameSender).Longitude, Latitude = getLocCustomer(0,item.NameSender).Latitude };
				double dis = distanceBetweenTwoPoints(locParcel.Latitude, locParcel.Longitude, drone.Loc.Latitude, drone.Loc.Longitude);
				if (disToParcel == -1 || dis < disToParcel)//if "dis" that is calculate in the previous line is smaller than the "disToParcel"= the smaller distanse right now so "disToParcel = dis"
				{ disToParcel = dis; p = item; }
			}
			return p.Id;
		}
		#endregion

		#region SIMULATOR
		/// <summary>
		/// simulator constructor
		/// </summary>
		public class Background_Worker
        {
			private System.ComponentModel.BackgroundWorker backgroundWorker1 = new BackgroundWorker();
			private const int timer = 500;
			public Background_Worker(BL ibl,int droneId, Action<BO.Drone> ReportProgressSimulator,Func<bool> Cancellation)
            {
				BlApi.IBL bl= ibl;
				BO.Drone drone = new();
				drone = bl.GetDrone(droneId);
				bool flag = true;
				while (flag)
				{
					if (drone.Status == BO.DroneStatuses.Maintenance)
					{
						while (drone.Battery != 100)
						{
							Thread.Sleep(timer);
							bl.Fullycharged_simulator(drone.Id);
							drone = bl.GetDrone(drone.Id);	
							ReportProgressSimulator(drone);
						}
					}
					else if (drone.Status == BO.DroneStatuses.free)
					{
						try
						{
							bl.AffectParcelToDrone(drone.Id);
							drone = bl.GetDrone(drone.Id);
							ReportProgressSimulator(drone);
						}
						catch (Exception)
						{
							Thread.Sleep(2000);
							if (drone.Battery != 100)
							{
								double bat = drone.Battery;
								try
								{
									bl.DroneToCharge(drone.Id);
									bat -= bl.GetDrone(drone.Id).Battery;
									bl.Minus_Battery(drone.Id, (int)bat);
									for (double i = bat; i > 2; i -= 2)
									{
										bl.Minus_Battery(drone.Id, -2);
										drone = bl.GetDrone(drone.Id);
										ReportProgressSimulator(drone);
										Thread.Sleep(timer);
									}
								}
                                catch (Exception)
								{
									Thread.Sleep(2000);
                                }
							}
						}

					}
					else if (drone.Status == BO.DroneStatuses.Shipping)
					{
						try
						{
							double bat = drone.Battery;
							bl.ParcelCollection(drone.Id);
							bat -= bl.GetDrone(drone.Id).Battery;
							bl.Minus_Battery(drone.Id, (int)bat);
							for (double i = bat; i > 2; i -= 2)
							{
								bl.Minus_Battery(drone.Id, -2);
								drone = bl.GetDrone(drone.Id);
								ReportProgressSimulator(drone);
								Thread.Sleep(timer);
							}

							bat = drone.Battery;
							bl.ParcelDeliverd(drone.Id);
							bat -= bl.GetDrone(drone.Id).Battery;
							bl.Minus_Battery(drone.Id, (int)bat);
							for (double i = bat; i > 2; i -= 2)
							{
								bl.Minus_Battery(drone.Id, -2);
								drone = bl.GetDrone(drone.Id);
								ReportProgressSimulator(drone);
								Thread.Sleep(timer);
							}
						}
						catch (Exception)
						{
							Thread.Sleep(timer);
						}
					}
					if (Cancellation())
					{
						flag = false;
					}

				}
			}
			
		}
        #endregion
    }
}