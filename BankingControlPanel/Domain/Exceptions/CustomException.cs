using System.Collections.Generic;

namespace BankingControlPanel.Domain.Exceptions
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException() : base("Email Is Already Existing") { }
    }
    public class MobileNumberAlreadyExistsException : Exception
    {
        public MobileNumberAlreadyExistsException() : base("Mobile Number Is Already Existing") { }
    }
    public class PersonalIdAlreadyExistsException : Exception
    {
        public PersonalIdAlreadyExistsException() : base("PersonalId Is Already Existing") { }
    }
    public class BankAccountAlreadyExistsException : Exception
    {
        public BankAccountAlreadyExistsException() : base("Bank Account Is Already Existing") { }
    }
    public class RequiredAtLeastOneBankAccountException : Exception
    {
        public RequiredAtLeastOneBankAccountException() : base("At Least One Account Is Required") { }
    }

}
