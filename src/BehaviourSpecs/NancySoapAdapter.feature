Feature: NancySoapAdapter

Scenario: Should correctly host a Nancy service in-process
Given a valid request for the SOAP 1.1 service
And the SOAP 1.1 service will respond with a successful payload
When the request is sent to the SOAP 1.1 service
Then the SOAP 1.1 service response is as expected

Scenario: Should correctly process different SOAP Actions on the same route - executes the first action
Given two SOAP actions with the same route pattern
When the route is called with the first SOAP action
Then the correct handler was executed

Scenario: Should correctly process different SOAP Actions on the same route - executes the second action
Given two SOAP actions with the same route pattern
When the route is called with the second SOAP action
Then the correct handler was executed