using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ColumnAttribute = SQLite.ColumnAttribute;
using TableAttribute = SQLite.TableAttribute;

namespace Neminaj.Models;

[Table(nameof(SavedCartItem))]
public class SavedCartItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ForeignKey(nameof(SavedCart))]
    public int SavedCart_Id { get; set; }

    [ForeignKey(nameof(Item))]
    public int Item_Id { get; set; }

    [Column(nameof(CntOfItem))]
    public int CntOfItem { get; set; }
}
