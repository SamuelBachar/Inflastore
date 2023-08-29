using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.API.DatabaseModels;

public class Region
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Company_Id { get; set; }
}
