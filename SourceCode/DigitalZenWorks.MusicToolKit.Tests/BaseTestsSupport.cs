/////////////////////////////////////////////////////////////////////////////
// <copyright file="BaseTestsSupport.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Tests
{
	using System.IO;
	using NUnit.Framework;
	using NUnit.Framework.Internal;

	/// <summary>
	/// Base test support class.
	/// </summary>
	[SetUpFixture]
	internal class BaseTestsSupport
	{
		private string temporaryPath;
		private string testFile;

		/// <summary>
		/// Gets the temporary path.
		/// </summary>
		/// <value>The temporary path.</value>
		public string TemporaryPath { get => temporaryPath; }

		/// <summary>
		/// Gets the test file.
		/// </summary>
		/// <value>The test file.</value>
		public string TestFile { get => testFile; }

		/// <summary>
		/// The one time setup method.
		/// </summary>
		[OneTimeSetUp]
		public void BaseOneTimeSetUp()
		{
			temporaryPath = Path.GetTempFileName();
			File.Delete(temporaryPath);
			Directory.CreateDirectory(temporaryPath);

			testFile = temporaryPath + @"\Music\Artist\Album\Sakura.mp4";
		}

		/// <summary>
		/// One time tear down method.
		/// </summary>
		[OneTimeTearDown]
		public void BaseOneTimeTearDown()
		{
			bool result = Directory.Exists(temporaryPath);

			if (result == true)
			{
				Directory.Delete(temporaryPath, true);
			}
		}

		/// <summary>
		/// Make test file copy.
		/// </summary>
		/// <param name="directory">The directory to create.</param>
		/// <param name="fileName">The file name to create.</param>
		/// <returns>The file path of the copy file.</returns>
		protected string MakeTestFileCopy(string directory, string fileName)
		{
			string newPath = temporaryPath + directory;
			Directory.CreateDirectory(newPath);

			string newFileName = newPath + @"\" + fileName;

			if (File.Exists(newFileName))
			{
				File.Delete(newFileName);
			}

			File.Copy(testFile, newFileName);

			return newFileName;
		}
	}
}
