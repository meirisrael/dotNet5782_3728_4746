using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class CustomerInParcel
		{
			public int Id { set; get; }
			public string Name { get; set; }

			public override string ToString()
			{
				return $"Customer:\n" +
					$" Id: {this.Id}\n" +
					$" Name: {this.Name}"
					;
			}
		}
	}
}
