namespace SecurityService.IntegrationTests.Common
{
    using System;
    using System.Collections.Generic;
    using DataTransferObjects;

    public class IntegrationTestsTestData
    {
        public static String Password = "123456";
        public static String NoPassword = String.Empty;

        public static RegisterUserRequest GetRegisterUserRequest
        {
            get
            {
                RegisterUserRequest request = new RegisterUserRequest();

                request.GivenName = "test";
                request.FamilyName = "user";
                request.EmailAddress = "testemail1@testing.co.uk";
                request.PhoneNumber = "07777777777";
                request.Roles = new List<String>
                {
                    "Club Administrator"
                };
                request.Claims = new Dictionary<String, String>()
                {
                    {"Claim1", "Claim1Value"},
                    {"Claim2", "Claim2Value"}
                };

                return request;
            }
        }

        public static CreateRoleRequest GetCreateRoleRequest
        {
            get
            {
                CreateRoleRequest request = new CreateRoleRequest();

                request.RoleName = "Test Role";

                return request;
            }
        }
    }
}
