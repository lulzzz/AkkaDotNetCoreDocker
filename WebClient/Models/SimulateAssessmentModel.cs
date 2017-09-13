using System;
using System.Collections.Generic;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace WebClient.Models
{
    public class SimulateAssessmentModel
    {
        public SimulateAssessmentModel(List<InvoiceLineItem> lineItems)
        {
            LineItems = lineItems;
        }

        public List<InvoiceLineItem> LineItems { get; private set; }
    }
}

