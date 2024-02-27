using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App.Application.Handlers.Profit.CalculatProfit
{
    public class PCS : IPCS
    {

        public PCS()
        {

        }

        public List<InvoicesData> calculateItemsProfite(ProfiteRequest parameter)
        {

            double cost = 0.0;
            double QTY = 0.0;
            double TotalPrice = 0.0;
            List<InvoicesData> InvoicesNotUpdated = new List<InvoicesData>();
            double lastPurchasesInvoiceSerialize = parameter.Invoices.Where(a => Lists.InvoicesTypeAffectToCost.Contains(a.InvoiceType)).Select(h => h.Serialize).FirstOrDefault();
            //فى حاله لو الصنف ليس له فواتير ادخال كميات
            if (lastPurchasesInvoiceSerialize == 0 && parameter.lastDataDTO.QTyOfPurchase <= 0)
            {
                parameter.Invoices.Select(h => h.Cost = parameter.ItemDataDTO.Where(a => a.ConversionFactor == h.factor).Select(q => q.PurchasePrice).FirstOrDefault()).ToList();
                parameter.Invoices.Select(h => h.AvgPrice = parameter.ItemDataDTO.Where(a => a.ConversionFactor == h.factor).Select(q => q.PurchasePrice).FirstOrDefault()).ToList();
                return parameter.Invoices;
            }

            lastInvoiceEditedDTO lastdata = parameter.lastDataDTO;
            foreach (var invoice in parameter.Invoices)
            {

                TotalPrice = invoice.PriceWithVate ? invoice.Price / (100 + invoice.VatRatio) * 100 : invoice.Price;

                if (Lists.InvoicesTypeAffectToCost.Contains(invoice.InvoiceType))
                {
                    if (lastdata.LastQTY <= 0)
                        lastdata.LastQTY = 0;

                    cost = ((lastdata.LastQTY * lastdata.Cost) + (invoice.QTY * TotalPrice)) /
                        ((lastdata.LastQTY + invoice.QTY) == 0 ? 1 : (lastdata.LastQTY + (invoice.QTY * invoice.factor)));

                    QTY = (lastdata.LastQTY + (invoice.QTY * invoice.factor));

                   

                }
                else if (Lists.InvoicesTypeNotAffectToCost.Contains(invoice.InvoiceType))
                {
                    if (lastdata.QTyOfPurchase <= 0 && invoice.Serialize < lastPurchasesInvoiceSerialize)
                    {
                        InvoicesNotUpdated.Add(invoice);
                        continue;
                    }

                    cost = lastdata.Cost;
                    QTY = lastdata.LastQTY + invoice.QTY;
                }

                lastdata.Cost = cost;
                if (cost == 0)
                {
                    cost = parameter.ItemDataDTO.Where(a => a.ConversionFactor == invoice.factor).Select(q => q.PurchasePrice).FirstOrDefault();
                }
                lastdata.LastQTY = QTY;
                invoice.Cost = cost;

            }
            //فى حاله لو الصنف ملوش فاتوره مشتريات  فى الاول باخد اول فاتوره تقابلنى احسب عليها اللى فات 
            if (InvoicesNotUpdated.Count > 0)
            {
                double costOfFirstPurchase = parameter.Invoices.Where(h => h.Serialize == lastPurchasesInvoiceSerialize).Select(a => a.Cost).FirstOrDefault();
                parameter.Invoices.Where(a => a.Serialize < lastPurchasesInvoiceSerialize).Select(h => h.Cost = costOfFirstPurchase).ToList();
            }
            return parameter.Invoices;
            //throw new NotImplementedException();
        }
    }
}
