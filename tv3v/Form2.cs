using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using MaterialSkin;
using ZedGraph;
using MathNet.Numerics;


namespace tv3v
{
    public partial class Гипотеза : MaterialForm
    {
        double alpha;
        List<Form1.RandVar> SeriesDist;// ряд распределения
        List<Form1.RandVar> SampleSeries;//выборочный ряд распределения 
        double n, k;
        double R0;
        double FR0;
        int right = 17;
        public Гипотеза(int n_, List<Form1.RandVar> SeriesDist_, List<Form1.RandVar> SampleSeries_)
        {
            n = n_;
            SeriesDist = SeriesDist_;
            SampleSeries = SampleSeries_;
            InitializeComponent();
        }

        private double Factorial2(double x)
        {
            double r;
            r = SpecialFunctions.Gamma(x + 1);
            return r;
        }

        private int GetNj(int i, int j)
        {

            int value = 0;
            for (int k = 0; k < SampleSeries.Count; k++)
            {
                //int tmp = Convert.ToInt32(data.Rows[0].Cells[k].Value);
                if (i <= SampleSeries[k].val && SampleSeries[k].val < j)
                    value += (int)SampleSeries[k].count;
            }
            return value;
        }
        private double Calculate_R0()
        {
            double result = 0;
            for (int i = 0; i < k; i++)
            {
                int i1 = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value);
                int i2 = (i != k - 1) ? Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value) : right;
                int nj = GetNj(i1, i2);
                double qj = Convert.ToDouble(dataGridView2.Rows[1].Cells[i].Value);
                double tmp = (nj - n * qj) * (nj - n * qj) / (n * qj);
                result = result + tmp;
            }
            return result;
        }
        private double f(double x)
        {
            //var chart = new Chart();
            double r = k - 1;

            return (Math.Pow(2, -r / 2.0) * 1 / (Factorial2(r/2)) * Math.Pow(x, r / 2.0 - 1) * Math.Pow(Math.E, -x / 2.0));
        }
        private double F()
        {
            return (1 - Integrate());
        }

        private double Integrate()
        {
            double result = 0;
            for (int i = 2; i <= 1005; i++)
            {
                result += (f(R0 * (i - 1) / 1000.0) + f(R0 * i / 1000.0)) * R0 / (2 * 1000.0);
            }
            return result;
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            alpha = Convert.ToDouble(materialSingleLineTextField2.Text);
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.RowCount = 2;
            dataGridView2.ColumnCount = (int)k;
            for(int i = 0;i < k; i++)
            {
                dataGridView2.Rows[0].Cells[i].Value = "q" + Convert.ToString(i + 1);
                double value = 0;
                for (int j = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value); j < ((i == k - 1) ? 17 : Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value)); j++)
                {
                    double tmp = SeriesDist[j].count;
                    value += tmp;
                }
                dataGridView2.Rows[1].Cells[i].Value = value;
            }
            R0 = Calculate_R0();
            materialLabel5.Text += R0;
            FR0 = F();
            materialLabel6.Text += FR0;
            if (FR0 >= alpha)
            {
                materialLabel7.Text = "Гипотеза:";
                materialLabel7.Text += " принята";
                materialLabel7.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                materialLabel7.Text = "Гипотеза:";
                materialLabel7.Text += " не принята";
                materialLabel7.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void Гипотеза_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            //this.materialSingleLineTextField1.Text = "1";
            if (materialSingleLineTextField1.Text == "")
            {
                k = 0;
            }
            else
            {
                k = int.Parse(materialSingleLineTextField1.Text);
            }
            dataGridView1.RowCount = (int)k;
            dataGridView1.ColumnCount = 3;
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = true;
            //this.dataGridView1.RowCount = int.Parse(this.materialSingleLineTextField1.Text);
            for (int i = 0;i < k;i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = i + 1;
            }          
        }

       
    }
}
