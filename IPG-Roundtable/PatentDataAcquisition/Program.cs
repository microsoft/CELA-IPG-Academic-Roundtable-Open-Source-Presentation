/*
   Copyright (c) Microsoft Corporation

   All rights reserved. 

   MIT License

   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using HtmlAgilityPack;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CELA.PatentDataAcquisition
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IPG Academic Roundtable-Patent Data Utility Started");
            //Parse USPTO Data
            USPTOPatentDataParser parser = new USPTOPatentDataParser();
            parser.processData(@"C:\Users\jabarnwe\Source\Repos\OSSCELA-IPG-Roundtable\IPG-Roundtable\PatentDataAcquisition\Data\USPTO_Top_Filers", @"C:\Users\jabarnwe\Source\Repos\OSSCELA-IPG-Roundtable\IPG-Roundtable\PatentDataAcquisition\Data\top_US_patent_filers_by_year.csv");

            Console.ReadLine();
        }
    }

    //Note that the USPTO server kept terminating the connection so the files were downloaded manually
    class PatentDataDownloadUtility
    {
        public string localOperatingDirectory { get; set; }
        public PatentDataDownloadUtility(string _localOperatingDirectory)
        {
            localOperatingDirectory = _localOperatingDirectory;
        }

        public bool downloadPatentHTMLFiles(string remoteDownloadDirectory)
        {
            WebClient webClient = new WebClient();
            for (int i = 2; i < 16; i++)
            {
                string patentDataHTMLURLFileName = null;
                if (i < 10)
                {
                    patentDataHTMLURLFileName = "topo_" + "0" + i.ToString() + ".htm";
                }
                else
                {
                    patentDataHTMLURLFileName = "topo_" + i.ToString() + ".htm";
                }
                string downloadFileURL = remoteDownloadDirectory + patentDataHTMLURLFileName;
                string saveFilePath = localOperatingDirectory + patentDataHTMLURLFileName;
                try
                {
                    Console.WriteLine("Downloading " + downloadFileURL);
                    webClient.DownloadFile(downloadFileURL, saveFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return true;
        }
    }

    class PatentFilingRecord
    {
        public string PatentFilerName { get; set; }
        public string PatentFilingYear { get; set; }
        public int PatentFilingsInYear { get; set; }
    }

    class USPTOPatentDataParser
    {
        public USPTOPatentDataParser()
        {
        }

        public bool processData(string dataFilesFolder, string outputFilePath)
        {
            Console.WriteLine("Processing patent data files");
            List<PatentFilingRecord> patents = getFilingsInDocumentsInDirectory(dataFilesFolder);
            Console.WriteLine("Done processing patent files");
            writePatentRecordsToCSV(patents, outputFilePath);
            Console.WriteLine("Done writing output file");
            return true;
        }

        public bool writePatentRecordsToCSV(List<PatentFilingRecord> patentFilingRecords, string filePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("PatentFilerName,PatentFilingYear,PatentFilingsInYear");
            foreach (var patentFilingRecord in patentFilingRecords)
            {
                sb.AppendLine("\"" + patentFilingRecord.PatentFilerName + "\", " + patentFilingRecord.PatentFilingYear + "," + patentFilingRecord.PatentFilingsInYear);
            }
            File.WriteAllText(filePath, sb.ToString());
            return true;
        }

        public List<PatentFilingRecord> getFilingsInDocumentsInDirectory(string directory)
        {
            List<PatentFilingRecord> patents = new List<PatentFilingRecord>();
            string[] fileEntries = Directory.GetFiles(directory);
            foreach (var filePath in fileEntries)
            {
                HtmlDocument document = getHTMLDocument(filePath);
                getFilingsInDocument(document, patents);
            }
            return patents;
        }

        public HtmlDocument getHTMLDocument(string localFilePath)
        {
            string html = File.ReadAllText(localFilePath);
            HtmlDocument filingListDocument = new HtmlDocument();
            filingListDocument.LoadHtml(html);
            return filingListDocument;
        }

        public List<PatentFilingRecord> getFilingsInDocument(HtmlDocument htmlDocument, List<PatentFilingRecord> patents)
        {
            HtmlNodeCollection tables = htmlDocument.DocumentNode.SelectNodes("//table");
            string filingYear = tables.ElementAt(4).SelectSingleNode("thead").SelectSingleNode("tr").SelectSingleNode("th[3]").InnerText;
            HtmlNode tableBody = tables.ElementAt(4).SelectSingleNode("tbody");
            HtmlNodeCollection rows = tableBody.SelectNodes("tr");

            foreach (var row in rows)
            {
                PatentFilingRecord record = new PatentFilingRecord();
                record.PatentFilingYear = filingYear;
                HtmlDocument rowDoc = new HtmlDocument();
                rowDoc.LoadHtml(row.InnerHtml);
                var rowQuery = from data in rowDoc.DocumentNode.SelectNodes("//th").Cast<HtmlNode>()
                               select new { dataText = data.InnerText };

                int i = 0;
                foreach (var dataItem in rowQuery)
                {
                    if (i == 0)
                    {
                        record.PatentFilerName = dataItem.dataText.Trim();
                    }
                    i++;
                }

                rowQuery = from data in rowDoc.DocumentNode.SelectNodes("//td").Cast<HtmlNode>()
                               select new { dataText = data.InnerText };

                int j = 0;
                foreach (var dataItem in rowQuery)
                {
                    if (j == 0)
                    {
                        record.PatentFilingsInYear = Int32.Parse(dataItem.dataText.Trim());
                    }
                    j++;
                }
                patents.Add(record);
            }
            return patents;

        }
    }
}
