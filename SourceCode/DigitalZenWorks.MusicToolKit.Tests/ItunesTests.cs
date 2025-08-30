/////////////////////////////////////////////////////////////////////////////
// <copyright file="ItunesTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Tests
{
	using System.Collections.Generic;
	using System.IO;
	using DigitalZenWorks.Common.Utilities;
	using iTunesLib;
	using NUnit.Framework;

	/// <summary>
	/// iTunes tests class.
	/// </summary>
	[TestFixture]
	internal sealed class ItunesTests
	{
		/// <summary>
		/// The are file and track the same yes test.
		/// </summary>
		[Test]
		public void AreFileAndTrackTheSameYes()
		{
			using ITunesManager iTunesManager = new ();

			iTunesApp iTunes = iTunesManager.ItunesCom;

			if (iTunes != null)
			{
				string searchName = "The Things We Do For Love";

				IITLibraryPlaylist playList = iTunes.LibraryPlaylist;
				IITTrackCollection tracks = playList.Search(
					searchName,
					ITPlaylistSearchField.ITPlaylistSearchFieldAll);

				Assert.That(tracks, Is.Not.Null);

				if (tracks != null)
				{
					string fileName = iTunesManager.ItunesLibraryLocation +
						@"Music\10cc\The Very Best Of 10cc\" +
						"The Things We Do For Love.mp3";

					// tracks is a list of potential matches
					foreach (IITTrack track in tracks)
					{
						bool same = iTunesManager.IsFileAndTrackSame(
							fileName, track);
						Assert.That(same, Is.True);
					}
				}
			}
		}

		/// <summary>
		/// The get itunes path depth method test.
		/// </summary>
		[Test]
		public void GetItunesPathDepth()
		{
			using ITunesManager iTunesManager = new ();
			iTunesApp iTunes = iTunesManager.ItunesCom;

			if (iTunes != null)
			{
				string location = iTunesManager.ItunesLibraryLocation;
				int iTunesDepth = Paths.GetItunesDirectoryDepth(location);

				Assert.That(iTunesDepth, Is.GreaterThanOrEqualTo(6));
			}
		}

		/// <summary>
		/// ITunes path location method test.
		/// </summary>
		[Test]
		public void ITunesPathLocation()
		{
			using ITunesManager iTunesManager = new ();
			string location = iTunesManager.ItunesLibraryLocation;

			Assert.That(location, Is.Not.Null);
			Assert.That(location, Is.Not.Empty);
		}

		/// <summary>
		/// iTunes Xml Create with invalid file test.
		/// </summary>
		[Test]
		public void ItunesXmlFileCreateInvalidFile()
		{
			ITunesXmlFile iTunesXmlFile = null;

			string nonExistantFilePath = Path.GetTempFileName();
			File.Delete(nonExistantFilePath);

			FileNotFoundException exception =
				Assert.Throws<FileNotFoundException>(() =>
				iTunesXmlFile = new ITunesXmlFile(nonExistantFilePath));

			Assert.Multiple(() =>
			{
				Assert.That(exception, Is.Not.Null);
				Assert.That(iTunesXmlFile, Is.Null);
			});
		}

		/// <summary>
		/// iTunes Xml Create success test.
		/// </summary>
		[Test]
		public void ItunesXmlFileCreateSuccess()
		{
			string xmlFile = Path.GetTempFileName();
			File.Delete(xmlFile);

			FileUtils.CreateFileFromEmbeddedResource(
				"DigitalZenWorks.MusicToolKit.Tests.XMLFile.xml", xmlFile);

			ITunesXmlFile iTunesXmlFile = new (xmlFile);

			Assert.That(iTunesXmlFile, Is.Not.Null);
		}

		/// <summary>
		/// Load iTunes XML file method test.
		/// </summary>
		[Test]
		public void LoadiTunesXmlFile()
		{
			using ITunesManager iTunesManager = new ();
			iTunesApp iTunes = iTunesManager.ItunesCom;

			if (iTunes != null)
			{
				string xmlFilePath = iTunesManager.ITunesLibraryXMLPath;

				Dictionary<string, object> result =
					ITunesXmlFile.LoadItunesXmlFile(xmlFilePath);

				Assert.That(result, Is.Not.Null);

				int count = result.Count;
				Assert.That(count, Is.GreaterThanOrEqualTo(1));
			}
		}

		/// <summary>
		/// Load iTunes XML file not exists test.
		/// </summary>
		[Test]
		public void LoadiTunesXmlFileNotExists()
		{
			string temporaryPath = Path.GetTempFileName();
			File.Delete(temporaryPath);

			Dictionary<string, object> result =
				ITunesXmlFile.LoadItunesXmlFile(temporaryPath);

			Assert.That(result, Is.Null);
		}

		/// <summary>
		/// Load iTunes XML file not xml filetest.
		/// </summary>
		[Test]
		public void LoadiTunesXmlFileNotXmlFile()
		{
			string temporaryPath = Path.GetTempFileName();

			Dictionary<string, object> result =
				ITunesXmlFile.LoadItunesXmlFile(temporaryPath);

			Assert.That(result, Is.Null);
		}

		/// <summary>
		/// The update iTunes test.
		/// </summary>
		[Test]
		public void UpdateItunes()
		{
			using ITunesManager iTunesManager = new ();

			iTunesApp iTunes = iTunesManager.ItunesCom;

			if (iTunes != null)
			{
				string location = iTunesManager.ItunesLibraryLocation;

				string fileName = @"Music\10cc\The Very Best Of 10cc\" +
					"The Things We Do For Love.mp3";
				string fullPath = Path.Combine(location, fileName);
				FileInfo fileInfo = new (fullPath);

				bool updated = iTunesManager.UpdateItunesLibrary(fileInfo);

				Assert.That(updated, Is.False);
			}
		}
	}
}
