using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableAttribute = SQLite.TableAttribute;

namespace Neminaj.Models;


[Table(nameof(Company))]
public class Company
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, MaxLength(250)]
    public string Name { get; set; }

    [Unique]
    public byte[] Image { get; set; }
}