using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUtility
{
	[TestFixture]
	class UnitTests
	{
		[Test]
		public void ITunesPathLocation()
		{
			MusicUtility musicUtility = new MusicUtility();
			string location = musicUtility.ITunesLibraryLocation;

			Assert.IsNotEmpty(location);
		}

		[Test]
		public void GetItunesPathDepth()
		{
			MusicUtility musicUtility = new MusicUtility();
			string location = musicUtility.ITunesLibraryLocation;
			int iTunesDepth = Paths.GetItunesDirectoryDepth(location);

			Assert.AreEqual(iTunesDepth, 6);
		}

		[Test]
		public void GetArtistNameFromPath()
		{
			MusicUtility musicUtility = new MusicUtility();
			string location = musicUtility.ITunesLibraryLocation;

			Assert.IsNotEmpty(location);
		}
	}
}
