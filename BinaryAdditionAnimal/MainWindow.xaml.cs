using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinaryAdditionAnimal;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string CORRECT_PASSWORD = "FADE";
    private GlitchTextBlock _accessGrantedGlitch;
    private GlitchTextBlock _keysGlitch;
    private List<AudioSequence> _audioSequences;
    private MediaPlayer _mediaPlayer;
    private bool _isPlaying;
    private bool jumpscareTriggered = false;

    private void InitializeAudioSequences()
    {
        _audioSequences = new List<AudioSequence>();

        // Verify audio files exist
        string[] requiredFiles = { "bark.mp3", "meow.mp3" };
        foreach (string file in requiredFiles)
        {
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
            if (!System.IO.File.Exists(fullPath))
            {
                MessageBox.Show($"Required audio file not found: {file}\nExpected path: {fullPath}\n\n" +
                                "Please ensure the audio files are in the application directory.",
                    "Missing Files",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        for (int i = 0; i < CORRECT_PASSWORD.Length; i++)
        {
            char hexChar = CORRECT_PASSWORD[i];
            string binary = Convert.ToString(Convert.ToInt32(hexChar.ToString(), 16), 2).PadLeft(4, '0');
            _audioSequences.Add(new AudioSequence
            {
                // Label = $"Character {i + 1} ({hexChar}) [{binary}]: ",
                Label = $"Character {i + 1}: ",
                BinarySequence = binary
            });
        }

        AudioButtons.ItemsSource = _audioSequences;
    }

    private void PlayStartAudio()
    {
        new Thread(() =>
        {
            try
            {
                var audioPlayer = new MediaPlayer();
                string audioPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "start.mp3");
                audioPlayer.Open(new Uri(audioPath));
                audioPlayer.Play();

                Thread.Sleep(4000);
            }
            catch (Exception ex)
            {
                ;
            }
        }).Start();
    }

    private async void PlayAudio_Click(object sender, RoutedEventArgs e)
    {
        if (_isPlaying) return;

        var button = (Button)sender;
        var binarySequence = (string)button.Tag;
        _isPlaying = true;
        button.IsEnabled = false;

        try
        {
            await Task.Run(async () =>
            {
                foreach (char bit in binarySequence)
                {
                    try
                    {
                        // Dispatcher needed for UI operations

                        var soundPlayer = new MediaPlayer();
                        string soundFile = bit == '1' ? "bark.mp3" : "meow.mp3";
                        string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, soundFile);

                        if (!System.IO.File.Exists(fullPath))
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                MessageBox.Show($"Sound file not found: {soundFile}\nExpected path: {fullPath}",
                                    "File Not Found Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            });

                            return;
                        }

                        try
                        {
                            soundPlayer.Open(new Uri(fullPath, UriKind.Absolute));
                            soundPlayer.Play();
                            // Fixed delay of 1.5 seconds before next sound
                            await Task.Delay(1200);
                        }
                        catch (Exception ex)
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                MessageBox.Show($"Error playing audio: {ex.Message}",
                                    "Playback Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            MessageBox.Show($"Error in audio sequence: {ex.Message}",
                                "Sequence Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        });
                        break;
                    }
                }
            });
        }
        finally
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _isPlaying = false;
                button.IsEnabled = true;
            });
        }
    }

    // Add these fields for jumpscare
    private Window _jumpscareWindow;
    private MediaElement _jumpscarePlayer;

    public MainWindow()
    {
        InitializeComponent();
        InitializeGlitchTexts();
        InitializeAudioSequences();
        InitializeJumpscare(); // Add this line
        PasswordInput.Focus();
    }

    private void InitializeJumpscare()
    {
        _jumpscareWindow = new Window
        {
            WindowStyle = WindowStyle.None,
            WindowState = WindowState.Maximized,
            Topmost = true,
            ShowInTaskbar = false,
            Background = Brushes.Black,
            Visibility = Visibility.Hidden // Hide window by default
        };

        _jumpscarePlayer = new MediaElement
        {
            Source = new Uri("fun.mp4", UriKind.Relative),
            LoadedBehavior = MediaState.Manual,
            UnloadedBehavior = MediaState.Stop,
            Stretch = Stretch.Uniform,
            Volume = 0, // Start muted
            IsMuted = false
        };

        _jumpscarePlayer.MediaEnded += (s, e) =>
        {
            _jumpscareWindow.Hide();
            _jumpscarePlayer.Position = TimeSpan.Zero; // Reset position
            _jumpscarePlayer.Play(); // Keep playing in background
            _jumpscarePlayer.Volume = 0;

            if (fnafSound)
            {
                PlayStartAudio();
                fnafSound = false;
            }
        };

        _jumpscareWindow.Content = _jumpscarePlayer;
        _jumpscareWindow.Loaded += (s, e) =>
        {
            _jumpscarePlayer.Play(); // Start playing (muted) when window is loaded
        };

        // Create but don't show the window - this preloads the video
        _jumpscareWindow.Show();
        _jumpscareWindow.Hide();
    }

    private bool fnafSound = false;
    private void InitializeGlitchTexts()
    {
        _accessGrantedGlitch = new GlitchTextBlock("ACCESS GRANTED");
        _accessGrantedGlitch.FontSize = 72;
        _accessGrantedGlitch.Foreground = Brushes.LightGreen;
        _accessGrantedGlitch.FontFamily = new FontFamily("Consolas");
        _accessGrantedGlitch.Effect = new DropShadowEffect
        {
            Color = Colors.Green,
            BlurRadius = 20,
            ShadowDepth = 0
        };

        // _keysGlitch = new GlitchTextBlock("You may now find the keys");
        _keysGlitch = new GlitchTextBlock("you may now proceed to the next room");
        _keysGlitch.FontSize = 36;
        _keysGlitch.Foreground = Brushes.LightGreen;
        _keysGlitch.FontFamily = new FontFamily("Consolas");
        _keysGlitch.Margin = new Thickness(0, 30, 0, 0);
        _keysGlitch.Effect = new DropShadowEffect
        {
            Color = Colors.Green,
            BlurRadius = 20,
            ShadowDepth = 0
        };
    }

    private void CheckPassword()
    {
        bool isCorrect = PasswordInput.Text.Equals(CORRECT_PASSWORD, StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            ShowAccessGranted();
        }
        else
        {
            ShowAccessDenied();
        }
    }


    private void PlayJumpscare()
    {
        // Reset position to start (with small offset to prevent black frame)
        _jumpscarePlayer.Position = TimeSpan.FromMilliseconds(1);
        _jumpscarePlayer.Volume = 1; // Set full volume
        _jumpscareWindow.Show(); // Show the window
        fnafSound = true;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _jumpscarePlayer?.Close();
        _jumpscareWindow?.Close();
        base.OnClosing(e);
    }

    private async void ShowAccessGranted()
    {
        if (!jumpscareTriggered)
        {
            jumpscareTriggered = true;
            PlayJumpscare();
        }

        //  PlayStartAudio();

        AccessGrantedOverlay.Visibility = Visibility.Visible;
        MainContent.Visibility = Visibility.Collapsed;

        _accessGrantedGlitch.StartGlitch();
        _keysGlitch.StartGlitch();

        var glitchAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.3,
            Duration = TimeSpan.FromSeconds(0.1),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };
        AccessGrantedOverlay.BeginAnimation(OpacityProperty, glitchAnimation);

        await Task.Delay(5000);

        _accessGrantedGlitch.StopGlitch();
        _keysGlitch.StopGlitch();

        // Reset jumpscare trigger for next player
        jumpscareTriggered = false;
    }

    private async void ShowAccessDenied()
    {
        if (!jumpscareTriggered)
        {
            jumpscareTriggered = true;
            PlayJumpscare();
        }

        AccessDeniedOverlay.Visibility = Visibility.Visible;
        MainContent.Visibility = Visibility.Collapsed;

        var glitchAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.3,
            Duration = TimeSpan.FromSeconds(0.05),
            AutoReverse = true,
            RepeatBehavior = new RepeatBehavior(10)
        };

        AccessDeniedOverlay.BeginAnimation(OpacityProperty, glitchAnimation);

        await Task.Delay(2000);

        AccessDeniedOverlay.Visibility = Visibility.Collapsed;
        MainContent.Visibility = Visibility.Visible;
        PasswordInput.Clear();
        PasswordInput.Focus();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        CheckPassword();
    }

    private void PasswordInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CheckPassword();
        }
    }


    public class AudioSequence
    {
        public string Label { get; set; }
        public string BinarySequence { get; set; }
    }
}