using ChannelAdam.Nancy.Soap.Abstractions;
using Nancy;

namespace BehaviourSpecs
{
    public class TestCaseNancyModule : NancyModule
    {
        public TestCaseNancyModule(INancySoapAdapter soapAdapter)
        {
            DefineSoapRoute(TestCaseRoutes.MySoap11Service, soapAdapter);
            DefineSoapRoute(TestCaseRoutes.MySoap12Service, soapAdapter);
        }

        private void DefineSoapRoute(string routePattern, INancySoapAdapter soapAdapter)
        {
            Post(routePattern, args => soapAdapter.ProcessRequest(routePattern, base.Request, args));
        }
    }
}
