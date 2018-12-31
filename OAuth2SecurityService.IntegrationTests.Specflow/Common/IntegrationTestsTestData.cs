using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuth2SecurityService.DataTransferObjects;

namespace OAuth2SecurityService.IntegrationTests.Specflow.Common
{
    public class IntegrationTestsTestData
    {
        public static String Password = "123456";
        public static String NoPassword = String.Empty;

        public static RegisterUserRequest GetRegisterUserRequest
        {
            get
            {
                RegisterUserRequest request = new RegisterUserRequest();

                request.EmailAddress = "testemail1@testing.co.uk";
                request.PhoneNumber = "07777777777";
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
