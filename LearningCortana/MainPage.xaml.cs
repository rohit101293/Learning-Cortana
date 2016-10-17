using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.SpeechRecognition;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LearningCortana
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        String mystring;
        SpeechSynthesizer speech;
        MediaElement mediael;
        public MainPage()
        {
            this.InitializeComponent();
            speech = new SpeechSynthesizer();
            mediael = new MediaElement();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            mystring = texttospeak.Text.ToString();
            ReadText(mediael, texttospeak.Text.ToString());
        }

        private async void ReadText(MediaElement mediael, string v)
        {
            SpeechSynthesisStream stream = await speech.SynthesizeTextToStreamAsync(v);
            mediael.SetSource(stream, stream.ContentType);
            mediael.Play();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SpeechRecognitionResult speechrecognition = e.Parameter as SpeechRecognitionResult;

            if (speechrecognition !=null)
            {
                string recordtext = speechrecognition.Text;
                string voicecommandname = speechrecognition.RulePath.First();
                switch(voicecommandname)
                {
                    case "voicecommand": string newtxt = this.SemanticInterpretation("name", speechrecognition);
                        if (newtxt != null)
                            recordtext = newtxt;
                        ReadText(new MediaElement(), recordtext);
                        break;
                    default:break;
                }
            }

        }

        private string SemanticInterpretation(string v, SpeechRecognitionResult speechrecognition)
        {
            return speechrecognition.SemanticInterpretation.Properties[v].FirstOrDefault();
        }
    }
}
