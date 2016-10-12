using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public interface IDataManager
    {
        event EventHandler OnDataReceived;

        void WriteData(object data, object target);
    }
}
