using Shared.Data.EventArguments;
using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public interface IDataManager
    {
        event EventHandler<MessageEventArgs> OnDataReceived;

        void WriteData(Message data, object target);
    }
}
