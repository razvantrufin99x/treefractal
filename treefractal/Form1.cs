using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace treefractal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        public float angle = 30.0f;
        public float distance = 90.0f;
        public float x1 = 220.0f;
        public float y1 = 250.0f;
        public float x2 = 112.0f;
        public float y2 = 121.0f;
        public float speed = 9.0f;
        public float delta = 20.0f;
        public static float _currentX = 300.0f;
        public static float _currentY = 222.0f;
        public int countermoves = 0;
        public int counterturns = 0; 

        public void move(float x)
        {
            Thread.Sleep(50);
            countermoves++;
            float num = (float)(angle / 180.0 * System.Math.PI);
            float newY = _currentY - distance * (float)System.Math.Cos(num);
            float newX = _currentX + distance * (float)System.Math.Sin(num);
            x1 = _currentX;
            y1 = _currentY;
                 x2 = newX;
                 y2 = newY;
                 try
                 {
                     this.userControl11.g.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
                 }
                 catch { }
            _currentX = newX;
            _currentY = newY;
            
         
        }
        public void turn(float x)
        {
            angle += x;
            counterturns++;
        }

        public void drawtree()
        {
            
            if (distance > 0 && countermoves < 100)
            {
                userControl11.Refresh();
                Text = countermoves.ToString() + " : " + counterturns.ToString();
                
                move(distance);
                turn(angle);
                distance -= delta;
                drawtree();
                
                turn(-angle * 2);
                drawtree();
                
                turn(angle);
                move(-distance);

                this.textBox1.Text += " \r\n " +
                x1.ToString() + " : " + y1.ToString() + " : " +
                x2.ToString() + " : " + y2.ToString();

            }
            else if (distance <= 0 && countermoves < 100)
            {
                distance = 90.0f;
                _currentX = 300.0f;
                _currentY = 222.0f;
            }
            else
            {
                Thread.Sleep(1);
                MessageBox.Show("close");
                Application.Exit();
            }

        }

        public void moveto(float x, float y)
        {
            x2 = x1;
            y2 = y1;
            float num = (x - x1) * (x - x1) + (y - y1) * (y - y1);
            if (num != 0.0f)
            {
                float num2 = (float)System.Math.Sqrt(num);
                float num3 = y1 - y;
                float num4 = (float)(System.Math.Acos(num3 / num2) * 180.0 / System.Math.PI);
                if ((bool)(x < x1))
                {
                    num4 = 360.0f - num4;
                }
                float num5 = num4 - (float)((int)angle % 360);
                if (num5 > 180.0)
                {
                    num5 -= 360.0f;
                }
                turn(num5);
                move(num2);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
           
            drawtree();
            userControl11.Refresh();
            
        }
    }
}
