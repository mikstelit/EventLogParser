using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;
using System.Xml;
using System.IO;
using System.IO.Compression;

namespace EventLogParser
{
    class EventLogParser
    {
        private string fileExtensionInZIP;
        private string usernameToFind;
        private string archivedLogPath;
        private string outputFilename;

        public EventLogParser(string fileExtensionInZIP, string usernameToFind, string archivedLogPath, string outputFilepath)
        {
            this.fileExtensionInZIP = fileExtensionInZIP;
            this.usernameToFind = usernameToFind;
            this.archivedLogPath = archivedLogPath;
            this.outputFilename = outputFilepath + usernameToFind + ".csv";
        }

        private List<string> ExtractLogFiles(string sourceArchiveFileName)
        {
            List<string> eventLogNames = new List<string>();

            using (ZipArchive archive = ZipFile.OpenRead(sourceArchiveFileName))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(this.fileExtensionInZIP, StringComparison.OrdinalIgnoreCase))
                    {
                        eventLogNames.Add(Path.GetTempPath() + entry.Name);
                        ZipFile.ExtractToDirectory(sourceArchiveFileName, Path.GetTempPath());
                    }
                }
            }

            return eventLogNames;
        }

        private void ParseLogs(List<string> eventLogNames)
        {
            foreach (string eventLogName in eventLogNames)
            {
                using (StreamWriter outputFile = new StreamWriter(this.outputFilename, true))
                using (EventLogReader reader = new EventLogReader(eventLogName, PathType.FilePath))
                {
                    EventRecord record;
                    while ((record = reader.ReadEvent()) != null)
                    {
                        using (record)
                        {
                            if (record.Id == 4624)
                            {
                                XmlDocument xmlRecord = new XmlDocument();
                                xmlRecord.LoadXml(record.ToXml());
                                string targetUserName = xmlRecord.ChildNodes[0].ChildNodes[1].ChildNodes[5].InnerText;

                                if (targetUserName == usernameToFind)
                                {
                                    outputFile.WriteLine("{0},{1},{2}", record.TimeCreated,
                                                                        targetUserName,
                                                                        xmlRecord.ChildNodes[0].ChildNodes[1].ChildNodes[18].InnerText);
                                }
                            }
                        }
                    }
                }

                File.Delete(eventLogName);
            }
        }

        public void ProcessArchives()
        {
            string[] archiveFiles = Directory.GetFiles(this.archivedLogPath);
            foreach (string archiveFileName in archiveFiles)
            {
                List<string> eventLogNames = this.ExtractLogFiles(archiveFileName);
                this.ParseLogs(eventLogNames);
            }
        }
    }
}
