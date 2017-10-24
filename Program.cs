using System;

namespace clearzombies
{
	internal class Program
	{
		public Program()
		{
		}

		private static void Main(string[] args)
		{
			clearZombies clearZomby = new clearZombies();
			if (clearZomby.validateArgs(args))
			{
				clearZomby.clear();
			}
		}
	}
}