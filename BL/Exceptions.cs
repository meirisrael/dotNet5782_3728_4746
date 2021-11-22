using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		// exceptions - problem with id
		public class IdExist : Exception
		{
			public IdExist(string type_) : base($"IBL-ERROR: THIS {type_} ID ALREADY EXIST\n") { }
			public IdExist(string ms, string type_) : base($"{ms}\nIBL-ERROR: THIS {type_} ID ALREADY EXIST\n") { }
		}
		public class IdNotExist : Exception
		{
			public IdNotExist(string type_) : base($"IBL-ERROR: THIS {type_} ID DO NOT EXIST\n") { }
			public IdNotExist(string ms, string type_) : base($"{ ms}\nIBL-ERROR: THIS {type_} ID DO NOT EXIST\n") { }
		}
		public class InvalidId : Exception
		{
			public InvalidId(string type_) : base($"IBL-ERROR: ID OF {type_} TO BE BIGGER THAN ZERO\n") { }
			public InvalidId(string ms, string type_) : base($"{ms}\nIBL-ERROR: ID OF {type_} TO BE BIGGER THAN ZERO\n") { }
		}

		//
		public class InvalidLoc : Exception
		{
			public InvalidLoc(string type_, string range) : base($"IBL-ERROR: {type_} NEED TO BE BEHTWEEN {range}\n") { }
			public InvalidLoc(string ms, string type_, string range) : base($"{ms}\nIBL-ERROR: {type_} NEED TO BE BEHTWEEN {range}\n") { }
		}

		// exceptions - problem with Categories
		public class InvalidCategory : Exception//WEIGHT PRIORITIES
		{
			public InvalidCategory(string type_) : base($"IBL-ERROR: THIS OPTION FOR {type_} IS NOT EXIST, YOU NEED TO GIVE AN OPTION BETWEEN 1 TO 3\n") { }
			public InvalidCategory(string ms, string type_) : base($"{ms}\nIBL-ERROR: THIS OPTION FOR {type_} IS NOT EXIST, YOU NEED TO GIVE AN OPTION BETWEEN 1 TO 3\n") { }
		}

		// exceptions for base
		public class InvalidChargeSlot : Exception
		{
			public InvalidChargeSlot(string ms) : base($"{ms}\nIBL-ERROR: CHARGE SLOT NEED TO BE BIGGER THAN ZERO OR EQUAL TO ZERO\n") { }
			public InvalidChargeSlot() : base("IBL-ERROR: CHARGE SLOT NEED TO BE BIGGER THAN ZERO OR EQUAL TO ZERO\n") { }
		}
		// exceptions for drone

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

		// exceptions for parcel

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
		public class SenderNameNotExist : Exception
		{
			public SenderNameNotExist() : base("IBL-ERROR: THIS SENDER NAME DO NOT EXIST\n") { }
		}
		public class TargetNameNotExist : Exception
		{
			public TargetNameNotExist() : base("IBL-ERROR: THIS TARGET NAME DO NOT EXIST\n") { }
		}
		public class AllParcelAssoc : Exception
		{
			public AllParcelAssoc() : base("IBL: ALL PARCEL ALREDY ASSOCIATED\n") { }
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
