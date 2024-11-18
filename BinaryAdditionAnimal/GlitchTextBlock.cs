using System.Windows.Controls;
using System.Windows.Threading;

namespace BinaryAdditionAnimal;

public class GlitchTextBlock: TextBlock
{
    private readonly Random _random = new Random();
    private readonly DispatcherTimer _glitchTimer;
    private readonly string _originalText;
    private const string GlitchChars = "!@#$%^&*()_+-=[]{}|;:,.<>?/";

    public GlitchTextBlock(string text)
    {
        _originalText = text;
        Text = text;
        _glitchTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        _glitchTimer.Tick += GlitchTimer_Tick;
    }

    public void StartGlitch()
    {
        _glitchTimer.Start();
    }

    public void StopGlitch()
    {
        _glitchTimer.Stop();
        Text = _originalText;
    }

    private void GlitchTimer_Tick(object sender, EventArgs e)
    {
        if (_random.NextDouble() > 0.7)
        {
            var charArray = _originalText.ToCharArray();
            int pos = _random.Next(charArray.Length);
            charArray[pos] = GlitchChars[_random.Next(GlitchChars.Length)];
            Text = new string(charArray);
        }
        else
        {
            Text = _originalText;
        }
    }
}