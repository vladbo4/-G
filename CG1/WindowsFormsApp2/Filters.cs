using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace WindowsFormsApp2
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y); // вычисление значение каждого пикселя

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public virtual Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker) //создание новой картинки с другими пикселями в зависимости от функции calculatenewpix
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                if (i == 630)
                {

                }

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }
    }

    // матричные

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
                );
        }
    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    }

    class GaussianFilter : MatrixFilter
    {
        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }

        public void createGaussianKernel(int radius, float sigma)
        {
            //размер ядра
            int size = 2 * radius + 1; //размер ядра
            kernel = new float[size, size]; // ядро
            float norm = 0; // коэфф. номировки ядра
            //расчитывание ядра линейного фильтра
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            //нормировка ядра
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;

        }
    }

    class SobelFilter : MatrixFilter
    {
        public SobelFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            kernel[0, 0] = -1.0f;
            kernel[0, 1] = 0f;
            kernel[0, 2] = 1.0f;
            kernel[1, 0] = -2.0f;
            kernel[1, 1] = 0f;
            kernel[1, 2] = 2.0f;
            kernel[2, 0] = -1.0f;
            kernel[2, 1] = 0f;
            kernel[2, 2] = 1.0f;

        }

        public void swap()
        {
            float tmp;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    tmp = kernel[i, j];
                    kernel[i, j] = kernel[j, i];
                    kernel[j, i] = tmp;
                }
            }
        }
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap Image1 = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / Image1.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Image1.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            Bitmap Image2 = new Bitmap(sourceImage.Width, sourceImage.Height);
            swap();
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / Image2.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Image2.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            Bitmap resuitImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resuitImage.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color Image1Color = Image1.GetPixel(i, j);
                    Color Image2Color = Image2.GetPixel(i, j);
                    double resultR = Math.Sqrt(Image1Color.R * Image1Color.R + Image2Color.R * Image2Color.R);
                    double resultG = Math.Sqrt(Image1Color.G * Image1Color.G + Image2Color.G * Image2Color.G);
                    double resultB = Math.Sqrt(Image1Color.B * Image1Color.B + Image2Color.B * Image2Color.B);

                    Color result = Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
                    resuitImage.SetPixel(i, j, result);
                }
            }
            return resuitImage;
        }

    }

    class Sharpness : MatrixFilter
    {
        public Sharpness()
        {
            kernel = new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
        }
    }

    class Tysnenie : MatrixFilter
    {
        public Tysnenie()
        {
            kernel = new float[3, 3] { { 0, 1, 0 }, { 1, 0, -1 }, { 0, -1, 0 } };
        }
    }


    // точечные
    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R,
                                                255 - sourceColor.G,
                                                255 - sourceColor.B);
            return resultColor;
        }
    }

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            double intensity = sourceColor.R * 0.299 + sourceColor.G * 0.587 + sourceColor.B * 0.114;
            Color resultColor = Color.FromArgb(Clamp(((int)intensity), 0, 255),
                                                Clamp(((int)intensity), 0, 255),
                                                Clamp(((int)intensity), 0, 255));
            return resultColor;
        }
    }

    class Sepia : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            const int k = 20;
            Color sourceColor = sourceImage.GetPixel(x, y);
            double intensity = sourceColor.R * 0.299 + sourceColor.G * 0.587 + sourceColor.B * 0.114;
            Color resultColor = Color.FromArgb(Clamp(((int)intensity) + 2 * k, 0, 255),
                                                Clamp(((int)intensity) + k / 2, 0, 255),
                                                Clamp(((int)intensity) - k, 0, 255));
            return resultColor;
        }
    }

    class BrightnessIncrease : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            const int k = 50;
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(Clamp(sourceColor.R + k, 0, 255),
                                                Clamp(sourceColor.G + k, 0, 255),
                                                Clamp(sourceColor.B + k, 0, 255));
            return resultColor;
        }
    }

    class TurnFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int x0 = sourceImage.Width / 2;
            int y0 = sourceImage.Height / 2;
            double center = Math.PI / 2;

            int newX = Clamp((int)((x - x0) * Math.Cos(center) - (y - y0)
                        * Math.Sin(center) + x0), 0, sourceImage.Width - 1);
            int newY = Clamp((int)((x - x0) * Math.Sin(center) - (y - y0)
                        * Math.Cos(center) + y0), 0, sourceImage.Height - 1);

            return sourceImage.GetPixel(newX, newY);
        }

    }

    //морфологические

    abstract class MorfologeFilter : Filters
    {
        protected int MW = 3, MH = 3;
        protected int[,] Mask = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = MW / 2; i < sourceImage.Width - MW / 2; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = MH / 2; j < sourceImage.Height - MH / 2; j++)
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
            }

            return resultImage;
        }
    }

    class ErosionFilter : MorfologeFilter
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color min = Color.FromArgb(255, 255, 255);

            for (int j = -MH / 2; j <= MH / 2; j++)
                for (int i = -MW / 2; i <= MW / 2; i++)
                {
                    Color pixel = sourceImage.GetPixel(x + i, y + j);

                    if (Mask[i + MW / 2, j + MH / 2] != 0 && pixel.R < min.R && pixel.G < min.G && pixel.B < min.B)
                        min = pixel;
                }

            return min;
        }
    }

    class DilationFilter : MorfologeFilter
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color max = Color.FromArgb(0, 0, 0);

            for (int j = -MH / 2; j <= MH / 2; j++)
                for (int i = -MW / 2; i <= MW / 2; i++)
                {
                    Color pixel = sourceImage.GetPixel(x + i, y + j);

                    if (Mask[i + MW / 2, j + MH / 2] == 1 && pixel.R > max.R && pixel.G > max.G && pixel.B > max.B)
                        max = pixel;
                }

            return max;
        }
    }

    class OpeningFilter : MorfologeFilter
    {
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap erosion = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = MW / 2; i < erosion.Width - MW / 2; i++)
            {
                worker.ReportProgress((int)((float)i / erosion.Width * 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = MH / 2; j < erosion.Height - MH / 2; j++)
                    erosion.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
            }

            Bitmap result = new Bitmap(erosion);

            for (int i = MW / 2; i < result.Width - MW / 2; i++)
            {
                worker.ReportProgress((int)((float)i / result.Width * 50 + 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = MH / 2; j < result.Height - MH / 2; j++)
                    result.SetPixel(i, j, calcDilation(erosion, i, j));
            }

            return result;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color min = Color.FromArgb(255, 255, 255);

            for (int j = -MH / 2; j <= MH / 2; j++)
                for (int i = -MW / 2; i <= MW / 2; i++)
                {
                    Color pixel = sourceImage.GetPixel(x + i, y + j);

                    if (Mask[i + MW / 2, j + MH / 2] != 0 && pixel.R < min.R && pixel.G < min.G && pixel.B < min.B)
                        min = pixel;
                }

            return min;
        }

        private Color calcDilation(Bitmap sourceImage, int x, int y)
        {
            Color max = Color.FromArgb(0, 0, 0);

            for (int j = -MH / 2; j <= MH / 2; j++)
                for (int i = -MW / 2; i <= MW / 2; i++)
                {
                    Color pixel = sourceImage.GetPixel(x + i, y + j);

                    if (Mask[i + MW / 2, j + MH / 2] == 1 && pixel.R > max.R && pixel.G > max.G && pixel.B > max.B)
                        max = pixel;
                }

            return max;
        }
    }

    class ClosingFilter : MorfologeFilter
    {
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap dilation = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = MW / 2; i < dilation.Width - MW / 2; i++)
            {
                worker.ReportProgress((int)((float)i / dilation.Width * 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = MH / 2; j < dilation.Height - MH / 2; j++)
                    dilation.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
            }

            Bitmap result = new Bitmap(dilation);

            for (int i = MW / 2; i < result.Width - MW / 2; i++)
            {
                worker.ReportProgress((int)((float)i / result.Width * 50 + 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = MH / 2; j < result.Height - MH / 2; j++)
                    result.SetPixel(i, j, calcErosion(dilation, i, j));
            }

            return result;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {

            Color max = Color.FromArgb(0, 0, 0);

            for (int j = -MH / 2; j <= MH / 2; j++)
                for (int i = -MW / 2; i <= MW / 2; i++)
                {
                    Color pixel = sourceImage.GetPixel(x + i, y + j);

                    if (Mask[i + MW / 2, j + MH / 2] == 1 && pixel.R > max.R && pixel.G > max.G && pixel.B > max.B)
                        max = pixel;
                }

            return max;
        }

        private Color calcErosion(Bitmap sourceImage, int x, int y)
        {
            Color min = Color.FromArgb(255, 255, 255);

            for (int j = -MH / 2; j <= MH / 2; j++)
                for (int i = -MW / 2; i <= MW / 2; i++)
                {
                    Color pixel = sourceImage.GetPixel(x + i, y + j);

                    if (Mask[i + MW / 2, j + MH / 2] != 0 && pixel.R < min.R && pixel.G < min.G && pixel.B < min.B)
                        min = pixel;
                }

            return min;
        }
    }
}