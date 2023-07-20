using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using TableAttribute = SQLite.TableAttribute;

namespace Neminaj.Models;

[Table(nameof(Item))]
public class Item
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, NotNull, MaxLength(250)]
    public string Name { get; set; }

    [ForeignKey("Unit"), NotNull,]
    public int Unit_Id { get; set; }
}
