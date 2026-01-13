/////////////////////////////////////////////////////////////////////////////
// <copyright file="BaseTestsSupport.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Tests;

using System;
using System.IO;
using DigitalZenWorks.Common.Utilities;
using DigitalZenWorks.RulesLibrary;
using NUnit.Framework;

/// <summary>
/// Base test support class.
/// </summary>
internal class BaseTestsSupport
{
	private Rules rules;
	private string temporaryPath;
	private string testFile;

	/// <summary>
	/// Gets or sets the rules object.
	/// </summary>
	/// <value>The rules object.</value>
	public Rules Rules { get => rules; set => rules = value; }

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
		temporaryPath = CreateUniqueDirectory();

		testFile = temporaryPath + @"\Music\Artist\Album\Sakura.mp4";

		FileUtils.CreateFileFromEmbeddedResource(
			"DigitalZenWorks.MusicToolKit.Tests.Sakura.mp4", testFile);

		rules = MusicManager.GetDefaultRules();
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

	private string CreateUniqueDirectory()
	{
		string guid = Guid.NewGuid().ToString("N");
		string uniqueDirectoryName = "MusicManTests-" + guid;
		string temporaryPath = Path.GetTempPath();
		string uniqueDirectoryPath = Path.Combine(temporaryPath, uniqueDirectoryName);
		Directory.CreateDirectory(uniqueDirectoryPath);

		return uniqueDirectoryPath;
	}
}
