using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class ParcelAtCustomer
		{
			public int Id { set; get; }
			public WeightCategories Weight { get; set; }
			public Priorities Priority { get; set; }
			public ParcelStatues status { get; set; }
			public CustomerInParcel SenderOrTraget { get; set; }

			public override string ToString()
			{
				return $"Parcel at customer:\n" +
					$" Id: { this.Id}\n" +
					$" Weight: {this.Weight}\n" +
					$" Priority: {this.Priority}\n" +
					$" status of parcel: {this.status}\n" +
					$" Is: {this.SenderOrTraget}"
					;


			}
		}
	}
}
