using System;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace DR_RTM
{
    public class MyLabel : Label
    {
        public static Label Set(string Text = "", Font Font = null, Color ForeColor = new Color(), Color BackColor = new Color())
        {
            Label l = new Label();
            l.Text = Text;
            l.Font = SystemFonts.MessageBoxFont;
            l.ForeColor = (ForeColor == new Color()) ? Color.White : ForeColor;
            l.BackColor = (BackColor == new Color()) ? Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32))))) : BackColor;
            l.AutoSize = true;
            return l;
        }
    }
    public class MyButton : Button
    {
        public static Button Set(string Text = "", int Width = 102, int Height = 30, Font Font = null, Color ForeColor = new Color(), Color BackColor = new Color())
        {
            Button b = new Button();
            b.Text = Text;
            b.Width = Width;
            b.Height = Height;
            b.Font = (Font == null) ? new Font("Calibri", 12) : Font;
            b.ForeColor = (ForeColor == new Color()) ? Color.White : ForeColor;
            b.BackColor = (BackColor == new Color()) ? Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32))))) : BackColor;
            b.UseVisualStyleBackColor = (b.BackColor == SystemColors.Control);
            return b;
        }
    }
    public partial class MyMessageBox : Form
    {
        private MyMessageBox()
        {
            this.panText = new FlowLayoutPanel();
            this.panButtons = new FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // panText
            // 
            this.panText.Parent = this;
            this.panText.AutoScroll = true;
            this.panText.AutoSize = true;
            this.panText.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.panText.Location = new Point(90, 90);
            this.panText.Margin = new Padding(0);
            this.panText.MaximumSize = new Size(500, 300);
            this.panText.MinimumSize = new Size(108, 50);
            this.panText.Size = new Size(108, 50);
            // 
            // panButtons
            // 
            this.panButtons.AutoSize = true;
            this.panButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.panButtons.FlowDirection = FlowDirection.RightToLeft;
            this.panButtons.Location = new Point(89, 89);
            this.panButtons.Margin = new Padding(0);
            this.panButtons.MaximumSize = new Size(580, 150);
            this.panButtons.MinimumSize = new Size(108, 0);
            this.panButtons.Size = new Size(108, 35);
            // 
            // MyMessageBox
            //
            this.BackColor = Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.AutoScaleDimensions = new SizeF(8F, 19F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(206, 133);
            this.Controls.Add(this.panButtons);
            this.Controls.Add(this.panText);
            this.Font = SystemFonts.MessageBoxFont;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Margin = new Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(168, 132);
            this.Name = "MyMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }
        public static string Show(Label Label, string Title = "", List<Button> Buttons = null, PictureBox Image = null)
        {
            List<Label> Labels = new List<Label>();
            Labels.Add(Label);
            return Show(Labels, Title, Buttons, Image);
        }
        public static string Show(string Label, string Title = "", List<Button> Buttons = null, PictureBox Image = null)
        {
            List<Label> Labels = new List<Label>();
            Labels.Add(MyLabel.Set(Label));
            return Show(Labels, Title, Buttons, Image);
        }
        public static string Show(List<Label> Labels = null, string Title = "", List<Button> Buttons = null, PictureBox Image = null)
        {
            if (Labels == null) Labels = new List<Label>();
            if (Labels.Count == 0) Labels.Add(MyLabel.Set(""));
            if (Buttons == null) Buttons = new List<Button>();
            if (Buttons.Count == 0) Buttons.Add(MyButton.Set("OK"));
            List<Button> buttons = new List<Button>(Buttons);
            buttons.Reverse();

            int ImageWidth = 0;
            int ImageHeight = 0;
            int LabelWidth = 0;
            int LabelHeight = 0;
            int ButtonWidth = 0;
            int ButtonHeight = 0;
            int TotalWidth = 0;
            int TotalHeight = 0;

            MyMessageBox mb = new MyMessageBox();

            mb.Text = Title;

            //Labels
            List<int> il = new List<int>();
            mb.panText.Location = new Point(9 + ImageWidth, 9);
            foreach (Label l in Labels)
            {
                mb.panText.Controls.Add(l);
                l.Location = new Point(200, 50);
                l.MaximumSize = new Size(480, 2000);
                il.Add(l.Width);
            }
            int mw = Labels.Max(x => x.Width);
            il.ToString();
            Labels.ForEach(l => l.MinimumSize = new Size(Labels.Max(x => x.Width), 1));
            mb.panText.Height = Labels.Sum(l => l.Height);
            mb.panText.MinimumSize = new Size(Labels.Max(x => x.Width) + mb.ScrollBarWidth(Labels), ImageHeight);
            mb.panText.MaximumSize = new Size(Labels.Max(x => x.Width) + mb.ScrollBarWidth(Labels), 300);
            LabelWidth = mb.panText.Width;
            LabelHeight = mb.panText.Height;

            //Buttons
            foreach (Button b in buttons)
            {
                mb.panButtons.Controls.Add(b);
                b.Location = new Point(3, 3);
                b.TabIndex = Buttons.FindIndex(i => i.Text == b.Text);
                b.Click += new EventHandler(mb.Button_Click);
            }
            ButtonWidth = mb.panButtons.Width;
            ButtonHeight = mb.panButtons.Height;

            //Set Widths
            if (ButtonWidth > ImageWidth + LabelWidth)
            {
                Labels.ForEach(l => l.MinimumSize = new Size(ButtonWidth - ImageWidth - mb.ScrollBarWidth(Labels), 1));
                mb.panText.Height = Labels.Sum(l => l.Height);
                mb.panText.MinimumSize = new Size(Labels.Max(x => x.Width) + mb.ScrollBarWidth(Labels), ImageHeight);
                mb.panText.MaximumSize = new Size(Labels.Max(x => x.Width) + mb.ScrollBarWidth(Labels), 300);
                LabelWidth = mb.panText.Width;
                LabelHeight = mb.panText.Height;
            }
            TotalWidth = ImageWidth + LabelWidth;

            //Set Height
            TotalHeight = LabelHeight + ButtonHeight;

            mb.panButtons.Location = new Point(TotalWidth - ButtonWidth + 9, mb.panText.Location.Y + mb.panText.Height);

            mb.Size = new Size(TotalWidth + 25, TotalHeight + 47);
            mb.ShowDialog();
            return mb.Result;
        }

        private FlowLayoutPanel panText;
        private FlowLayoutPanel panButtons;
        private int ScrollBarWidth(List<Label> Labels)
        {
            return (Labels.Sum(l => l.Height) > 300) ? 23 : 6;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Result = ((Button)sender).Text;
            Close();
        }

        private string Result = "";
    }
    public class Form1 : Form
    {

        private delegate void TimeDisplayUpdateCallback(string text);

        private IContainer components;

        public string CurrentlyOn;

        public static bool spawnEnemies;

        public static int MaxSkips;

        ContextMenu DummyMenu = new ContextMenu();

        public TextBox textBox1;

        public Button button1;
        public Button button2;
        public Button button3;

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label timeDisplay;

        private CheckBox checkBox1;
        private CheckBox checkBox2;

        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private RadioButton radioButton5;

        public static DateTime TimeRandomized;


        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private unsafe static extern bool WriteProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, int nSize, void* lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern int CloseHandle(IntPtr hObject);

        public static IntPtr MemoryOpen(int ProcessID)
        {
            return OpenProcess(2035711u, bInheritHandle: false, ProcessID);
        }

        public unsafe void Write(uint mAddress, byte[] Buffer, int ProcessID)
        {
            if (!(MemoryOpen(ProcessID) == (IntPtr)0))
            {
                WriteProcessMemory(MemoryOpen(ProcessID), mAddress, Buffer, Buffer.Length, null);
            }
        }

        public void TimeDisplayUpdate(string text)
        {
            if (timeDisplay.InvokeRequired)
            {
                TimeDisplayUpdateCallback method = TimeDisplayUpdate;
                try
                {
                    Invoke(method, text);
                    return;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
            }
            timeDisplay.Text = text;
            if (TimeSkip.TimeskipOrder == null)
            {
                CurrentlyOn = "A";
            }
            if (TimeSkip.TimeskipOrder.ElementAt(TimeSkip.currentSkip) != " ")
            {
                CurrentlyOn = TimeSkip.TimeskipOrder.ElementAt(TimeSkip.currentSkip);
            }
            else if (TimeSkip.TimeskipOrder.ElementAt(TimeSkip.currentSkip) == " " && TimeSkip.cutsceneID != 52)
            {
                CurrentlyOn = "Finished";
            }
            else
            {
                CurrentlyOn = "Return to the Security Room";
            }
            if (TimeSkip.RandomizerStarted == true)
            {
                label7.Text = $"Current case is: {CurrentlyOn}";
            }
            if (TimeSkip.RandomizerStarted == false)
            {
                label7.Text = "Current case is: 1-1 has not been triggered yet";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            TimeSkip.UpdateTimer.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = string.Empty;
        }
        public void disable_Radios(bool flag)
        {
            if (flag)
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                radioButton5.Enabled = false;
            }
            if (!flag)
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                radioButton5.Enabled = true;
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {
            Process[] processesByName = Process.GetProcessesByName("DeadRising");
            if (processesByName.Length == 0)
            {
                MyMessageBox.Show("The game process was not detected!\n\nPlease open the game.\r\n ", "Error");
                return;
            }
            TimeSkip.GameProcess = processesByName[0];
            label1.Text = string.Format("Connected to PID {0}", processesByName[0].Id.ToString("X8"));
            TimeSkip.UpdateTimer.Elapsed += TimeSkip.UpdateEvent;
            TimeSkip.UpdateTimer.AutoReset = true;
            TimeSkip.UpdateTimer.Enabled = true;
            TimeSkip.form = this;
        }
        public void button2_Click(object sender, EventArgs e)
        {

        }
        public void button3_Click(object sender, EventArgs e)
        {

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timeDisplay = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(7, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(144, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "The current time is:";
            // 
            // timeDisplay
            // 
            this.timeDisplay.AutoSize = true;
            this.timeDisplay.ForeColor = System.Drawing.Color.White;
            this.timeDisplay.Location = new System.Drawing.Point(169, 14);
            this.timeDisplay.Name = "timeDisplay";
            this.timeDisplay.Size = new System.Drawing.Size(53, 13);
            this.timeDisplay.TabIndex = 5;
            this.timeDisplay.Text = "<missing>";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(6, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Leave empty for random seed";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(9, 185);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(141, 20);
            this.textBox1.TabIndex = 7;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            this.textBox1.ContextMenu = DummyMenu;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.button2.FlatAppearance.BorderSize = 2;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(164, 153);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(73, 30);
            this.button2.TabIndex = 9;
            this.button2.Text = "Randomize";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(6, 170);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Input numbers for seed:";
            // 
            // label5
            // 
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 23);
            this.label5.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.checkBox1.ForeColor = System.Drawing.Color.White;
            this.checkBox1.Location = new System.Drawing.Point(9, 222);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(228, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Click to have Special Forces always active";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.checkBox2.ForeColor = System.Drawing.Color.White;
            this.checkBox2.Location = new System.Drawing.Point(9, 241);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(196, 17);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "Add Overtime to Timeskip?";
            this.checkBox2.UseVisualStyleBackColor = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(6, 258);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Seed = N/A";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(172, 195);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(64, 21);
            this.button3.TabIndex = 15;
            this.button3.Text = "Reveal";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(7, 274);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(224, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Current case is: Game must be connected";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(10, 51);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(67, 17);
            this.radioButton1.TabIndex = 17;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Timeskip";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            this.radioButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.radioButton1.ForeColor = System.Drawing.Color.White;
            this.radioButton1.Checked = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(10, 74);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(79, 17);
            this.radioButton2.TabIndex = 18;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Psychoskip";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            this.radioButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.radioButton2.ForeColor = System.Drawing.Color.White;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(10, 97);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(100, 17);
            this.radioButton3.TabIndex = 19;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "All Bosses";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            this.radioButton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.radioButton3.ForeColor = System.Drawing.Color.White;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(10, 120);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(113, 17);
            this.radioButton4.TabIndex = 20;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "All Survivors (TBA)";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            this.radioButton4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.radioButton4.ForeColor = System.Drawing.Color.White;
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Location = new System.Drawing.Point(10, 143);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(105, 17);
            this.radioButton5.TabIndex = 21;
            this.radioButton5.TabStop = true;
            this.radioButton5.Text = "All Scoops (TBA)";
            this.radioButton5.UseVisualStyleBackColor = true;
            this.radioButton5.CheckedChanged += new System.EventHandler(this.radioButton5_CheckedChanged);
            this.radioButton5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.radioButton5.ForeColor = System.Drawing.Color.White;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(241, 292);
            this.Controls.Add(this.radioButton5);
            this.Controls.Add(this.radioButton4);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.timeDisplay);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Skip Randomizer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        public void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.MaxLength = 9;
            if (textBox1.Text != "")
            {
                try
                {
                    int seedInput = Convert.ToInt32(textBox1.Text);
                    TimeSkip.Seed = seedInput;
                }
                catch (FormatException)
                {
                    textBox1.Text = "";
                    MyMessageBox.Show("Pasted seed contained non digit characters  \r\n ", "Error");
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 3 || e.KeyChar == 8 || e.KeyChar == 22);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            TimeRandomized = DateTime.UtcNow;
            TimeSkip.OnlyTriggerOnce = false;
            TimeSkip.CreateList();
            TimeSkip.RandomizerStarted = false;
            TimeSkip.FactsTriggered = false;
            if (textBox1.Text == "")
            {
                TimeSkip.SeedRandomizer();
            }
            TimeSkip.Randomize();
            TimeSkip.TimeskipOrder.Add(" ");
            TimeSkip.TimeskipOrder.Add(" ");
            TimeSkip.currentSkip = 0;
            MaxSkips = TimeSkip.TimeskipOrder.Count;
            textBox1.Text = null;
            button3.Text = "Reveal";
            label6.Text = $"Seed = {TimeSkip.Seed}";
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (TimeSkip.SelectedCategory == "Timeskip" && TimeSkip.includeOvertime == false && MaxSkips < 17 && TimeRandomized != DateTime.MinValue)
            {
                MyMessageBox.Show(TimeSkip.TimeskipOrder.ElementAt(0) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(1) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(2) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(3) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(4) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(5) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(6) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(7) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(8) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(9) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(10) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(11) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(12) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(13) + "\r\n" + "\r\n" + "Randomized: " + TimeRandomized);
                button3.Text = "Revealed";
            }
            if (TimeSkip.SelectedCategory == "Timeskip" && TimeSkip.includeOvertime == true && MaxSkips > 16 && TimeRandomized != DateTime.MinValue)
            {
                MyMessageBox.Show(TimeSkip.TimeskipOrder.ElementAt(0) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(1) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(2) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(3) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(4) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(5) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(6) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(7) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(8) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(9) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(10) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(11) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(12) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(13) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(14) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(15) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(16) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(17) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(18) + "\r\n" + "\r\n" + "Randomized: " + TimeRandomized);
                button3.Text = "Revealed";
            }
            if (TimeSkip.SelectedCategory == "Psychoskip" && TimeRandomized != DateTime.MinValue)
            {
                MyMessageBox.Show(TimeSkip.TimeskipOrder.ElementAt(0) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(1) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(2) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(3) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(4) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(5) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(6) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(7) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(8) + "\r\n" + "\r\n" + "Randomized: " + TimeRandomized + "\r\n ");
                button3.Text = "Revealed";
            }
            if (TimeSkip.SelectedCategory == "All Bosses" && TimeRandomized != DateTime.MinValue)
            {
                MyMessageBox.Show(TimeSkip.TimeskipOrder.ElementAt(0) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(1) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(2) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(3) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(4) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(5) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(6) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(7) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(8) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(9) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(10) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(11) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(12) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(13) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(14) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(15) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(16) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(17) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(18) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(19) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(20) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(21) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(22) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(23) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(24) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(25) + "\r\n" + TimeSkip.TimeskipOrder.ElementAt(26) + "\r\n" + "\r\n" + "Randomized: " + TimeRandomized);
                button3.Text = "Revealed";
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                spawnEnemies = true;
            }
            else
            {
                spawnEnemies = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                TimeSkip.includeOvertime = true;
            }
            else
            {
                TimeSkip.includeOvertime = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            TimeSkip.SelectedCategory = "Timeskip";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            TimeSkip.SelectedCategory = "Psychoskip";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            TimeSkip.SelectedCategory = "All Bosses";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            TimeSkip.SelectedCategory = "All Survivors";
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            TimeSkip.SelectedCategory = "All Scoops";
        }
        private void label6_Click(object sender, EventArgs e)
        {
            Clipboard.SetText($"{TimeSkip.Seed}");
        }
    }
}