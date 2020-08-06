# JDE-Auto-Clear-Zombies
Clear Zombies in JDE Automatically Using Command Line Utility Tool


CNC folks have been battling with zombies for years now and Oracle still has not come up with an option to remove/clear zombies periodically/automatically from enterprise servers. Zombies are those process which have gone defunct and are no longer useful. But at times the parent process (jdenet_n) does not release the resources used by these processes. Zombies may be created due to database issue, faulty code or process errors. When such zombie processes pile up, they consume your server’s valuable resources, leading to slowness of system. And slowness in production environment, is something that can affect your business to a large extent and may cost you time and money.

There are only two ways to clear these zombies:

Clear zombies for affected enterprise server from Server Manager
Kill zombie’s parent process – which would involve restarting JD Edwards services on enterprise servers (this is certainly out of the picture for production environment)
To solve this problem, I have created a command line utility “clearzombies”. This tool accepts several parameters as input and clears zombies from an enterprise server by making web calls to Server Manager. You can schedule this tool, as a command or a batch file script, using windows task scheduler.

To avoid the problem of passing password as a plaintext (as this is a cause of security concern for many organizations), I have also created another utility called “encryptSVMpass” to generate an encrypted password which can be passed to “clearzombies” utility. encryptSVMpass usage is as follows:

<p align="center">
  <img src="http://nimishprabhu.com/wp-content/uploads/2017/08/encryptSVMpass-700x65.jpg" width="700"/>
</p>

Following is usage guide for clearzombies utility:

Usage:

	1	clearzombies SVMHost SVMPort SVMUserID SVMPasswordEncrypted
	2	 ESHostName ESInstanceName JDEAgentInstallPath [debugon]
		




	1	SVMHost                 Hostname of the server on which server manager is installed
	2	SVMPort                 Port number on which server manager is running, generally it is 8999
	3	SVMUserID               User ID to login to Server Manager
	4	SVMPasswordEncrypted    Use encryption tool 'encryptSVMpass' to generate encrypted password
	5	ESHostName              Hostname of Enterprise Server as registered in Server Manager
	6	ESInstanceName          Instance name of Enterprise Server as registered in Server Manager
	7	JDEAgentInstallPath     Full path to directory where agent is installed, can be obtained from Server Manager
	8	JDEAgentInstallPath     Full path to directory where agent is installed, can be obtained from Server Manager


					

Example Usage 1:

	1	clearzombies JDEDEPSVR 8999 jde_admin 6pNmxcl6jImEn0aD81KYPNi4NyEG5FkKEV2lTbnUAPCC32QcfZvzNm3h4YGFHJFI9cGJIE9t+/KODu6XnIKYXzMOF+BhCSjfRx66uXZ4DURu/7hIu6Z4KHycqB61K/6s JDEENTSVR Ent_Prod /u01/apps/jdedwards/agent	


Example Usage 2:

	1	clearzombies JDEDEPSVR.company.com 8999 jde_admin 6pNmxcl6jImEn0aD81KYPNi4NyEG5FkKEV2lTbnUAPCC32QcfZvzNm3h4YGFHJFI9cGJIE9t+/KODu6XnIKYXzMOF+BhCSjfRx66uXZ4DURu/7hIu6Z4KHycqB61K/6s JDEUBESVR.company.com Ent_Prod /u01/apps/jdedwards/e900/jde_home/agent	

<p align="center">
  <img src="http://nimishprabhu.com/wp-content/uploads/2017/08/Parameter-details-700x238.png" width="700"/>
</p>

 Description:
clearzombies is a command line utility which takes necessary inputs and performs web calls to login to Server Manager, access process details page for provided enterprise server, determine if there are any zombie processes and if found, it sends a payload request to Server Manager’s ajax handler servlet, which clears the zombie process.

Note:
Ensure that ESHostname, ESInstanceName and JDEInstallPath are exactly as registered in Server Manager. These details can be obtained from Server Manager Dashboard page and you are advised to copy paste it from there instead of typing, to avoid human error.

Following is a screenshot of functioning of clearzombies utility:


<p align="center">
  <img src="http://nimishprabhu.com/wp-content/uploads/2017/08/clear-zombie-jde-utility-700x188.jpg" width="700"/>
</p>

If you have multiple enterprise servers, save commands for each server, one server per line, in a batch file (copy the command with full path to a text file and save it with .bat extension) and schedule batch file to run, say for example, every 1 hour. Follow this link to know how to schedule a task in windows task scheduler, second half of the article describes scheduling a program/script.

Following is an example of batch script for 2 servers, it can be modified for any number of servers:

	1	@echo off
	2	REM Set the path in which clearzombies.exe has been placed
	3	SET dirpath="C:\CNC_Tools\ClearZombies"
	4	cd %dirpath%
	5	
	6	REM Initialize variables
	7	SET SVMHost="JDEDEPSVR"
	8	SET SVMPort="8999"
	9	SET SVMUserID="jde_admin"
	10	SET SVMPasswordEncrypted="6pNmxcl6jImEn0aD81KYPNi4NyEG5FkKEV2lTbnUAPCC32QcfZvzNm3h4YGFHJFI9cGJIE9t+/KODu6XnIKYXzMOF+BhCSjfRx66uXZ4DURu/7hIu6Z4KHycqB61K/6s"
	11	
	12	SET ESHostName="JDEDEVSVR"
	13	SET ESInstanceName="ent_dev"
	14	SET JDEAgentInstallPath="/u01/apps/jdedwards/agent"
	15	
	16	clearzombies %SVMHost% %SVMPort% %SVMUserID% %SVMPasswordEncrypted% %ESHostName% %ESInstanceName% %JDEAgentInstallPath%
	17	
	18	SET ESHostName="JDEPRDSVR"
	19	SET ESInstanceName="ent_prd"
	20	SET JDEAgentInstallPath="/u01/apps/jdedwards/agent"
	21	
	22	clearzombies %SVMHost% %SVMPort% %SVMUserID% %SVMPasswordEncrypted% %ESHostName% %ESInstanceName% %JDEAgentInstallPath%
	
Save this code in a file with .bat extension and schedule it in task scheduler to automate clearing of zombies in your JD Edwards setup.

I have tested this tool with latest server manager releases and it works fine. Please add comments in case you face any issues or to report bugs with the tool. If you would like to have something customized for your project in a similar manner, please use contact form to reach me and I will get back to you. 

https://timsoftdz.blogspot.com/
