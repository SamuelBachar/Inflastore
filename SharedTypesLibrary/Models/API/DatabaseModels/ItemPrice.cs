using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.API.DatabaseModels;

public class ItemPrice
{
    public int Id { get; set; }

    public List<Company> Company_ { get; set; }

    public List<Item> Item_ { get; set; }

    public float Price { get; set; }
    public float PriceDiscount { get; set; }
}
