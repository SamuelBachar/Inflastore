using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableAttribute = SQLite.TableAttribute;

namespace Neminaj.Models;

[Table(nameof(Unit))]
public class Unit
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, NotNull, MaxLength(250)]
    public string Name { get; set; }

    [Unique, NotNull, MaxLength(250)]
    public string Tag { get; set; }
}
