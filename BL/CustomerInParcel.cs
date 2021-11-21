using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		public class CustomerInParcel
		{
			public int Id { set; get; }
			public string Name { get; set; }

			public override string ToString()
			{
				return $"\n	Customer -\n" +
					$"		Id: {this.Id}\n" +
					$"		Name: {this.Name}"
					;
			}
		}
	}
}
