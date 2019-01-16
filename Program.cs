using NPGSQLTypeMapIssue.Domain;
using System;

namespace NPGSQLTypeMapIssue
{
    class Program
    {
        enum Operations
        {
            Install,
            Reset,
            Uninstall,
            Add,
            Test
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentOutOfRangeException();
            }
            var connString = args[0];
            var operation = (Operations)(Enum.Parse(typeof(Operations), args[1]));
            var id = args.Length > 2 ? args[2] : "";

            var dbHelper = new DBHelper();
            switch (operation)
            {
                case Operations.Install:
                    {
                        dbHelper.Install(connString);
                    }
                    break;
                case Operations.Uninstall:
                    {
                        dbHelper.Uninstall(connString);
                    }
                    break;
                case Operations.Reset:
                    {
                        dbHelper.Uninstall(connString);
                        dbHelper.Install(connString);
                    }
                    break;
                case Operations.Add:
                    {
                        try
                        {
                            var ticks = DateTime.Now.Ticks;
                            var title = $"Title-{ticks}";
                            var user = $"User-{ticks}";
                            var newItem = new Poco() { Title = title, CreatedBy = user };
                            dbHelper.Add(newItem, connString);
                        }
                        catch (Exception ex)
                        {
                            DBHelper.RegisterDapperNPGSQL();
                            Console.Write(ex.ToString());
                        }
                    }
                    break;
                case Operations.Test:
                    {
                        DBHelper.RegisterDapperNPGSQL();
                        while (true)
                        {
                            try
                            {
                                var results = dbHelper.GetPoco(connString);
                                Console.WriteLine($"{operation} - Found {results.Count} - Completed");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
            }
        }
    }
}
