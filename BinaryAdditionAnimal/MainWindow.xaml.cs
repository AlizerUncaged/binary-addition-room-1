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
    private const string CORRECT_PASSWORD = "DCE077ABD";
    private GlitchTextBlock _accessGrantedGlitch;
    private GlitchTextBlock _keysGlitch;
    private List<AudioSequence> _audioSequences;
    private MediaPlayer _mediaPlayer;
    private bool _isPlaying;


    private void InitializeAudioSequences()
    {
        _mediaPlayer = new MediaPlayer();
        _audioSequences = new List<AudioSequence>();
    
        for (int i = 0; i < CORRECT_PASSWORD.Length; i++)
        {
            char hexChar = CORRECT_PASSWORD[i];
            string binary = Convert.ToString(Convert.ToInt32(hexChar.ToString(), 16), 2).PadLeft(4, '0');
            _audioSequences.Add(new AudioSequence 
            { 
                Label = $"Character {i + 1} ({hexChar}): ",
                BinarySequence = binary
            });
        }
    
        AudioButtons.ItemsSource = _audioSequences;
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
            foreach (char bit in binarySequence)
            {
                string soundFile = bit == '1' ? "bark.mp3" : "meow.mp3";
                _mediaPlayer.Open(new Uri(soundFile, UriKind.Relative));
                _mediaPlayer.Play();
            
                await Task.Delay(500); // Wait 0.5 seconds between bits
            }
        }
        finally
        {
            _isPlaying = false;
            button.IsEnabled = true;
        }
    }
    public MainWindow()
    {
    InitializeComponent();
    InitializeGlitchTexts();
    InitializeAudioSequences();
    PasswordInput.Focus();
    }

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

        _keysGlitch = new GlitchTextBlock("You may now find the keys");
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

    private async void ShowAccessGranted()
    {
        AccessGrantedOverlay.Visibility = Visibility.Visible;
        MainContent.Visibility = Visibility.Collapsed;

        _accessGrantedGlitch.StartGlitch();
        _keysGlitch.StartGlitch();

        // Create glitch animation
        var glitchAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.3,
            Duration = TimeSpan.FromSeconds(0.1),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };
        AccessGrantedOverlay.BeginAnimation(OpacityProperty, glitchAnimation);

        await Task.Delay(5000); // Show for 5 seconds

        _accessGrantedGlitch.StopGlitch();
        _keysGlitch.StopGlitch();
    }

    private async void ShowAccessDenied()
    {
        AccessDeniedOverlay.Visibility = Visibility.Visible;
        MainContent.Visibility = Visibility.Collapsed;

        // Create glitch animation
        var glitchAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.3,
            Duration = TimeSpan.FromSeconds(0.05),
            AutoReverse = true,
            RepeatBehavior = new RepeatBehavior(10)
        };

        AccessDeniedOverlay.BeginAnimation(OpacityProperty, glitchAnimation);

        await Task.Delay(2000); // Show for 2 seconds

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