using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	// exceptions - problem with id
	public class IdExist : Exception//id of drone, base.... exist
	{
		public IdExist(string type_) : base($"IBL-ERROR: THIS {type_} ID ALREADY EXIST\n") { }
		public IdExist(string ms, string type_) : base($"{ms}\nIBL-ERROR: THIS {type_} ID ALREADY EXIST\n") { }
	}
	public class IdNotExist : Exception//id of drone, base.... not exist
	{
		public IdNotExist(string type_) : base($"IBL-ERROR: THIS {type_} ID DO NOT EXIST\n") { }
		public IdNotExist(string ms, string type_) : base($"{ ms}\nIBL-ERROR: THIS {type_} ID DO NOT EXIST\n") { }
	}
	public class InvalidId : Exception// the id of base,drone.... is negative or equal to zero
	{
		public InvalidId(string type_) : base($"IBL-ERROR: ID OF {type_} TO BE BIGGER THAN ZERO\n") { }
		public InvalidId(string ms, string type_) : base($"{ms}\nIBL-ERROR: ID OF {type_} TO BE BIGGER THAN ZERO\n") { }
	}

	//the range of longi or lati is not correct 
	public class InvalidLoc : Exception
	{
		public string type;
		public string range;
		public InvalidLoc(string type_, string range_) : base($"IBL-ERROR: {type_} NEED TO BE BEHTWEEN {range_}\n") { type = type_;range = range_; }
		public InvalidLoc(string ms, string type_, string range_) : base($"{ms}\nIBL-ERROR: {type_} NEED TO BE BEHTWEEN {range_}\n") { type = type_; range = range_; }
	}

	// exceptions - problem with Categories
	public class InvalidCategory : Exception//WEIGHT PRIORITIES
	{
		public InvalidCategory(string type_) : base($"IBL-ERROR: THIS OPTION FOR {type_} IS NOT EXIST, YOU NEED TO GIVE AN OPTION BETWEEN 1 TO 3\n") { }
		public InvalidCategory(string ms, string type_) : base($"{ms}\nIBL-ERROR: THIS OPTION FOR {type_} IS NOT EXIST, YOU NEED TO GIVE AN OPTION BETWEEN 1 TO 3\n") { }
	}
	//if the name not exist
	public class NameNotExist : Exception
	{
		public NameNotExist(string type_) : base($"IBL-ERROR: THIS {type_} NAME DO NOT EXIST\n") { }
	}

	// if user inser negativ value for charge slots
	public class InvalidChargeSlot : Exception
	{
		public InvalidChargeSlot(string ms) : base($"{ms}\nIBL-ERROR: CHARGE SLOT NEED TO BE BIGGER THAN ZERO OR EQUAL TO ZERO\n") { }
		public InvalidChargeSlot() : base("IBL-ERROR: CHARGE SLOT NEED TO BE BIGGER THAN ZERO OR EQUAL TO ZERO\n") { }
	}

	// exceptions specific for drone
	public class NegativeDroneId : Exception//if the user give an negative value when he need to give a drone id for parcel
	{
		public NegativeDroneId() : base("IBL-ERROR: THE DRONE ID NEED TO BE POSITIVE OR ZERO(0=NOT ASSIGNE TO AN SPECIFIC DRONE, BIGEER THAN ZERO=ASSIGNE TO AN SPECIFIC DRONE\n") { }
	}
	public class DroneNotFree : Exception//if the drone is not free
	{
		public DroneNotFree() : base("IBL-ERROR: The drone can't be assigned to a charge station since it's not free.\n") { }
	}
	public class DroneNotMaintenance : Exception//if the user want to bring a drone leave a charge station but the drone is not in Maintenance
	{
		public DroneNotMaintenance() : base("IBL-ERROR: The drone can't leave a charge station since it's not in maintenance.\n") { }
	}
	public class DroneNotInBase : Exception//if the user want to bring a drone leave a charge station but the drone is not in base station 
	{
		public DroneNotInBase() : base("IBL-ERROR: The drone can't leave a charge station since it's not in a base station.\n") { }
	}
	public class NotEnoughBattery : Exception//if the drone no has enought battery to do the shipping
	{
		public NotEnoughBattery() : base("IBL-ERROR: The drone hasn't enough battery to join the closer base station\n") { }
	}
	public class NoDroneCanParcel : Exception//if the drone cant take any parcel 
	{
		public NoDroneCanParcel() : base("IBL-ERROR: No drone can ship this parcel\n") { }
	}
	public class NoParcelId : Exception//if the parcel is not affiliated to this drone
	{
		public NoParcelId() : base("IBL-ERROR: This parcel isn't affiliated to this drone\n") { }
	}
	public class DroneNotInCharge : Exception// if the drone is not charging right now
	{
		public DroneNotInCharge() : base("IBL-ERROR: ERROR IN THE DATA BASE AN DRONE IS NOT CHARGE\n") { }
	}

	// exceptions specific for parcel
	public class AlreadyPickedUp : Exception//if the parcel alredy  picked up
	{
		public AlreadyPickedUp() : base("IBL-ERROR: This parcel has already been picked up\n") { }
	}
	public class NotScheduledYet : Exception//if the parcel no Scheduled Yet
	{
		public NotScheduledYet() : base("IBL-ERROR: This parcel hasn't been requested yet\n") { }
	}
	public class NotPickedUpYet : Exception//if the parcel no picked up
	{
		public NotPickedUpYet() : base("IBL-ERROR: This parcel hasn't been picked up yet\n") { }
	}
	public class AlreadyDelivered : Exception//if the parcel alredy 
	{
		public AlreadyDelivered() : base("IBL-ERROR: This parcel has already been delivered\n") { }
	}
	
	public class AllParcelAssoc : Exception// if all parcel is alredy associated
	{
		public AllParcelAssoc() : base("IBL: ALL PARCEL ALREDY ASSOCIATED\n") { }
	}

	// exceptions for Weight that drone can't take
	public class ParcelTooHeavy : Exception//if the parcel is too heavy and the drone cant take 
	{
		public ParcelTooHeavy() : base("IBL-ERROR: THE CATEGORIE OF PARCEL IS TOO HEAVY AND THE DRONE CAN'T TAKE THEM\n") { }
	}
	public class DistanceTooBigger: Exception//if the distance betwwen two customer is too bigger
	{
		public DistanceTooBigger(): base("IBL-ERROR: THE DISTANCE IS TOO BIGGER AND WE CAN'T DO THE SHIPPING SORRY\n"){ }
	}

	// if not request the right factory 
	public class FactoryError : Exception
	{
		public FactoryError() : base($"IBL-ERROR: ONLY TWO FACTORY ARE EXISTS\n") { }
		public FactoryError(string ms) : base($"{ms}\nIBL-ERROR: ONLY TWO FACTORY ARE EXISTS\n") { }
	}
}
