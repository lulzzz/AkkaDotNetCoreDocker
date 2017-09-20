using System;
using System.Collections.Generic;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.api.Models
{
    public class SimulateAssessmentModel
    {
        public SimulateAssessmentModel()
        {
            LineItems = new List<InvoiceLineItem>();
        }
        public SimulateAssessmentModel(List<InvoiceLineItem> lineItems)
        {
            LineItems = lineItems ?? new List<InvoiceLineItem>();
        }

        public List<InvoiceLineItem> LineItems { get; set; }

    }
}

