using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableAttribute = SQLite.TableAttribute;

namespace Neminaj.Models;


[Table(nameof(ItemPrice))]
public class ItemPrice
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // todo neskor doriesit unique vramci dvoch stlpcov Company_Id a Item_Id

    [ForeignKey("Company")]
    public int Company_Id { get; set; }

    [ForeignKey("Item")]
    public int Item_Id { get; set; }

    [NotNull]
    public float Price { get; set; }
    [NotNull]
    public float PriceDiscount { get; set; }
}
