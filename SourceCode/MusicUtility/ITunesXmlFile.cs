﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="ITunesXmlFile.cs" company="Digital Zen Works">
// Copyright © 2019 - 2020 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Xml;

namespace MusicUtility
{
	public class ITunesXmlFile
	{
		private static readonly ILog Log = LogManager.GetLogger(
			MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly ResourceManager StringTable =
			new ResourceManager(
				"MusicUtility.Resources", Assembly.GetExecutingAssembly());

		private readonly XmlDocument xmlDocument;

		public ITunesXmlFile(string filePath)
		{
			xmlDocument = new XmlDocument();
			xmlDocument.Load(filePath);
		}

		public string ITunesFolderLocation
		{
			get
			{
				string path = null;
				string value = GetValue("Music Folder");
				Uri uri = new Uri(value);
				if (uri.IsFile)
				{
					path =
						uri.LocalPath.Replace("\\\\localhost\\", string.Empty);
					path = Path.GetFullPath(path);
				}

				return path;
			}
		}

		public static Dictionary<string, object> LoadItunesXmlFile(
			string iTunesMusicLibXMLPath)
		{
			Dictionary<string, object> strs;
			try
			{
				XmlTextReader xmlTextReader = new XmlTextReader(iTunesMusicLibXMLPath)
				{
					XmlResolver = null
				};
				XmlReaderSettings xmlReaderSetting = new XmlReaderSettings()
				{
					DtdProcessing = DtdProcessing.Ignore,
					ValidationType = ValidationType.None,
					XmlResolver = null
				};
				XmlReader xmlReader = XmlReader.Create(xmlTextReader, xmlReaderSetting);
				xmlReader.ReadStartElement("plist");
				if (!(!object.ReferenceEquals(xmlReader, "None") & xmlReader != null))
				{
					strs = null;
				}
				else
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(xmlReader);
					Dictionary<string, object> strs1 =
						(Dictionary<string, object>)ITunesXmlFile.
						ReadKeyAsDictionaryEntry(xmlDocument.ChildNodes[0]);
					xmlReader.Close();
					foreach (Dictionary<string, object> value in ((Dictionary<string, object>)strs1["Tracks"]).Values)
					{
						Dictionary<string, object> strs2 = value;
						ITunesXmlFile.SetToHTMLDecode("Name", ref strs2);
						ITunesXmlFile.SetToHTMLDecode("Artist", ref strs2);
						ITunesXmlFile.SetToHTMLDecode("Album", ref strs2);
						ITunesXmlFile.SetToHTMLDecode("Genre", ref strs2);
						ITunesXmlFile.SetToHTMLDecode("Kind", ref strs2);
						ITunesXmlFile.SetToURLDecode("Location", ref strs2);
						ITunesXmlFile.SetToHTMLDecode("Comments", ref strs2);
					}

					strs = strs1;
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
				Log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
				strs = null;
			}

			return strs;
		}

		private static string GetURLDecodeOfString(string value)
		{
			string localPath = value;

			try
			{
				value = value.Replace("+", "%2b");
				string url = HttpUtility.UrlDecode(value);

				if (url.StartsWith(
					"file://localhost/", StringComparison.InvariantCulture))
				{
					url = url.Remove(0, 17);
				}

				Uri uri = new Uri(url, UriKind.Absolute);

				localPath = uri.LocalPath;
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is ArgumentNullException ||
				exception is ArgumentOutOfRangeException ||
				exception is UriFormatException)
			{
				Log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
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
						Dictionary<string, object> strs = new Dictionary<string, object>();
						for (int i = 0; i <= currentElement.ChildNodes.Count - 2; i += 2)
						{
							string str1 = (string)ITunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.ChildNodes[i]);
							object obj = ITunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.ChildNodes[i + 1]);
							strs.Add(str1, obj);
						}

						return strs;
					case "array":
						ArrayList arrayLists = new ArrayList();
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

		private static void SetToHTMLDecode(string key, ref Dictionary<string, object> thisDict)
		{
			if (thisDict.ContainsKey(key))
			{
				thisDict[key] = WebUtility.HtmlDecode((string)thisDict[key]);
			}
		}

		private static void SetToURLDecode(string key, ref Dictionary<string, object> thisDict)
		{
			if (thisDict.ContainsKey(key))
			{
				thisDict[key] = ITunesXmlFile.GetURLDecodeOfString((string)thisDict[key]);
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
				Log.Info("value: " + innerText);

				value = innerText;
			}

			return value;
		}
	}
}
