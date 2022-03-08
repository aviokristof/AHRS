/*
 * Attitude and heading reference system using complementary filter
 * by Krzysztof Ragan
 * 
 * 
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        /*
         * Global variables
        */
        double Ax, Ay, Az, Gx, Gy, Gz, Mx, My, Mz, Gx_radians, Gy_radians, Gz_radians;
        double phi_acc, theta_acc, phi_gyro, theta_gyro, psi_gyro;
        double sum_phi, sum_theta, phi_kom, theta_kom, sum_psi, psi_kom, psi_kom_deg;
        double phi_acc_deg, theta_acc_deg, phi_kom_deg, theta_kom_deg, sum_phi_deg, sum_theta_deg,psi_mag,psi_mag_deg, sum_psi_deg;
        double phi_gyro_deg, theta_gyro_deg, psi_gyro_deg;
        double chart1_data, chart2_data;
        double time = 0;
        double result;
        string data=null;
        double chart_phi_flag = 0, chart_theta_flag = 0, chart_psi_flag = 0;
        string[]buffor=new string [6];
        string[] frame = new string[8];
        int test = 0;
        //complementary filter coefficients
        static double t = 0.1;
        static double T_phi = 0.3;
        static double T_theta = 0.3;
        static double T_psi = 0.3;
        double alpha_phi = Math.Exp(-t / T_phi);
        double alpha_theta = Math.Exp(-t / T_theta);
        double alpha_psi = Math.Exp(-t / T_psi);
        SerialPort myserialPort = new SerialPort("COM4",38400,Parity.None,8,StopBits.One);
        private Thread thread;
        /*
         * End of global variables         
        */
        public Form1()
        {
            InitializeComponent();
        }
        private void Start_Click(object sender, EventArgs e)
        {
            /*
             * Starting program via button "Start"
             */
            chart_phi_flag = 1;
            myserialPort.Open();
            StartThread();
            timer1.Start();
        }
        private void Stop_Click(object sender, EventArgs e)
        {      
            /*
             * Stops program via button "Stop"
             */
            myserialPort.Close();
            thread.Abort();
            timer1.Stop();
            test = 0;
        }
        





        public void StartThread ()
        {
            /*
             * Starts function "ThreadLoop" and sets labels parameters
             */

            label1.Font = new Font("Trebuchet MS", 60, FontStyle.Bold);
            this.chart1.Titles.Add("Filtered");
            chart1.Titles[0].Font = new Font("Trebuchet MS", 16, FontStyle.Bold);
            this.chart2.Titles.Add("Not filtered");
            chart2.Titles[0].Font = new Font("Trebuchet MS", 16, FontStyle.Bold);
            //creates and starts the thread
            thread = new Thread(ThreadLoop);
            thread.Start();
        }
        public void ThreadLoop()
        {
            /*
             * Main thread
             */
            while (myserialPort.IsOpen)
            {
                /*
                 * Loop reads data via UART from STM32 microcontroller
                 * Calculate Euler Angles using complementary filter
                 */
                
                while (test<9)
                {
                    /*
                     * Delay loop
                     */
                    data = myserialPort.ReadTo("\n");
                    test++;
                }

                time = time + 0.1;
                time = Math.Round(time,1);
                
                /*
                 * Reading frame from UART and changing it to double variables
                 */

                data = myserialPort.ReadTo("\n");
                frame = data.Split(';');
                Ax = Convert.ToDouble(frame[0], new CultureInfo("en-US"));// raw value of acceleration in axis X
                Ay = Convert.ToDouble(frame[1], new CultureInfo("en-US"));// raw value of acceleration in axis Y
                Az = Convert.ToDouble(frame[2], new CultureInfo("en-US"));// raw value of acceleration in axis Z
                Gx = Convert.ToDouble(frame[3], new CultureInfo("en-US"));// raw value of angular velocity in axis X
                Gy = Convert.ToDouble(frame[4], new CultureInfo("en-US"));// raw value of angular velocity in axis Y
                Gz = Convert.ToDouble(frame[5], new CultureInfo("en-US"));// raw value of angular velocity in axis Z
                Mx = Convert.ToDouble(frame[6], new CultureInfo("en-US"));// raw value of magnetometer heading in axis X
                My = Convert.ToDouble(frame[7], new CultureInfo("en-US"));// raw value of magnetometer heading in axis Y
                Mz = Convert.ToDouble(frame[8], new CultureInfo("en-US"));// raw value of magnetometer heading in axis Z

                /*
                 * End of reading from UART
                 */
                
                /*
                 * Recalculating angular velocity values from degrees to radians
                 */
                Gx_radians = Gx * (Math.PI/180);//recalculated angular velocity in axis X
                Gy_radians = Gy * (Math.PI / 180);//recalculated angular velocity in axis Y
                Gz_radians = Gz * (Math.PI / 180);//recalculated angular velocity in axis Z

                /*
                 * End of recalculating angular velocity values from degrees to radians
                 */


                /*
                 * Complementary filter 
                 */
                phi_acc = Math.Atan2(Ay, Az);//atan2(ay/az)
                theta_acc = Math.Atan2(-(Ax), ((Ay * Math.Sin(phi_kom)) + (Az * Math.Cos(phi_kom))));

                phi_gyro = Gx_radians + (Gy_radians * Math.Sin(phi_kom) * Math.Tan(theta_kom)) + (Gz_radians * Math.Cos(phi_kom) * Math.Tan(theta_kom));
                theta_gyro = (Gy_radians * Math.Cos(phi_kom))- (Gz_radians * Math.Sin(theta_kom));
                psi_gyro = Gx_radians + (Gy_radians * Math.Sin(phi_kom) * (1 / Math.Cos(theta_kom))) + (Gz_radians * Math.Cos(phi_kom) * (1 / Math.Cos(theta_kom)));

                psi_mag = Math.Atan2((Mz*Math.Sin(phi_kom))-(My*Math.Cos(phi_kom)),(Mx*Math.Cos(theta_kom))+(My*Math.Sin(theta_kom) *Math.Sin(phi_kom))+(Mz*Math.Sin(theta_kom) *Math.Cos(phi_kom)));


                sum_phi = phi_acc + (phi_gyro * T_phi);
                sum_theta = theta_acc + (theta_gyro * T_theta);
                sum_psi = psi_mag + (psi_gyro * T_psi);

                phi_kom = (phi_kom * alpha_phi) + ((1 - alpha_phi) * sum_phi);
                theta_kom = (theta_kom * alpha_theta) + ((1 - alpha_theta) * sum_theta);
                psi_kom = (psi_kom * alpha_psi) + ((1 - alpha_psi) * sum_psi);

                /*
                 * End of complementary filter calculations
                 */

                /*
                 * Recalculating values from radians to degrees to plot in charts
                 */

                phi_acc_deg = phi_acc * (180 / Math.PI);//radians to degrees
                theta_acc_deg = theta_acc * (180 / Math.PI);//radians to degrees
                psi_mag_deg = psi_mag * (180 / Math.PI);//radians to degrees

                phi_gyro_deg = phi_gyro * (180 / Math.PI);//radians to degrees
                theta_gyro_deg = theta_gyro * (180 / Math.PI);//radians to degrees
                psi_gyro_deg = psi_gyro * (180 / Math.PI);//radians to degrees

                sum_phi_deg = sum_phi * (180 / Math.PI);//radians to degrees
                sum_theta_deg = sum_theta * (180 / Math.PI);//radians to degrees
                sum_psi_deg= sum_psi* (180 / Math.PI);//radians to degrees

                //final values from complementary filter
                phi_kom_deg = phi_acc * (180 / Math.PI);//radians to degrees
                theta_kom_deg = theta_acc * (180 / Math.PI);//radians to degrees
                psi_kom_deg = psi_kom * (180 / Math.PI);//radians to degrees

                /*
                 * End of recalculating values from radians to degrees to plot in charts
                 */

                Thread.Sleep(100);
               
            }
            
         
        }


        private void Phi_CheckedChanged(object sender, EventArgs e)
        {
            /*
             * Radio button "Phi" changing ploting series on charts to "Phi" series. Prints phi Euler angle values in degrees.
             */

            chart_phi_flag = 1;

        }

        private void Theta_CheckedChanged(object sender, EventArgs e)
        {
            /*
             * Radio button "Theta" changing ploting series on charts to "Theta" series. Prints theta Euler angle values in degrees.
             */
            chart_theta_flag = 1;

        }

        private void Psi_CheckedChanged(object sender, EventArgs e)
        {
            /*
             * Radio button "Psi" changing ploting series on charts to "Psi" series. Prints psi Euler angle values in degrees.
             */
            chart_psi_flag = 1;

        }
                                    
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            /*
             * Timer used to plot variables in charts
             */

            // Chart 1
            chart1.ChartAreas[0].AxisX.Title = "Time [ s ]"; // Chart1 X axis Title name
            chart1.ChartAreas[0].AxisY.Title = "Degrees [ ° ]"; // Chart1 Y axis Title name
            chart1.ChartAreas[0].AxisX.TitleFont = new Font("Trebuchet MS", 10, FontStyle.Bold); // Chart1 X axis Title Font
            chart1.ChartAreas[0].AxisY.TitleFont = new Font("Trebuchet MS", 10, FontStyle.Bold); // Chart1 Y axis Title Font
            chart1.ChartAreas[0].RecalculateAxesScale();

            if (chart_phi_flag == 1)
            {
                chart1.Series["Phi"].Points.AddXY(time,phi_kom_deg);
                chart1.Series["Phi"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }
            if (chart_theta_flag == 1)
            {
                chart1.Series["Theta"].Points.AddXY(time, theta_kom_deg);
                chart1.Series["Theta"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }
            if (chart_psi_flag == 1)
            {
                chart1.Series["Psi"].Points.AddXY(time, psi_kom_deg);
                chart1.Series["Psi"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }

            if (chart_phi_flag == 0 && chart_theta_flag == 0 && chart_psi_flag == 0)
            {
                chart1.Series["Phi"].Points.RemoveAt(0);
            }
            if (chart_phi_flag == 1 && (chart1.Series["Phi"].Points.Count > 100 || chart1.Series["Theta"].Points.Count > 100 || chart1.Series["Psi"].Points.Count > 100))
            {
                chart1.Series["Phi"].Points.RemoveAt(0);
            }
            if (chart_theta_flag == 1 && (chart1.Series["Theta"].Points.Count > 100 || chart1.Series["Theta"].Points.Count > 100 || chart1.Series["Psi"].Points.Count > 100))
            {
                chart1.Series["Theta"].Points.RemoveAt(0);
            }
            if ( chart_psi_flag == 1 && (chart1.Series["Psi"].Points.Count > 100 || chart1.Series["Theta"].Points.Count > 100 || chart1.Series["Psi"].Points.Count > 100))
            {
                chart1.Series["Psi"].Points.RemoveAt(0);
            }

            // Chart 2

            chart2.ChartAreas[0].AxisX.Title = "Time [ s ]";// Chart1 X axis Title name
            chart2.ChartAreas[0].AxisY.Title = "Degrees [ ° ]";// Chart1 Y axis Title name
            chart2.ChartAreas[0].AxisX.TitleFont = new Font("Trebuchet MS", 10, FontStyle.Bold); // Chart1 X axis Title font
            chart2.ChartAreas[0].AxisY.TitleFont = new Font("Trebuchet MS", 10, FontStyle.Bold); // Chart1 Y axis Title font
            chart2.ChartAreas[0].RecalculateAxesScale();

            if (chart_phi_flag == 1)
            {
                chart2.Series["Phi"].Points.AddXY(time, phi_gyro_deg);
                chart2.Series["Phi"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }
            if (chart_theta_flag == 1)
            {
                chart2.Series["Theta"].Points.AddXY(time, theta_gyro_deg);
                chart2.Series["Theta"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }
            if (chart_psi_flag == 1)
            {
                chart2.Series["Psi"].Points.AddXY(time, psi_gyro_deg);
                chart2.Series["Psi"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }

            if (chart_phi_flag == 0 && chart_theta_flag == 0 && chart_psi_flag == 0)
            {
                chart2.Series["Phi"].Points.RemoveAt(0);
            }
            if (chart_phi_flag == 1 && (chart2.Series["Phi"].Points.Count > 100 || chart2.Series["Theta"].Points.Count > 100 || chart2.Series["Psi"].Points.Count > 100))
            {
                chart2.Series["Phi"].Points.RemoveAt(0);
            }
            if (chart_theta_flag == 1 && (chart2.Series["Theta"].Points.Count > 100 || chart2.Series["Theta"].Points.Count > 100 || chart2.Series["Psi"].Points.Count > 100))
            {
                chart2.Series["Theta"].Points.RemoveAt(0);
            }
            if (chart_psi_flag == 1 && (chart2.Series["Psi"].Points.Count > 100 || chart2.Series["Theta"].Points.Count > 100 || chart2.Series["Psi"].Points.Count > 100))
            {
                chart2.Series["Psi"].Points.RemoveAt(0);
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
          
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

    }
}
