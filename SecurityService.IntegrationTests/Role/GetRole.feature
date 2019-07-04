@role
Feature: Get Role
	In order to use the OAuth2 Security Service
	As an administrator
	I want to be able to retrieve roles

Background: 
	Given the Security Service is running

Scenario: Get a Role By Name
	Given A role with then name 'TestRole' has been created
	When I request to retrieve the role 'TestRole'
	Then the role details should be returned
