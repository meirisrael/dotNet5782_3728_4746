using System;

namespace Ex0
{
	partial class Program
	{
		static void Main(string[] args)
		{
			welcome3728();
			welcome4746();

			Console.ReadKey();
		}

		static partial void welcome4746();
		private static void welcome3728()
		{
			Console.Write("Enter your name: ");
			string name = Console.ReadLine();
			Console.WriteLine($"{name}, welcome to my first console application");
		}
	}
}
