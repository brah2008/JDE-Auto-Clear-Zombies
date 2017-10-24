# JDE-Auto-Clear-Zombies
Clear Zombies in JDE Automatically Using Command Line Utility Tool


CNC folks have been battling with zombies for years now and Oracle still has not come up with an option to remove/clear zombies periodically/automatically from enterprise servers. Zombies are those process which have gone defunct and are no longer useful. But at times the parent process (jdenet_n) does not release the resources used by these processes. Zombies may be created due to database issue, faulty code or process errors. When such zombie processes pile up, they consume your server’s valuable resources, leading to slowness of system. And slowness in production environment, is something that can affect your business to a large extent and may cost you time and money.

There are only two ways to clear these zombies:

Clear zombies for affected enterprise server from Server Manager
Kill zombie’s parent process – which would involve restarting JD Edwards services on enterprise servers (this is certainly out of the picture for production environment)
To solve this problem, I have created a command line utility “clearzombies”. This tool accepts several parameters as input and clears zombies from an enterprise server by making web calls to Server Manager. You can schedule this tool, as a command or a batch file script, using windows task scheduler.

To avoid the problem of passing password as a plaintext (as this is a cause of security concern for many organizations), I have also created another utility called “encryptSVMpass” to generate an encrypted password which can be passed to “clearzombies” utility. encryptSVMpass usage is as follows:


