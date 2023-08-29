using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.API.DatabaseModels;

public class NavigationShopData
{
    public int Id { get; set; }

    public int Company_Id { get; set; }

    public string FullAddress { get; set; }

    public float Latitude { get; set; }

    public float Longtitude { get; set; }
}
