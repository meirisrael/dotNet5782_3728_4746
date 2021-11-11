using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class Customer : Location
		{
			public int Id { set; get; }
			public string Name { get; set; }
			public string Phone { get; set; }
			public Location Loc { get; set; }
			public List<ParcelAtCustomer> ParcelFromCustomer { get; set; }
			public List<ParcelAtCustomer> ParcelToCustomer { get; set; }

			public override string ToString()
			{
				return $"Customer:\n" +
					$" Id: {this.Id}\n" +
					$" Name: {this.Name}\n" +
					$" Phone: {this.Phone}\n" +
					$" Location: {this.Loc.ToString()}"
					;
			}
		}
	}
}