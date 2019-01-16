# NPGSQLTypeMapIssue
C# Console application to reproduce an NPGSQL issue with Type Mapping

## Summary
Using NpgsqlConnection.GlobalTypeMapper globally fails after a Drop Database occurs without restarting the process.
In a multi-tenant system where a Tenant DB can ce re-created, the Type Mapping cannot be used until a process restart.

## Repro Steps 
 
1. Get and Build the solution
2. Open 3 cmd prompts
3. From **CMD1** Call the application with the **Test** parameter
    eg. 
    .\NPGSQLTypeMapIssue.exe "Server=localhost;Port=5432;Database=db1;User Id=postgres;Password=XXXX;Max Auto Prepare=
20;Maximum Pool Size=90;Timeout=60;Command Timeout=120" **Test**
4. **CMD1** will start displaying
  "**Test - Found 1 - Completed**"
5. From **CMD2** Call the application with the **Add** parameter
  eg. 
    .\NPGSQLTypeMapIssue.exe "Server=localhost;Port=5432;Database=db1;User Id=postgres;Password=XXXX;Max Auto Prepare=
  20;Maximum Pool Size=90;Timeout=60;Command Timeout=120" **Add**
6. From **CMD3** Call the application with the **Reset** parameter
  eg. 
    .\NPGSQLTypeMapIssue.exe "Server=localhost;Port=5432;Database=db1;User Id=postgres;Password=XXXX;Max Auto Prepare=
  20;Maximum Pool Size=90;Timeout=60;Command Timeout=120" **Reset**
7. From **CMD2** Call the application with the **Add** parameter
  eg. 
    .\NPGSQLTypeMapIssue.exe "Server=localhost;Port=5432;Database=db1;User Id=postgres;Password=XXXX;Max Auto Prepare=
  20;Maximum Pool Size=90;Timeout=60;Command Timeout=120" **Add**
8. **CMD1** will start displaying
  "**Can't cast database type .<unknown> to Poco**"
