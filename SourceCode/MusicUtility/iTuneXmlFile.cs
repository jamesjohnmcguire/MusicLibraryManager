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
	public class ItunesXmlFile
	{
		private string filePath = null;

		private static readonly ILog log = LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly ResourceManager stringTable =
			new ResourceManager("MusicUtility.Resources",
			Assembly.GetExecutingAssembly());

		private XmlDocument xmlDocument = null;

		public string ITunesFolderLocation
		{
			get
			{
				string path = null;
				string value = GetValue("Music Folder");
				Uri uri = new Uri(value);
				if (uri.IsFile)
				{
					path = uri.LocalPath.Replace("\\\\localhost\\", "");
					path = Path.GetFullPath(path);
				}

				return path;
			}
		}

		public ItunesXmlFile(string filePath)
		{
			this.filePath = filePath;

			xmlDocument = new XmlDocument();
			xmlDocument.Load(filePath);
		}

		private string GetValue(string key)
		{
			string value = null;
			string expression = "plist/dict/key[.='" + key +
				"']/following-sibling::string[1]";

			XmlNode location = xmlDocument.SelectSingleNode(expression);

			string innerText = location.InnerText;

			//innerText = xmlNodeList[0].InnerText;
			if (!string.IsNullOrWhiteSpace(innerText))
			{
				log.Info("value: " + innerText);

				value = innerText;
			}

			return value;
		}

		private static string GetURLDecodeOfString(string value)
		{
			string localPath = value;

			try
			{
				value = value.Replace("+", "%2b");
				string uri = HttpUtility.UrlDecode(value);
				if (uri.StartsWith("file://localhost/"))
				{
					uri = uri.Remove(0, 17);
				}

				localPath = (new Uri(uri, UriKind.Absolute)).LocalPath;
			}
			catch (Exception exception)
			{
				log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
				localPath = null;
			}

			return localPath;
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
						(Dictionary<string, object>)ItunesXmlFile.
						ReadKeyAsDictionaryEntry(xmlDocument.ChildNodes[0]);
					xmlReader.Close();
					foreach (Dictionary<string, object> value in ((Dictionary<string, object>)strs1["Tracks"]).Values)
					{
						Dictionary<string, object> strs2 = value;
						ItunesXmlFile.SetToHTMLDecode("Name", ref strs2);
						ItunesXmlFile.SetToHTMLDecode("Artist", ref strs2);
						ItunesXmlFile.SetToHTMLDecode("Album", ref strs2);
						ItunesXmlFile.SetToHTMLDecode("Genre", ref strs2);
						ItunesXmlFile.SetToHTMLDecode("Kind", ref strs2);
						ItunesXmlFile.SetToURLDecode("Location", ref strs2);
						ItunesXmlFile.SetToHTMLDecode("Comments", ref strs2);
					}
					strs = strs1;
				}
			}
			catch (Exception exception)
			{
				log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
				strs = null;
			}
			return strs;
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
					{
						return ItunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.FirstChild);
					}
				case "integer":
				case "real":
				case "data":
				case "date":
					{
						return currentElement.InnerText;
					}
				case "key":
					{
						return currentElement.InnerText;
					}
				case "string":
					{
						return currentElement.InnerText;
					}
				case "dict":
					{
						Dictionary<string, object> strs = new Dictionary<string, object>();
						for (int i = 0; i <= currentElement.ChildNodes.Count - 2; i = i + 2)
						{
							string str1 = (string)ItunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.ChildNodes[i]);
							object obj = ItunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.ChildNodes[i + 1]);
							strs.Add(str1, obj);
						}
						return strs;
					}
				case "array":
					{
						ArrayList arrayLists = new ArrayList();
						for (int j = 0; j <= currentElement.ChildNodes.Count - 1; j++)
						{
							arrayLists.Add(ItunesXmlFile.ReadKeyAsDictionaryEntry(currentElement.ChildNodes[j]));
						}
						return arrayLists;
					}
				case "true":
				case "false":
					{
						return currentElement.Name;
					}
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
				thisDict[key] = ItunesXmlFile.GetURLDecodeOfString((string)thisDict[key]);
			}
		}
	}
}