@gettoken
Feature: GetClientToken
	In order to use the OAuth2 Security Service
	As a user
	I want to be able to request a Client Token

Background: 
	Given the Security Service is running

Scenario: Get Client Token
	Given I have the Client Id 'golfhandicap.testdatagenerator'
	And the secret 'golfhandicap.testdatagenerator'
	When I request a client token
	Then my client token request is successful
	And a client token is returned to me
