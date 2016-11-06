//-----------------------------------------------------------------------
// <copyright file="Soap11NancyResponseFactory.cs">
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
    using System.Xml.Linq;

    using ChannelAdam.Soap;
    using ChannelAdam.Soap.Abstractions;
    using global::Nancy;

    public static class Soap11NancyResponseFactory
    {
        #region Public Methods

        public static Response Create(ISoap11EnvelopeBuilder envelopeBuilder, HttpStatusCode httpStatusCode)
        {
            if (envelopeBuilder == null)
            {
                throw new ArgumentNullException(nameof(envelopeBuilder));
            }

            return Create(envelopeBuilder.Build(), httpStatusCode);
        }

        public static Response Create(XContainer soap11Envelope, HttpStatusCode httpStatusCode)
        {
            if (soap11Envelope == null)
            {
                throw new ArgumentNullException(nameof(soap11Envelope));
            }

            return Create(soap11Envelope.ToString(), httpStatusCode);
        }

        public static Response Create(string soap11EnvelopeXml, HttpStatusCode httpStatusCode)
        {
            return NancyResponseFactory.CreateFromString(soap11EnvelopeXml, httpStatusCode, Soap11Constants.ContentType);
        }

        public static Response CreateFault(ISoap11EnvelopeBuilder envelopeBuilder)
        {
            if (envelopeBuilder == null)
            {
                throw new ArgumentNullException(nameof(envelopeBuilder));
            }

            return CreateFault(envelopeBuilder.Build());
        }

        public static Response CreateFault(XContainer envelope)
        {
            return Create(envelope, HttpStatusCode.InternalServerError);
        }

        #endregion Public Methods
    }
}