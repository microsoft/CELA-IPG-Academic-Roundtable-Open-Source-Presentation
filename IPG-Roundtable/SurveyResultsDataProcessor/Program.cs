/*
   Copyright (c) Microsoft Corporation

   All rights reserved. 

   MIT License

   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CELA.SurveyResultsDataProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IPG Academic Roundtable-Survey Data Processor Started");
            var engine = new FileHelperEngine<SurveyResponse>();
            engine.Options.IgnoreFirstLines = 1;
            var result = engine.ReadFile(@"C:\Users\jabarnwe\Source\Repos\OSSCELA-IPG-Roundtable\IPG-Roundtable\PatentDataAcquisition\Data\IPG_Academic_Roundtable-Wisdom_of_Crowds_on_Patents_and_Open_Source_v2.csv");
            List<OrganizationIPSentiment> orgIPSentiment = new List<OrganizationIPSentiment>();
            foreach (var response in result)
            {
                Console.WriteLine(response.Amazon_Supports_Open_Source);
                OrganizationIPSentiment responseToAdd1 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd1);
                responseToAdd1.Organization = "Amazon";
                responseToAdd1.PatentSupport = response.Amazon_Supports_Patent_Rights;
                responseToAdd1.OpenSourceSupport = response.Amazon_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd2 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd2);
                responseToAdd2.Organization = "Apple";
                responseToAdd2.PatentSupport = response.Apple_Supports_Patent_Rights;
                responseToAdd2.OpenSourceSupport = response.Apple_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd3 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd3);
                responseToAdd3.Organization = "Cisco";
                responseToAdd3.PatentSupport = response.Cisco_Supports_Patent_Rights;
                responseToAdd3.OpenSourceSupport = response.Cisco_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd4 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd4);
                responseToAdd4.Organization = "Facebook";
                responseToAdd4.PatentSupport = response.Facebook_Supports_Patent_Rights;
                responseToAdd4.OpenSourceSupport = response.Facebook_Supports_Open_Source;

                //OrganizationIPSentiment responseToAdd5 = new OrganizationIPSentiment();
                //orgIPSentiment.Add(responseToAdd5);
                //responseToAdd5.Organization = "Fujitsu";

                OrganizationIPSentiment responseToAdd6 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd6);
                responseToAdd6.Organization = "Google";
                responseToAdd6.PatentSupport = response.Google_Supports_Patent_Rights;
                responseToAdd6.OpenSourceSupport = response.Google_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd7 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd7);
                responseToAdd7.Organization = "Huawei";
                responseToAdd7.PatentSupport = response.Huawei_Supports_Patent_Rights;
                responseToAdd7.OpenSourceSupport = response.Huawei_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd8 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd8);
                responseToAdd8.Organization = "IBM";
                responseToAdd8.PatentSupport = response.IBM_Supports_Patent_Rights;
                responseToAdd8.OpenSourceSupport = response.IBM_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd9 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd9);
                responseToAdd9.Organization = "Intel";
                responseToAdd9.PatentSupport = response.Intel_Supports_Patent_Rights;
                responseToAdd9.OpenSourceSupport = response.Intel_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd10 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd10);
                responseToAdd10.Organization = "Microsoft";
                responseToAdd10.PatentSupport = response.Microsoft_Supports_Patent_Rights;
                responseToAdd10.OpenSourceSupport = response.Microsoft_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd11 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd11);
                responseToAdd11.Organization = "Oracle";
                responseToAdd11.PatentSupport = response.Oracle_Supports_Patent_Rights;
                responseToAdd11.OpenSourceSupport = response.Oracle_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd12 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd12);
                responseToAdd12.Organization = "Qualcomm";
                responseToAdd12.PatentSupport = response.Qualcomm_Supports_Patent_Rights;
                responseToAdd12.OpenSourceSupport = response.Qualcomm_Supports_Open_Source;

                OrganizationIPSentiment responseToAdd13 = new OrganizationIPSentiment();
                orgIPSentiment.Add(responseToAdd13);
                responseToAdd13.Organization = "Samsung";
                responseToAdd13.PatentSupport = response.Samsung_Supports_Patent_Rights;
                responseToAdd13.OpenSourceSupport = response.Samsung_Supports_Open_Source;
            }

            var engine2 = new FileHelperEngine<OrganizationIPSentiment>();
            engine2.HeaderText = engine2.GetFileHeader();
            engine2.WriteFile(@"C:\Users\jabarnwe\Source\Repos\OSSCELA-IPG-Roundtable\IPG-Roundtable\PatentDataAcquisition\Data\IPG_Academic_Roundtable-Patents_and_Open_Source_Sentiment.csv", orgIPSentiment);

            Console.ReadLine();

        }
    }
}
