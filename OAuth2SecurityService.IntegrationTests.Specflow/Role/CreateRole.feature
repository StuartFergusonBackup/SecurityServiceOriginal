@role
Feature: Create Role
	In order to use the OAuth2 Security Service
	As an administrator
	I want to be able to create roles

Background: 
	Given the Security Service is running

Scenario: Create a Role
	Given The details of a role I want to create
	When I request to create the role
	Then the role should be created
	And the new role Id will be returned
