using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableAttribute = SQLite.TableAttribute;

namespace Neminaj.Models;

[Table(nameof(SavedCart))]
public class SavedCart
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, MaxLength(250)]
    public string Name { get; set; }

    [MaxLength(250)]
    public string Note { get; set; }



    [Ignore]
    public bool IsChecked { get; set; } = false;
}
