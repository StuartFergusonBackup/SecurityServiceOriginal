@user
Feature: Register User
	In order to use the OAuth2 Security Service
	As a user
	I want to be able to register my details

Background: 
	Given the Security Service is running

@mytag
Scenario: Register with a password
	Given I have my user details to register
	And I have provided a password
	When I register
	Then my details should be registered
	And my new User Id will be returned
