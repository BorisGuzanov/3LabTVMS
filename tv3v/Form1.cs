using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Drawing;
//using System.Threading.Tasks;
using MaterialSkin.Controls;
using MaterialSkin;
//using System.Runtime.dll

namespace tv3v
{
    
    public partial class Form1 : MaterialForm
    {
        List<RandVar> eta;// случайная величина
        List<RandVar> seriesdist;// ряд распределения
        double p;
        double q;
        double u;//случайная величина 
        int ValueTuple;
        public class RandVar
        {
            public int val;
            public double count;
            public RandVar(int _val, double _count)
            {
                val = _val;
                count = _count;
            }
        }
        public Form1()
        {
            var Skinmanager = MaterialSkinManager.Instance;
            Skinmanager.AddFormToManage(this);
            Skinmanager.Theme = MaterialSkinManager.Themes.LIGHT;
            Skinmanager.ColorScheme = new ColorScheme(Primary.Red900, Primary.BlueGrey900, Primary.BlueGrey600,Accent.Orange400,TextShade.BLACK);
            InitializeComponent();
        }
        private double CheckSum(double sum, double pj)
        {
            double tmp1_sum = sum;
            double tmp2_sum = sum;
            tmp1_sum = Math.Truncate(1000000 * tmp1_sum) / 1000000;
            tmp2_sum = tmp2_sum + pj;
            tmp2_sum = Math.Truncate(1000000 * tmp2_sum) / 1000000;
            if (tmp1_sum == tmp2_sum)
                return 1;
            return tmp2_sum;
        }
        private List<RandVar> Simplify(List<RandVar> list, int j)
        {
            for (int i = 0;i<list.Count;i++)
            {
                if(list[i].val == j)
                {
                    var tmp = list[i];
                    tmp.count++;
                    list[i] = tmp;
                    return list;
                }
            }
            RandVar ker = new RandVar(j,1);
            list.Add(ker);
            return list;
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();       
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            materialFlatButton1_Click(sender, e);
            //for (int j = 0;j < ValueTuple;j++)
            //DataGridView dataGridView1 = new DataGridView();
            //DataGridViewTextBoxColumn column0 = new DataGridViewTextBoxColumn();
            //column0.Name = "id";
            //column0.HeaderText = "ID";

            //DataGridViewTextBoxColumn column= new DataGridViewTextBoxColumn();
            //dataGridView1.Columns.AddRange(column0);
            //this.Controls.Add(dataGridView1);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            ValueTuple = int.Parse(textBox2.Text);
            p = double.Parse(textBox1.Text);
            q = 1 - p;
            eta = new List<RandVar>();
            seriesdist = new List<RandVar>();



            double pj1;
            for (int i = 0; i < ValueTuple; i++)
            {
                int j = 1;
                pj1 = p;
                double sum = pj1;
                u = rand.NextDouble();
                /*
                while (u > p)
                {
                    u = rand.NextDouble();
                    j++;
                }
                eta = Simplify(eta, j);
                */
                while (true)
                {
                    if (sum > u)
                    {
                        eta = Simplify(eta, j);
                        break;
                    }
                    pj1 = pj1 * q;
                    sum = CheckSum(sum, pj1);
                    j++;
                    if (sum == 0)
                    {
                        eta = Simplify(eta, j);
                        break;
                    }
                }
            }
            RandVar tmp = new RandVar(1, p);
            seriesdist.Add(tmp);
            for (int i = 1; i < 17; i++)
            {
                double P = seriesdist[i - 1].count * q;
                RandVar tmp2 = new RandVar(i+1, P);
                seriesdist.Add(tmp2);
            }
            eta.Sort((x, y) => x.val.CompareTo(y.val));
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowCount = 3;
            dataGridView1.ColumnCount = eta.Count;
            for (int i = 0; i < eta.Count; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = eta[i].val;
                dataGridView1.Rows[1].Cells[i].Value = eta[i].count;
                dataGridView1.Rows[2].Cells[i].Value = (eta[i].count + "/" + ValueTuple);
            }
            Graphics form2 = new Graphics(eta, seriesdist, ValueTuple, p);
            form2.Draw();
            form2.Show();
        }

        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            Гипотеза form3 = new Гипотеза(ValueTuple,seriesdist,eta);
            form3.Show();
        }
    }
}
