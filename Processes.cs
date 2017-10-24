using System;
using System.Runtime.CompilerServices;

namespace clearzombies
{
	public class Processes
	{
		public string host
		{
			get;
			set;
		}

		public string objectName
		{
			get;
			set;
		}

		public string port
		{
			get;
			set;
		}

		public string processID
		{
			get;
			set;
		}

		public string processIndex
		{
			get;
			set;
		}

		public string processName
		{
			get;
			set;
		}

		public string processType
		{
			get;
			set;
		}

		public string status
		{
			get;
			set;
		}

		public Processes()
		{
		}

		public string generatePayload()
		{
			return string.Concat(new string[] { "<request><invoke host=\"", this.host, "\" port=\"", this.port, "\" objectName=\"", this.objectName, "\" method=\"clearZombie\"><parameter name=\"processIndex\" type=\"int\" value=\"", this.processIndex, "\"/></invoke></request>" });
		}
	}
}