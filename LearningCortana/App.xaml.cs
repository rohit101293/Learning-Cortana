using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace LearningCortana
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        SpeechSynthesizer speech;
        Type navigationTo;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            speech = new SpeechSynthesizer();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
            try
            {
                StorageFile vcdStorageFile = await Package.Current.InstalledLocation.GetFileAsync(@"vcd.xml");
                await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
            }

        }
         protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            if (args.Kind == ActivationKind.VoiceCommand)
            {
                VoiceCommandActivatedEventArgs voiceCommandArgs = args as VoiceCommandActivatedEventArgs;
                Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = voiceCommandArgs.Result;
                String text = speechRecognitionResult.Text;
                string voiceCommandName = speechRecognitionResult.RulePath[0];
                switch(voiceCommandName)
                {
                    case "voicecommand":
                        string newtxt = this.SemanticInterpretation("name", speechRecognitionResult);
                        if(newtxt!=null)
                        text = newtxt;
                        ReadText(new MediaElement(), text);
                        Frame rootFrame = Window.Current.Content as Frame;

                        // Do not repeat app initialization when the Window already has content,
                        // just ensure that the window is active
                        if (rootFrame == null)
                        {
                            // Create a Frame to act as the navigation context and navigate to the first page
                            rootFrame = new Frame();


                            rootFrame.NavigationFailed += OnNavigationFailed;
                            rootFrame.Navigate(typeof(MainPage), speechRecognitionResult);
                            // Place the frame in the current Window
                            Window.Current.Content = rootFrame;
                        }
                        else
                        {
                            
                            Window.Current.Content = rootFrame;
                        }

                        // Since we're expecting to always show a details page, navigate even if 
                        // a content frame is in place (unlike OnLaunched).
                        // Navigate to either the main trip list page, or if a valid voice command
                        // was provided, to the details page for that trip.


                        // Ensure the current window is active
                        Window.Current.Activate();
                        break;
                    default:
                        ReadText(new MediaElement(), text);
                        break;


                }
            }
            else if(args.Kind == ActivationKind.Protocol)
            {
               
            }
           
        }

        private string SemanticInterpretation(string v, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[v].FirstOrDefault();
        }

        private async void ReadText(MediaElement mediael, string v)
        {
            SpeechSynthesisStream stream = await speech.SynthesizeTextToStreamAsync(v);
            mediael.SetSource(stream, stream.ContentType);
            mediael.Play();
        }
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
