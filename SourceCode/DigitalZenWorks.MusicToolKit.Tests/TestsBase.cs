/////////////////////////////////////////////////////////////////////////////
// <copyright file="TestsBase.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.Common.Utilities;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

[assembly: CLSCompliant(false)]

namespace DigitalZenWorks.MusicToolKit.Tests
{
	/// <summary>
	/// Base class for automated tests.
	/// </summary>
	public class TestsBase
	{
		private TagSet tags;
		private string temporaryPath;
		private string testFile;

		/// <summary>
		/// Gets the tags property.
		/// </summary>
		/// <value>The tags property.</value>
		public TagSet Tags { get { return tags; } }

		/// <summary>
		/// Gets the temporary directory path.
		/// </summary>
		/// <value>The temporary directory path.</value>
		public string TemporaryPath { get { return temporaryPath; } }

		/// <summary>
		/// Gets the test file.
		/// </summary>
		/// <value>The test file.</value>
		public string TestFile { get { return testFile; } }

		/// <summary>
		/// The one time setup method.
		/// </summary>
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			temporaryPath = Path.GetTempFileName();
			File.Delete(temporaryPath);
			Directory.CreateDirectory(temporaryPath);

			testFile = temporaryPath + @"\Artist\Album\sakura.mp4";
			FileUtils.CreateFileFromEmbeddedResource(
				"DigitalZenWorks.MusicToolKit.Tests.sakura.mp4", testFile);

			tags = new ();

			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			tags.Album = original;

			tags.Artists = new string[1];
			tags.Performers = new string[1];
			tags.Artists[0] = "Various Artists";
			tags.Performers[0] = "The Solos";
		}

		/// <summary>
		/// One time tear down method.
		/// </summary>
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			bool result = Directory.Exists(temporaryPath);

			if (true == result)
			{
				Directory.Delete(temporaryPath, true);
			}
		}
	}
}
