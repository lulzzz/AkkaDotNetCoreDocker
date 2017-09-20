namespace Loaner.BoundedContexts.MaintenanceBilling.Models
{
    public class InvoiceLineItem
    {
        public InvoiceLineItem()
        {
            Item = FinancialConcept.Dues;
        }

        public InvoiceLineItem(FinancialConcept item, int units, double unitAmount, double totalAmount)
        {
            Item = item;
            Units = units;
            UnitAmount = unitAmount;
            TotalAmount = totalAmount;
        }

        public FinancialConcept Item { get; set; }
        public int Units { get; set; }
        public double UnitAmount { get; set; }
        public double TotalAmount { get; set; }
    }
}