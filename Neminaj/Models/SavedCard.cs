using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    public byte[] Image { get; set; }

    public string CardInfo { get; set; }

    public bool IsBarCode { get; set; }

    public bool IsQRCode { get; set; }
}