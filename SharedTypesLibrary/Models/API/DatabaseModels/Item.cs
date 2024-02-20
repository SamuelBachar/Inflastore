using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.API.DatabaseModels;

public class Item
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string ImageUrl { get; set; }

    public string Path { get; set; }

    public int Unit_Id { get; set; }

    public int Category_Id { get; set; }
}
