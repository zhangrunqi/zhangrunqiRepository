using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace 串口程序
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 程序启动生成串口号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //for (int i = 0; i < 20; i++)
            //{
            //    comboBox1.Items.Add("COM" + i.ToString());
            //}
            //comboBox1.Text = "COM1";//串口号默认值
            ovalShape1.FillColor = Color.Red;
            comboBox2.Text = "4800";//波特率默认值
            comboBox3.Text = "8";
            comboBox4.Text = "1";
            comboBox5.Text = "None";
            SearchAddSerialToComboBox(serialPort1, comboBox1);
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);//必须手动添加事件处理
        }
        private void port_DataReceived(object sender,SerialDataReceivedEventArgs e)//串口数据接收事件
        {
            if (!rdoReceiveNum.Checked)//如果接收模式为字符模式
            {
                string str = serialPort1.ReadExisting();//字符串方式读
                txtReceive.AppendText(str);//添加内容
            }
            else  //如果接收模式为数值
            {
                byte data;
                data = (byte)serialPort1.ReadByte();//此处有强制转换
                string str = Convert.ToString(data, 16).ToUpper();//转换为大写十六进制字符串
                txtReceive.AppendText("0x" + (str.Length == 1 ? "0" + str : str) + " ");//空位补0
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text, 10);//十进制数据转换
                serialPort1.Parity = (Parity)Convert.ToInt32(comboBox5.SelectedIndex.ToString());//数据校验
                serialPort1.DataBits = Convert.ToInt32(comboBox3.Text.ToString());
                serialPort1.StopBits = (StopBits)Convert.ToInt32(comboBox4.Text);
                serialPort1.Open();//此处易出错
                btnClose.Enabled = true;
                btnOpen.Enabled = false;
                ovalShape1.FillColor = Color.Green;
            }
            catch
            {
                MessageBox.Show("端口错误，请重新检查端口", "错误");
                ovalShape1.FillColor = Color.Red;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
                ovalShape1.FillColor = Color.Red;
            }
            catch(Exception ex)
            {

            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1];
            if(serialPort1.IsOpen && txtSend.Text != "")//判断串口是否打开和发送为空
            {
                if (!rdoSendNum.Checked)
                    {
                        try
                        {
                            serialPort1.WriteLine(txtSend.Text);
                        }
                        catch
                        {
                            MessageBox.Show("串口数据写入错误", "错误");
                            serialPort1.Close();
                            btnOpen.Enabled = true;
                            btnClose.Enabled = false;
                        }
                    }
                else
                {
                    try
                    {
                        for (int i = 0; i < (txtSend.Text.Length-txtSend.Text.Length%2)/2 ; i++)
                        {
                            Data[0] = Convert.ToByte(txtSend.Text.Substring(i * 2, 2), 16);
                            serialPort1.Write(Data, 0, 1);//循环发送（如果输入字符为0a0bb 则只发送0a 0b）
                        }
                        if (txtSend.Text.Length % 2 != 0)
                        {
                            Data[0] = Convert.ToByte(txtSend.Text.Substring(txtSend.Text.Length - 1, 1), 16);//判断最后一位
                            serialPort1.Write(Data, 0, 1);//发送
                        }
                    }
                    catch
                    {
                        MessageBox.Show("错误");
                    }
                }
                    
            }
        }
        /// <summary>
        /// 搜索可用串口，并将可用串口名称添加到combobox中
        /// </summary>
        /// <param name="MyPort">串口</param>
        /// <param name="MyBox">列表展示可用串口</param>
        private void SearchAddSerialToComboBox(SerialPort MyPort ,ComboBox MyBox)
        {
            string[] MyString = new string[20];//最多容纳20个
            string Buffer;//缓存
            MyBox.Items.Clear();//清空combobox中的内容
            int count = 0;
            for (int i = 1; i < 20; i++)
            {
                try
                {
                    Buffer = "COM" + i.ToString();
                    MyPort.PortName = Buffer;
                    MyPort.Open();
                    MyString[count] = Buffer;
                    MyBox.Items.Add(Buffer);
                    MyPort.Close();
                    count++;
                }
                catch 
                {

                }
                MyBox.Text = MyString[0];//初始化
            }
        }
        private void btnSurch_Click(object sender, EventArgs e)
        {
            SearchAddSerialToComboBox(serialPort1, comboBox1);//扫描并将可用串口添加至下拉列表
        }
    }
}
