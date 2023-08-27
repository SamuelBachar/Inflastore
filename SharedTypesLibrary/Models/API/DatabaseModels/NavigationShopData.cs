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

    public List<Company> Company_ { get; set; }

    public List<Company> Region_ { get; set; }

    public string FullAddress { get; set; }

    public float Latitude { get; set; }

    public float Longtitude { get; set; }
}
