using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using OAuth2SecurityService.DataTransferObjects;

namespace OAuth2SecurityService.UnitTests
{
    public class SecurityServiceManagerTestData
    {
        public static String UserName = "00000001";
        public static String EmailAddress = "testemail1@testing.co.uk";
        public static String Password = "123456";
        public static String PhoneNumber = "07777777777";

        public static List<String> Roles = new List<String>
        {
            "Role1",
            "Role2"
        };

        public static Dictionary<String, String> Claims = new Dictionary<String, String>()
        {
            {"Claim1", "Claim1Value"},
            {"Claim2", "Claim2Value"}
        };

        public static RegisterUserRequest GetRegisterUserRequest
        {
            get
            {
                RegisterUserRequest request = new RegisterUserRequest();

                request.EmailAddress = EmailAddress;
                request.Password = Password;
                request.PhoneNumber = PhoneNumber;
                request.Roles = Roles;
                request.Claims = Claims;

                return request;
            }
        }

        public static String TestDataUserId = String.Empty;
        public static String User1Id = "04911949-321d-4a9b-af31-b259160ba94f";
        public static String User2Id = "b8937737-ec2c-488f-bd95-7d053cb1d36e";
        public static String User3Id = "da6ce793-1ed5-4116-af89-2878b075ec5a";

        public static List<IdentityUser> UserList = new List<IdentityUser>
        {
            new IdentityUser
            {
                UserName = "00000001",
                NormalizedUserName = "00000001",
                Email = "00000001@testemail.com",
                NormalizedEmail = "00000001@testemail.com",
                Id = User1Id
            },
            new IdentityUser
            {
                UserName = "00000002",
                NormalizedUserName = "00000002",
                Email = "00000002@testemail.com",
                NormalizedEmail = "00000002@testemail.com",
                Id = User2Id
            },
            new IdentityUser
            {
                UserName = "00000003",
                NormalizedUserName = "00000003",
                Email = "00000003@testemail.com",
                NormalizedEmail = "00000003@testemail.com",
                Id = User3Id
            }
        };

        public static Dictionary<String, List<String>> UserRoles = new Dictionary<String, List<String>>
        {
            {User1Id, new List<String>() {"Role1", "Role2"}},
            {User2Id, new List<String>() {"Role3"}},
            {User3Id, new List<String>() {"Role4"}},
        };

        public static Dictionary<String, List<Claim>> UserClaims = new Dictionary<String, List<Claim>>
        {
            {
                User1Id, new List<Claim>()
                {
                    new Claim("ClaimType1", "Value1")
                }
            },
            {
                User2Id, new List<Claim>()
                {
                    new Claim("ClaimType1", "Value2")
                }
            },
            {
                User3Id, new List<Claim>()
                {
                    new Claim("ClaimType1", "Value3")
                }
            },
        };

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

    
