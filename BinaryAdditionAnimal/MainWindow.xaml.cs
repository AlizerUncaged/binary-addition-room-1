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

    public MainWindow()
    {
        InitializeComponent();
        InitializeGlitchTexts();
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
}