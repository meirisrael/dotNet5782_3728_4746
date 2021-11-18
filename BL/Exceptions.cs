using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		public class IdExist : Exception
		{
			public IdExist(string type_) : base($"IBL-ERROR: THIS {type_} ID ALREADY EXIST\n") { }
		}
		public class IdNotExist : Exception
		{
			public IdNotExist(string type_) : base($"IBL-ERROR: THIS {type_} ID DO NOT EXIST\n") { }
		}
		public class InvalidId : Exception
		{
			public InvalidId(string type_) : base($"IBL-ERROR: ID OF {type_} TO BE BIGGER THAN ZERO\n") { }
		}
		public class InvalidLoc : Exception
		{
			public InvalidLoc(string type_,string range) : base($"IBL-ERROR: {type_} NEED TO BE BEHTWEEN {range}\n") { }
		}

		// exceptions for base
		public class BaseIdExist : Exception/////////////////////
		{
			public BaseIdExist(string ms) : base($"{ms}\nIBL-ERROR: THIS BASE-STATION ID ALREADY EXIST\n") { }
			public BaseIdExist() : base("IBL-ERROR: THIS BASE-STATION ID ALREADY EXIST\n") { }
		}
		public class BaseIdNotExist : Exception////////////////////
		{
			public BaseIdNotExist(string ms) : base($"{ms}\nIBL-ERROR: THIS BASE-STATION ID DO NOT EXIST\n") { }
			public BaseIdNotExist() : base("IBL-ERROR: THIS BASE-STATION ID DO NOT EXIST\n") { }
		}
		public class InvalidBaseId : Exception////////////////////
		{
			public InvalidBaseId(string ms) : base($"{ms}\nIBL-ERROR: ID OF BASE STATION NEED TO BE BIGGER THAN ZERO\n") { }
			public InvalidBaseId() : base("IBL-ERROR: ID OF BASE STATION NEED TO BE BIGGER THAN ZERO\n") { }
		}
		public class InvalidChargeSlot : Exception
		{
			public InvalidChargeSlot(string ms) : base($"{ms}\nIBL-ERROR: CHARGE SLOT NEED TO BE BIGGER THAN ZERO OR EQUAL TO ZERO\n") { }
			public InvalidChargeSlot() : base("IBL-ERROR: CHARGE SLOT NEED TO BE BIGGER THAN ZERO OR EQUAL TO ZERO\n") { }
		}
		public class InvalidLongitude : Exception//////////////////////////
		{
			public InvalidLongitude(string ms) : base($"{ms}\nIBL-ERROR: LONGITUD NEED TO BE BEHTWEEN -180 TO 180\n") { }
			public InvalidLongitude() : base("IBL-ERROR: LONGITUD NEED TO BE BEHTWEEN -180 TO 180\n") { }
		}
		public class InvalidLatitude : Exception///////////////////////
		{
			public InvalidLatitude(string ms) : base($"{ms}\nIBL-ERROR: LATITUDE NEED TO BE BEHTWEEN -90 TO 90\n") { }
			public InvalidLatitude() : base("IBL-ERROR: LATITUDE NEED TO BE BEHTWEEN -90 TO 90\n") { }
		}

		// exceptions for drone
		public class DroneIdExist : Exception//////////////////////
		{
			public DroneIdExist(string ms) : base($"{ms}\nIBL-ERROR: THIS DRONE ID ALREADY EXIST\n") { }
			public DroneIdExist() : base("IBL-ERROR: THIS DRONE ID ALREADY EXIST\n") { }
		}
		public class DroneIdNotExist : Exception////////////////////
		{
			public DroneIdNotExist(string ms) : base($"{ms}\nIBL-ERROR: THIS DRONE ID DO NOT EXIST\n") { }
			public DroneIdNotExist() : base("IBL-ERROR: THIS DRONE ID DO NOT EXIST\n") { }
		}
		public class InvalidDroneId : Exception///////////////////////////
		{
			public InvalidDroneId(string ms) : base($"{ms}\nIBL-ERROR: ID OF DRONE NEED TO BE BIGGER THAN ZERO\n") { }
			public InvalidDroneId() : base("IBL-ERROR: ID OF DRONE NEED TO BE BIGGER THAN ZERO\n") { }
		}
		public class InvalidWeight : Exception
		{
			public InvalidWeight(string ms) : base($"{ms}\nIBL-ERROR: THIS OPTION FOR WEIGHT IS NOT EXIST, YOU NEED TO GIVE AN OPTION BETWEEN 1 TO 3\n") { }
			public InvalidWeight() : base("IBL-ERROR: THIS OPTION FOR WEIGHT IS NOT EXIST, YOU NEED TO GIVE AN OPTION BETWEEN 1 TO 3\n") { }
		}
		public class NegativeDroneId : Exception
		{
			public NegativeDroneId() : base("IBL-ERROR: THE DRONE ID NEED TO BE POSITIVE OR ZERO(0=NOT ASSIGNE TO AN SPECIFIC DRONE, BIGEER THAN ZERO=ASSIGNE TO AN SPECIFIC DRONE\n") { }
		}
		public class DroneNotFree : Exception
		{
			public DroneNotFree() : base("IBL-ERROR: The drone can't be assigned to a charge station since it's not free.\n") { }
		}
		public class DroneNotMaintenance : Exception
		{
			public DroneNotMaintenance() : base("IBL-ERROR: The drone can't leave a charge station since it's not in maintenance.\n") { }
		}
		public class DroneNotInBase : Exception
		{
			public DroneNotInBase() : base("IBL-ERROR: The drone can't leave a charge station since it's not in a base station.\n") { }
		}
		public class NotEnoughBattery : Exception
		{
			public NotEnoughBattery() : base("IBL-ERROR: The drone hasn't enough battery to join the closer base station\n") { }
		}
		public class NoDroneCanParcel : Exception
		{
			public NoDroneCanParcel() : base("IBL-ERROR: No drone can ship this parcel\n") { }
		}
		public class NoParcelId : Exception
		{
			public NoParcelId() : base("IBL-ERROR: This parcel isn't affiliated to this drone\n") { }
		}
		public class DroneNotInCharge : Exception
		{
			public DroneNotInCharge() : base("IBL-ERROR: ERROR IN THE DATA BASE AN DRONE IS NOT CHARGE\n") { }
		}

		// exceptions for customer
		public class CustomerIdExist : Exception///////////////////////
		{
			public CustomerIdExist(string ms) : base($"{ms}\nIBL-ERROR: THIS CUSTOMER ID ALREADY EXIST\n") { }
			public CustomerIdExist() : base("IBL-ERROR: THIS CUSTOMER ID ALREADY EXIST\n") { }
		}
		public class CustomerIdNotExist : Exception/////////////////////////
		{
			public CustomerIdNotExist(string ms) : base($"{ms}\nIBL-ERROR: THIS CUSTOMER ID DO NOT EXIST\n") { }
			public CustomerIdNotExist() : base("IBL-ERROR: THIS CUSTOMER ID DO NOT EXIST\n") { }
		}
		public class InvalidCustomerId : Exception//////////////////////
		{
			public InvalidCustomerId(string ms) : base($"{ms}\nIBL-ERROR: ID OF CUSTOMER NEED TO BE BIGGER THAN ZERO\n") { }
			public InvalidCustomerId() : base("IBL-ERROR: ID OF CUSTOMER NEED TO BE BIGGER THAN ZERO\n") { }
		}

		// exceptions for parcel
		public class ParcelIdExist : Exception/////////////////////////
		{
			public ParcelIdExist(string ms) : base($"{ms}\nIBL-ERROR: THIS PARCEL ID ALREADY EXIST\n") { }
			public ParcelIdExist() : base("IBL-ERROR: THIS PARCEL ID ALREADY EXIST\n") { }
		}
		public class ParcelIdNotExist : Exception////////////////////////
		{
			public ParcelIdNotExist(string ms) : base($"{ms}\nIBL-ERROR: THIS PARCEL ID DO NOT EXIST\n") { }
			public ParcelIdNotExist() : base("IBL-ERROR: THIS PARCEL ID DO NOT EXIST\n") { }
		}
		public class InvalidParcelId : Exception///////////////////////
		{
			public InvalidParcelId(string ms) : base($"{ms}\nIBL-ERROR: ID OF PARCEL NEED TO BE BIGGER THAN ZERO\n") { }
			public InvalidParcelId() : base("IBL-ERROR: ID OF PARCEL NEED TO BE BIGGER THAN ZERO\n") { }
		}
		public class InvalidPriority : Exception
		{
			public InvalidPriority(string ms) : base($"{ms}\nIBL-ERROR: THIS  OPTION FOR PRIORITIES IS NOT EXIST, YOU NEED TO GIVE AN  OPTION BETWEEN 1 TO 3\n") { }
			public InvalidPriority() : base("IBL-ERROR: THIS  OPTION FOR PRIORITIES IS NOT EXIST, YOU NEED TO GIVE AN  OPTION BETWEEN 1 TO 3\n") { }
		}
		public class AlreadyPickedUp : Exception
		{
			public AlreadyPickedUp() : base("IBL-ERROR: This parcel has already been picked up\n") { }
		}
		public class NotScheduledYet : Exception
		{
			public NotScheduledYet() : base("IBL-ERROR: This parcel hasn't been requested yet\n") { }
		}
		public class NotPickedUpYet : Exception
		{
			public NotPickedUpYet() : base("IBL-ERROR: This parcel hasn't been picked up yet\n") { }
		}
		public class AlreadyDelivered : Exception
		{
			public AlreadyDelivered() : base("IBL-ERROR: This parcel has already been delivered\n") { }
		}
		// exceptions for sender
		public class SenderIdNotExist : Exception//////////////////////
		{
			public SenderIdNotExist(string ms) : base($"{ms}\nIBL-ERROR: THIS SENDER ID DO NOT EXIST\n") { }
			public SenderIdNotExist() : base("IBL-ERROR: THIS SENDER ID DO NOT EXIST\n") { }
		}
		public class SenderNameNotExist : Exception
		{
			public SenderNameNotExist() : base("IBL-ERROR: THIS SENDER NAME DO NOT EXIST\n") { }
		}
		public class InvalidSenderId : Exception////////////////////
		{
			public InvalidSenderId(string ms) : base($"{ms}\nIBL-ERROR: ID OF SENDER NEED TO BE BIGGER THAN ZERO\n") { }
			public InvalidSenderId() : base("IBL-ERROR: ID OF SENDER NEED TO BE BIGGER THAN ZERO\n") { }
		}

		// exceptions for target
		public class TargetIdNotExist : Exception/////////////////////////
		{
			public TargetIdNotExist(string ms) : base($"{ms}\nIBL-ERROR: THIS TARGET ID DO NOT EXIST\n") { }
			public TargetIdNotExist() : base("IBL-ERROR: THIS TARGET ID DO NOT EXIST\n") { }
		}
		public class TargetNameNotExist : Exception
		{
			public TargetNameNotExist() : base("IBL-ERROR: THIS TARGET NAME DO NOT EXIST\n") { }
		}
		public class InvalidTargetId : Exception/////////////////////////
		{
			public InvalidTargetId(string ms) : base($"{ms}\nIBL-ERROR: ID OF SENDER NEED TO BE BIGGER THAN ZERO\n") { }
			public InvalidTargetId() : base("IBL-ERROR: ID OF SENDER NEED TO BE BIGGER THAN ZERO\n") { }
		}

		// exceptions for Weight that drone can't take
		public class ParcelTooHeavy : Exception
		{
			public ParcelTooHeavy() : base("IBL-ERROR: THE CATEGORIE OF PARCEL IS TOO HEAVY AND THE DRONE CAN'T TAKE THEM\n") { }
		}
		public class DistanceTooBigger: Exception
		{
			public DistanceTooBigger(): base("IBL-ERROR: THE DISTANCE IS TOO BIGGER AND WE CAN'T DO THE SHIPPING SORRY\n"){ }
		}
	}
}
