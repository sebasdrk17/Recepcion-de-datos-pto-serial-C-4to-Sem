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
using System.IO;
using System.Net;

namespace puerto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SerialPort miPuerto;
        int count = 0,archivosCreados=0;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                miPuerto = new SerialPort("COM7", 9600, Parity.None, 8, StopBits.One);
                miPuerto.Open();
                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                miPuerto.Close();
                timer1.Stop();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string cad = "";
            while (miPuerto.BytesToRead > 0)
            {
                cad = miPuerto.ReadExisting();
                count++;
                listTemp.Items.Add(cad);
            }
            if (count == 15)
            {
                crearArchivo();
                count = 0;
                archivosCreados++;
            }
        }
        private void crearArchivo()
        {
            try
            {
                string nombreArchivo = "datos" + archivosCreados.ToString() + ".csv";
                FileStream fs = new FileStream(nombreArchivo, FileMode.Create);
                StreamWriter escritor = new StreamWriter(fs);
                escritor.WriteLine(DateTime.Now.ToString());
                for (int i = 0; i < listTemp.Items.Count; i++)
                {
                    escritor.WriteLine(listTemp.Items[i]);
                }
                escritor.Close();
                fs.Close();
                listTemp.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSubirArchivo_Click(object sender, EventArgs e)
        {
            int indice = 0;
            bool estado = false;
            if(count!=10 && archivosCreados > 0)
            {
                archivosCreados--;
            }
            try
            {
                for (; indice <= archivosCreados; indice++)
                {
                    string rutaFTP = "ftp://" + "localhost";
                    string rutaArchivo = "C:/Users/Alexis Hernandez/source/repos/puerto/puerto/bin/Debug/datos" + indice.ToString() + ".csv";
                    string usuario = "pi";
                    string contraseña = "pi";
                    try
                    {
                        FtpWebRequest consulta = (FtpWebRequest)FtpWebRequest.Create(rutaFTP + "/" + Path.GetFileName(rutaArchivo));
                        consulta.Method = WebRequestMethods.Ftp.UploadFile;
                        consulta.Credentials = new NetworkCredential(usuario, contraseña);
                        consulta.UsePassive = true;
                        consulta.UseBinary = true;
                        consulta.KeepAlive = false;
                        FileStream cargar = File.OpenRead(rutaArchivo);
                        byte[] buffer = new byte[cargar.Length];
                        cargar.Read(buffer, 0, buffer.Length);
                        cargar.Close();
                        Stream streamQuery = consulta.GetRequestStream();
                        streamQuery.Write(buffer, 0, buffer.Length);
                        streamQuery.Close();
                        estado = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        estado = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (estado)
            {
                MessageBox.Show("Archivos subidos exitosamente");
            }
        }
    }
}
