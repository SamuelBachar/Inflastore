using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.API;

public class ClubCardDTO
{
    public int Id { get; set; }

    public int Company_Id { get; set; }

    public string Name { get; set; }

    public byte[] Image { get; set; }

    public string Url { get; set; }
}
