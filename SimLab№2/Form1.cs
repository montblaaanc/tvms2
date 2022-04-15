using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SimLab_2
{
    public partial class Form1 : Form
    {
        Random rand = new Random();
        public Form1()
        {
            InitializeComponent();
            
        }
        public double F(double x)
        {
            var result = new MathNet.Numerics.Distributions.Normal();
            return result.InverseCumulativeDistribution(x);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var quantOfseries = Convert.ToInt32(textBox2.Text);
            var quantOfexper = Convert.ToInt32(textBox1.Text);
            var time = Convert.ToInt32(textBox4.Text);

            var q = (double)1 - ((double)time / (double)60);
            var P = (double)1 - q * q;
            var Q = 1 - P;


            chart1.ChartAreas[0].AxisX.Maximum = quantOfexper;
            chart1.ChartAreas[0].AxisX.Minimum = 1;
            chart2.ChartAreas[0].AxisX.Maximum = quantOfexper;
            chart2.ChartAreas[0].AxisX.Minimum = 1;

            chart1.ChartAreas[0].AxisX.IsLogarithmic = true;
            chart2.ChartAreas[0].AxisX.IsLogarithmic = true;
            if (checkBox1.Checked != true)
            {
                chart1.ChartAreas[0].AxisX.IsLogarithmic = false;
                chart2.ChartAreas[0].AxisX.IsLogarithmic = false;
            }
            Create_graf(quantOfseries, quantOfexper, time);
            Create_graf_task2(quantOfseries, quantOfexper, time, P, Q);
            Rezult_Group(quantOfexper, P);

        }

        private void Rezult_Group(int quantOfexper,double P)
        {
            var last_rez = (decimal)Math.Round(chart1.Series["Average"].Points[quantOfexper - 1].YValues[0], 15);

            label1.Text = "Результат:" + last_rez + " ± " + Math.Round(chart2.Series[1].Points[quantOfexper - 1].YValues[0], 4);
            label2.Text = "Отклонение:" + Math.Abs(last_rez - (decimal)P);

            chart2.ChartAreas[0].AxisX.Title = "Номер эксперемента";
            chart2.ChartAreas[0].AxisY.Title = "Погрешность";

            chart1.ChartAreas[0].AxisX.Title = "Номер эксперемента";
            chart1.ChartAreas[0].AxisY.Title = "Вероятность";
        }

        private void Create_graf_task2(int quantOfseries, int quantOfexper, int time, double P, double Q)
        {
            chart1.Series.Add(new Series
            {
                Name = "Average",
                ChartType = SeriesChartType.Spline,
                BorderWidth = 2,
                Color = Color.GreenYellow
            });

            chart1.Series.Add(new Series
            {
                Name = "Accuracy",
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                Color = Color.Gold
            });

            chart1.Series.Add(new Series
            {
                Name = "Accuracy2",
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                Color = Color.Gold
            });

            foreach (var b in chart2.Series) b.Points.Clear();

            double y, f = F(Convert.ToDouble(textBox3.Text) / 2 + 0.5);
            var NValues = new List<double>();
            var AmountToDel = (int)((1 - Convert.ToDouble(textBox3.Text)) / 2 * quantOfseries);

            for (int i = 0; i < quantOfexper; i++)
            {
                y = 0;
                NValues.Clear();
                for (int j = 0; j < quantOfseries; j++)
                {
                    y += chart1.Series[j].Points[i].YValues[0];
                    NValues.Add(chart1.Series[j].Points[i].YValues[0]);
                }

                NValues.Sort();
                NValues.RemoveRange(NValues.Count - AmountToDel, AmountToDel);
                NValues.RemoveRange(0, AmountToDel);

                chart1.Series["Accuracy"].Points.AddXY(i + 1, NValues[0]);
                chart1.Series["Accuracy2"].Points.AddXY(i + 1, NValues[NValues.Count - 1]);

                chart2.Series[2].Points.AddXY(i + 1, (NValues[NValues.Count - 1] - NValues[0]) / 2);

                var sred = y / (double)quantOfseries;

                chart1.Series["Average"].Points.AddXY(i + 1, sred);
                chart2.Series[0].Points.AddXY(i + 1, Math.Abs(sred - P));
                chart2.Series[1].Points.AddXY(i + 1, f * Math.Sqrt(P*Q / (i + 1)));
            }

        }

        private void Create_graf(int quantOfseries, int quantOfexper, int time)
        {
            chart1.Series.Clear();
            foreach (var v in chart3.Series) v.Points.Clear();

            int good;
            double stud1, stud2;
            for (int i = 0; i < quantOfseries; i++) // Количество серий
            {
                good = 0;
                chart1.Series.Add(new Series
                {
                    Name = i.ToString(),
                    ChartType = SeriesChartType.Line,
                    Color = Color.Gray
                });

                for (int j = 0; j < quantOfexper; j++) // Количество опытов
                {

                    stud1 = rand.Next(61*60);
                    stud2 = rand.Next(61*60);
                    if (Math.Abs(stud1-stud2) <= time*60) // Проверка на выпадение орла
                    {
                        good++;
                        chart3.Series[0].Points.AddXY(stud1/60,stud2/60);
                    }
                    else chart3.Series[1].Points.AddXY(stud1/60, stud2/60);
                    chart1.Series[i].Points.AddXY(j + 1, (double)good / (double)(j + 1));
                }
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked != true)
            {
                chart1.ChartAreas[0].AxisX.IsLogarithmic = false;
                chart2.ChartAreas[0].AxisX.IsLogarithmic = false;
                var a = Convert.ToInt32(textBox1.Text);
                chart1.ChartAreas[0].AxisX.Maximum = a;
                chart2.ChartAreas[0].AxisX.Maximum = a;
            }
            else
            {
                chart1.ChartAreas[0].AxisX.IsLogarithmic = true;
                chart2.ChartAreas[0].AxisX.IsLogarithmic = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0 && Convert.ToDouble(textBox3.Text) < 1 && Convert.ToDouble(textBox3.Text) > 0 && Convert.ToDouble(textBox4.Text) > 0 && Convert.ToDouble(textBox4.Text) < 61)
            {
                button1.Enabled = true;
                
            }
            else
            {
                button1.Enabled = false;
                
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var time = Convert.ToInt32(textBox4.Text);

            var q = (double)1 - ((double)time / (double)60);
            var P = (double)1 - q * q;
            var Q = 1 - P;
            var i = 1;
            double f = F(Convert.ToDouble(textBox3.Text) / 2 + 0.5);
            double y = f * Math.Sqrt(P * Q / 1);
            while(y >= Convert.ToDouble(textBox5.Text))
            {
                i++;
                y = f * Math.Sqrt(P * Q / i);
            }
            textBox6.Text = i.ToString();
        }
    }
}
