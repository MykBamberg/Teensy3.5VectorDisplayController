using System.Windows.Forms;
using System.IO.Ports;


public class VectorOutput : IDisposable
{
    private SerialPort Serial;

    private bool[] SpriteEnabled = new bool[] { false, false, false, false, false, false, false, false };

    private int[][] SpritePosition = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 } };

    private ushort[] CursorPosition = new ushort[] { 0, 0 };

    bool CursorEnabled = false;

    ushort CursorIntensity = 30;

    private ushort[][][] Sprites = new ushort[8][][];

    private ushort[][] _Rectangles = new ushort[0][];

    private ushort[][] _Lines = new ushort[0][];

    private bool StaticChange = false;

    private bool[] SpriteChange = new bool[] { false, false, false, false, false, false, false, false };

    private bool[] SpritePositionChange = new bool[] { false, false, false, false, false, false, false, false };

    private bool SpriteEnabledChange = false;

    private bool[] SpriteStamp = new bool[] { false, false, false, false, false, false, false, false };

    private bool CursorChange = false;

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

    public void SetSprite(ushort[][] lines, int index, bool enabled)
    {
        Sprites[index] = lines.Where(a => a.Max() <= 4095).ToArray();
        SpriteChange[index] = true;
        SetSpriteEnabled(index, enabled);
        SetSpritePosition(0, 0, index);
    }

    public void SetSpritePosition(int x, int y, int index)
    {
        SpritePosition[index] = new int[] { x, y };
        SpritePositionChange[index] = true;
    }

    public void StampSprite(int index)
    {
        SpriteStamp[index] = true;
    }

    public void SetSpriteEnabled(int index, bool b)
    {
        SpriteEnabled[index] = b;
        SpriteEnabledChange = true;
    }

    public void SetCursor(ushort x, ushort y, ushort Intensity, bool enabled)
    {
        CursorChange = true;
        CursorPosition = new ushort[] { x, y };
        CursorIntensity = Intensity;
        CursorEnabled = enabled;
    }

    public void AddLines(ushort[][] lines)
    {
        _Lines = _Lines.Concat(lines).ToArray();
        StaticChange = true;
    }

    public void AddRectangles(ushort[][] rectangles)
    {
        _Rectangles = _Rectangles.Concat(rectangles).ToArray();
        StaticChange = true;
    }

    public delegate void ErrorMessenger(string Error);

    private ErrorMessenger? _ErrorMessenger = null;

    public ErrorMessenger? errorMessenger
    {
        get { return _ErrorMessenger; }
        set { _ErrorMessenger = value; }
    }

    public ushort[][] Rectangles
    {
        get
        {
            return _Rectangles;
        }
        set
        {
            _Rectangles = value != null ? value.Where(a => a.Max() <= 4095).ToArray() : new ushort[0][];
            StaticChange = true;
        }
    }

    public ushort[][] Lines
    {
        get
        {
            return _Lines;
        }
        set
        {
            _Lines = value != null ? value.Where(a => a.Max() <= 4095).ToArray() : new ushort[0][];
            StaticChange = true;
        }
    }

    public static string? GetComPort()
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

    public void DrawFrame()
    {
        char[] outBuffer = new char[0];
        char[] outBufferSprites = new char[0];
        char[] outBufferStamps = new char[0];
        char[] outBufferSpritePosition = new char[0];
        char[] outBufferCursor = new char[0];

        if (StaticChange)
        {
            outBuffer = new char[(_Lines.Length + _Rectangles.Length) * 9];
            if (outBuffer.Length > 0)
            {
                for (int i = 0; i < _Lines.Length; i++)
                {
                    outBuffer[i * 9] = '|';
                    outBuffer[i * 9 + 1] = (char)((_Lines[i][0] % 64) + 32);
                    outBuffer[i * 9 + 2] = (char)((_Lines[i][0] >> 6) + 32);
                    outBuffer[i * 9 + 3] = (char)((_Lines[i][1] % 64) + 32);
                    outBuffer[i * 9 + 4] = (char)((_Lines[i][1] >> 6) + 32);
                    outBuffer[i * 9 + 5] = (char)((_Lines[i][2] % 64) + 32);
                    outBuffer[i * 9 + 6] = (char)((_Lines[i][2] >> 6) + 32);
                    outBuffer[i * 9 + 7] = (char)((_Lines[i][3] % 64) + 32);
                    outBuffer[i * 9 + 8] = (char)((_Lines[i][3] >> 6) + 32);
                }
                for (int i = 0; i < _Rectangles.Length; i++)
                {
                    outBuffer[(_Lines.Length + i) * 9] = 'r';
                    outBuffer[(_Lines.Length + i) * 9 + 1] = (char)((_Rectangles[i][0] % 64) + 32);
                    outBuffer[(_Lines.Length + i) * 9 + 2] = (char)((_Rectangles[i][0] >> 6) + 32);
                    outBuffer[(_Lines.Length + i) * 9 + 3] = (char)((_Rectangles[i][1] % 64) + 32);
                    outBuffer[(_Lines.Length + i) * 9 + 4] = (char)((_Rectangles[i][1] >> 6) + 32);
                    outBuffer[(_Lines.Length + i) * 9 + 5] = (char)((_Rectangles[i][2] % 64) + 32);
                    outBuffer[(_Lines.Length + i) * 9 + 6] = (char)((_Rectangles[i][2] >> 6) + 32);
                    outBuffer[(_Lines.Length + i) * 9 + 7] = (char)((_Rectangles[i][3] % 64) + 32);
                    outBuffer[(_Lines.Length + i) * 9 + 8] = (char)((_Rectangles[i][3] >> 6) + 32);
                }
            }
            else
            {
                outBuffer = new char[] { '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
            }
            outBuffer[0] += (char)2;
        }

        if (SpriteChange.Contains(true) || SpriteEnabledChange)
        {
            List<char> tempSpriteOutBuffer = new List<char>();
            for (int i = 0; i < 8; i++)
            {
                if (SpriteChange[i])
                {
                    tempSpriteOutBuffer.Add('l');
                    tempSpriteOutBuffer.Add((char)(100 + i)); //100 is 'd' in ASCII
                    tempSpriteOutBuffer.AddRange(new char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ' });
                    for (int j = 0; j < Sprites[i].Length; j++)
                    {
                        tempSpriteOutBuffer.Add((char)(100 + i));
                        tempSpriteOutBuffer.Add((char)((Sprites[i][j][0] % 64) + 32));
                        tempSpriteOutBuffer.Add((char)((Sprites[i][j][0] >> 6) + 32));
                        tempSpriteOutBuffer.Add((char)((Sprites[i][j][1] % 64) + 32));
                        tempSpriteOutBuffer.Add((char)((Sprites[i][j][1] >> 6) + 32));
                        tempSpriteOutBuffer.Add((char)((Sprites[i][j][2] % 64) + 32));
                        tempSpriteOutBuffer.Add((char)((Sprites[i][j][2] >> 6) + 32));
                        tempSpriteOutBuffer.Add((char)((Sprites[i][j][3] % 64) + 32));
                        tempSpriteOutBuffer.Add((char)((Sprites[i][j][3] >> 6) + 32));
                    }
                }
            }
            tempSpriteOutBuffer.Add('m');
            for (int i = 0; i < 8; i++)
            {
                tempSpriteOutBuffer.Add(SpriteEnabled[i] ? '!' : ' ');
            }
            outBufferSprites = tempSpriteOutBuffer.ToArray();
        }

        if (SpritePositionChange.Contains(true))
        {
            List<char> tempPosOutBuffer = new List<char>();
            for (int i = 0; i < 8; i++)
            {
                if (SpritePositionChange[i])
                {
                    tempPosOutBuffer.Add('p');
                    tempPosOutBuffer.Add((char)(100 + i));
                    tempPosOutBuffer.Add(SpritePosition[i][0] > 0 ? ' ' : '!');
                    tempPosOutBuffer.Add((char)((Math.Abs(SpritePosition[i][0]) % 64) + 32));
                    tempPosOutBuffer.Add((char)((Math.Abs(SpritePosition[i][0]) >> 6) + 32));
                    tempPosOutBuffer.Add(SpritePosition[i][1] > 0 ? ' ' : '!');
                    tempPosOutBuffer.Add((char)((Math.Abs(SpritePosition[i][1]) % 64) + 32));
                    tempPosOutBuffer.Add((char)((Math.Abs(SpritePosition[i][1]) >> 6) + 32));
                    tempPosOutBuffer.Add(' ');
                }
            }
            outBufferSpritePosition = tempPosOutBuffer.ToArray();
        }

        if (SpriteStamp.Contains(true))
        {
            List<char> tempStampOutBuffer = new List<char>();
            for (int i = 0; i < 8; i++)
            {
                if (SpriteStamp[i])
                {
                    tempStampOutBuffer.Add('n');
                    tempStampOutBuffer.Add((char)(100 + i));
                    tempStampOutBuffer.AddRange(new char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ' });
                }
            }
            outBufferStamps = tempStampOutBuffer.ToArray();
        }

        if (CursorChange)
        {
            outBufferCursor = new char[]
            {
                'c',
                (char)((CursorPosition[0] % 64) + 32),
                (char)((CursorPosition[0] >> 6) + 32),
                (char)((CursorPosition[1] % 64) + 32),
                (char)((CursorPosition[1] >> 6) + 32),
                (char)((CursorIntensity % 64) + 32),
                (char)((CursorIntensity >> 6) + 32),
                CursorEnabled ? '!' : ' ',
                ' '
            };
        }

        try
        {
            Serial.Write(outBuffer, 0, outBuffer.Length);
            Serial.Write(outBufferSprites, 0, outBufferSprites.Length);
            Serial.Write(outBufferSpritePosition, 0, outBufferSpritePosition.Length);
            Serial.Write(outBufferStamps, 0, outBufferStamps.Length);
            Serial.Write(outBufferCursor, 0, outBufferCursor.Length);

            StaticChange = false;
            SpriteEnabledChange = false;
            CursorChange = false;
            SpriteChange = new bool[] { false, false, false, false, false, false, false, false };
            SpritePositionChange = SpriteChange;
            SpriteStamp = SpriteChange;
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void IUAddLines(ushort[][] lines)
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

    public void IUAddRectangles(ushort[][] rectangles)
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

    public void IUAddLinesToSprite(ushort[][] lines, int index) //Instant Update
    {
        Sprites[index] = Sprites[index].Concat(lines).ToArray();
        char[] outBuffer = new char[lines.Length * 9];
        for (int i = 0; i < lines.Length; i++)
        {
            outBuffer[i * 9 + 0] = (char)(100 + i);
            outBuffer[i * 9 + 1] = (char)((Lines[i][0] % 64) + 32);
            outBuffer[i * 9 + 2] = (char)((Lines[i][0] >> 6) + 32);
            outBuffer[i * 9 + 3] = (char)((Lines[i][1] % 64) + 32);
            outBuffer[i * 9 + 4] = (char)((Lines[i][1] >> 6) + 32);
            outBuffer[i * 9 + 5] = (char)((Lines[i][2] % 64) + 32);
            outBuffer[i * 9 + 6] = (char)((Lines[i][2] >> 6) + 32);
            outBuffer[i * 9 + 7] = (char)((Lines[i][3] % 64) + 32);
            outBuffer[i * 9 + 8] = (char)((Lines[i][3] >> 6) + 32);
        }

        try
        {
            Serial.Write(outBuffer, 0, outBuffer.Length);

            StaticChange = false;
            SpriteEnabledChange = false;
            SpriteChange = new bool[] { false, false, false, false, false, false, false, false };
            SpritePositionChange = SpriteChange;
            SpriteStamp = SpriteChange;
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void IUSetSpritePosition(int x, int y, int index)
    {
        SpritePosition[index] = new int[] { x, y };
        char[] outBuffer = new char[9];
        outBuffer[0] = 'p';
        outBuffer[1] = (char)(100 + index);
        outBuffer[2] = x > 0 ? ' ' : '!';
        outBuffer[3] = (char)((Math.Abs(x) % 64) + 32);
        outBuffer[4] = (char)((Math.Abs(x) >> 6) + 32);
        outBuffer[5] = y > 0 ? ' ' : '!';
        outBuffer[6] = (char)((Math.Abs(y) % 64) + 32);
        outBuffer[7] = (char)((Math.Abs(y) >> 6) + 32);
        outBuffer[8] = (' ');

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

    public void IUSetres(byte res)
    {
        char[] outBuffer = new char[] { 'q', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        if (res > 8) res = 8;
        outBuffer[1] = (char)(res + 32);

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

    public void IUSetCursor(ushort x, ushort y, ushort Intensity, bool enabled)
    {
        CursorPosition = new ushort[] { x, y };
        CursorIntensity = Intensity;
        CursorEnabled = enabled;
        char[] outBuffer = new char[]
            {
                'c',
                (char)((CursorPosition[0] % 64) + 32),
                (char)((CursorPosition[0] >> 6) + 32),
                (char)((CursorPosition[1] % 64) + 32),
                (char)((CursorPosition[1] >> 6) + 32),
                (char)((CursorIntensity % 64) + 32),
                (char)((CursorIntensity >> 6) + 32),
                CursorEnabled ? '!' : ' ',
                ' '
            };
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

    public void IUClear()
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
                         "lk       ");

            SpriteEnabled = new bool[] { false, false, false, false, false, false, false, false };
            SpritePosition = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 }, new int[] { 0, 0 } };
            Sprites = new ushort[8][][];
            _Rectangles = new ushort[0][];
            _Lines = new ushort[0][];
            StaticChange = false;
            SpriteChange = new bool[] { false, false, false, false, false, false, false, false };
            SpritePositionChange = new bool[] { false, false, false, false, false, false, false, false };
            SpriteEnabledChange = false;
            SpriteStamp = new bool[] { false, false, false, false, false, false, false, false };
            CursorPosition = new ushort[] { 0, 0 };
            CursorEnabled = false;
            CursorIntensity = 30;
            CursorChange = false;
        }
        catch (Exception ex)
        {
            if (_ErrorMessenger != null)
            {
                _ErrorMessenger.Invoke(ex.Message);
            }
        }
    }

    public void Dispose()
    {
        Serial.Close();
    }
}
