using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	public class ParcelToList
	{
		public int Id { set; get; }
		public string NameSender { get; set; }
		public string NameTarget { get; set; }
		public WeightCategories Weight { get; set; }
		public Priorities Priority { get; set; }
		public ParcelStatues Status { get; set; }

		public override string ToString()
		{
			return $"Parcel\n" +
				$" Id: { this.Id}\n" +
				$" Name of sender: {this.NameSender}\n" +
				$" Name of Target: {this.NameTarget}\n" +
				$" Weight: {this.Weight}\n" +
				$" Priority: {this.Priority}\n" +
				$" status of parcel: {this.Status}"
				;
		}
	}
}
