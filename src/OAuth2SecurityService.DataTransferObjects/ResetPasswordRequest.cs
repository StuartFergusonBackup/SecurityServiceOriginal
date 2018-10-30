using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityService.DataTransferObjects
{
    public class ResetPasswordRequest
    {
        public String EmailAddress { get; set; }
        public String PasswordResetCode { get; set; }
        public String NewPassword { get; set; }

    }
}
