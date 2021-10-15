using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleFadeInOutImage
{
    public partial class Form1 : Form
    {
        private int currentPage = 1;
        private float preOpacityValue = 0.0f;
        private float nextOpacityValue = 1.0f;
        private Image preImage = Properties.Resources.image3;
        private Image nextImage = Properties.Resources.image1;
        private CancellationTokenSource cts = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            timer2.Stop();

            if (preOpacityValue != 0 && nextOpacityValue != 1)
            {
                currentPage--;
            }

            if (currentPage == 1)
            {
                preImage = Properties.Resources.image1;
                nextImage = Properties.Resources.image2;
                currentPage++;
            }
            else if (currentPage == 2)
            {
                preImage = Properties.Resources.image2;
                nextImage = Properties.Resources.image3;
                currentPage++;
            }
            else
            {
                preImage = Properties.Resources.image3;
                nextImage = Properties.Resources.image1;
                currentPage = 1;
            }
            preOpacityValue = 0.0f;
            nextOpacityValue = 1.0f;
            pictureBox1.Invalidate(true);

            var task1 = Task.Run(() =>
            {
                try
                {
                    Task.Delay(timer2.Interval, cts.Token).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
            task1.ContinueWith(_ =>
            {
                timer2.Start();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bitmap = new Bitmap(preImage);
            Bitmap bitmap2 = new Bitmap(nextImage);

            float[][] matrixItems ={
               new float[] {1, 0, 0, 0, 0},
               new float[] {0, 1, 0, 0, 0},
               new float[] {0, 0, 1, 0, 0},
               new float[] {0, 0, 0, preOpacityValue, 0},
               new float[] {0, 0, 0, 0, 1}};

            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
            ImageAttributes imageAtt = new ImageAttributes();
            imageAtt.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            float[][] matrixItems2 ={
               new float[] {1, 0, 0, 0, 0},
               new float[] {0, 1, 0, 0, 0},
               new float[] {0, 0, 1, 0, 0},
               new float[] {0, 0, 0, nextOpacityValue, 0},
               new float[] {0, 0, 0, 0, 1}};

            ColorMatrix colorMatrix2 = new ColorMatrix(matrixItems2);
            ImageAttributes imageAtt2 = new ImageAttributes();
            imageAtt2.SetColorMatrix(colorMatrix2, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int iWidth = pictureBox1.Width;
            int iHeight = pictureBox1.Height;
            e.Graphics.DrawImage(bitmap, new Rectangle(0, 0, iWidth, iHeight), 0.0f, 0.0f, iWidth, iHeight, GraphicsUnit.Pixel, imageAtt);
            e.Graphics.DrawImage(bitmap2, new Rectangle(0, 0, iWidth, iHeight), 0.0f, 0.0f, iWidth, iHeight, GraphicsUnit.Pixel, imageAtt2);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (preOpacityValue != 0 && nextOpacityValue != 1)
            {
                if (preOpacityValue < 0)
                {
                    preOpacityValue = 0.0f;
                }
                if (nextOpacityValue > 1)
                {
                    nextOpacityValue = 1.0f;
                }
                else
                {
                    preOpacityValue = preOpacityValue - 0.1f;
                    nextOpacityValue = nextOpacityValue + 0.1f;
                    pictureBox1.Invalidate(true);
                }
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (currentPage == 1)
            {
                preImage = Properties.Resources.image1;
                nextImage = Properties.Resources.image2;
                currentPage++;
            }
            else if (currentPage == 2)
            {
                preImage = Properties.Resources.image2;
                nextImage = Properties.Resources.image3;
                currentPage++;
            }
            else
            {
                preImage = Properties.Resources.image3;
                nextImage = Properties.Resources.image1;
                currentPage = 1;
            }
            preOpacityValue = 1.0f;
            nextOpacityValue = 0.0f;
        }
    }
}
