using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickHull
{

    public partial class Form1 : Form
    {

        private List<Point> points = new List<Point>();
        private List<Point> hull = new List<Point>();

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Pen pen = new Pen(Color.Red);
            points.Add(e.Location);
            if (points.Count >= 3)
            {
                hull.Clear();
                QuickHull();
            }
            pictureBox1.Refresh();
        }

        private void QuickHull()
        {
            if (points.Count <= 3)
            {
                foreach (var p in points)
                    hull.Add(p);
                return;
            }

            Point pmin = points.Aggregate((p1, p2) => p1.X < p2.X ? p1 : p2);
            Point pmax = points.Aggregate((p1, p2) => p1.X > p2.X ? p1 : p2);

            hull.Add(pmin);
            hull.Add(pmax);

            List<Point> left = new List<Point>();
            List<Point> right = new List<Point>();

            foreach (var p in points)
            {
                if (Side(pmin, pmax, p) == 1)
                    left.Add(p);
                else
                if (Side(pmin, pmax, p) == -1)
                    right.Add(p);
            }
            CreateHull(pmin, pmax, left);
            CreateHull(pmax, pmin, right);
        }
        private int Side(Point min, Point max, Point p)
        {
            int val = (p.Y - min.Y) * (max.X - min.X) -
                      (max.Y - min.Y) * (p.X - min.X);

            if (val > 0)
                return 1;
            if (val < 0)
                return -1;
            return 0;
        }

        private int Distance(Point p1, Point p2, Point p)
        {
            return Math.Abs((p.Y - p1.Y) * (p2.X - p1.X) - (p2.Y - p1.Y) * (p.X - p1.X));
        }

        private void CreateHull(Point a, Point b, List<Point> pts)
        {
            int pos = hull.IndexOf(b);

            if (pts.Count == 0)
                return;

            if (pts.Count == 1)
            {
                hull.Insert(pos, pts[0]);
                return;
            }

            int dist = int.MinValue;
            Point point = new Point();

            for (int i = 0; i < pts.Count; i++)
            {
                int distance = Distance(a, b, pts[i]);
                if (distance > dist)
                {
                    dist = distance;
                    point = pts[i];
                }
            }

            hull.Insert(pos, point);
            List<Point> ap = new List<Point>();
            List<Point> pb = new List<Point>();

            foreach (var i in pts)
                if (Side(a, point, i) == 1)
                    ap.Add(i);

            foreach (var i in pts)
                if (Side(point, b, i) == 1)
                    pb.Add(i);

            CreateHull(a, point, ap);
            CreateHull(point, b, pb);
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            points.Clear();
            hull.Clear();
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Red);
            foreach (var i in points)
            {
                e.Graphics.DrawRectangle(pen, i.X, i.Y, 1, 1);
            }
            if (hull.Count > 0)
            {
                e.Graphics.DrawPolygon(pen, hull.ToArray());
            }
        }
    }
}
