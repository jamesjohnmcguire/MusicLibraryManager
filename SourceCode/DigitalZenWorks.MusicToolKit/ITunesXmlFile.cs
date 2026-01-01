/////////////////////////////////////////////////////////////////////////////
// <copyright file="ITunesXmlFile.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Reflection;
	using System.Text;
	using System.Xml;
	using global::Common.Logging;

	/// <summary>
	/// iTunes xml file class.
	/// </summary>
	/// <remarks>
	/// The itunes xml file appeared to be no longer maintained by iTunes. But,
	/// currently it seems that is is being consistently updated as an export
	/// of the binary database.
	///
	/// Some web oriented URI decoding was required that put an
	/// usual dependency on System.Web, which is usually only used in web
	/// applications.  If this is ever needed again, add <ItemGroup><Reference
	/// Include = "System.Web" /></ItemGroup> to your csproj file and enable
	/// the code in USE_SYSTEMWEB.
	/// </remarks>
	public class ITunesXmlFile
	{
		private static readonly ILog Log = LogManager.GetLogger(
			MethodBase.GetCurrentMethod().DeclaringType);

		private readonly string libraryXMLPath;
		private readonly XmlDocument xmlDocument;

		/// <summary>
		/// Initializes a new instance of the <see cref="ITunesXmlFile"/> class.
		/// </summary>
		/// <param name="filePath">The path to iTunes xml file.</param>
		public ITunesXmlFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				libraryXMLPath = filePath;

				string fileText = File.ReadAllText(filePath, Encoding.UTF8);
				xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(fileText);
			}
			else
			{
				throw new FileNotFoundException();
			}
		}

		/// <summary>
		/// Gets the iTunes folder location.
		/// </summary>
		/// <value>The iTunes folder location.</value>
		public string ITunesFolderLocation
		{
			get
			{
				string path = null;
				string value = GetValue("Music Folder");
				Uri uri = new (value);

				if (uri.IsFile)
				{
					path =
						uri.LocalPath.Replace(
							"\\\\localhost\\",
							string.Empty,
							StringComparison.OrdinalIgnoreCase);
					path = Path.GetFullPath(path);
				}

				return path;
			}
		}

		/// <summary>
		/// Gets the library XML path.
		/// </summary>
		/// <value>The library XML path.</value>
		public string LibraryXMLPath
		{
			get { return libraryXMLPath; }
		}

		/// <summary>
		/// Load iTunes xml file.
		/// </summary>
		/// <param name="iTunesMusicLibXMLPath">Path to the xml file.</param>
		/// <returns>A dictionary of values from the xml file.</returns>
		public static Dictionary<string, object> LoadItunesXmlFile(
			string iTunesMusicLibXMLPath)
		{
			Dictionary<string, object> iTunesInformation = null;

			try
			{
				if (File.Exists(iTunesMusicLibXMLPath))
				{
					XmlReaderSettings settings = new ();
					settings.XmlResolver = null;
					settings.DtdProcessing = DtdProcessing.Ignore;
					settings.ValidationType = ValidationType.None;

					using XmlReader xmlReader =
						XmlReader.Create(iTunesMusicLibXMLPath, settings);

					xmlReader.ReadStartElement("plist");

					if (!object.ReferenceEquals(xmlReader, "None"))
					{
						Dictionary<string, object> preInfo =
							GetXmlInformation(xmlReader);

						object tracks = preInfo["Tracks"];
						var tracksDictionary =
							(Dictionary<string, object>)tracks;
						Dictionary<string, object>.ValueCollection values =
								tracksDictionary.Values;

						foreach (Dictionary<string, object> value in
							values.Cast<Dictionary<string, object>>())
						{
							Dictionary<string, object> strs2 = value;
							ITunesXmlFile.SetToHTMLDecode("Name", ref strs2);
							ITunesXmlFile.SetToHTMLDecode("Artist", ref strs2);
							ITunesXmlFile.SetToHTMLDecode("Album", ref strs2);
							ITunesXmlFile.SetToHTMLDecode("Genre", ref strs2);
							ITunesXmlFile.SetToHTMLDecode("Kind", ref strs2);
							ITunesXmlFile.SetToURLDecode(
								"Location", ref strs2);
							ITunesXmlFile.SetToHTMLDecode(
								"Comments", ref strs2);
						}

						iTunesInformation = preInfo;
					}
				}
			}
			catch (Exception exception) when
				(exception is ArgumentNullException ||
				exception is DirectoryNotFoundException ||
				exception is FileNotFoundException ||
				exception is InvalidOperationException ||
				exception is NullReferenceException ||
				exception is UriFormatException ||
				exception is WebException ||
				exception is XmlException)
			{
				Log.Error(exception.ToString());
			}

			return iTunesInformation;
		}

		private static Dictionary<string, object> GetXmlInformation(
			XmlReader xmlReader)
		{
			XmlDocument xmlDocument = new ();
			xmlDocument.Load(xmlReader);

			Dictionary<string, object> preInfo =
				(Dictionary<string, object>)ReadKeyAsDictionaryEntry(
					xmlDocument.ChildNodes[0]);
			xmlReader.Close();

			return preInfo;
		}

		private static string GetURLDecodeOfString(string value)
		{
			string localPath = value;

			try
			{
				value = value.Replace(
					"+", "%2b", StringComparison.OrdinalIgnoreCase);
				string url = value;
#if USE_SYSTEMWEB
				url = System.Web.HttpUtility.UrlDecode(value);
#endif
				if (url.StartsWith(
					"file://localhost/", StringComparison.InvariantCulture))
				{
					url = url[17..];
				}

				Uri uri = new (url, UriKind.Absolute);

				localPath = uri.LocalPath;
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is ArgumentNullException ||
				exception is ArgumentOutOfRangeException ||
				exception is UriFormatException)
			{
				Log.Error(exception.ToString());
				localPath = null;
			}

			return localPath;
		}

		private static object ReadKeyAsDictionaryEntry(XmlNode currentElement)
		{
			string name = currentElement.Name;
			string str = name;
			if (name != null)
			{
				switch (str)
				{
					case "plist":
						return ITunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.FirstChild);
					case "integer":
					case "real":
					case "data":
					case "date":
						return currentElement.InnerText;
					case "key":
						return currentElement.InnerText;
					case "string":
						return currentElement.InnerText;
					case "dict":
						Dictionary<string, object> strs = [];
						for (int i = 0; i <= currentElement.ChildNodes.Count - 2; i += 2)
						{
							string str1 = (string)ITunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.ChildNodes[i]);
							object obj = ITunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.ChildNodes[i + 1]);
							strs.Add(str1, obj);
						}

						return strs;
					case "array":
						ArrayList arrayLists = [];
						for (int j = 0; j <= currentElement.ChildNodes.Count - 1; j++)
						{
							arrayLists.Add(ITunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.ChildNodes[j]));
						}

						return arrayLists;
					case "true":
					case "false":
						return currentElement.Name;
				}
			}

			return null;
		}

		private static void SetToHTMLDecode(
			string key, ref Dictionary<string, object> thisDict)
		{
			if (thisDict.TryGetValue(key, out object value))
			{
				string decodedValue = value as string;
				thisDict[key] = WebUtility.HtmlDecode(decodedValue);
			}
		}

		private static void SetToURLDecode(
			string key, ref Dictionary<string, object> thisDict)
		{
			if (thisDict.TryGetValue(key, out object value))
			{
				string encodedValue = value as string;
				thisDict[key] =
					ITunesXmlFile.GetURLDecodeOfString(encodedValue);
			}
		}

		private string GetValue(string key)
		{
			string value = null;
			string expression = "plist/dict/key[.='" + key +
				"']/following-sibling::string[1]";

			XmlNode location = xmlDocument.SelectSingleNode(expression);

			string innerText = location.InnerText;

			if (!string.IsNullOrWhiteSpace(innerText))
			{
				value = innerText;
			}

			return value;
		}
	}
}
