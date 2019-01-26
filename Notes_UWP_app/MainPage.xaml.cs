using System;
using System.Text;
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
using System.Windows;
using Windows.UI.Popups;
using Windows.Media.Capture;
using Windows.Storage;
using System.Diagnostics;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.Media.SpeechSynthesis;
using System.Text.RegularExpressions;
using Windows.Globalization.DateTimeFormatting;
using Windows.Globalization;
using Windows.ApplicationModel.Appointments;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409


namespace Notes_UWP_app
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static int clicks = 0;
        TextBox objTextBox = new TextBox();

        MediaElement mediaElement = new MediaElement();
        SpeechSynthesisStream stream;

        //gia na exei to region mou , to xrisimopoiousa gia tin wra 
        GeographicRegion userRegion = new GeographicRegion();
        SpeechSynthesizer synth = new SpeechSynthesizer();


        //callendar


        // Specify which values to retrieve
        public FindAppointmentsOptions options1 = new FindAppointmentsOptions();



        public bool isChecked = false;
        public bool isTimeQuestion = false;
        public bool isDateQuestion = false;
        public bool isDayQuestion = false;

        const string startingText = "Χαίρε αφέντη, τι μπορώ να κάνω για εσάς;";
        const string questionForDaysAppointments = "Πόσες μέρες να ελέγξω για ραντεβού αφέντη;";
        const string goodbyeText = "Αντίο αφέντη, είστε ο καλύτερος!";
        public MainPage()
        {
            this.InitializeComponent();

        }


        private async void Starting_Function(object sender, RoutedEventArgs e)
        {
            objTextBox.Text = startingText;
            stream = await synth.SynthesizeTextToStreamAsync(objTextBox.Text);


            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        private  async Task CalendarCall(object sender, RoutedEventArgs e)
        {
            AppointmentStore calendar = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
            options1.FetchProperties.Add(AppointmentProperties.Subject);
            options1.FetchProperties.Add(AppointmentProperties.Details);
            options1.FetchProperties.Add(AppointmentProperties.DetailsKind);

            int DaysToCheckForAppointments;

            objTextBox.Text = questionForDaysAppointments;
            await  Speak( sender,  e);
            Debug.WriteLine("endiamesooooooooooooooooooooooooooo \n");
            await Task.Delay(TimeSpan.FromSeconds(3));
            await SpeakToComputer(sender, e);

            Debug.WriteLine("I said " + objTextBox.Text);
            if (!(Int32.TryParse(objTextBox.Text.Split(' ').First(), out DaysToCheckForAppointments)))
            {
                Debug.WriteLine(DaysToCheckForAppointments);
                DaysToCheckForAppointments = 1;
            }

            var iteratingAppointments = await calendar.FindAppointmentsAsync(DateTimeOffset.Now, TimeSpan.FromDays(DaysToCheckForAppointments), options1);

            if (!iteratingAppointments.Any())
            {
                objTextBox.Text = "Δεν υπάρχουν ραντεβού για τις επόμενες " + DaysToCheckForAppointments +" μέρες.";

                return;
            }
            foreach (var i in iteratingAppointments)
            {

                var eventName = i.Subject;
                var eventDay = i.StartTime.DayOfWeek;
                var eventPlace = i.Location;
                var timeFormatter = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("hour minute");
                var eventTime = timeFormatter.Format(i.StartTime.DateTime);

                objTextBox.Text = "Έχεις "+eventName+ " στις " + eventDay + " την "+ eventPlace + " την "+ eventTime;
                if (i == iteratingAppointments.ElementAt(iteratingAppointments.Count() - 1))
                {
                    return;
                }
                stream = await synth.SynthesizeTextToStreamAsync(objTextBox.Text);


                mediaElement.SetSource(stream, stream.ContentType);
                mediaElement.Play();
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            return;

        }


        private async void OnClick(object sender, RoutedEventArgs e)
        {

            //var messageShow = new MessageDialog("Num Of Clicks is :" + clicks);
            //await messageShow.ShowAsync();
            clicks++;



            //synth.Voice = SpeechSynthesizer.AllVoices.First(x => x.Gender == VoiceGender.Female);






            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            objTextBox.Text = regex.Replace(objTextBox.Text, " ");




            foreach (var x in objTextBox.Text.Split(' '))
            {
                if (x == "time")
                {
                    isTimeQuestion = true;
                }
            }

            foreach (var x in objTextBox.Text.Split(' '))
            {
                if (x == "date")
                {
                    isDateQuestion = true;
                }
            }

            foreach (var x in objTextBox.Text.Split(' '))
            {
                if (x == "day")
                {
                    isDayQuestion = true;
                }
            }

            if (isChecked)
            {
                switch (objTextBox.Text)
                {
                    
                    case ("open calendar"):

                        try
                        {
                            await CalendarCall(sender, e);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        Debug.WriteLine("calendar called" + objTextBox.Text);
                        break;
                    case ("good morning"):

                        objTextBox.Text = "Καλημέρα αφέντη";
                        break;
                    case ("how are you"):
                        objTextBox.Text = "Είμαι μια χαρά, εσείς πώς είστε αφέντη;";
                        break;
                    case ("hello"):
                        objTextBox.Text = "Χαίρε αφέντη, πώς είστε;";
                        break;
                    case ("bonjour"):

                        objTextBox.Text = "bonjour, commond ça va";
                        break;
                    case ("travis"):
                        objTextBox.Text = "Παρακαλώ αφέντη.";
                        break;
                    case ("lights"):
                        Light_Controll(sender, e);
                        break;
                    case ("goodbye travis"):
                        objTextBox.Text = goodbyeText;
                        
                        await Speak(sender, e);
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        
                        App.Current.Exit();
                        break;

                    default:
                        if (isTimeQuestion & isDateQuestion)
                        {
                            objTextBox.Text = "Η ημέρα και η ώρα είναι: " + System.DateTime.Now;
                            isDateQuestion = false;
                            isTimeQuestion = false;
                        }
                        else if (isTimeQuestion)
                        {

                            objTextBox.Text = "Η ώρα είναι: " + DateTime.Now.ToString("h:mm:ss");
                            isTimeQuestion = false;
                        }
                        else if (isDateQuestion)
                        {

                            objTextBox.Text = "" + DateTime.Today;
                            objTextBox.Text = "Η ημερομηνία είναι " + objTextBox.Text.Split().First();
                            isDateQuestion = false;
                        }
                        else if (isDayQuestion)
                        {
                            objTextBox.Text = "" + DateTime.Today.DayOfWeek;
                            objTextBox.Text = "Σημέρα είναι " + objTextBox.Text;
                            isDayQuestion = false;

                        }
                        else
                        {
                            Debug.WriteLine("gonna say my master");

                            var s = objTextBox.Text.Split(' ').First();
                            objTextBox.Text = s + " αφέντη";
                        }
                        break;

                }

            }


            Task Task1 = Speak(sender, e);
            await Task1;
        }
        /*private async void OnTextChanging(object sender, TextBoxTextChangingEventArgs e)
        {
            var synth = new SpeechSynthesizer();
            var textboxObj = (TextBox)sender;
            Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(textboxObj.Text);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();



        }*/

        private async Task SpeakToComputer(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("HEELEMOQHNOQOQWGWQGI\n");
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            // Compile the dictation grammar by default.
            await speechRecognizer.CompileConstraintsAsync();

            // Start recognition.
            Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeAsync();

            objTextBox.Text = speechRecognitionResult.Text;
        }
        private async Task SpeakToMachine(object sender, RoutedEventArgs e)
        {
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            // Compile the dictation grammar by default.
            await speechRecognizer.CompileConstraintsAsync();

            // Start recognition.
            Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeAsync();

            objTextBox.Text = speechRecognitionResult.Text;
            OnClick(sender, e);
        }

        private async Task  Speak(object sender, RoutedEventArgs e)
        {
            stream = await synth.SynthesizeTextToStreamAsync(objTextBox.Text);


            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
            return ;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            objTextBox = (TextBox)sender;

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isChecked = true;
        }

        private void CheckBox_NotChecked(object sender, RoutedEventArgs e)
        {
            isChecked = false;
        }

    
    private async void Speech_Recognition(object sender, RoutedEventArgs e)
    {
            // Create an instance of SpeechRecognizer.
           await  SpeakToMachine(sender,e);


    }


        private async void Light_Controll(object sender, RoutedEventArgs e)
        {
            objTextBox.Text = "Θέλετε να ανοίξετε ή να κλείσετε τα φώτα;";
            await Speak(sender,e);
            await Task.Delay(TimeSpan.FromSeconds(2));
            await SpeakToComputer(sender, e);

            //      Create a socket and send udp message
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
            ProtocolType.Udp);

            IPAddress serverAddr = IPAddress.Parse("192.168.2.20");

            IPEndPoint endPoint = new IPEndPoint(serverAddr, 5005);

            Debug.WriteLine(endPoint.ToString());

            string selector = SerialDevice.GetDeviceSelector("COM5");
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);
            Debug.WriteLine(devices);
            if (!devices.Any()) return;
            string deviceId = devices.First().Id;
            SerialDevice serialDevice = await SerialDevice.FromIdAsync(deviceId);
            serialDevice.BaudRate = 9600;
            serialDevice.DataBits = 8;
            serialDevice.StopBits = SerialStopBitCount.Two;
            serialDevice.Parity = SerialParity.None;
            serialDevice.Handshake = SerialHandshake.None;
            DataWriter dataWriter = new DataWriter(serialDevice.OutputStream);

            Debug.WriteLine(objTextBox.Text);
            switch (objTextBox.Text)
            {
                case ("open"):
                    string text = "1";
                    Debug.WriteLine("sending " + text);
                    byte[] send_buffer = Encoding.ASCII.GetBytes(text);
                    

                    
                    sock.SendTo(send_buffer, endPoint);
                    break;


                case ("open USB"):
                    dataWriter.WriteString("1");
                    await dataWriter.StoreAsync();
                    dataWriter.DetachStream();
                    dataWriter = null;
                   
                    break;

                case ("close USB"):
                    dataWriter.WriteString("0");
                    await dataWriter.StoreAsync();
                    dataWriter.DetachStream();
                    dataWriter = null;

                    break;

                    

                case ("close"):
                    text = "0";
                    send_buffer = Encoding.ASCII.GetBytes(text);
                    Debug.WriteLine("sending " + text);
                    sock.SendTo(send_buffer, endPoint);
          
                    break;

                default:
                    text = "1";
                    send_buffer = Encoding.ASCII.GetBytes(text);
                    Debug.WriteLine("sending " + text);
                   // sock.SendTo(send_buffer, endPoint);
                    await Task.Delay(TimeSpan.FromSeconds(1f));
                    text = "0";
                    send_buffer = Encoding.ASCII.GetBytes(text);
                    Debug.WriteLine("sending " + text);
                   // sock.SendTo(send_buffer, endPoint);
                    await Task.Delay(TimeSpan.FromSeconds(1f));
                    text = "1";
                    send_buffer = Encoding.ASCII.GetBytes(text);
                    Debug.WriteLine("sending " + text);
                   // sock.SendTo(send_buffer, endPoint);
                    await Task.Delay(TimeSpan.FromSeconds(1f));
                    text = "0";
                    send_buffer = Encoding.ASCII.GetBytes(text);
                    Debug.WriteLine("sending " + text);
                   // sock.SendTo(send_buffer, endPoint);
                    break;
            }
        }





    }

  

   
}
