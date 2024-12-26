using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.PeerToPeer;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HadwareRemoteControl
{
    public partial class FormImageTransform : Form
    {
        TransformationData transformationData;
        Bitmap source;
        double[] ZoomLevels = [0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1, 0.15, 0.2, 0.25, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 1.5, 2, 3, 4, 8];
        int ZoomLevel = 0;
        int PtSize = 10;
        int pointIndex = -1;
        int MX = 0;
        int MY = 0;


        public FormImageTransform()
        {
            InitializeComponent();
            ZoomLevel = ZoomLevels.ToList().IndexOf(0.5);
            pbScreen.MouseWheel += pbScreen_MouseWheel;
            this.MouseWheel += pbScreen_MouseWheel;
        }

        public static void ShowForm(Form parent, TransformationData transformationData, Bitmap source)
        {
            using FormImageTransform form = new();
            form.transformationData = transformationData;
            form.source = source;
            form.transformationData.InitPoints(source);
            if (source != null)
            {
                form.ShowDialog(parent);
            }
            source.Dispose();
        }

        private void pbScreen_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                tsbZoomIn_Click(sender, new EventArgs());
            }
            if (e.Delta < 0)
            {
                tsbZoomOut_Click(sender, new EventArgs());
            }
        }

        private void onTransformClick(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                tsbUseTransform.Text = item.Text;
                tsbUseTransform.Image = item.Image;
                tsbUseTransform.Tag = item.Tag;
                transformationData.UseTransform = (tsbUseTransform.Tag.ToString().ToInt32() != 0);
                pbScreen.Refresh();
            }
        }

        private void FormImageTransform_Load(object sender, EventArgs e)
        {
            onTransformClick(transformationData.UseTransform ? useTransformToolStripMenuItem : dontUseTransformToolStripMenuItem, new EventArgs());
            pbScreen.Refresh();
        }

        private void tsbZoomIn_Click(object sender, EventArgs e)
        {
            if (ZoomLevel < ZoomLevels.Length - 1)
            {
                ZoomLevel++;
                pbScreen.Refresh();
            }
        }

        private void tsbZoomOut_Click(object sender, EventArgs e)
        {
            if (ZoomLevel > 0)
            {
                ZoomLevel--;
                pbScreen.Refresh();
            }
        }

        void PointsBack(List<PointF> screenPoints)
        {
            var Zoom = ZoomLevels[ZoomLevel];
            var ScreenXZero = pbScreen.ClientSize.Width / 2;
            var ImageXZero = source.Width / 2;
            var ScreenYZero = pbScreen.ClientSize.Height / 2;
            var ImageYZero = source.Height / 2;

            for (int i = 0; i < 4; i++)
            {
                transformationData.destPoints[i].X = (float)(ImageXZero - ((ScreenXZero - screenPoints[i].X) / Zoom));
                transformationData.destPoints[i].Y = (float)(ImageYZero - ((ScreenYZero - screenPoints[i].Y) / Zoom));
            }
        }

        void GetPoints(out List<PointF> screenPoints)
        {
            screenPoints = new();
            var Zoom = ZoomLevels[ZoomLevel];
            var ScreenXZero = pbScreen.ClientSize.Width / 2;
            var ImageXZero = source.Width / 2;
            var ScreenYZero = pbScreen.ClientSize.Height / 2;
            var ImageYZero = source.Height / 2;

            foreach (var point in transformationData.destPoints)
            {
                screenPoints.Add(new(
                    (float)(ScreenXZero - (ImageXZero - point.X) * Zoom),
                    (float)(ScreenYZero - (ImageYZero - point.Y) * Zoom))
                    );
            }
        }

        private void pbScreen_Paint(object sender, PaintEventArgs e)
        {
            var Zoom = ZoomLevels[ZoomLevel];
            int Width = (int)(source.Width * Zoom);
            int Height = (int)(source.Height * Zoom);
            var transformed = transformationData.DoTransformation(source);
            e.Graphics.DrawImage(transformed, (pbScreen.ClientSize.Width - Width) / 2, (pbScreen.ClientSize.Height - Height) / 2, Width, Height);

            e.Graphics.DrawRectangle(new Pen(Color.LightBlue), new((pbScreen.ClientSize.Width - Width) / 2, (pbScreen.ClientSize.Height - Height) / 2, Width, Height));

            GetPoints(out var points);

            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            int N = 0;
            foreach (var point in points)
            {
                var prevP = points[(N + points.Count - 1) % points.Count];
                e.Graphics.DrawLine(new Pen(Color.Black, 3), prevP, point);
                e.Graphics.DrawLine(new Pen(Color.White, 1), prevP, point);
                N++;
            }

            N = 0;
            foreach (var point in points)
            {
                RectangleF Rect = new(point.X - PtSize / 2, point.Y - PtSize / 2, PtSize, PtSize);
                e.Graphics.FillRectangle(new SolidBrush(N == pointIndex ? Color.Blue : Color.White), Rect);
                e.Graphics.DrawRectangle(new Pen(Color.Black), Rect);
                N++;
            }

        }

        private int GetPointIndex(int X, int Y)
        {
            GetPoints(out var points);

            int N = 0;
            foreach (var point in points)
            {
                RectangleF Rect = new(point.X - PtSize / 2, point.Y - PtSize / 2, PtSize, PtSize);
                if (X >= Rect.X && X <= Rect.Right && Y >= Rect.Y && Y <= Rect.Bottom)
                {
                    return N;
                }
                N++;
            }
            return -1;
        }

        private void pbScreen_Resize(object sender, EventArgs e)
        {
            pbScreen.Refresh();
        }

        private void pbScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (pointIndex >= 0)
            {
                GetPoints(out var points);
                var P = points[pointIndex];
                P.X = e.X;
                P.Y = e.Y;
                points[pointIndex] = P;
                PointsBack(points);
                pbScreen.Refresh();
            }
            else if (GetPointIndex(e.X, e.Y) == -1)
            {
                pbScreen.Cursor = Cursors.Default;
            }
            else
            {
                pbScreen.Cursor = Cursors.Hand;
            }
        }

        private void pbScreen_MouseDown(object sender, MouseEventArgs e)
        {
            pointIndex = GetPointIndex(e.X, e.Y);
            if (pointIndex >= 0)
            {
                MX = e.X;
                MY = e.Y;
                pbScreen.Refresh();
                pbScreen.Cursor = Cursors.SizeAll;
            }
        }

        private void pbScreen_MouseUp(object sender, MouseEventArgs e)
        {
            pointIndex = -1;
            pbScreen.Refresh();
        }
    }
}
