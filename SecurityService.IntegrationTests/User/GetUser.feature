@user
Feature: Get User
	In order to use the Security Service
	As a user
	I want to be able to get a users my details

Background: 
	Given the Security Service is running
	And I have my user details to register
	And I have provided a password
	When I register
	Then my details should be registered
	And my new User Id will be returned

Scenario: Get User By Id
	Given I have the Id of a registered user
	When I get the users details by Id
	Then the user details will be returned
