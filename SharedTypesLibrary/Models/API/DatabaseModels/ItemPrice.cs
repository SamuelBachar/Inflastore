using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.API.DatabaseModels;

public class ItemPrice
{
    public int Id { get; set; }

    public int Item_Id { get; set; }

    public float Price { get; set; }

    public float PriceDiscount { get; set; }

    public int Company_Id { get; set; }
}
