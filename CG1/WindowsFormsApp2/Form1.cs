using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        Bitmap image; // картинка

        public Form1()
        {
            InitializeComponent();
        }

        private void фильтрыToolStripMenuItem_Click(object sender, EventArgs e){ }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files|*.png;*.jpg;*.bpm|All files(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image = newImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void textBox1_TextChanged(object sender, EventArgs e){ }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e){ }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)//
        {
            Bitmap bmpSave = (Bitmap)pictureBox2.Image;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Image files|*.png;*.jpg;*.bpm|All files(*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bmpSave.Save(dialog.FileName, ImageFormat.Bmp);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox2.Image = image;
                pictureBox2.Refresh();
            }
            progressBar1.Value = 0;
        }


        private void progressBar1_Click(object sender, EventArgs e) { }

        private void Form1_Load(object sender, EventArgs e) { }

        //точечные фильтры

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new InvertFilter();

            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void вОттенкахСерогоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();

            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sepia();

            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void увеличениеЯркостиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BrightnessIncrease();

            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void поворотToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new TurnFilter();

            backgroundWorker1.RunWorkerAsync(filter);

        }

        //матричные фильтры

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрГауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
           
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрСобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilter();

            backgroundWorker1.RunWorkerAsync(filter);

        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sharpness();
            
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void тиснениеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Filters filter = new Tysnenie();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        //морфологические

        private void открытиеToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Filters filter = new OpeningFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ErosionFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void расширениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new DilationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ClosingFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
