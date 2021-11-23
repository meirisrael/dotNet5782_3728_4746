using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		public class Customer
		{
			public int Id { set; get; }
			public string Name { get; set; }
			public string Phone { get; set; }
			public Location Loc { get; set; }
			public List<ParcelAtCustomer> ParcelFromCustomer { get; set; }
			public List<ParcelAtCustomer> ParcelToCustomer { get; set; }

			public Customer(): base() { Loc = new(); ParcelFromCustomer = new(); ParcelToCustomer = new(); }
			public override string ToString()
			{
				string from_ = null;
				string to_ = null;
				foreach (ParcelAtCustomer item in ParcelFromCustomer)
				{
					from_ += item.ToString();
				}
				foreach (ParcelAtCustomer item in ParcelToCustomer)
				{
					to_ += item.ToString();
				}
				if (from_ == null)
					from_ = "none";
				if(to_==null)
					to_="none";
				return $"Customer:\n" +
					$" Id: {this.Id}\n" +
					$" Name: {this.Name}\n" +
					$" Phone: {this.Phone}\n" +
					$" Location: {this.Loc.ToString()}\n" +
					$" Parcel From Customer: {from_}\n" +
					$" Parcel To Customer: {to_}\n"
					;
			}
		}
	}
}