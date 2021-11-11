using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class BaseToList
		{
			public int Id { get; set; }
			public int Name { get; set; }
			public int ChargeSlots { get; set; }
			public int ChargeBusy { get; set; }

			public override string ToString()
			{
				return $"Base For List:\n" +
					$" Id: {this.Id}\n" +
					$" Name: {this.Name}\n" +
					$" Nums of charge that are availble: {this.ChargeSlots}\n" +
					$" Nums of charge that are busy: {this.ChargeBusy}"
					;
			}
		}
	}
}
