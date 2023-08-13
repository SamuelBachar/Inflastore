using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableAttribute = SQLite.TableAttribute;

namespace Neminaj.Models;

[Table(nameof(SavedCard))]
public class SavedCard
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public byte[] Image { get; set; }

    [NotNull]
    public string CardCode { get; set; }

    [NotNull]
    public int CardFormat { get; set; }

    public bool IsKnownCard { get; set; }

    public string UnknownCardName { get; set; }

    public int UknownCardColor { get; set; }
}