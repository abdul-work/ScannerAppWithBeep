using GpioSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner_App
{
    // Scanner on/off - Reponse to MainPage
    class Scanner : GpioSdk.GpioResponseListener
    {

        bool isOn = false;
        CallBackResponse responseClass;

        public void setResponsClass(CallBackResponse who)
        {
            responseClass = who;
        }

        public void initScanner()
        {
            scannerOff();
            isOn = false;
        }

        public void scannerOn()
        {
            GpioManager manager = GpioManager.Instance;
            manager.setGpioResponseListener(this);
            manager.ScannerOn();
        }

        public void scannerOff()
        {
            GpioManager manager1 = GpioManager.Instance;
            manager1.setGpioResponseListener(this);
            manager1.ScannerOff();
        }

        public async void GpioResponse(bool res)
        {
            if (res)
            {
                if (!isOn)
                {
                    scannerOn();

                    isOn = true;
                }

                else
                {
                    responseClass.OnDone(WHOAMI.SCANNER);
                }
            }

        }
    }
}
