using Neminaj.GlobalEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Events;

public class ItemCountOf_Changed_EventArgs : EventArgs
{
    public int IndexInObservableCollection { get; private set; }
    
    public int Count { get; private set; }
    public string FinalName{ get; private set; }

    public ItemCountOf_Changed_EventArgs(int indexInObservableCollection, int count, string finalName)
    {
        IndexInObservableCollection = indexInObservableCollection;
        FinalName = finalName;
        Count = count;
    }
}

public class CompanyCheckBoxChanged_EventArgs : EventArgs
{
    public int CompanyId { get; private set; }
    public string Name { get; private set; }

    public CompanyCheckBoxChanged_EventArgs(int companyId, string name)
    {
        CompanyId = companyId;
        Name = name;
    }
}

public class ComparerMode_Changed_EventArgs : EventArgs
{
    public ComparerMode Mode { get; private set; }

    public ComparerMode_Changed_EventArgs(ComparerMode mode)
    {
        Mode = mode;
    }
}
