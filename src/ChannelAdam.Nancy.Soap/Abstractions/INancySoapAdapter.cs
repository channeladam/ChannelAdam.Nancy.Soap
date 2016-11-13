﻿//-----------------------------------------------------------------------
// <copyright file="INancySoapAdapter.cs">
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

namespace ChannelAdam.Nancy.Soap.Abstractions
{
    using System;

    using global::Nancy;

    public interface INancySoapAdapter
    {
        dynamic ProcessRequest(string routePattern, Request request);

        dynamic ProcessRequest(string routePattern, Request request, dynamic requestRouteArgs);

        void RegisterSoapActionHandler(string routePattern, string soapAction, NancyRequestHandler handler);

        Request TryWaitForRequest(string routePattern, string soapAction, int retryCount, TimeSpan sleepDuration);
    }
}