using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRO1
{
    public partial class CGP1 : Form
    {
        public const int radius = 10, edgeThickness = 2;
        public Rectangle rec;
        public Pen penVertex;
        public Pen penEdge, penEdgeHighlight, penEdgeConstLength, penEdgeRelation;
        public Brush brushVertex = Brushes.Black;
        public Brush brushVertexHighlight = Brushes.DarkOrange;
        public Brush brushVertexSelected = Brushes.Red;
        public Brush brushEdge = Brushes.Black;
        public Brush brushEdgeHighlight = Brushes.Orange;
        public Brush brushEdgeConstLength = Brushes.Green;
        public Brush brushEdgeRelation = Brushes.OrangeRed;
        public Graphics graph;
        public Bitmap drawArea;

        public enum AppState
        {
            ready,
            newPolygonBegin,
            newPolygonDrawing
        }
        public class Vertex
        {
            public Point point;
            public Brush brush { get; set; }
            public void ChangePlacement(Point _p)
            {
                point = _p;
            }
            public void ChangePlacement(int _X, int _Y)
            {
                point.X = _X;
                point.Y = _Y;
            }
            public Vertex(Point _p, Brush _brush)
            {
                point = _p;
                brush = _brush;
            }
        }
        public class Polygon
        {
            public List<Vertex> vertices;

            public void SetVertexBrush(Brush _brush)
            {
                foreach(Vertex v in this.vertices)
                {
                    v.brush = _brush;
                }
            }
        }

        public AppState state;
        public Polygon tempPolygon;
        public List<Polygon> polygons;

        public Vertex selectedVertex = null;
        public bool isPressedLMB = false;

        public CGP1()
        {
            InitializeComponent();
            rec = new Rectangle(0, 0, radius * 2, radius * 2);
            penVertex = new Pen(brushVertex);
            penEdge = new Pen(brushEdge, edgeThickness);
            penEdgeHighlight = new Pen(brushEdgeHighlight, edgeThickness);

            polygons = new List<Polygon>();
            drawArea = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            canvas.Image = drawArea;
            Graphics g = Graphics.FromImage(drawArea);
            g.Clear(Color.White);
            g.Dispose();

            state = AppState.ready;
        }

        private void buttonNewPolygon_SetState_Ready()
        {
            buttonNewPolygon.BackColor = SystemColors.Control;
            buttonNewPolygon.Text = "New Polygon";
        }

        private void buttonNewPolygon_SetState_newPolygon()
        {
            buttonNewPolygon.BackColor = SystemColors.GradientActiveCaption;
            buttonNewPolygon.Text = "Cancel Creation";
        }

        private void SetState_Ready()
        {
            tempPolygon = null;
            canvas.Invalidate();
            state = AppState.ready;
            buttonNewPolygon_SetState_Ready();
        }
        private void SetState_newPolygon()
        {
            state = AppState.newPolygonBegin;
            buttonNewPolygon_SetState_newPolygon();
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                switch (state)
                {
                    case AppState.newPolygonBegin:
                        SetState_Ready();
                        break;
                    case AppState.newPolygonDrawing:
                        tempPolygon.vertices.RemoveAt(tempPolygon.vertices.Count - 1);
                        if (tempPolygon.vertices.Count <= 0)
                        {
                            SetState_Ready();
                        }
                        else
                        {
                            canvas.Invalidate();
                        }
                        break;
                    default:
                        break;
                }

                return;
            }


            if (e.Button == MouseButtons.Left)
            {
                Point position = new Point(e.X, e.Y);
                switch (state)
                {
                    case AppState.newPolygonBegin:
                        tempPolygon = new Polygon();
                        tempPolygon.vertices = new List<Vertex>();
                        tempPolygon.vertices.Add(new Vertex(position, brushVertexHighlight));
                        //DrawVertex(v);
                        canvas.Invalidate();
                        state = AppState.newPolygonDrawing;
                        break;
                    case AppState.newPolygonDrawing:
                        int dx = position.X - tempPolygon.vertices[0].point.X;
                        int dy = position.Y - tempPolygon.vertices[0].point.Y;

                        // If most recent click is within the first vertex of the polygon, finish drawing polygon
                        if (dx * dx + dy * dy < 2 * radius * radius)
                        {
                            if (tempPolygon.vertices.Count > 2)
                            {
                                tempPolygon.SetVertexBrush(brushVertex);
                                polygons.Add(tempPolygon);                                
                                SetState_Ready();
                            }
                        }
                        else
                        {
                            foreach (Polygon p in polygons)
                            {
                                foreach (Vertex v in p.vertices)
                                {
                                    dx = position.X - v.point.X;
                                    dy = position.Y - v.point.Y;
                                    if (dx * dx + dy * dy < 4 * radius * radius) return;
                                }
                            }
                            foreach (Vertex v in tempPolygon.vertices)
                            {
                                dx = position.X - v.point.X;
                                dy = position.Y - v.point.Y;
                                if (dx * dx + dy * dy < 4 * radius * radius) return;
                            }

                            //Vertex newVertex = new Vertex(position);
                            tempPolygon.vertices.Add(new Vertex(position, brushVertexHighlight));
                            //DrawVertex(newVertex);
                            //DrawEdge(tempPolygon.vertices[tempPolygon.vertices.Count - 2], newVertex);
                        }
                        canvas.Invalidate();
                        break;
                    case AppState.ready:
                        selectedVertex = null;
                        foreach (Polygon p in polygons)
                        {
                            foreach (Vertex v in p.vertices)
                            {
                                dx = position.X - v.point.X;
                                dy = position.Y - v.point.Y;
                                if (dx * dx + dy * dy < 2 * radius * radius)
                                {
                                    selectedVertex = v;
                                    break;
                                }
                            }
                            if (selectedVertex != null) break;
                        }

                        if (selectedVertex == null)
                        {
                            buttonDebug.Text = "No vertex selected";
                            return;
                        }

                        buttonDebug.Text = "Vertex selected";

                        selectedVertex.brush = brushVertexSelected;
                        isPressedLMB = true;
                        buttonLMB.Text = "LMB true";
                        break;
                    default:
                        break;
                }

                return;
            }
        }
        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isPressedLMB == true)
                {
                    buttonDebug.Text = "Vertex unselected";
                    isPressedLMB = false;
                    selectedVertex.brush = brushVertex;
                    selectedVertex = null;
                    canvas.Invalidate();
                    buttonLMB.Text = "LMB false";
                }
            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            while (isPressedLMB == true && canvas.Focused)
            {
                buttonDebug.Text = "Vertex shmovin'";
                canvas.Invalidate();
                selectedVertex.ChangePlacement(e.Location);
                canvas.Invalidate();
            }
        }

        private void buttonNewPolygon_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                if(state == AppState.ready)
                {
                    SetState_newPolygon();
                }
                else
                {
                    SetState_Ready();
                }
        }

        private void buttonClearSpace_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                polygons.Clear();
                SetState_Ready();
            }
        }

        public void DrawVertex(Vertex v)
        {
            if (v == null) return;
            rec.Location = new System.Drawing.Point(v.point.X - radius, v.point.Y - radius);
            graph.FillEllipse(brushVertex, rec);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            canvas.Image.Dispose();
        }

        public void DrawEdge(Vertex vp, Vertex vk)
        {
            if (vp == null || vk == null) return;
            graph.DrawLine(penEdge, vp.point, vk.point);
        }
        public void DrawPolygon(Polygon p)
        {
            if (p == null) return;
            foreach (Vertex v in p.vertices)
            {
                DrawVertex(v);
            }

            for (int i = 0; i < p.vertices.Count; i++)
            {
                DrawEdge(p.vertices[i], p.vertices[(i + 1) % p.vertices.Count]);
            }
        }
        public void DrawUnfinishedPolygon(Polygon p)
        {
            if (p == null) return;

            foreach (Vertex v in tempPolygon.vertices)
            {
                DrawVertex(v);
            }

            if (tempPolygon.vertices.Count > 1)
                for (int i = 0; i < tempPolygon.vertices.Count - 1; i++)
                {
                    DrawEdge(p.vertices[i], p.vertices[i + 1]);
                }
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            //using (Graphics g = Graphics.FromImage(drawArea))
            //{
            //    if (tempPolygon != null)
            //    {
            //        foreach (Vertex v in tempPolygon.vertices)
            //        {
            //            //rec.Location = new System.Drawing.Point(v.point.X - radius, v.point.Y - radius);
            //            //g.FillEllipse(brushVertex, rec);
            //            g.FillEllipse(brushVertex, v.point.X - radius, v.point.Y - radius, 2 * radius, 2 * radius);
            //        }

            //        if (tempPolygon.vertices.Count > 1)
            //            for (int i = 0; i < tempPolygon.vertices.Count - 1; i++)
            //            {
            //                g.DrawLine(penEdge, tempPolygon.vertices[i].point, tempPolygon.vertices[i + 1].point);
            //            }
            //    }

            //    if (polygons != null)
            //        foreach (Polygon p in polygons)
            //        {
            //            foreach (Vertex v in p.vertices)
            //            {
            //                //rec.Location = new System.Drawing.Point(v.point.X - radius, v.point.Y - radius);
            //                //g.FillEllipse(brushVertex, rec);
            //                g.FillEllipse(brushVertex, v.point.X - radius, v.point.Y - radius, 2 * radius, 2 * radius);
            //            }

            //            for (int i = 0; i < p.vertices.Count; i++)
            //            {
            //                g.DrawLine(penEdge, p.vertices[i].point, p.vertices[(i + 1) % p.vertices.Count].point);
            //            }
            //        }
            //}

            if (tempPolygon != null)
            {
                if (tempPolygon.vertices.Count > 1)
                    for (int i = 0; i < tempPolygon.vertices.Count - 1; i++)
                    {
                        e.Graphics.DrawLine(penEdgeHighlight, tempPolygon.vertices[i].point, tempPolygon.vertices[i + 1].point);
                    }

                foreach (Vertex v in tempPolygon.vertices)
                {
                    e.Graphics.FillEllipse(v.brush, v.point.X - radius, v.point.Y - radius, 2 * radius, 2 * radius);
                }
            }

            if (polygons != null)
                foreach (Polygon p in polygons)
                {
                    for (int i = 0; i < p.vertices.Count; i++)
                    {
                        e.Graphics.DrawLine(penEdge, p.vertices[i].point, p.vertices[(i + 1) % p.vertices.Count].point);
                    }

                    foreach (Vertex v in p.vertices)
                    {
                        e.Graphics.FillEllipse(v.brush, v.point.X - radius, v.point.Y - radius, 2 * radius, 2 * radius);
                    }
                }
        }
    }
}
