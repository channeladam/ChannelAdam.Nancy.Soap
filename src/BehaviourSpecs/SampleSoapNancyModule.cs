using ChannelAdam.Nancy.Soap;
using ChannelAdam.Nancy.Soap.Abstractions;
using ChannelAdam.Soap;
using Nancy;

namespace BehaviourSpecs
{
    public class SampleSoapNancyModule : NancyModule
    {
        public SampleSoapNancyModule(INancySoapAdapter soapAdapter)
        {
            DefineHelloWorldSoapActionRoute(soapAdapter);
        }

        private void DefineHelloWorldSoapActionRoute(INancySoapAdapter soapAdapter)
        {
            const string mySoap11ServiceRoutePattern = "/MySampleSoap11Service";

            DefineSoapRoute(mySoap11ServiceRoutePattern, soapAdapter);

            soapAdapter.RegisterSoapActionHandler(
                mySoap11ServiceRoutePattern,
                "urn:MySampleSoap11Service#HelloWorldSoapAction",
                (request, routeArgs) =>
                    Soap11NancyResponseFactory.Create(
                        SoapBuilder.CreateSoap11Envelope().WithBody.AddEntry("<root>Hello SOAP World!</root>"),
                        HttpStatusCode.OK));

            soapAdapter.RegisterSoapActionHandler(
                mySoap11ServiceRoutePattern,
                "urn:MySampleSoap11Service#AnotherSoapAction",
                (request, routeArgs) =>
                    Soap11NancyResponseFactory.Create(
                        SoapBuilder.CreateSoap11Envelope().WithBody.AddEntry("<root>Hello Another SOAP World!</root>"),
                        HttpStatusCode.OK));
        }

        private void DefineSoapRoute(string routePattern, INancySoapAdapter soapAdapter)
        {
            Post[routePattern] = args => soapAdapter.ProcessRequest(routePattern, base.Request, args);
        }
    }
}
