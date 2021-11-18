using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	namespace DO
	{
		// exceptions for base
		public class BaseIdExist : Exception
		{
			public BaseIdExist() : base("DAL-ERROR: THIS BASE-STATION ID ALREADY EXIST\n") { }
		}
		public class BaseIdNotExist : Exception
		{
			public BaseIdNotExist() : base("DAL-ERROR: THIS BASE-STATION ID DO NOT EXIST\n") { }
		}
		public class InvalidBaseId : Exception
		{
			public InvalidBaseId() : base("DAL-ERROR: ID OF BASE STATION NEED TO BE BIGGER THAN ZERO\n") { }
		}
		public class InvalidChargeSlot : Exception
		{
			public InvalidChargeSlot() : base("DAL-ERROR: CHARGE SLOT NEED TO BE BIGGER THAN ZERO OR EQUAL TO ZERO\n") { }
		}
		public class InvalidLongitude : Exception
		{
			public InvalidLongitude() : base("DAL-ERROR: LONGITUD NEED TO BE BEHTWEEN -180 TO 180\n") { }
		}
		public class InvalidLatitude : Exception
		{
			public InvalidLatitude() : base("DAL-ERROR: LATITUDE NEED TO BE BEHTWEEN -90 TO 90\n") { }
		}

		// exceptions for drone
		public class DroneIdExist : Exception
		{
			public DroneIdExist() : base("DAL-ERROR: THIS DRONE ID ALREADY EXIST\n") { }
		}
		public class DroneIdNotExist : Exception
		{
			public DroneIdNotExist() : base("DAL-ERROR: THIS DRONE ID DO NOT EXIST\n") { }
		}
		public class InvalidDroneId : Exception
		{
			public InvalidDroneId() : base("DAL-ERROR: ID OF DRONE NEED TO BE BIGGER THAN ZERO\n") { }
		}
		public class InvalidWeight : Exception
		{
			public InvalidWeight() : base("DAL-ERROR: THIS OPTION FOR WEIGHT IS NOT EXIST, YOU NEED TO GIVE AN  OPTION BETWEEN 1 TO 3\n") { }
		}
		public class NegativeDroneId : Exception
		{
			public NegativeDroneId() : base("DAL-ERROR: THE DRONE ID NEED TO BE POSITIVE OR ZERO->NOT ASSIGNE TO AN SPECIFIC DRONE OR, BIGEER THAN ZERO->ASSIGNE TO AN SPECIFIC DRONE\n") { }
		}

		// exceptions for customer
		public class CustomerIdExist : Exception
		{
			public CustomerIdExist() : base("DAL-ERROR: THIS CUSTOMER ID ALREADY EXIST\n") { }
		}
		public class CustomerIdNotExist : Exception
		{
			public CustomerIdNotExist() : base("DAL-ERROR: THIS CUSTOMER ID DO NOT EXIST\n") { }
		}
		public class InvalidCustomerId : Exception
		{
			public InvalidCustomerId() : base("DAL-ERROR: ID OF CUSTOMER NEED TO BE BIGGER THAN ZERO\n") { }
		}

		// exceptions for parcel
		public class ParcelIdExist : Exception
		{
			public ParcelIdExist() : base("DAL-ERROR: THIS PARCEL ID ALREADY EXIST\n") { }
		}
		public class ParcelIdNotExist : Exception
		{
			public ParcelIdNotExist() : base("DAL-ERROR: THIS PARCEL ID DO NOT EXIST\n") { }
		}
		public class InvalidParcelId : Exception
		{
			public InvalidParcelId() : base("DAL-ERROR: ID OF PARCEL NEED TO BE BIGGER THAN ZERO\n") { }
		}
		public class InvalidPriority : Exception
		{
			public InvalidPriority() : base("DAL-ERROR: THIS  OPTION FOR PRIORITIES IS NOT EXIST, YOU NEED TO GIVE AN  OPTION BETWEEN 1 TO 3\n") { }
		}

		// exceptions for sender
		public class SenderIdNotExist : Exception
		{
			public SenderIdNotExist() : base("DAL-ERROR: THIS SENDER ID DO NOT EXIST\n") { }
		}
		public class InvalidSenderId : Exception
		{
			public InvalidSenderId() : base("DAL-ERROR: ID OF SENDER NEED TO BE BIGGER THAN ZERO\n") { }
		}
		// exceptions for target
		public class TargetIdNotExist : Exception
		{
			public TargetIdNotExist() : base("DAL-ERROR: THIS TARGET ID DO NOT EXIST\n") { }
		}
		public class InvalidTargetId : Exception
		{
			public InvalidTargetId() : base("DAL-ERROR: ID OF SENDER NEED TO BE BIGGER THAN ZERO\n") { }
		}

		// exceptions for Weight that drone can't take
		public class ParcelTooHeavy : Exception
		{
			public ParcelTooHeavy() : base("DAL-ERROR: THE CATEGORIE OF PARCEL IS TOO HEAVY AND THE DRONE CAN'T TAKE THEM\n") { }
		}
		public class InvalidChargeSlots : Exception
		{
			public InvalidChargeSlots() : base("DAL-ERROR: INVALID NUMBER OF CHARGESLOTS, Enter a higher number or remove charging drones from this station\n") { }
		}
	}
}

