using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	public class BaseStation
	{
		public int Id { get; set; }
		public int Name { get; set; }
		public int ChargeSlots { get; set; }
		public Location Loc { get; set; }
		public List<DroneCharge>? DroneInCharge { get; set; }

		public BaseStation() : base() { Loc = new(); DroneInCharge = new(); }
		public override string ToString()
		{
			string charge="none";
			if (this.DroneInCharge != null)
			{
				foreach (DroneCharge item in DroneInCharge)
				{
					charge += item.ToString();
				}
			}
			return $"Base Station:\n" +
				$" Id: {this.Id}\n" +
				$" Name: { this.Name}\n" +
				$" Charge Slots: {this.ChargeSlots}\n" +
				$" Location: {this.Loc.ToString()}\n" +
				$" Drone in charge: {charge}"
				;
		}
	}
}