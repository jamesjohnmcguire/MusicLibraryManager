using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicUtility;

namespace MusicClean
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				MusicUtility.MusicUtility musicUtility =
					new MusicUtility.MusicUtility();

				musicUtility.CleanMusicLibrary();
			}
			catch(Exception exception)
			{
				Console.WriteLine("Exception: " + exception.Message);
			}
		}
	}
}
