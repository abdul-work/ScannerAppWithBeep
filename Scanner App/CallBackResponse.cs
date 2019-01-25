using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner_App
{
    public enum WHOAMI
    {
        SCANNER,
        TRIGGER
    }

    public interface CallBackResponse
    {
        void OnDone(WHOAMI whoami);
    }
}
