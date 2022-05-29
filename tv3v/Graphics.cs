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


namespace tv3v
{
    public partial class Graphics : MaterialForm
    {
        List<Form1.RandVar> SeriesDist;// ряд распределения
        List<Form1.RandVar> SampleSeries;//выборочный ряд распределения 
        int n;//количество экспериментов
        LineItem myCurve1;
        LineItem myCurve2;
        double p;
        double q;
        public Graphics(List<Form1.RandVar> eta_, List<Form1.RandVar> seriesdist_, int n_, double p_)
        {
            SeriesDist =new List<Form1.RandVar>(seriesdist_);
            SampleSeries =new List<Form1.RandVar>( eta_);
            n = n_;
            p = p_;
            q = 1 - p;
            for(int i = 0; i < SampleSeries.Count; i++)
              {
                var tmp = SampleSeries[i];
                tmp.count = tmp.count / n;
                SampleSeries[i] = tmp;
            }
            InitializeComponent();
        }
        public Graphics()
        {
            InitializeComponent();
        }
        private double SampleMean()//выборочное среднее
        {
            double res = 0;
            for (int i = 0;i < SampleSeries.Count; i++)
            {
                res += SampleSeries[i].val*SampleSeries[i].count;
            }
            return res;
        }
        private double SampleVar()
        {
            double res = 0;
            double M = SampleMean();
            for (int i = 0; i < SampleSeries.Count; i++)
            {                
                res += (SampleSeries[i].val - M) * (SampleSeries[i].val - M) * SampleSeries[i].count;
            }
            return res;
        }
        private double Mediana()
        {
            double res = SampleSeries[0].count;
            double k = 0.5;
            int i = 1;
            if (n%2==1)//нечетное количество экспериментов
            {
                while((res<0.5)&&(i<SampleSeries.Count))//возможно выходим за индексы, обычно при вычислении медианы
                {
                    res += SampleSeries[i-1].count;
                    i++;
                }                
                return SampleSeries[i-1].val;
            }
            else//четное количество экспериментов
            {
                int v1 = 0, v2 = 0;
                while ((res < 0.5) && (i < SampleSeries.Count))
                {
                    res += SampleSeries[i].count;
                    i++;
                    v1++;
                    v2++;
                }
                if(res == 0.5)
                {
                    v2 = v1 + 1;
                }
                return (SampleSeries[v1].val + SampleSeries[v2].val) / 2.0;
            }
        }
        private double ScopeSample()//размах выборки
        {
            return (SampleSeries[SampleSeries.Count-1].val - SampleSeries[0].val);
        }
        private void DrawMyTable2()
        {
            dataGridView1.RowHeadersVisible = false;
            pictureBox1.BringToFront();
           
            dataGridView1.RowCount = 1;
            dataGridView1.ColumnCount = 8;
            dataGridView1.Rows[0].Cells[0].Value = 1.0/p;//мат. ожидание
            dataGridView1.Rows[0].Cells[1].Value = SampleMean();//выборочное среднее
            dataGridView1.Rows[0].Cells[2].Value = Math.Abs(1.0/p - SampleMean());
            dataGridView1.Rows[0].Cells[3].Value = p/(q*q);//дисперсия
            dataGridView1.Rows[0].Cells[4].Value = SampleVar();//выборочная дисперсия
            dataGridView1.Rows[0].Cells[5].Value = Math.Abs(p / (q * q) - SampleVar());
            dataGridView1.Rows[0].Cells[6].Value = Mediana();//медиана
            dataGridView1.Rows[0].Cells[7].Value = ScopeSample();//размах выборки
            DataGridViewColumn column1 = dataGridView1.Columns[0];
            column1.Width = 45;
            DataGridViewColumn column2 = dataGridView1.Columns[1];
            column2.Width = 45;
            DataGridViewColumn column3 = dataGridView1.Columns[2];
            column3.Width = 92;
            DataGridViewColumn column4 = dataGridView1.Columns[3];
            column4.Width = 45;
            DataGridViewColumn column5 = dataGridView1.Columns[4];
            column5.Width = 45;
            DataGridViewColumn column6 = dataGridView1.Columns[5];
            column6.Width = 107;
            DataGridViewColumn column7 = dataGridView1.Columns[6];
            column7.Width = 50;
            DataGridViewColumn column8 = dataGridView1.Columns[7];
            column8.Width = 45;
        }
        private void DrawMyTable3()
        {
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.RowCount = 4;
            dataGridView2.ColumnCount = SampleSeries.Count;
            for (int i = 0; i < SampleSeries.Count; i++)
            {
                if(i==7)
                {
                    i = 7;
                }
                dataGridView2.Rows[0].Cells[i].Value = SampleSeries[i].val;
                dataGridView2.Rows[1].Cells[i].Value = SeriesDist[SampleSeries[i].val].count;
                dataGridView2.Rows[2].Cells[i].Value = SampleSeries[i].count;
            }
        }
        private double F(double x_)
        {
            double sum = 0;
            if (x_ <= 0)
            {
                return 0;
            }      
            if(x_ > SeriesDist[SeriesDist.Count-1].val)
            {
                return 1;
            }
            for(int i = 0;i < SeriesDist.Count;i++)
            {
                if(x_<=SeriesDist[i].val)
                {
                    return sum;
                }
                sum = sum + SeriesDist[i].count;
            }
            return -1;
        }
        private double F_(double x_)
        {
            double sum = 0;
            if (x_ <= 0)
            {
                return 0;
            }
            if (x_ > SampleSeries[SampleSeries.Count - 1].val)
            {
                return 1;
            }
            for (int i = 0; i < SampleSeries.Count; i++)
            {
                if (x_ <= SampleSeries[i].val)
                {
                    return sum;
                }
                sum = sum + SampleSeries[i].count;
            }
            return -1;
        }
        public void Draw()
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.CurveList.Clear();
            pane.Title.Text = "Графики выборочной и теоретической функций распределения";
            pane.XAxis.Title.Text = "";
            pane.YAxis.Title.Text = "";
            PointPairList pointlist1 = new PointPairList();
            PointPairList pointlist2 = new PointPairList();
            double xmin = 0;
            double xmax = SeriesDist[SeriesDist.Count - 1].val;
            for(double x = xmin;x <= xmax;x+=0.001)
            {
                pointlist1.Add(x, F(x));
                pointlist2.Add(x, F_(x));
            }
            myCurve1 = pane.AddCurve("F(x) - теоретическая ф-я распр-я", pointlist1, Color.Blue, SymbolType.None);
            myCurve2 = pane.AddCurve("F*(x) - выборочная ф-я распр-я", pointlist2, Color.Red, SymbolType.None);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            myCurve1.IsVisible = true;
            myCurve2.IsVisible = true;
            DrawMyTable2();
            DrawMyTable3();
            int k = 0;
            double Max = MeasureDisc(ref k);
            materialLabel2.Text += Convert.ToString(Math.Round(Max, 6)) + " на [ " + Convert.ToString(k) + " ; " + Convert.ToString(k + 1) + " ]";
            materialLabel1.Text += Convert.ToString(Math.Round(MaximumDeviation(ref k),6)) + " при значении " + Convert.ToString(k);
        }
        private double MeasureDisc(ref int k)
        { //вычисляем меру расхождения
            double result = 0;
            for (int x = 0; x < SeriesDist.Count; x++)
            {
                double point = x + 1 / 2.0;
                double prom_result = Math.Abs(F(point) - F_(point));
                if (prom_result > result)
                {
                    result = prom_result;
                    k = x;
                }
            }
            return result;
        }
        private double MaximumDeviation(ref int k)
        { //максимальное отклонение
            double result = 0;
            for (int i = 0; i < SampleSeries.Count; i++)
            {
                double prom_result = Math.Abs(SeriesDist[SampleSeries[i].val].count - SampleSeries[i].count);
                if (prom_result > result)
                {
                    result = prom_result;
                    k = SampleSeries[i].val;
                }
            }
            return result;
        }
        private void Graphics_Load(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
