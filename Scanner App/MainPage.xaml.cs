using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GpioSdk;
using Windows.UI.Core;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Scanner_App
{
    public sealed partial class MainPage : Page, CallBackResponse, GpioResponseListener
    {
        bool trigger = false;
        Scanner sc = new Scanner();  //Scanner Object to carry out all scanner operations
        GpioManager manager;
        DispatcherTimer dispatcherTimer; //Timer to manage timeout time

        bool beepFlag = false; //It will be set false when there is no need to play beep and vice versa
        public MainPage()
        {
            this.InitializeComponent();
            //Intialising Scanner 
            sc.initScanner(); 

            //Setting this class as a reponse class to Scanner class
            sc.setResponsClass(this);

            //Setting timer
            DispatcherTimerSetup();
        }

        private void Scan_Button_Click(object sender, RoutedEventArgs e)
        {
            txt_ScannedData.Text = "";
            beepFlag = true;
            {
                txt_ScannedData.Focus(FocusState.Pointer);
                {
                    if (trigger == false)
                    {
                        manager.TriggerOn();
                        trigger = true;
                        dispatcherTimer.Start();
                    }
                    else { txt_ScannedData.Text = "trigger disabled"; }
                }
            }
        }
        public async void OnDone(WHOAMI whoami)
        {

            if (whoami == WHOAMI.SCANNER)
            {
                manager = GpioManager.Instance;
                manager.setGpioResponseListener(this);
                manager.TriggerOff();

                trigger = false;

                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // This now works, because it's running on the UI thread:
                    // txt_Search.Text = res.ToString();
                    //lst_Category.Visibility = Visibility.Visible;
                    pr_scan.Visibility = Visibility.Collapsed;
                    pr_scan.IsActive = false;
                    Info_Text.Visibility = Visibility.Collapsed;
                    txt_ScannedData.Visibility = Visibility.Visible; ;
                    btn_Scan.Visibility = Visibility.Visible;
                    //Scan_btn.IsEnabled = true;
                    //btn_Checkout.IsEnabled = true;
                    //btn_CartView.IsEnabled = true;
                    //btn_SearchItem.IsEnabled = true;
                });
            }
        }
        public async void GpioResponse(bool res)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // This now works, because it's running on the UI thread:
                //txt_ScannedData.Text = res.ToString();
            });


        }
        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);

            dispatcherTimer.Start();
            //txt_ScannedData.Text = "timer setup";


        }
        void dispatcherTimer_Tick(object sender, object e)
        {

            dispatcherTimer.Stop();


            if (trigger == true)
            {
                //txt_ScannedData.Text = "timer expired";
                manager.TriggerOff();
                trigger = false;
            }

        }

        private void txt_ScannedData_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NeedToPlayBeep() && beepFlag)
            {
                PlayBeep();
                beepFlag = false;
            }
            if (trigger == true && txt_ScannedData.Text.Length > 1)
            {
                trigger = false;
                manager.TriggerOff();
                dispatcherTimer.Stop();
            }
        }

        private bool NeedToPlayBeep()
        {
            if (txt_ScannedData.Text.Length > 1)
            {
                if (txt_ScannedData.Text != "trigger disabled")
                {
                    return true;
                }
            }
            return false;
        }

        private async void PlayBeep()
        {
            MediaElement myBeep = new MediaElement();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("beep.wav");
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            myBeep.SetSource(stream, file.ContentType);
            myBeep.Play();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txt_ScannedData.Visibility = Visibility.Collapsed;
            btn_Scan.Visibility = Visibility.Collapsed;
            pr_scan.Visibility = Visibility.Visible;
            pr_scan.IsActive = true;
            Info_Text.Visibility = Visibility.Visible;
        }
    }
}
