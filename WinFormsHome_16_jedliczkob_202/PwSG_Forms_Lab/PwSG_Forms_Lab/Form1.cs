using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Text.Json.Serialization;

namespace PwSG_Forms_Lab
{


    public partial class Form1 : Form
    {
        List<Vertex> vertices;
        List<Line> lines;
        int deltaX, deltaY=0;
        Point mouse_position;
        Point mouse_prev_position;
        readonly int R = 30;
        Rectangle rec;
        Size s;
        readonly Pen pen;
        readonly Pen vertice_pen;
        bool found = false;
        readonly Pen dashedpen;
        readonly Pen line_pen;
        bool line_spotted = false;
        int prev_dotted = -1;
        ComponentResourceManager resourceManager;
        //public class Graph
        //{
        //    public List<Vertex> ver {get;set;}
        //    public List<Line> lines { get; set; }
        //    [JsonConstructor]
        //    public Graph(List<Vertex> v, List<Line> l)
        //    {
        //        ver = v;
        //        lines = l;
        //    }
        //}
        public class Vertex
        {
            public Point point;
            public Color color;
            public bool dotted;
            [JsonConstructor]
            public Vertex(Point p, Color c, bool f)
            {
                point = p;
                color = c;
                dotted = f;
            }

            public Point P
            {
                get { return point; }
                set { point = value; }
            }

            public Color C
            {
                get { return color; }
                set { color = value; }
            }

            public bool D
            {
                get { return dotted; }
                set { dotted = value; }
            }
        }

        public class Line
        {
            public Point p1;
            public Point p2;

            public Line(Point pA, Point pB)
            {
                p1 = pA;
                p2 = pB;
            }
        }

        public Form1()
        {
            CultureInfo culInfo = new CultureInfo("pl");
            Thread.CurrentThread.CurrentCulture = culInfo;
            Thread.CurrentThread.CurrentUICulture = culInfo;
            InitializeComponent();
            vertices = new List<Vertex>();
            lines = new List<Line>();
            rec = new Rectangle(0, 0, R, R);
            pen = new Pen(Brushes.Black, 3);
            vertice_pen = new Pen(Brushes.Black, 3);
            line_pen = new Pen(Brushes.Black, 3);
            dashedpen = new Pen(Brushes.Black, 3);
            dashedpen.DashPattern = new float[] {2,1};
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VertexChange);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                mouse_position = new Point(e.X, e.Y);

                for (int i = 0; i < vertices.Count; i++)
                {
                    if (Collide(mouse_position, vertices[i].point))
                    {
                        if(prev_dotted!=-1)
                        {
                            line_spotted = false;
                            for(int j=0; j<lines.Count;j++)
                            {
                                if ( (lines[j].p1.X == vertices[i].point.X && lines[j].p2.X == vertices[prev_dotted].point.X)
                                    || (lines[j].p2.X == vertices[i].point.X && lines[j].p1.X == vertices[prev_dotted].point.X))
                                {
                                    lines.Remove(lines[j]);
                                    line_spotted = true;
                                }
                                    
                            }
                            if(line_spotted==false)
                                lines.Add(new Line(vertices[i].point, vertices[prev_dotted].point));
                            pictureBox1.Invalidate();
                        }
                        return;
                    }
                        
                }
                vertices.Add(new Vertex(mouse_position, vertice_pen.Color,false));
            }
            if(e.Button == MouseButtons.Right)
            {
                mouse_position = new Point(e.X, e.Y);
                found = false;
                for(int i=0; i<vertices.Count;i++)
                {
                    if(Collide(mouse_position,vertices[i].point))
                    {
                        vertices[i].dotted = true;
                        deleteButton.Enabled = true;
                        if (prev_dotted != -1 && prev_dotted != i)
                        {
                            vertices[prev_dotted].dotted = false;
                            
                        }
                        prev_dotted = i;
                        found = true;
                    }
                }
                if(found==false)
                {
                    if (prev_dotted != -1)
                    {
                        vertices[prev_dotted].dotted = false;
                        deleteButton.Enabled = false;
                        prev_dotted = -1;
                    }
                }
            }




            pictureBox1.Invalidate();
        }

        private void VertexChange(object sender, MouseEventArgs e11)
        {
            if (e11.Button == MouseButtons.Middle)
            {
                if (prev_dotted != -1)
                {
                    var prev_vertex_position = new Point(vertices[prev_dotted].point.X, vertices[prev_dotted].point.Y);
                    mouse_position = new Point(e11.X, e11.Y);
                    deltaX = mouse_position.X - mouse_prev_position.X;
                    deltaY = mouse_position.Y - mouse_prev_position.Y;
                    mouse_prev_position = new Point(e11.X, e11.Y); 
                    vertices[prev_dotted].point.X += deltaX;
                    vertices[prev_dotted].point.Y += deltaY;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].p1 == prev_vertex_position)
                            lines[i].p1 = vertices[prev_dotted].point;
                        if (lines[i].p2 == prev_vertex_position)
                            lines[i].p2 = vertices[prev_dotted].point;
                    }
                    
                    pictureBox1.Invalidate();
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            for (int i = 0; i < lines.Count; i++)
            {
                e.Graphics.DrawLine(line_pen, lines[i].p1, lines[i].p2);
            }

            for (int i=0; i<vertices.Count;i++)
            {
                rec.Location = new Point(vertices[i].point.X - R / 2, vertices[i].point.Y - R / 2);
                if(vertices[i].dotted==false)
                {
                    pen.Color = vertices[i].color;
                    e.Graphics.FillEllipse(Brushes.White, rec);
                    e.Graphics.DrawEllipse(pen, rec);
                    DrawStr(e, i,pen);

                }
                else
                {
                    dashedpen.Color = vertices[i].color;
                    e.Graphics.FillEllipse(Brushes.White, rec);
                    e.Graphics.DrawEllipse(dashedpen, rec);
                    DrawStr(e, i, dashedpen);
                }
            }
            
        }

        private void DrawStr(PaintEventArgs e11, int i11, Pen pen11)
        {
            if (i11 + 1 < 10) e11.Graphics.DrawString((i11 + 1).ToString(), this.Font, pen11.Brush, new Point(vertices[i11].point.X - 1 * R / 6, vertices[i11].point.Y - 2 * R / 7));
            else e11.Graphics.DrawString((i11 + 1).ToString(), this.Font, pen11.Brush, new Point(vertices[i11].point.X - 1 * 2 * R / 7, vertices[i11].point.Y - 2 * R / 7));
        }

        private bool Collide(Point p1, Point p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y) <= (R * R);
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            ColorDialog cdial = new ColorDialog();
            if(cdial.ShowDialog()==DialogResult.OK)
            {
                pictureBox2.BackColor = cdial.Color;
                pen.Color = cdial.Color;
                vertice_pen.Color = cdial.Color;
                if (prev_dotted != -1) { vertices[prev_dotted].color = cdial.Color; pictureBox1.Invalidate(); };
            }
        }

        // delete button
        private void button1_Click(object sender, EventArgs e)
        {
            if (prev_dotted != -1)
            {
                int k = 0;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (vertices[prev_dotted].point.X == lines[k].p1.X || vertices[prev_dotted].point.X == lines[k].p2.X)
                    {
                        lines.Remove(lines[k]);
                        k--;
                        i--;
                    }
                    k++;
                }
                vertices.Remove(vertices[prev_dotted]);
                prev_dotted = -1;
                deleteButton.Enabled = false;
                pictureBox1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                mouse_prev_position = new Point(e.X, e.Y);
        }

        private void polishButton_Click(object sender, EventArgs e)
        {
            CultureInfo culInfo = new CultureInfo("pl");
            Thread.CurrentThread.CurrentCulture = culInfo;
            Thread.CurrentThread.CurrentUICulture = culInfo;
            s = this.Size;
            resourceManager = new ComponentResourceManager(typeof(Form1));
            resourceManager.ApplyResources(this, "$this", culInfo);
            foreach (Control c in this.Controls)
            {
                ChangeLanguage(c.Controls, resourceManager, culInfo);
            }
            this.Size = s;
        }

        private void englishButton_Click(object sender, EventArgs e)
        {
            CultureInfo culInfo = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = culInfo;
            Thread.CurrentThread.CurrentUICulture = culInfo;
            s = this.Size;
            resourceManager = new ComponentResourceManager(typeof(Form1));
            resourceManager.ApplyResources(this, "$this", culInfo); //źródło: https://stackoverflow.com/questions/59258368/how-to-change-form-title-when-you-change-form-language
            foreach (Control c in this.Controls)
            {
                ChangeLanguage(c.Controls, resourceManager, culInfo);
            }
            this.Size = s;
        }

        private void ChangeLanguage(Control.ControlCollection c, ComponentResourceManager r, CultureInfo culture)
        {
            foreach (Control con in c)
            {
                r.ApplyResources(con, con.Name, culture);   //źródło: https://www.dotnetcurry.com/ShowArticle.aspx?ID=174
                ChangeLanguage(con.Controls,r,culture);
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //Graph G = new Graph(vertices, lines);
            SaveClass.JsonSerialize(vertices,"ver");
        }

        private void clearGraphButton_Click(object sender, EventArgs e)
        {
            lines.Clear();
            vertices.Clear();
            deleteButton.Enabled = false;
            prev_dotted = -1;
            pictureBox1.Invalidate();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            List<Vertex> G1 = SaveClass.JsonDeserialize("ver");
            vertices = G1;
        }


        // delete button shortcut
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteButton.PerformClick();
                deleteButton.Enabled = false;
            }
                

        }
    }
}
