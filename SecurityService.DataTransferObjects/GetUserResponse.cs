using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityService.DataTransferObjects
{
    public class GetUserResponse
    {
        public Guid UserId { get; set; }
        public String UserName { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public List<String> Roles { get; set; }
        public Dictionary<String,String> Claims { get; set; }
    }
}
