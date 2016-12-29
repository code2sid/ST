using STLibs;
using System;
using System.Windows.Forms;

namespace CodeGeneration
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string code = string.Empty;



        private void Form1_Load(object sender, EventArgs e)
        {
            code = string.Format("01-{1}-{0}~{2}-{1}-{0}"
               , DateTime.Today.Year
               , DateTime.Today.ToString("MMM")
               , DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));

            label2.Text = code;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = string.Format("{0}~{1}", code, textBox1.Text);
            textBox1.Text = Security.Encrypt(string.Format("{0}~{1}", textBox1.Text, code), STLibs.Utilitiy.passKey);
            //var dt = Utilitiy.GetNistTime();
            //var dt2 = dateTimePicker1.Value;
            //textBox1.Text = DateTime.Compare(dt, dt2).ToString();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            var dt = dateTimePicker1.Value;

            code = string.Format("{5}-{1}-{0}~{2}-{3}-{4}"
              , DateTime.Today.Year
              , DateTime.Today.ToString("MMM")
              , dt.Day
              , dt.ToString("MMM")
              , dt.Year
              , DateTime.Today.Day);

            label2.Text = code;
        }
    }
}
