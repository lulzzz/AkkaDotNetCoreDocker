using System;
using System.Collections.Generic;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.api.Models
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

