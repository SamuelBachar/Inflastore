using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.API.DatabaseModels;

public class ClubCard
{
    public int Id { get; set; }

    public int Company_Id { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }
}
