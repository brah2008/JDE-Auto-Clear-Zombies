using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace clearzombies
{
	public class clearZombies
	{
		public static string host;

		public static string username;

		public static string password;

		public static string procURL;

		public static string entHost;

		public static string instName;

		public static string jdeHome;

		public static int port;

		public static bool debugon;

		public static string passPhrase;

		static clearZombies()
		{
			clearZombies.host = "";
			clearZombies.username = "";
			clearZombies.password = "";
			clearZombies.procURL = "";
			clearZombies.entHost = "";
			clearZombies.instName = "";
			clearZombies.jdeHome = "";
			clearZombies.debugon = false;
			clearZombies.passPhrase = "npjdeclearzombieautomation";
		}

		public clearZombies()
		{
		}

		public void clear()
		{
			string str = "";
			string str1 = "";
			Scraper scraper = new Scraper();
			str = string.Concat(new object[] { "http://", clearZombies.host, ":", clearZombies.port, "/manage/home" });
			clearZombies.debugmsg(string.Concat("Generating session ID cookie by accessing URL: ", str));
			string str2 = scraper.Load(str, str1, null, 60000, string.Concat(clearZombies.host, ":", clearZombies.port), "", "", "application/x-www-form-urlencoded");
			if ((str2.Contains("Sign In") ? false : !str2.Contains("Login")))
			{
				Console.WriteLine("Could not access server manager URL, please make sure server manager is up and details provided are correct!");
			}
			else
			{
				clearZombies.debugmsg("Page loaded sucessfully");
				str2 = "";
				str = string.Concat(new object[] { "http://", clearZombies.host, ":", clearZombies.port, "/manage/j_security_check" });
				clearZombies.debugmsg(string.Concat("Authenticating session ID by sending credentials to URL: ", str));
				str1 = string.Concat(new string[] { "j_username=", clearZombies.username, "&j_password=", clearZombies.password, "&url=%2Fmanage%2Fhome" });
				str2 = scraper.Load(str, str1, null, 60000, string.Concat(clearZombies.host, ":", clearZombies.port), "", "", "application/x-www-form-urlencoded");
				if (!str2.Contains("logout"))
				{
					Console.WriteLine("Could not login to server manager, please make sure credential details provided are correct!");
				}
				else
				{
					clearZombies.debugmsg("Login to Server Manager was successful");
					str2 = "";
					str = string.Concat(new object[] { "http://", clearZombies.host, ":", clearZombies.port, "/manage/target?&hostName=", clearZombies.entHost, "&instanceName=", clearZombies.instName, "&targetType=entserver&jdeHome=", clearZombies.jdeHome, "&action=processes" });
					clearZombies.debugmsg(string.Concat("Accessing process details page at URL: ", str));
					str1 = "";
					str2 = scraper.Load(str, str1, null, 60000, string.Concat(clearZombies.host, ":", clearZombies.port), "", "", "application/x-www-form-urlencoded");
					clearZombies.debugmsg("Checking for zombie kernels on process details page...");
					ArrayList arrayLists = clearZombies.findZombies(str2);
					int count = arrayLists.Count;
					if (count != 0)
					{
						Console.WriteLine(string.Concat("Total number of zombies detected: ", count));
						Console.WriteLine("Initiating cleanup... following are details of zombies cleared:");
						Console.WriteLine(string.Concat(new string[] { "Hostname".PadRight(20), "Process ID".PadRight(12), "Process Type".PadRight(20), "Process Name".PadRight(20), "Status".PadRight(8) }));
						for (int i = 0; i < count; i++)
						{
							Processes item = (Processes)arrayLists[i];
							str = string.Concat(new object[] { "http://", clearZombies.host, ":", clearZombies.port, "/manage/ajax" });
							str1 = item.generatePayload();
							str2 = scraper.Load(str, str1, null, 60000, string.Concat(clearZombies.host, ":", clearZombies.port), "", "", "application/xml");
							if (!str2.Contains("success"))
							{
								item.status = "Failed";
							}
							else
							{
								item.status = "Success";
							}
							Console.WriteLine(string.Concat(new string[] { item.host.PadRight(20), item.processID.PadRight(12), item.processType.PadRight(20), item.processName.PadRight(20), item.status.PadRight(8) }));
							item = null;
						}
					}
					else
					{
						clearZombies.displayNoZombies();
					}
				}
			}
		}

		public static void debugmsg(string msg)
		{
			if (clearZombies.debugon)
			{
				Console.WriteLine(msg);
			}
		}

		public static void displayHelp()
		{
			Console.WriteLine("Usage: clearzombies SVMHost SVMPort SVMUserID SVMPasswordEncrypted\r\n        ESHostName ESInstanceName JDEAgentInstallPath [debugon]\r\n\r\nOptions:\r\n\tSVMHost\t\t\t\t\tHostname of the server on which server manager is installed\r\n\tSVMPort\t\t\t\t\tPort number on which server manager is running, generally it is 8999\r\n\tSVMUserID\t\t\t\tUser ID to login to Server Manager\r\n\tSVMPasswordEncrypted\tUse encryption tool 'encryptSVMpass' to generate encrypted password\r\n\tESHostName\t\t\t\tHostname of Enterprise Server as registered in Server Manager\r\n\tESInstanceName\t\t\tInstance name of Enterprise Server as registered in Server Manager\r\n\tJDEAgentInstallPath\t\tFull path to directory where agent is installed, can be obtained from Server Manager\r\n\tdebugon\t\t\t\t\tOptional parameter, will output debug messages while execution\r\n\t\r\nExample Usage 1:\r\n\tclearzombies JDEDEPSVR 8999 jde_admin 6pNmxcl6jImEn0aD81KYPNi4NyEG5FkKEV2lTbnUAPCC32QcfZvzNm3h4YGFHJFI9cGJIE9t+/KODu6XnIKYXzMOF+BhCSjfRx66uXZ4DURu/7hIu6Z4KHycqB61K/6s JDEENTSVR Ent_Prod /u01/apps/jdedwards/agent\r\n\r\nExample Usage 2:\r\n\tclearzombies JDEDEPSVR.company.com 8999 jde_admin 6pNmxcl6jImEn0aD81KYPNi4NyEG5FkKEV2lTbnUAPCC32QcfZvzNm3h4YGFHJFI9cGJIE9t+/KODu6XnIKYXzMOF+BhCSjfRx66uXZ4DURu/7hIu6Z4KHycqB61K/6s JDEENTSVR.company.com Ent_Prod /u01/apps/jdedwards/agent\r\n\t\r\nDescription:\r\n\tclearzombies is a command line utility which takes necessary inputs and performs web calls\r\n\tto login to Server Manager, access process details page for provided enterprise server, \r\n\tdetermine if there are any zombie processes and if found, it sends a payload request to\r\n\tServer Manager's ajax handler servlet, which clears the zombie process.\r\n\t\r\nNote:\r\n\tEnsure that ESHostname, ESInstanceName and JDEInstallPath are exactly as registered in Server Manager.\r\n\tThese details can be obtained from Server Manager Dashboard page and you are advised to copy paste it \r\n\tfrom there instead of typing, to avoid human error.\r\n\t\r\nAuthor: Nimish Prabhu\r\n\r\nWebsite: http://www.nimishprabhu.com\r\n\r\nEmail: iam@nimishprabhu.com");
		}

		public static void displayNoZombies()
		{
			Console.WriteLine("No zombies found");
		}

		public static ArrayList findZombies(string data)
		{
			string str = "clearZombie=\"clearZombie\\('([^']+)', '([^']+)', '([^']+)', '([^']+)'\\)\" /\\>\\</td\\>\\<td\\>\\<a href=\"[^\"]+\"\\>([^\\<]+)\\</a\\>\\</td\\>\\<td\\>([^\\<]+)\\</td\\>\\<td\\>([^\\<]+)\\</td\\>";
			int num = 0;
			ArrayList arrayLists = new ArrayList();
			foreach (Match match in Regex.Matches(data, str))
			{
				if (match.Groups.Count == 8)
				{
					Processes process = new Processes();
					for (int i = 1; i < match.Groups.Count; i++)
					{
						switch (i)
						{
							case 1:
							{
								process.host = match.Groups[i].Value;
								break;
							}
							case 2:
							{
								process.port = match.Groups[i].Value;
								break;
							}
							case 3:
							{
								process.objectName = match.Groups[i].Value;
								break;
							}
							case 4:
							{
								process.processIndex = match.Groups[i].Value;
								break;
							}
							case 5:
							{
								process.processName = match.Groups[i].Value;
								break;
							}
							case 6:
							{
								process.processType = match.Groups[i].Value;
								break;
							}
							case 7:
							{
								process.processID = match.Groups[i].Value;
								break;
							}
						}
					}
					arrayLists.Add(process);
					num++;
				}
			}
			return arrayLists;
		}

		private static bool HelpRequired(string param)
		{
			return (param == "-h" || param == "--help" ? true : param == "/?");
		}

		public bool validateArgs(string[] args)
		{
			bool flag;
			if (args == null)
			{
				clearZombies.displayHelp();
				flag = false;
			}
			else if (((int)args.Length != 1 ? true : !clearZombies.HelpRequired(args[0])))
			{
				int length = (int)args.Length;
				if ((length < 7 ? false : length <= 8))
				{
					clearZombies.host = args[0];
					clearZombies.username = args[2];
					clearZombies.entHost = args[4];
					clearZombies.instName = args[5];
					clearZombies.jdeHome = Uri.EscapeDataString(args[6]);
					if ((!int.TryParse(args[1], out clearZombies.port) || clearZombies.port < 0 ? true : clearZombies.port > 65535))
					{
						Console.WriteLine(string.Concat("Port number \"", args[1], "\" is invalid"));
						clearZombies.displayHelp();
						flag = false;
					}
					else if (Uri.CheckHostName(clearZombies.host) == UriHostNameType.Unknown)
					{
						Console.WriteLine("Server manager host name is invalid!");
						clearZombies.displayHelp();
						flag = false;
					}
					else if (Uri.CheckHostName(clearZombies.entHost) != UriHostNameType.Unknown)
					{
						try
						{
							clearZombies.password = StringCipher.Decrypt(args[3], clearZombies.passPhrase);
						}
						catch (Exception exception)
						{
							Console.WriteLine("Encrypted password invalid! Please ensure you are using encryptSVMpass tool to encrypt password");
							clearZombies.displayHelp();
							flag = false;
							return flag;
						}
						if ((length != 8 ? false : args[7].Equals("debugon")))
						{
							clearZombies.debugon = true;
						}
						flag = true;
					}
					else
					{
						Console.WriteLine("Enterprise host name is invalid!");
						clearZombies.displayHelp();
						flag = false;
					}
				}
				else
				{
					Console.WriteLine("Invalid number of arguments provided!");
					clearZombies.displayHelp();
					flag = false;
				}
			}
			else
			{
				clearZombies.displayHelp();
				flag = false;
			}
			return flag;
		}
	}
}