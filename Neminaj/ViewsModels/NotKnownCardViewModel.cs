using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

public class ResultNotKnownCard
{
    public Color Color { get; set; } = Colors.Orange;
    public string CardName { get; set; } = string.Empty;
}

[QueryProperty(nameof(ResultNotKnownCard), nameof(ResultNotKnownCard))]
public partial class NotKnownCardViewModel : ObservableObject
{
    [ObservableProperty]
    private ResultNotKnownCard resultNotKnownCard;
}
