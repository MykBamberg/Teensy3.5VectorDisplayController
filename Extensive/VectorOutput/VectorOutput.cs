using System.Windows.Forms;
using System.IO.Ports;

public class VectorOutput : IDisposable
{
    public delegate void    ErrorMessenger(string Error);
    private bool            disposedValue;

    private ErrorMessenger? _ErrorMessenger = null;
    private SerialPort      Serial;

    public ushort[][]       Rectangles =    new ushort[0][];
    public ushort[][]       Lines =         new ushort[0][];


    public VectorOutput(string ComPort)
    {
        Serial = new SerialPort(ComPort, 50000000);
        Serial.Open();
    }

    public VectorOutput(string ComPort, ErrorMessenger errorMessenger)
    {
        Serial = new SerialPort(ComPort, 50000000);
        Serial.Open();
        _ErrorMessenger = errorMessenger;
    }

    public void DrawFrame()
    {
        char[] outBuffer = new char[0];

        outBuffer = new char[(Lines.Length + Rectangles.Length) * 9];

        if (outBuffer.Length > 0)
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                outBuffer[i * 9] = '|';
                outBuffer[i * 9 + 1] = (char)((Lines[i][0] % 64) + 32);
                outBuffer[i * 9 + 2] = (char)((Lines[i][0] >> 6) + 32);
                outBuffer[i * 9 + 3] = (char)((Lines[i][1] % 64) + 32);
                outBuffer[i * 9 + 4] = (char)((Lines[i][1] >> 6) + 32);
                outBuffer[i * 9 + 5] = (char)((Lines[i][2] % 64) + 32);
                outBuffer[i * 9 + 6] = (char)((Lines[i][2] >> 6) + 32);
                outBuffer[i * 9 + 7] = (char)((Lines[i][3] % 64) + 32);
                outBuffer[i * 9 + 8] = (char)((Lines[i][3] >> 6) + 32);
            }
            for (int i = 0; i < Rectangles.Length; i++)
            {
                outBuffer[(Lines.Length + i) * 9] = 'r';
                outBuffer[(Lines.Length + i) * 9 + 1] = (char)((Rectangles[i][0] % 64) + 32);
                outBuffer[(Lines.Length + i) * 9 + 2] = (char)((Rectangles[i][0] >> 6) + 32);
                outBuffer[(Lines.Length + i) * 9 + 3] = (char)((Rectangles[i][1] % 64) + 32);
                outBuffer[(Lines.Length + i) * 9 + 4] = (char)((Rectangles[i][1] >> 6) + 32);
                outBuffer[(Lines.Length + i) * 9 + 5] = (char)((Rectangles[i][2] % 64) + 32);
                outBuffer[(Lines.Length + i) * 9 + 6] = (char)((Rectangles[i][2] >> 6) + 32);
                outBuffer[(Lines.Length + i) * 9 + 7] = (char)((Rectangles[i][3] % 64) + 32);
                outBuffer[(Lines.Length + i) * 9 + 8] = (char)((Rectangles[i][3] >> 6) + 32);
            }
        }
        else
        {
            outBuffer = new char[] { '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        }
        outBuffer[0] += (char)2;

        try
        {
            Serial.Write(outBuffer, 0, outBuffer.Length);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void AddLines(ushort[][] lines)
    {
        Lines = Lines.Concat(lines).ToArray();

        char[] outBuffer = new char[lines.Length * 9];

        for (int i = 0; i < lines.Length; i++)
        {
            outBuffer[i * 9] = '|';
            outBuffer[i * 9 + 1] = (char)((lines[i][0] % 64) + 32);
            outBuffer[i * 9 + 2] = (char)((lines[i][0] >> 6) + 32);
            outBuffer[i * 9 + 3] = (char)((lines[i][1] % 64) + 32);
            outBuffer[i * 9 + 4] = (char)((lines[i][1] >> 6) + 32);
            outBuffer[i * 9 + 5] = (char)((lines[i][2] % 64) + 32);
            outBuffer[i * 9 + 6] = (char)((lines[i][2] >> 6) + 32);
            outBuffer[i * 9 + 7] = (char)((lines[i][3] % 64) + 32);
            outBuffer[i * 9 + 8] = (char)((lines[i][3] >> 6) + 32);
        }

        try
        {
            Serial.Write(outBuffer, 0, outBuffer.Length);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }
    public void AddLines(ushort[] line)
    {
        AddLines(new ushort[][] { line });
    }

    public void AddRectangles(ushort[][] rectangles)
    {
        Rectangles = Rectangles.Concat(rectangles).ToArray();

        char[] outBuffer = new char[rectangles.Length * 9];

        for (int i = 0; i < rectangles.Length; i++)
        {
            outBuffer[i * 9] = 'r';
            outBuffer[i * 9 + 1] = (char)((rectangles[i][0] % 64) + 32);
            outBuffer[i * 9 + 2] = (char)((rectangles[i][0] >> 6) + 32);
            outBuffer[i * 9 + 3] = (char)((rectangles[i][1] % 64) + 32);
            outBuffer[i * 9 + 4] = (char)((rectangles[i][1] >> 6) + 32);
            outBuffer[i * 9 + 5] = (char)((rectangles[i][2] % 64) + 32);
            outBuffer[i * 9 + 6] = (char)((rectangles[i][2] >> 6) + 32);
            outBuffer[i * 9 + 7] = (char)((rectangles[i][3] % 64) + 32);
            outBuffer[i * 9 + 8] = (char)((rectangles[i][3] >> 6) + 32);
        }

        try
        {
            Serial.Write(outBuffer, 0, outBuffer.Length);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }
    public void AddRectangles(ushort[] rectangle)
    {
        AddLines(new ushort[][] { rectangle });
    }

    public void SetSpriteLines(ushort[][] lines, int index)
    {
        index %= 8;
        char[] outBuffer = new char[lines.Length * 9];

        for (int i = 0; i < lines.Length; i++)
        {
            outBuffer[i * 9] = (char)(index + 'd');
            outBuffer[i * 9 + 1] = (char)((lines[i][0] % 64) + 32);
            outBuffer[i * 9 + 2] = (char)((lines[i][0] >> 6) + 32);
            outBuffer[i * 9 + 3] = (char)((lines[i][1] % 64) + 32);
            outBuffer[i * 9 + 4] = (char)((lines[i][1] >> 6) + 32);
            outBuffer[i * 9 + 5] = (char)((lines[i][2] % 64) + 32);
            outBuffer[i * 9 + 6] = (char)((lines[i][2] >> 6) + 32);
            outBuffer[i * 9 + 7] = (char)((lines[i][3] % 64) + 32);
            outBuffer[i * 9 + 8] = (char)((lines[i][3] >> 6) + 32);
        }

        try
        {
            Serial.Write(new char[] { 'l', (char)(index + 'd'), ' ', ' ', ' ', ' ', ' ', ' ', ' ' }, 0, 9);
            Serial.Write(outBuffer, 0, outBuffer.Length);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void SetSpritePosition(int index, short x, short y)
    {
        try
        {
            Serial.Write(
                new char[] 
                { 
                    'p', 
                    (char)(index + 'd'), 
                    x < 0 ? '!' : ' ',
                    (char)((Math.Abs(x) % 64) + 32),
                    (char)((Math.Abs(x) >> 6) + 32),
                    y < 0 ? '!' : ' ',
                    (char)((Math.Abs(y) % 64) + 32),
                    (char)((Math.Abs(y) >> 6) + 32),
                    ' '
                }, 0, 9);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void StampSprite(int index)
    {
        try
        {
            Serial.Write(new char[] { 'n', (char)(index + 'd'), ' ', ' ', ' ', ' ', ' ', ' ', ' ' }, 0, 9);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }
    public void StampSprite(int index, short x, short y)
    {
        try
        {
            Serial.Write(
                new char[]
                {
                    'p',
                    (char)(index + 'd'),
                    x < 0 ? '!' : ' ',
                    (char)((Math.Abs(x) % 64) + 32),
                    (char)((Math.Abs(x) >> 6) + 32),
                    y < 0 ? '!' : ' ',
                    (char)((Math.Abs(y) % 64) + 32),
                    (char)((Math.Abs(y) >> 6) + 32),
                    ' '
                }, 0, 9);
            Serial.Write(new char[] { 'n', (char)(index + 'd'), ' ', ' ', ' ', ' ', ' ', ' ', ' ' }, 0, 9);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void SetSpritesEnabled(bool[] enabled)
    {
        if (enabled.Length != 8)
        {
            throw new Exception("bool[] enabled has to have a length of 8");
        }
        try
        {
            Serial.Write(
                new char[] 
                { 
                    'm',
                    enabled[0] ? '!' : ' ',
                    enabled[1] ? '!' : ' ',
                    enabled[2] ? '!' : ' ',
                    enabled[3] ? '!' : ' ',
                    enabled[4] ? '!' : ' ',
                    enabled[5] ? '!' : ' ',
                    enabled[6] ? '!' : ' ',
                    enabled[7] ? '!' : ' ',
                }, 0, 9);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void Setres(byte res)
    {
        try
        {
            Serial.Write(
                new char[] 
                { 
                    'q', 
                    (char)(Math.Max(res, (byte)8) + 32), 
                    ' ', ' ', ' ', ' ', ' ', ' ', ' ' 
                }, 0, 9);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void SetCursor(ushort x, ushort y, ushort intensity, bool enabled)
    {
        try
        {
            Serial.Write(new char[]
                        {
                            'c',
                            (char)((x % 64) + 32),
                            (char)((x >> 6) + 32),
                            (char)((y % 64) + 32),
                            (char)((y >> 6) + 32),
                            (char)((intensity % 64) + 32),
                            (char)((intensity >> 6) + 32),
                            enabled ? '!' : ' ',
                            ' '
                        }, 0, 9);
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void Clear()
    {
        try
        {
            Serial.Write("~        " +
                         "c        " +
                         "q        " +
                         "ld       " +
                         "le       " +
                         "lf       " +
                         "lg       " +
                         "lh       " +
                         "li       " +
                         "lj       " +
                         "lk       " );

            Rectangles = new ushort[0][];
            Lines = new ushort[0][];
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public static string? ComPortForm()
    {
        Form prompt = new Form()
        {
            Width = 350,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = "COM port selection",
            StartPosition = FormStartPosition.CenterScreen
        };

        string[] comPorts = SerialPort.GetPortNames();

        Label label = new Label() { Left = 50, Top = 20, Text = "Select Teensy port" };

        ComboBox comboBox = new ComboBox() { Left = 50, Top = 15, Width = 250 };
        comboBox.Items.AddRange(comPorts);

        Button OKButton = new Button() { Text = "Ok", Left = 200, Width = 100, Top = 70, DialogResult = DialogResult.OK };
        OKButton.Click += (sender, e) => { prompt.Close(); };

        prompt.Controls.Add(comboBox);
        prompt.Controls.Add(OKButton);
        prompt.Controls.Add(label);
        prompt.AcceptButton = OKButton;

        if (prompt.ShowDialog() != DialogResult.OK)
        {
            return null;
        }

        if (!comPorts.Contains(comboBox.Text))
        {
            return null;
        }

        return comboBox.Text;
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Serial.Close();
                ((IDisposable)Serial).Dispose();
            }

            Rectangles = new ushort[0][];
            Lines = new ushort[0][];

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}