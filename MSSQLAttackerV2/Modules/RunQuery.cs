using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;



namespace MSSQLAttackerV2.Modules
{
    internal class RunQuery
    {
        public List<string> getQueryResult(SqlConnection con, String query)
        {

            String querylogin = query;
            SqlCommand command = new SqlCommand(querylogin, con);
            SqlDataReader reader = command.ExecuteReader();
            List<string> output = new List<string>();
            while (reader.Read())
            {

                output.Add(reader[0].ToString());
            }
            reader.Close();
            return output;

        }
        public void customQuery(SqlConnection con, [Optional, DefaultParameterValue(null)] String queryIn)
        {
            var helpWrite = new Helpwrite();
            if (string.IsNullOrEmpty(queryIn))
            {
                while (true)
                {
                    try
                    {
                        Console.Write("Query> ");
                        String query = Console.ReadLine().ToString();
                        if (query == "exit") { helpWrite.doWrite(0, "Exiting!"); break; }
                        String execQuery = String.Format("{0};", query);
                        String output = helpWrite.getFilteredResult(getQueryResult(con, execQuery));
                        helpWrite.doWrite(2, output);
                    }
                    catch (Exception)
                    {
                        helpWrite.doWrite(0, "Query Execution Failed");
                        return;

                    }
                }

            }
            try
            {
                String query = String.Format("{0};", queryIn);
                Console.WriteLine(query);
                String queryout = helpWrite.getFilteredResult(getQueryResult(con, query));
                helpWrite.doWrite(2, queryout);
                return;

            }
            catch (Exception)
            {

                helpWrite.doWrite(0, "Missing Privileges || Ex.Sysadmin");
            }


        }

    }

}
