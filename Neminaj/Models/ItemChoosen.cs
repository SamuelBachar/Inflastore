using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Models;

public class ItemChoosen
{
    // For PriceComparer
    public int Id { get; set; }
    public int IdInList { get; set; }
    public string Name { get; set; }

    public string FinalName { get; set; }
    public int CntOfItems { get; set; }

    public string UnitTag { get; set; }

    public string Price1 { get; set; }
    public string PriceDiscount1 { get; set; }
    public string Price2 { get; set; }
    public string PriceDiscount2 { get; set; }
    public string Price3 { get; set; }
    public string PriceDiscount3 { get; set; }

    // For CartView
    public string ItemImgUrl { get; set; }
    public int Company_Id { get; set; }
    public string CompanyImgUrl { get; set; }
    public string PriceCartOrig { get; set; }
    public string PriceDiscountOrig { get; set; }
    public string PriceCartCalc { get; set; }
    public string PriceDiscountCalc { get; set; }
}
