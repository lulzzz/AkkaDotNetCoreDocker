namespace Loaner.BoundedContexts.MaintenanceBilling.Models
{
    public class InvoiceLineItem
    {
        public InvoiceLineItem(FinancialConcept item, int units, double unitAmount, double totalAmount){
            Item = item;
            Units = units;
            UnitAmount = unitAmount;
            TotalAmount = totalAmount;
        }
        public FinancialConcept Item { get; private set; }
        public int Units { get; private set; }
        public double UnitAmount { get; private set; }
        public double TotalAmount { get; private set; }
    }
}