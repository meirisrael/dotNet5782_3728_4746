using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		public class ParcelAtCustomer
		{
			public int Id { set; get; }
			public WeightCategories Weight { get; set; }
			public Priorities Priority { get; set; }
			public ParcelStatues status { get; set; }
			public CustomerInParcel SenderOrTraget { get; set; }

			public ParcelAtCustomer() : base() { SenderOrTraget = new(); }
			public override string ToString()
			{
				return $"\n" +
					$"	Id: { this.Id}\n" +
					$"	Weight: {this.Weight}\n" +
					$"	Priority: {this.Priority}\n" +
					$"	status of parcel: {this.status}\n" +
					$"	The sender or the target: {this.SenderOrTraget}"
					;


			}
		}
	}
}
