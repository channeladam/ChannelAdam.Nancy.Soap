//-----------------------------------------------------------------------
// <copyright file="NancySoapAdapter.cs">
//     Copyright (c) 2016 Adam Craven. All rights reserved.
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
    using System;
    using System.Collections.Generic;

    using ChannelAdam.Nancy.Soap.Abstractions;
    using ChannelAdam.Nancy.Soap;
    using global::Nancy;
    using Logging;
    using Polly;

    /// <summary>
    /// A handler for processing a Nancy Request.
    /// </summary>
    /// <param name="request">A Nancy <see cref="Request"/>.</param>
    /// <param name="requestRouteArgs">The values from the URL corresponding to those in the route pattern.</param>
    /// <returns>A Nancy <see cref="Response"/>.</returns>
    public delegate Response NancyRequestHandler(Request request, dynamic requestRouteArgs);

    /// <summary>
    /// Provides a mechanism to process SOAP messages in Nancy.
    /// </summary>
    public class NancySoapAdapter : INancySoapAdapter
    {
        #region Private Classes

        private class RequestAndHandlerPair
        {
            #region Public Properties

            public Request LastRequest { get; set; }

            public NancyRequestHandler RequestHandler { get; set; }

            #endregion Public Properties
        }

        #endregion Private Classes

        #region Private Fields

        private readonly ISimpleLogger logger;
        private readonly Dictionary<string, RequestAndHandlerPair> soapActionRouteToRequestHandlerMap;

        #endregion Private Fields

        #region Public Constructors

        public NancySoapAdapter() : this(null)
        {
        }

        public NancySoapAdapter(ISimpleLogger logger)
        {
            this.logger = logger;
            this.soapActionRouteToRequestHandlerMap = new Dictionary<string, RequestAndHandlerPair>();
        }

        #endregion Public Constructors

        #region Public Methods

        public Response ProcessRequest(string routePattern, Request request)
        {
            return ProcessRequest(routePattern, request, null);
        }

        public Response ProcessRequest(string routePattern, Request request, dynamic requestRouteArgs)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var soapAction = request.GetSoapAction();
            this.logger?.Log($"Request path '{request.Path}' was called with SOAP action '{soapAction}'");

            var mapKey = BuildMapKey(routePattern, soapAction);
            if (!this.soapActionRouteToRequestHandlerMap.ContainsKey(mapKey))
            {
                var error = $"Error: The request handler for route pattern '{routePattern}' with SOAP action '{soapAction}' has not been defined yet";
                this.logger?.Log(error);
                throw new InvalidOperationException(error);
            }

            RequestAndHandlerPair pair = this.soapActionRouteToRequestHandlerMap[mapKey];
            pair.LastRequest = request;

            return pair.RequestHandler?.Invoke(request, requestRouteArgs);
        }

        public void RegisterSoapActionHandler(string routePattern, string soapAction, NancyRequestHandler handler)
        {
            var mapKey = BuildMapKey(routePattern, soapAction);
            this.soapActionRouteToRequestHandlerMap[mapKey] = new RequestAndHandlerPair { RequestHandler = handler };
        }

        /// <summary>
        /// Wait for a request to be received - up to a given number of iterations with a given interval between each polling attempt.
        /// </summary>
        /// <param name="routePattern">The route pattern.</param>
        /// <param name="soapAction">The SOAP action.</param>
        /// <param name="retryCount">The number of times to retry detecting whether a request has been received.</param>
        /// <param name="sleepDuration">The duration to sleep between retry attempts.</param>
        /// <returns>The last request that was performed, or null if no request was received.</returns>
        public Request TryWaitForRequest(string routePattern, string soapAction, int retryCount, TimeSpan sleepDuration)
        {
            var retryPolicy = Policy.HandleResult<Request>(req => req == null)
                    .WaitAndRetry(retryCount, retryAttempt => sleepDuration);

            return retryPolicy.Execute(() => GetLastRequest(routePattern, soapAction));
        }

        #endregion

        #region Private Methods

        private static string BuildMapKey(string routePattern, string soapAction)
        {
            return $"{routePattern}-{soapAction}";
        }

        private Request GetLastRequest(string routePattern, string soapAction)
        {
            Request result = null;

            var mapKey = BuildMapKey(routePattern, soapAction);
            if (this.soapActionRouteToRequestHandlerMap.ContainsKey(mapKey))
            {
                result = this.soapActionRouteToRequestHandlerMap[mapKey].LastRequest;
            }

            return result;
        }

        #endregion
    }
}
