/////////////////////////////////////////////////////////////////////////////
// <copyright file="ItunesTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.Common.Utilities;
using iTunesLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace DigitalZenWorks.MusicToolKit.Tests
{
	/// <summary>
	/// iTunes tests class.
	/// </summary>
	[TestFixture]
	public class ItunesTests
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

				Assert.NotNull(tracks);

				if (null != tracks)
				{
					string fileName = iTunesManager.ItunesLibraryLocation +
						@"Music\10cc\The Very Best Of 10cc\" +
						"The Things We Do For Love.mp3";

					// tracks is a list of potential matches
					foreach (IITTrack track in tracks)
					{
						bool same = ITunesManager.AreFileAndTrackTheSame(
							fileName, track);
						Assert.True(same);
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

				Assert.GreaterOrEqual(iTunesDepth, 6);
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

			Assert.IsNotEmpty(location);
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

			Assert.NotNull(exception);

			Assert.Null(iTunesXmlFile);
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

			Assert.NotNull(iTunesXmlFile);
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

				Assert.NotNull(result);

				int count = result.Count;
				Assert.GreaterOrEqual(count, 1);
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

			Assert.Null(result);
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

			Assert.Null(result);
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

				bool updated = iTunesManager.UpdateItunes(fileInfo);

				Assert.False(updated);
			}
		}
	}
}
