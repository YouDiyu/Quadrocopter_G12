using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpDX.XInput;
using vKTCQuadroControl;
using System.IO.Ports;
using System.Threading;
using KTCQuadroControl;

namespace Quadrocopter_G12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public delegate void DelString(string hexString);
    //public delegate void QcSender();
    public partial class MainWindow : Window
    {
        private Controller XBox;
        private State myState;
        private vQuadroControl vmyQC;
        DelString DelStringObject;
        //QcSender QcSenderObj; 
        Thread TH_Heartbeats;
        Thread MainwindowUp;
        Thread UAVTaklSender;
        SerialPort COMXY;
        QuadroControl myQC;
        float Row_X, Pitch_Y, Throttle_Val, Yaw_X, limit;
        string Syn_Val, MessageTyp, Length, ObjectID, InstanceID, Accessory_Val, Throttle, Roll, Pitch, Yaw, Collective, Thrust, Channel, Connected, FlightModeSwitchPosition;

        private void ArmingB_Click(object sender, RoutedEventArgs e)
        {
            if (Verbstatus)
            {
                if (!Armingstatus)
                {
                    Arm();
                }
                else
                {
                    DisconnectUarming();
                }
            }
        }

        private void VerbundenB_Click(object sender, RoutedEventArgs e)
        {
            if (!Verbstatus)
            {
                if (!Armingstatus)
                {
                    PortConnect();
                }
                else
                {
                    DisconnectUarming();
                }
            }
            else
            {
                DisconnectUarming();
            }
        }

        bool Verbstatus, Armingstatus;
        bool ThreadError;
        public MainWindow()
        {
           
            InitializeComponent();
            //vmyQC = new vQuadroControl();
            WPF_Initia();
            XBox = new Controller(UserIndex.One);
            myQC = new QuadroControl();
            DelStringObject = new DelString(SchickeHexString);
            MainwindowUp = new Thread(XboxState);
           
            MainwindowUp.Start();
           // UAVTaklSender.Start();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            if(Help1.Visibility == Visibility.Hidden|| Help2.Visibility == Visibility.Hidden)
            {
                Help1.Visibility = Visibility.Visible;
                Help2.Visibility = Visibility.Visible;
            }
            else
            {
                Help1.Visibility = Visibility.Hidden;
                Help2.Visibility = Visibility.Hidden;
            }
            
        }
        private void WPF_Initia()
        {
            Help1.Visibility = Visibility.Hidden;
            Help2.Visibility = Visibility.Hidden;
            LeftShoulder.Visibility = Visibility.Hidden;
            RightShoulder.Visibility = Visibility.Hidden;
            Other.Value = 0;
            Throttle_Pg.Value = 0;
            Canvas.SetLeft(LThumbsticks, LocationTransX(0));
            Canvas.SetTop(LThumbsticks, LocationTransY(0));
            Canvas.SetLeft(RThumbsticks, LocationTransX(0));
            Canvas.SetTop(RThumbsticks, LocationTransY(0));
            ButtonA.Visibility = Visibility.Visible;
            ButtonB.Visibility = Visibility.Visible;
            ButtonX.Visibility = Visibility.Visible;
            ButtonY.Visibility = Visibility.Visible;
            COM.Text = "COM4";
            LimitSlider.Value = 100;
            Row_X = 0;
            Pitch_Y = 0;
            Yaw_X = 0;
            Throttle_Val = 0;
            Verbstatus = false;
            Armingstatus = false;
            ThreadError = false;
        }
        private double LocationTransX(double x)
        {
            double X;
            return X = x + 40;
        }
        private double LocationTransY(double x)
        {
            double X;
            return X =- x + 40;
        }
        private void PortConnect()
        {
            
            /*COMXY = new SerialPort();
            if (COM.Text.Contains("COM"))
            {
                COMXY = new SerialPort(COM.Text);
                if (COMXY.IsOpen == true)
                {
                    this.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        Verbinden.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    MessageBox.Show("Port Error");
                }
            }

            else
            {
                MessageBox.Show("Port Name Error", "Port Name Error", MessageBoxButton.OK);
            }*/
            myQC.Verbinde("COM4");
            if (myQC.Verbunden)
            {
                Verbstatus = true;
            }
            else
            {
                Verbstatus = false;
                MessageBox.Show("Port Error");
            }
            
        }
        public void SchickeHexString(String hexString)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, DelStringObject, hexString);
            }
            else
            {
                //vmyQC.v_SchickeHexString(hexString);
                QuadroControl.SchickeHexString(hexString);
            }

        }
        private void Heartbeats()
        {
            while (true)
            {
                DelStringObject("3C 20 2F 00 0A DC D1 CA 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03");
                Thread.Sleep(1500);
            }
        }
        private void DisconnectUarming()
        {
            if (Verbstatus)
            {
                if (Armingstatus) { Disarm(); }
                else { Trennen(); }
            }
        }
        private void Arm()
        {
            DelStringObject("3C 20 2F 00 0A DC D1 CA 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            DelStringObject("3C 20 2F 00 0A DC D1 CA 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01");
            DelStringObject("3C 20 2F 00 0A DC D1 CA 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03");
            DelStringObject("3c 20 12 00 81 74 10 c4 00 00 41 00 c0 07 64 00 00 00");
            DelStringObject("3c 20 12 00 5c 98 09 c4 00 00 41 00 d0 07 64 00 00 00");
            TH_Heartbeats = new Thread(Heartbeats);
            TH_Heartbeats.Start();
            DelStringObject("3C 20 38 00 80 74 10 C4 00 00 00 00 80 BF " +
              "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " +
              "80 BF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " +
              "00 00 00 00 01 00");
            DelStringObject("3C 20 0E 00 5A 98 09 C4 00 00 00 00 80 3F");
            //Arming true??? Data Receive
            if (true)
            {
                Armingstatus = true;
            }
            else
            {
                Armingstatus = false;
            }
           
            
        }
        private void Disarm()
        {
            DelStringObject("3C 20 0E 00 5A 98 09 C4 00 00 00 00 80 BF");
            ////condition!!arming
            if (true)
            {
                Armingstatus = false;
            }

        }
        private void Trennen()
        {
            //COMXY.Close();
            myQC.VerbindungTrennen();
            TH_Heartbeats.Abort();
            if (!myQC.Verbunden)
            {
                Verbstatus = false;
            }
        }
        private void m_Sender()
        {
            Syn_Val = "3C";
            MessageTyp = "20";
            Length = "38 00";
            ObjectID = "80 74 10 C4";
            InstanceID = "00 00";
            Throttle = QuadroControl.KonvertiereFloatZuHexString(Throttle_Val);
            Roll = QuadroControl.KonvertiereFloatZuHexString(Row_X);
            Pitch = QuadroControl.KonvertiereFloatZuHexString(Pitch_Y);
            Yaw = QuadroControl.KonvertiereFloatZuHexString(Yaw_X);
            Collective = "00 00 00 00";
            Thrust = Throttle;
            Channel = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
            Connected = "01";
            FlightModeSwitchPosition = "00";
            DelStringObject(Syn_Val + " " + MessageTyp + " " + Length + " " + ObjectID + " " + InstanceID + " " + Throttle + " " + Roll + " " + Pitch + " " + Yaw + " " + Collective + " " + Thrust + " " + Channel + " " + Connected + " " + FlightModeSwitchPosition);
        } 
        private void KeyboardControll(object sender, KeyboardEventArgs e)
        {
            while (true)
            {
                
            }
        }
        /// <summary>
        /// gamepad offline -> Keyboard control
        /// </summary>
        private void XboxState()
        {
            
            try
            {
                while (true)
                {
                    if (XBox.IsConnected)
                    {
                        ThreadError = false;
                        myState = XBox.GetState();
                        Pitch_Y = myState.Gamepad.LeftThumbY / (32767 / limit);
                        if (Math.Abs(Pitch_Y) < 0.05) { Pitch_Y = 0; };
                        Row_X = myState.Gamepad.LeftThumbX / (32767 / limit);
                        if (Math.Abs(Row_X) < 0.05) { Row_X = 0; };
                        Throttle_Val = myState.Gamepad.RightTrigger / (255 / limit);
                        Yaw_X = myState.Gamepad.RightThumbX / (32767 / limit);
                        if (Math.Abs(Yaw_X) < 0.05) { Yaw_X = 0; };
                        m_Sender();

                        this.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            //Test.Text = Syn_Val + " " + MessageTyp + " " + Length + " " + ObjectID + " " + InstanceID + " " + Throttle + " " + Roll + " " + Pitch + " " + Yaw + " " + Collective + " " + Thrust + " " + Channel + " " + Connected + " " + FlightModeSwitchPosition;
                            limit = (float)LimitSlider.Value / 100;
                            String GamePadButtons = myState.Gamepad.Buttons.ToString();
                            Test.Text = GamePadButtons;
                            if(Verbstatus)
                            {
                                VerbundenB.Background = Brushes.LawnGreen;
                            }
                            else
                            {
                                VerbundenB.Background = Brushes.Red;
                            }
                            if(Armingstatus)
                            {
                                ArmingB.Background = Brushes.LawnGreen;
                            }
                            else
                            {
                                ArmingB.Background = Brushes.Red;
                            }
                            if (GamePadButtons.Contains("A"))
                            { this.ButtonA.Visibility = Visibility.Hidden; }
                            else { this.ButtonA.Visibility = Visibility.Visible; }
                            if (!GamePadButtons.Contains("Back"))
                            {
                                if (GamePadButtons.Contains("B"))
                                { this.ButtonB.Visibility = Visibility.Hidden; }
                                else { this.ButtonB.Visibility = Visibility.Visible; }
                            }
                           
                            if (GamePadButtons.Contains("X"))
                            { this.ButtonX.Visibility = Visibility.Hidden; }
                            else { this.ButtonX.Visibility = Visibility.Visible; }
                            if (GamePadButtons.Contains("Y"))
                            { this.ButtonY.Visibility = Visibility.Hidden; }
                            else { this.ButtonY.Visibility = Visibility.Visible; }

                            Canvas.SetLeft(LThumbsticks, LocationTransX((double)myState.Gamepad.LeftThumbX / 32768 * 45));
                            Canvas.SetTop(LThumbsticks, LocationTransY((double)myState.Gamepad.LeftThumbY / 32768 * 45));
                            Canvas.SetLeft(RThumbsticks, LocationTransX((double)myState.Gamepad.RightThumbX / 32768 * 45));
                            Throttle_Pg.Value = myState.Gamepad.RightTrigger / 2.55;

                            if (GamePadButtons.Contains("Back"))
                            {
                                DisconnectUarming();
                            }
                            if (GamePadButtons.Contains("LeftShoulder"))
                            {
                                if (!Verbstatus)
                                {
                                    PortConnect();
                                    Thread.Sleep(10);
                                }
                                else
                                {
                                    //MessageBox.Show("Status: Verbunden");
                                }
                                LeftShoulder.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                LeftShoulder.Visibility = Visibility.Hidden;
                            }
                            if (GamePadButtons.Contains("RightShoulder"))
                            {
                                if (!Armingstatus)
                                {
                                    Arm();
                                    Thread.Sleep(10);

                                }
                                else
                                {
                                    //MessageBox.Show("Status: Arming");
                                }
                                RightShoulder.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                RightShoulder.Visibility = Visibility.Hidden;
                            }
                            


                        });
                    }
                    else
                    {
                        if (!ThreadError)
                        {
                            MessageBox.Show("Kein Gamepad");
                        }
                        Thread.Sleep(1000);
                        ThreadError = true;
                        //switch Keyboard Modul
                    }
                    Thread.Sleep(20);
                }
            }
            catch (ThreadAbortException)
            {
                MessageBox.Show("Thread Error");
            }
        }


    }
}
