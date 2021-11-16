using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class BaseStation
		{
			public int Id { get; set; }
			public int Name { get; set; }
			public int ChargeSlots { get; set; }
			public Location Loc { get; set; }
			public List<DroneCharge> DroneInCharge { get; set; }

			public override string ToString()
			{
				return $"Base Station:\n" +
					$" Id: {this.Id}\n" +
					$" Name: { this.Name}\n" +
					$" Charge Slots: {this.ChargeSlots}\n" +
					$" Location: {this.Loc.ToString()}\n" +
					$" Drone in charge: {this.DroneInCharge.ToString()}"
					;
			}
		}
	}
}