using System.Net.Http;
using System;

using ChannelAdam.Nancy;
using ChannelAdam.Soap;
using Nancy.Hosting.Self;
using Nancy;
using TechTalk.SpecFlow;
using ChannelAdam.Nancy.Hosting.Self;
using ChannelAdam.TestFramework.MSTestV2.Abstractions;
using ChannelAdam.Nancy.Soap;

namespace BehaviourSpecs
{
    [Binding]
    [Scope(Feature = "NancySoapAdapter")]
    public class NancySoapAdapterUnitSteps : TestEasy
    {
        #region Fields

        private const string NancyBaseUrl = "http://localhost:8087";

        private const string HelloWorldSampleRouteUrl = "/MySampleSoap11Service";

        private const string HelloWorldSampleAction1 = "urn:MySampleSoap11Service#HelloWorldSoapAction";

        private const string HelloWorldSampleAction1ExpectedResponse =
@"<env:Envelope xmlns:env=""http://schemas.xmlsoap.org/soap/envelope/"">
  <env:Body>
    <root>Hello SOAP World!</root>
  </env:Body>
</env:Envelope>";

        private const string HelloWorldSampleAction2 = "urn:MySampleSoap11Service#AnotherSoapAction";

        private const string HelloWorldSampleAction2ExpectedResponse =
@"<env:Envelope xmlns:env=""http://schemas.xmlsoap.org/soap/envelope/"">
  <env:Body>
    <root>Hello Another SOAP World!</root>
  </env:Body>
</env:Envelope>";

        private static readonly HttpClient MyHttpClient = new HttpClient();

        private NancySoapAdapter nancySoapAdapter;
        private NancyHost nancyHost;

        private string request;
        private string expectedResponse;
        private string actualResponse;

        #endregion

        #region Before/After

        [BeforeScenario]
        public void BeforeScenario()
        {
            this.nancySoapAdapter = new NancySoapAdapter(base.Logger);
            this.nancyHost = NancySelfHostFactory.CreateAndStartNancyHostInBackgroundThread(new NancySoapAdapterBootstrapper(this.nancySoapAdapter), new Uri(NancyBaseUrl));
        }

        [AfterScenario]
        public void AfterScenario()
        {
            this.nancyHost?.Stop();
        }

        #endregion

        #region Given

        [Given(@"a valid request for the SOAP 1.1 service")]
        public void GivenAValidRequestForTheSOAP11Service()
        {
            this.request = SoapBuilder.CreateSoap11Envelope()
                .WithHeader.AddAction(TestCaseSoapActions.MySoap11ServiceTestAction)
                .WithBody.AddEntry("<TestAction>Hello</TestAction>")
                .Build().ToString();
            Logger.Log("Request: " + this.request);
        }

        [Given(@"the SOAP 1.1 service will respond with a successful payload")]
        public void GivenTheSOAPServiceWillRespondWithASuccessfulPayload()
        {
            this.expectedResponse = SoapBuilder.CreateSoap11Envelope()
                .WithBody.AddEntry("<TestActionResponse>World!</TestActionResponse>")
                .Build().ToString();
            Logger.Log("Expected response: " + this.expectedResponse);

            this.nancySoapAdapter.RegisterSoapActionHandler(
                TestCaseRoutes.MySoap11Service,
                TestCaseSoapActions.MySoap11ServiceTestAction,
                (request, _) =>
                {
                    Logger.Log($"Fake service for action '{TestCaseSoapActions.MySoap11ServiceTestAction}' is executing...");
                    Logger.Log("Request message was: " + request.GetRequestBodyAsString());

                    return Soap11NancyResponseFactory.Create(this.expectedResponse, HttpStatusCode.OK);
                });
        }

        [Given(@"two SOAP actions with the same route pattern")]
        public void GivenTwoSOAPActionsWithTheSameRoutePattern()
        {
            Logger.Log($"The Sample Soap Nancy Module should have been loaded already with 2 Hello World actions on the same route '{HelloWorldSampleRouteUrl}'");
        }

        #endregion

        #region When

        [When(@"the request is sent to the SOAP 1.1 service")]
        public void WhenTheRequestIsSentToTheSOAP11Service()
        {
            Logger.Log("About to wait for a request (that will not come)");
            var waitedForRequest = this.nancySoapAdapter.TryWaitForRequest(
                TestCaseRoutes.MySoap11Service,
                TestCaseSoapActions.MySoap11ServiceTestAction,
                5,
                TimeSpan.FromSeconds(1));
            LogAssert.IsTrue("No request has been received", waitedForRequest == null);

            Logger.Log("About to send the SOAP request");
            var content = new StringContent(this.request);
            var responseMsgTask = MyHttpClient.PostAsync($"{NancyBaseUrl}/{TestCaseRoutes.MySoap11Service}", content).ConfigureAwait(true);

            Logger.Log("About to wait for a request");
            waitedForRequest = this.nancySoapAdapter.TryWaitForRequest(
                TestCaseRoutes.MySoap11Service,
                TestCaseSoapActions.MySoap11ServiceTestAction,
                5,
                TimeSpan.FromSeconds(1));
            LogAssert.IsTrue("Request was been received", waitedForRequest != null);

            var responseMsg = responseMsgTask.GetAwaiter().GetResult();
            this.actualResponse = responseMsg.Content.ReadAsStringAsync().ConfigureAwait(true).GetAwaiter().GetResult();
            Logger.Log($"Actual response was: {actualResponse}");
            responseMsg.EnsureSuccessStatusCode();
        }

        [When(@"the route is called with the first SOAP action")]
        public void WhenTheRouteIsCalledWithTheFirstSOAPAction()
        {
            CallHelloWorldSoapAction(HelloWorldSampleAction1, HelloWorldSampleAction1ExpectedResponse);
        }

        [When(@"the route is called with the second SOAP action")]
        public void WhenTheRouteIsCalledWithTheSecondSOAPAction()
        {
            CallHelloWorldSoapAction(HelloWorldSampleAction2, HelloWorldSampleAction2ExpectedResponse);
        }

        #endregion

        #region Then

        [Then(@"the correct handler was executed")]
        [Then(@"the SOAP 1.1 service response is as expected")]
        public void ThenTheSOAP11ServiceResponseIsAsExpected()
        {
            LogAssert.AreEqual("Response", this.expectedResponse, this.actualResponse);
        }

        #endregion

        #region Private Methods

        private void CallHelloWorldSoapAction(string soapAction, string expectedResponseMessage)
        {
            Logger.Log($"About to send the SOAP request for Action: '{soapAction}' to endpoint '{HelloWorldSampleRouteUrl}'");

            this.expectedResponse = expectedResponseMessage;
            Logger.Log($"Expected response is: '{this.expectedResponse}'");

            this.request = SoapBuilder.CreateSoap11Envelope()
                .WithHeader.AddAction(soapAction)
                .Build().ToString();
            Logger.Log($"SOAP request about to be sent is: {this.request}");

            var content = new StringContent(this.request);
            var responseMsgTask = MyHttpClient.PostAsync($"{NancyBaseUrl}/{HelloWorldSampleRouteUrl}", content).ConfigureAwait(true);
            var responseMsg = responseMsgTask.GetAwaiter().GetResult();
            this.actualResponse = responseMsg.Content.ReadAsStringAsync().ConfigureAwait(true).GetAwaiter().GetResult();
            Logger.Log($"Actual response was: {actualResponse}");
            responseMsg.EnsureSuccessStatusCode();
        }

        #endregion
    }
}
