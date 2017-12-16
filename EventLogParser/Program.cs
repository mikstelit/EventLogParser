using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EventLogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileExtensionInZIP = ".evtx";
            string usernameToFind = "FredF";
            string archivedLogPath = @"C:\Users\mike.littlefield\Desktop\Domain Controller Security Logs\";
            string outputFilepath = @"C:\users\mike.littlefield\desktop\";

            EventLogParser SecurityLog = new EventLogParser(fileExtensionInZIP, usernameToFind, archivedLogPath, outputFilepath);
            SecurityLog.ProcessArchives();
        }
    }
}
