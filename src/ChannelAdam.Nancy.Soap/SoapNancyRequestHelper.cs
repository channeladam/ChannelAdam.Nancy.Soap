//-----------------------------------------------------------------------
// <copyright file="SoapNancyRequestHelper.cs">
//     Copyright (c) 2016-2018 Adam Craven. All rights reserved.
// </copyright>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

namespace ChannelAdam.Nancy.Soap
{
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using global::Nancy;
    using System;

    public static class SoapNancyRequestHelper
    {
        #region Public Methods

        public static string GetSoapAction(Request request)
        {
            string soapAction = GetSoapActionFromHeader(request);

            if (string.IsNullOrWhiteSpace(soapAction))
            {
                soapAction = GetSoapActionFromSoapHeaderActionNode(request);
            }

            return soapAction ?? string.Empty;
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetSoapActionFromHeader(Request request)
        {
            var soapActionHeaderItems = request.Headers["SOAPAction"];
            if (soapActionHeaderItems?.Any() == true)
            {
                return soapActionHeaderItems.FirstOrDefault()?.Replace("\"", string.Empty);
            }

            return null;
        }

        private static string GetSoapActionFromSoapHeaderActionNode(Request request)
        {
            string requestBody = NancyRequestHelper.GetRequestBodyAsString(request);

            try
            {
                // TODO: handle multi-part messages - don't assume the body can be parsed as XML directly
                XElement requestBodyElement = XElement.Parse(requestBody);

                var actionElements = requestBodyElement.XPathSelectElements("/*[local-name()='Header']/*[local-name()='Action']");
                if (actionElements?.Any() == true)
                {
                    return actionElements.First().Value;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: {ex}");
                Console.Error.WriteLine($"ERROR: Could not parse the request body as XML:{Environment.NewLine}{requestBody}");
            }

            return null;
        }

        #endregion Private Methods
    }
}
