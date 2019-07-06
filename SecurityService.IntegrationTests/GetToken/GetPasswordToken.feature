@gettoken
Feature: GetPasswordToken
	In order to use the OAuth2 Security Service
	As a user
	I want to be able to request a Password Token

Background: 
	Given the Security Service is running
	And I have a user registered with the following details
	| UserName                      | Password | GivenName | FamilyName |
	| clubadministrator1@test.co.uk | 123456   | Admin     | User       |

Scenario: Get Password Token
	Given I have the Client Id 'developerClient'
	And the secret 'developerClient'
	And the username 'clubadministrator1@test.co.uk'
	And the password '123456'
	When I request a password token
	Then my password token request is successful
	And a password token is returned to me

