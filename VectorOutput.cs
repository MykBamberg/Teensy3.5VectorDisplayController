using System;
using System.Windows.Forms;
using System.IO.Ports;

namespace VectorDisplay
{
    public static class VectorOutput
    {
        public static string GetComPort()
        {
            Form prompt = new Form()
            {
                Width = 350,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "COM port selection",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = "Select Teensy port" };
            ComboBox comboBox = new ComboBox() { Left = 50, Top = 15, Width = 250 };
            comboBox.Items.AddRange(SerialPort.GetPortNames());
            Button confirmation = new Button() { Text = "Ok", Left = 200, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(comboBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            return prompt.ShowDialog() == DialogResult.OK && (comboBox.Text != "") ? comboBox.Text : null;
        }

        public static void DrawFrame(ushort[][] frame, string comPort)
        {
            if (frame.Length > 0)
            {
                char[] outBuffer = new char[frame.Length * 9 + 9];
                for (int i = 0; i < frame.Length; i++)
                {
                    outBuffer[i * 9] = '|';
                    outBuffer[i * 9 + 1] = (char)((frame[i][0] % 64) + 32);
                    outBuffer[i * 9 + 2] = (char)((frame[i][0] >> 6) + 32);
                    outBuffer[i * 9 + 3] = (char)((frame[i][1] % 64) + 32);
                    outBuffer[i * 9 + 4] = (char)((frame[i][1] >> 6) + 32);
                    outBuffer[i * 9 + 5] = (char)((frame[i][2] % 64) + 32);
                    outBuffer[i * 9 + 6] = (char)((frame[i][2] >> 6) + 32);
                    outBuffer[i * 9 + 7] = (char)((frame[i][3] % 64) + 32);
                    outBuffer[i * 9 + 8] = (char)((frame[i][3] >> 6) + 32);
                }
                outBuffer[0] = '~';
                try
                {
                    using (SerialPort Serial = new SerialPort(comPort, 50000000))
                    {
                        Serial.Open();
                        Serial.Write(new string(outBuffer));
                        Serial.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
