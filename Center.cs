using DiscreetScreenshots.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace DiscreetScreenshots
{
    public partial class Center : Form
    {
        public Center()
        {
            InitializeComponent();
            InitializeElements();
        }

        #region Vars

        //Boton de formato (ahora path)
        //Boton de configuracion
        //Boton de informacion
        bool isPushFormat;
        bool isPushConfiguration;
        bool isPushInformation;
        //Elementos que usamos
        string locate = String.Empty;
        string format = String.Empty;
        string date = String.Empty;
        string hour = String.Empty;


        //Metodos y variables que deben ser llenadas antes de iniciar
        private void InitializeElements()
        {
            //Verificamos si tiene una ruta valida
            GetSavePath();

            //Cargamos los valores por defecto
            locate = txtPath.Text;
            isPushFormat = true;
            isPushConfiguration = true;
            isPushInformation = true;
            format = "JPEG";
            date = "yyyy-MM-dd";
            hour = "hh-mm-ss";

            //Minizamos para lo mostrar el path
            //Es puesto al final porque debemos cargar los valores iniciales
            VisibilityPanelPlus();
        }


        #endregion

        #region Bottons

        //Boton para elegir carpeta
        private void btnSearch_Click(object sender, EventArgs e)
        {
            //Abrirmos el explorador de carpetas
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                //Guardamos la ruta y actualziamos el textbox
                locate = folderBrowser.SelectedPath;
                txtPath.Text = locate;
            }
        }

        //Boton para hacer la captura
        private void btnCapture_Click(object sender, EventArgs e)
        {
            CaptureScreen();

            //Restauramos la vista
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        //Boton para guardar
        private void btnSave_Click(object sender, EventArgs e)
        {
            //Metodo que guardar
            SaveScreen();
        }

        //Boton para limpiar
        private void btnClean_Click(object sender, EventArgs e)
        {
            picture.Image = null;
            //picture.InitialImage = null
        }

        //Boton para mostrar imagen
        private void btnPlus_Click(object sender, EventArgs e)
        {
            VisibilityPanelPlus();
        }

        //Boton para mostrar configuraciones
        private void btnConfiguration_Click(object sender, EventArgs e)
        {
            //Cambiamos la visibilidad
            VisibilityPanelConfiguracion();
        }

        //Boton para mostrar panel informacion
        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (isPushInformation)
            {
                //Desactivamos los botones
                btnConfiguration.Enabled = false;
                btnPlus.Enabled = false;
                btnConfiguration.BackColor = Color.FromName("ControlDark");
                btnPlus.BackColor = Color.FromName("ControlDark");

                panelInfo.Location = new Point(12, 33);

                //Ocultamos el panel de configuracion (si estuviera abierto)
                //Mostramos nuestro nuevo panel
                panelConfiguration.Visible = false;
                panelInfo.Visible = true;

                //Activamos el boton
                isPushInformation = false;
            }
            else
            {
                //Activamos los botones
                btnConfiguration.Enabled = true;
                btnPlus.Enabled = true;
                btnConfiguration.BackColor = Color.FromName("Transparent");
                btnPlus.BackColor = Color.FromName("Transparent");

                //Ocutamos el panel de informacion
                //Mostramos los panel por defecto
                panelInfo.Visible = false;
                panelCapture.Visible = true;
                panelFormat.Visible = true;

                //Desactivamos el boton
                isPushInformation = true;
            }
        }

        //Boton para guardar configuracion
        private void btnSaveConfiguration_Click(object sender, EventArgs e)
        {
            // Format
            if (radioBFormat1.Checked == true) format = "JPEG";
            if (radioBFormat2.Checked == true) format = "PNG";
            if (radioBFormat3.Checked == true) format = "BMP";
            if (radioBFormat4.Checked == true) format = "TIFF";
            //Date
            if (radioBDate1.Checked == true) date = "yyyy-MM-dd";
            if (radioBDate2.Checked == true) date = "dd-MM-yyyy";
            //Hours
            if (radioBHour1.Checked == true) hour = "hh-mm-ss";
            if (radioBHour2.Checked == true) hour = "HH-mm-ss";

            //Regremos a la pantalla principal
            VisibilityPanelConfiguracion();
        }


        #endregion

        #region Metods

        //Obtener ruta de guardado
        private void GetSavePath()
        {
            string path = String.Empty;
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                path = Clipboard.GetText();

                if (Directory.Exists(path))
                {
                    txtPath.Text = path;
                }
            }
        }

        //Metodo para la captura de pantalla
        private void CaptureScreen()
        {
            try
            {
                //limpiar el picture
                picture.Image = null;

                //Capture
                //Minimizamos y de damos tiempo para que el programa no se vea en la captura
                this.WindowState = FormWindowState.Minimized;
                Thread.Sleep(100);

                //Calcula el tamaño de la pantalla
                Rectangle desk = Screen.GetBounds(this.ClientRectangle);
                Bitmap bitmap = new Bitmap(desk.Width, desk.Height);

                //Pintamos la imagen
                Graphics graph = Graphics.FromImage(bitmap);
                graph.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

                //Liberamos memoria
                graph.Flush();

                //Monstramos la captura
                picture.SizeMode = PictureBoxSizeMode.StretchImage;
                picture.Image = bitmap;

            }
            catch
            {
                MessageBox.Show("Failed to take screenshot", "Warning",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Show();
            }
        }

        //Metodo para guardar captura de pantalla
        private void SaveScreen()
        {
            //Validamos si tenemos todos los datos necesarios
            if (format == String.Empty)
            {
                MessageBox.Show("Select the format to continue", "Warning",
                       MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Show();
                return;
            }
            if (locate == String.Empty)
            {
                MessageBox.Show("Select the save path to be able to continue", "Warning",
                       MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Show();
                return;
            }
            if (picture.Image == null)
            {
                MessageBox.Show("Take a screenshot to continue", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Show();
                return;
            }

            try
            {
                //Verificamos
                if (Directory.Exists(Path.GetDirectoryName(locate)))
                {
                    //Nombramos el archivo
                    string filename = locate+"\\"+DateTime.Now.ToString(date+" "+hour);

                    //Buscamos en que formato guardar
                    switch (format)
                    {
                        case "JPEG":
                            picture.Image.Save(filename + "." + format, ImageFormat.Jpeg);
                            break;
                        case "PNG":
                            picture.Image.Save(filename + "." + format, ImageFormat.Png);
                            break;
                        case "BMP":
                            picture.Image.Save(filename + "." + format, ImageFormat.Bmp);
                            break;
                        case "TIFF":
                            picture.Image.Save(filename + "." + format, ImageFormat.Tiff);
                            break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Failed to save", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Show();
                return;
            }
        }

        //Metodo para visibilidad del panel
        private void VisibilityPanelPlus()
        {
            //Ocultamos el panel de configuracion (Si se encontrara abierto)
            panelConfiguration.Visible = false;

            if (isPushFormat)
            {
                //Desactivamos el boton de informacion
                btnInfo.Enabled = false;
                btnInfo.BackColor = Color.FromName("ControlDark");

                //Desactivamos el boton de configuracion
                btnConfiguration.Enabled = false;
                btnConfiguration.BackColor = Color.FromName("ControlDark");

                //Ocultamos los paneles de Captura y formato
                panelCapture.Visible = false;
                panelFormat.Location = new Point(12, 33);
                this.Size = new Size(450, 120);

                //Actualizamos el estado
                isPushFormat = false;
            }
            else
            {
                //Activamos el boton de informacion
                btnInfo.Enabled = true;
                btnInfo.BackColor = Color.FromName("Transparent");

                //Activamos el boton de configuracion
                btnConfiguration.Enabled = true;
                btnConfiguration.BackColor = Color.FromName("Transparent");

                //Mostramos los paneles de Captura y formato
                panelCapture.Visible = true;
                panelFormat.Location = new Point(12, 195);
                this.Size = new Size(450, 280);

                //Actualizamos el estado
                isPushFormat = true;
            }
        }

        //Visibililidad del panel de configuracon
        private void VisibilityPanelConfiguracion()
        {
            if (isPushConfiguration)
            {
                //Desactivamos el boton de capturas y le cambiamos el color
                btnPlus.Enabled = false;
                btnPlus.BackColor = Color.FromName("ControlDark");

                //Desactivamos el panel de capturas y formato
                panelCapture.Visible = false;
                panelFormat.Visible = false;

                //Mostramos el panel de configuracion
                panelConfiguration.Visible = true;
                panelConfiguration.Location = new Point(12, 33);

                //Actualizamos el estado
                isPushConfiguration = false;
            }
            else
            {
                //Activamos el boton de capturas
                btnPlus.Enabled = true;
                btnPlus.BackColor = Color.FromName("Transparent");

                //Activamos el panel de capturas y formato
                //Ocultamos el panel de configuracion
                panelConfiguration.Visible = false;
                panelCapture.Visible = true;
                panelFormat.Visible = true;

                //Actualizamos el estado
                isPushConfiguration = true;
            }
        }

        //Cuando cambie el tamaño de la pantalla (Minimizar)
        private void Center_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //Ocultamos
                this.Hide();

                //Mostramos el icono con un mensaje
                notiIcon.Icon = SystemIcons.Shield;
                notiIcon.Text = "Antivirus\nTodo parece estar en orden";
            }
        }

        #endregion

        #region Menu Strip / Notify Icon

        //Menu

        //Closed
        private void closedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Rstore
        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        //Capture
        private void captureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.WindowState != FormWindowState.Normal || this.WindowState != FormWindowState.Maximized)
                {
                    CaptureScreen();

                    SaveScreen();
                }
            }
            catch
            {
                MessageBox.Show("Something has gone wrong", "Warning",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Show();
                return;
            }
        }



        //Notify Icon

        //Open
        private void notiIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }


        #endregion


    }

}
