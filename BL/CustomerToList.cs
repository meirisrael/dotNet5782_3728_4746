using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		public class CustomerToList
		{
			public int Id { set; get; }
			public string Name { get; set; }
			public string Phone { get; set; }
			public int ParcelDelivred { get; set; }
			public int ParcelSentNotDelivred { get; set; }
			public int ParcelRecived { get; set; }
			public int ParcelInTransit { get; set; }

			public override string ToString()
			{
				return $"Customer:\n"+
					$" Id: { this.Id}\n" +
					$" Name: {this.Name}\n" +
					$" Phone: {this.Phone}\n" +
					$" Nums of parcels that are delivered: {this.ParcelDelivred}\n"+
					$" Nums of parcels that are sent but not delivered: {this.ParcelSentNotDelivred}\n"+
					$" Nums of parcels that he recived: {this.ParcelRecived}\n" +
					$" Nums of parcels that are in transit: {this.ParcelDelivred}"
					;
			}
		}
	}
}
