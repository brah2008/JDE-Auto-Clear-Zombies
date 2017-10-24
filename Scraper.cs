using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace clearzombies
{
	public class Scraper
	{
		public CookieContainer Container
		{
			get;
			set;
		}

		public string ContentType
		{
			get;
			set;
		}

		public CookieCollection Cookies
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public string UserAgent
		{
			get;
			set;
		}

		public string Username
		{
			get;
			set;
		}

		public Scraper()
		{
			this.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
			this.ContentType = "application/x-www-form-urlencoded";
			this.Cookies = new CookieCollection();
			this.Container = new CookieContainer();
		}

		private static CookieCollection ConvertCookieArraysToCookieCollection(ArrayList al, string strHost)
		{
			string[] strArrays;
			CookieCollection cookieCollections = new CookieCollection();
			int count = al.Count;
			for (int i = 0; i < count; i++)
			{
				string str = al[i].ToString();
				string[] strArrays1 = str.Split(new char[] { ';' });
				int length = (int)strArrays1.Length;
				string empty = string.Empty;
				string empty1 = string.Empty;
				string str1 = string.Empty;
				Cookie cookie = new Cookie();
				for (int j = 0; j < length; j++)
				{
					if (j == 0)
					{
						empty = strArrays1[j];
						if (empty != string.Empty)
						{
							int num = empty.IndexOf("=");
							string str2 = empty.Substring(0, num);
							string str3 = empty.Substring(num + 1, empty.Length - (num + 1));
							cookie.Name = str2;
							cookie.Value = str3;
						}
					}
					else if (strArrays1[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						empty1 = strArrays1[j];
						if (empty1 != string.Empty)
						{
							strArrays = empty1.Split(new char[] { '=' });
							if (strArrays[1] == string.Empty)
							{
								cookie.Path = "/";
							}
							else
							{
								cookie.Path = strArrays[1];
							}
						}
					}
					else if (strArrays1[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						empty1 = strArrays1[j];
						if (empty1 != string.Empty)
						{
							strArrays = empty1.Split(new char[] { '=' });
							if (strArrays[1] == string.Empty)
							{
								cookie.Domain = strHost;
							}
							else
							{
								cookie.Domain = strArrays[1];
							}
						}
					}
				}
				if (cookie.Path == string.Empty)
				{
					cookie.Path = "/";
				}
				if (cookie.Domain == string.Empty)
				{
					cookie.Domain = strHost;
				}
				cookieCollections.Add(cookie);
			}
			return cookieCollections;
		}

		private static ArrayList ConvertCookieHeaderToArrayList(string strCookHeader)
		{
			strCookHeader = strCookHeader.Replace("\r", "");
			strCookHeader = strCookHeader.Replace("\n", "");
			string[] strArrays = strCookHeader.Split(new char[] { ',' });
			ArrayList arrayLists = new ArrayList();
			int num = 0;
			int length = (int)strArrays.Length;
			while (num < length)
			{
				if (strArrays[num].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) <= 0)
				{
					arrayLists.Add(strArrays[num]);
				}
				else
				{
					arrayLists.Add(string.Concat(strArrays[num], ",", strArrays[num + 1]));
					num++;
				}
				num++;
			}
			return arrayLists;
		}

		public static CookieCollection GetAllCookiesFromHeader(string strHeader, string strHost)
		{
			ArrayList arrayLists = new ArrayList();
			CookieCollection cookieCollections = new CookieCollection();
			if (strHeader != string.Empty)
			{
				cookieCollections = Scraper.ConvertCookieArraysToCookieCollection(Scraper.ConvertCookieHeaderToArrayList(strHeader), strHost);
			}
			return cookieCollections;
		}

		public string Load(string uri, string postData = "", NetworkCredential creds = null, int timeout = 60000, string host = "", string referer = "", string requestedwith = "", string cType = "application/x-www-form-urlencoded")
		{
			StringBuilder stringBuilder;
			HttpWebRequest container = (HttpWebRequest)WebRequest.Create(uri);
			container.CookieContainer = this.Container;
			container.CookieContainer.Add(this.Cookies);
			container.UserAgent = this.UserAgent;
			container.AllowWriteStreamBuffering = true;
			container.ProtocolVersion = HttpVersion.Version11;
			container.AllowAutoRedirect = true;
			container.ContentType = cType;
			container.PreAuthenticate = true;
			container.Host = host;
			if (requestedwith.Length > 0)
			{
				container.Headers["X-Requested-With"] = requestedwith;
			}
			if (host.Length > 0)
			{
				container.Host = host;
			}
			if (referer.Length > 0)
			{
				container.Referer = referer;
			}
			if (timeout > 0)
			{
				container.Timeout = timeout;
			}
			if (creds != null)
			{
				container.Credentials = creds;
			}
			if (postData.Length <= 0)
			{
				container.Method = "GET";
			}
			else
			{
				container.Method = "POST";
				byte[] bytes = (new ASCIIEncoding()).GetBytes(postData);
				container.ContentLength = (long)((int)bytes.Length);
				Stream requestStream = container.GetRequestStream();
				requestStream.Write(bytes, 0, (int)bytes.Length);
				requestStream.Close();
			}
			HttpWebResponse response = (HttpWebResponse)container.GetResponse();
			this.Cookies = response.Cookies;
			using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
			{
				stringBuilder = new StringBuilder(streamReader.ReadToEnd());
				stringBuilder = stringBuilder.Replace("\r\n", "");
				stringBuilder = stringBuilder.Replace("\r", "");
				stringBuilder = stringBuilder.Replace("\n", "");
				stringBuilder = stringBuilder.Replace("\t", "");
			}
			return Regex.Replace(stringBuilder.ToString(), ">\\s+<", "><");
		}
	}
}