
using System;
namespace Loaner.api.Models
{
    public class SimulateBoardingOfAccountModel
    {
        public SimulateBoardingOfAccountModel(string clientName, string clientAccountsFilePath, string obligationsFilePath)
        {
            ClientAccountsFilePath = clientAccountsFilePath;
            ClientName = clientName;
            ObligationsFilePath = obligationsFilePath;
        }
        public string ClientName { get; private set; }
        public string ClientAccountsFilePath { get; private set; }
        public string ObligationsFilePath { get; private set; }

    }
}
