using System;
using System.Windows.Forms;
using System.IO.Ports;

namespace Asteroids
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

        public static void DrawFrame(int[][] frame, string comPort)
        {
            if (frame.Length > 0)
            {
                string outBuffer = "|"
                    + ((ushort)Math.Max(Math.Min(frame[0][0], 0xFFF), 0)).ToString("X3")
                    + ((ushort)Math.Max(Math.Min(frame[0][1], 0xFFF), 0)).ToString("X3")
                    + ((ushort)Math.Max(Math.Min(frame[0][2], 0xFFF), 0)).ToString("X3")
                    + ((ushort)Math.Max(Math.Min(frame[0][3], 0xFFF), 0)).ToString("X3")
                    + "~";
                for (int i = 1; i < frame.Length; i++)
                {
                    outBuffer += "|"
                        + ((ushort)Math.Max(Math.Min(frame[i][0], 0xFFF), 0)).ToString("X3")
                        + ((ushort)Math.Max(Math.Min(frame[i][1], 0xFFF), 0)).ToString("X3")
                        + ((ushort)Math.Max(Math.Min(frame[i][2], 0xFFF), 0)).ToString("X3")
                        + ((ushort)Math.Max(Math.Min(frame[i][3], 0xFFF), 0)).ToString("X3")
                        + " ";
                }
                try
                {
                    using (SerialPort Serial = new SerialPort(comPort, 50000000))
                    {
                        Serial.Open();
                        Serial.Write(outBuffer);
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
