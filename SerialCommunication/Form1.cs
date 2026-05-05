using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();
                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;

                comboBoxBaudrate.SelectedIndex = comboBoxBaudrate.Items.IndexOf("115200");
            }
            catch (Exception)
            { }
        }

        private void cboPoort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string selected = (string)comboBoxPoort.SelectedItem;
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();

                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);

                comboBoxPoort.SelectedIndex = comboBoxPoort.Items.IndexOf(selected);
            }
            catch (Exception)
            {
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            // Controleer of poort bestaat en open is --> verbreken, anders maken
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    Disconnect();
                }
                else
                {
                    Connect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error";
            }
        }

        private void Connect()
        {
            try
            {
                if (comboBoxPoort.SelectedItem == null)
                {
                    MessageBox.Show("Select a COM port first.", "No port selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Apply settings from UI
                serialPortArduino.PortName = comboBoxPoort.SelectedItem.ToString();

                int baud = 115200;
                if (comboBoxBaudrate.SelectedItem != null)
                    int.TryParse(comboBoxBaudrate.SelectedItem.ToString(), out baud);
                serialPortArduino.BaudRate = baud;

                serialPortArduino.DataBits = (int)numericUpDownDatabits.Value;

                // Parity
                if (radioButtonParityEven.Checked) serialPortArduino.Parity = Parity.Even;
                else if (radioButtonParityOdd.Checked) serialPortArduino.Parity = Parity.Odd;
                else if (radioButtonParityNone.Checked) serialPortArduino.Parity = Parity.None;
                else if (radioButtonParityMark.Checked) serialPortArduino.Parity = Parity.Mark;
                else if (radioButtonParitySpace.Checked) serialPortArduino.Parity = Parity.Space;

                // StopBits
                if (radioButtonStopbitsNone.Checked) serialPortArduino.StopBits = StopBits.None;
                else if (radioButtonStopbitsOne.Checked) serialPortArduino.StopBits = StopBits.One;
                else if (radioButtonStopbitsOnePointFive.Checked) serialPortArduino.StopBits = StopBits.OnePointFive;
                else if (radioButtonStopbitsTwo.Checked) serialPortArduino.StopBits = StopBits.Two;

                // Handshake
                if (radioButtonHandshakeNone.Checked) serialPortArduino.Handshake = Handshake.None;
                else if (radioButtonHandshakeRTS.Checked) serialPortArduino.Handshake = Handshake.RequestToSend;
                else if (radioButtonHandshakeRTSXonXoff.Checked) serialPortArduino.Handshake = Handshake.RequestToSendXOnXOff;
                else if (radioButtonHandshakeXonXoff.Checked) serialPortArduino.Handshake = Handshake.XOnXOff;

                serialPortArduino.DtrEnable = checkBoxDtrEnable.Checked;
                serialPortArduino.RtsEnable = checkBoxRtsEnable.Checked;

                serialPortArduino.Open();

                radioButtonVerbonden.Checked = true;
                buttonConnect.Text = "Disconnect";
                labelStatus.Text = $"Connected on {serialPortArduino.PortName} ({serialPortArduino.BaudRate})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening serial port: {ex.Message}", "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error";
            }
        }

        private void Disconnect()
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    serialPortArduino.Close();
                }

                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
                labelStatus.Text = "Disconnected";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing serial port: {ex.Message}", "Disconnection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error";
            }
        }

        private void checkBoxDigital2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string cmd = checkBoxDigital2.Checked ? "set d2 high" : "set d2 low";
                    serialPortArduino.WriteLine(cmd);
                    labelStatus.Text = $"Sent: {cmd}";
                }
                else
                {
                    labelStatus.Text = "Not connected";
                }
            }
            catch (Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
                serialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
            }
        }

        private void checkBoxDigital3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string cmd = checkBoxDigital3.Checked ? "set d3 high" : "set d3 low";
                    serialPortArduino.WriteLine(cmd);
                    labelStatus.Text = $"Sent: {cmd}";
                }
                else
                {
                    labelStatus.Text = "Not connected";
                }
            }
            catch (Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
                serialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
            }
        }

        private void checkBoxDigital4_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino != null && serialPortArduino.IsOpen)
                {
                    string cmd = checkBoxDigital4.Checked ? "set d4 high" : "set d4 low";
                    serialPortArduino.WriteLine(cmd);
                    labelStatus.Text = $"Sent: {cmd}";
                }
                else
                {
                    labelStatus.Text = "Not connected";
                }
            }
            catch (Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
                serialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
            }
        }

        private void trackBarPWM9_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string commando = string.Format("set pwm9 {0}",trackBarPWM9.Value); //set pwm9 0..255
                   serialPortArduino.WriteLine(commando);
                    
                }
                
                

            }
            catch ( Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
                serialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
            }
        }

        private void trackBarPWM10_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string commando = string.Format("set pwm10 {0}", trackBarPWM10.Value); //set pwm9 0..255
                    serialPortArduino.WriteLine(commando);

                }



            }
            catch (Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
                serialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
            }
        }

        private void trackBarPWM11_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string commando = string.Format("set pwm11 {0}", trackBarPWM11.Value); //set pwm9 0..255
                    serialPortArduino.WriteLine(commando);

                }



            }
            catch (Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
                serialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
            }
        }
    }
}
