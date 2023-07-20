using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableAttribute = SQLite.TableAttribute;

namespace Neminaj.Models;

[Table(nameof(NavigationShopData))]
public class NavigationShopData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ForeignKey(nameof(Company_Id)), NotNull]
    public int Company_Id { get; set; }

    [NotNull]
    public string FullAddress { get; set; }

    [NotNull]
    public float Latitude { get; set; }

    [NotNull]
    public float Longtitude { get; set; }
}
