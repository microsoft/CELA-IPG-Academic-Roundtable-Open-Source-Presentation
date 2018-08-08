/*
   Copyright (c) Microsoft Corporation

   All rights reserved. 

   MIT License

   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace Microsoft.CELA.SurveyResultsDataProcessor
{
    [DelimitedRecord(",")]
    class SurveyResponse
    {
        public string ID;
        public string StartDate;
        public string SubmitDate;
        public string Responder;
        public string ResponderName;
        public string Amazon_Supports_Patent_Rights;
        public string Amazon_Supports_Open_Source;
        public string Apple_Supports_Patent_Rights;
        public string Apple_Supports_Open_Source;
        public string Cisco_Supports_Patent_Rights;
        public string Cisco_Supports_Open_Source;
        public string Facebook_Supports_Patent_Rights;
        public string Facebook_Supports_Open_Source;
        public string Google_Supports_Patent_Rights;
        public string Google_Supports_Open_Source;
        public string Huawei_Supports_Patent_Rights;
        public string Huawei_Supports_Open_Source;
        public string IBM_Supports_Patent_Rights;
        public string IBM_Supports_Open_Source;
        public string Intel_Supports_Patent_Rights;
        public string Intel_Supports_Open_Source;
        public string Microsoft_Supports_Patent_Rights;
        public string Microsoft_Supports_Open_Source;
        public string Oracle_Supports_Patent_Rights;
        public string Oracle_Supports_Open_Source;
        public string Qualcomm_Supports_Patent_Rights;
        public string Qualcomm_Supports_Open_Source;
        public string Samsung_Supports_Patent_Rights;
        public string Samsung_Supports_Open_Source;
        public string Presentation_Enjoyment;

    }
}
