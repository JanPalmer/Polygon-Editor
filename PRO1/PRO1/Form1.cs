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
    //Co działa:
    //    Dodawanie nowego wielokąta
    //    Przesuwanie wierzchołka
    //    Dodawanie wierzchołka w środku wybranej krawędzi
    //    Przesuwanie całej krawędzi
    //Nie działa:
    //    Przesuwanie krawędzi po dodaniu wierzchołka - po dodaniu wierzchołka klikając w drugą połowę krawędzi
    //    (nowy wierzchołek pojawia się przed kursorem wg kolejności wierzchołków w liście danego wielokąta)
    //    złapanie krawędzi w celu jej przesunięcia przesuwa krawędź poprzednią
    //    Przesuwanie całego wielokąta
    //Nie zaimplementowane:
    //    Algorytm Bresenhama
    //    Przełączanie się między algorytmami
    //    Usuwanie wielokątów (może używając spacji?)
    //    Ograniczenia
    //    Predefiniowana scena
    //Dodatkowo dodane:
    //    Usuwanie wierzchołków w trakcie tworzenia wielokąta poprzez wciśnięcie PPM
    //    Podświetlanie wierzchołków i krawędzi różnorakie

    public partial class CGP1 : Form
    {
        public const int radius = 10, edgeThickness = 2, toleranceSquared = 50, toleranceRadius = 4;
        public Rectangle rec;
        // 0 - normal, 1 - highlight/selected, 2 - constant length, 3 - relation
        public enum brushesColor {normal, highlight, constlen, relation }
        public Brush[] brushesVertex = { Brushes.Black, Brushes.DarkOrange, Brushes.Green, Brushes.Red };
        public Pen[] pensEdge;
        public Graphics graph;
        public Bitmap drawArea;

        public AppState state;
        public Polygon tempPolygon;
        public List<Polygon> polygons;

        public Vertex selectedVertex = null;
        public (Vertex v1, Vertex v2)? selectedEdge = null;
        public Polygon selectedPolygon = null;
        public bool isPressedLMB = false;
        public (int x, int y)[] positionRelative; // array used when moving edges - holds relative positions of 2 vertices based on the point we're holding the edge by

        public Timer clickTimer;
        public TimeSpan doubleClickMaxTime;
        public DateTime lastClick;
        public bool inDoubleClick;
        public Point originalMousePosition;
        public int vertexIndex;

        public bool isPressedSpace = false;

        public enum AppState
        {
            ready,
            newPolygonBegin,
            newPolygonDrawing,
            movePolygon,
            moveEdge,
            moveVertex
        }
        public class Vertex
        {
            public Point point;
            public brushesColor brush { get; set; }
            public void ChangePlacement(Point _p)
            {
                point = _p;
            }
            public void ChangePlacement(int _X, int _Y)
            {
                point.X = _X;
                point.Y = _Y;
            }
            public void Move(int _dX, int _dY)
            {
                point.X += _dX;
                point.Y += _dY;
            }
            public Vertex(Point _p, brushesColor _brush)
            {
                point = _p;
                brush = _brush;
            }
        }
        public class Polygon
        {
            public List<Vertex> vertices;

            public void SetVertexBrush(brushesColor _brush)
            {
                foreach(Vertex v in this.vertices)
                {
                    v.brush = _brush;
                }
            }
        }

        public int EdgeLenSquared(Vertex v1, Vertex v2)
        {
            int x = v1.point.X - v2.point.X;
            int y = v1.point.Y - v2.point.Y;
            return x * x + y * y;
        }
        public int EdgeLenSquared(Point p1, Point p2)
        {
            int x = p1.X - p2.X;
            int y = p1.Y - p2.Y;
            return x * x + y * y;
        }
        public int EdgeLenSquared((int X, int Y) p1, (int X, int Y) p2)
        {
            int x = p1.X - p2.X;
            int y = p1.Y - p2.Y;
            return x * x + y * y;
        }
        public int DistanceFromEdgeSquared(Point p1, Point p2, Point p)
        {
            int A = p.X - p1.X;
            int B = p.Y - p1.Y;
            int C = p2.X - p1.X;
            int D = p2.Y - p1.Y;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = dot / len_sq;

            //if (param <= 0 || param >= 1) return int.MaxValue;

            float xx = p1.X + param * C;
            float yy = p1.Y + param * D;
            int dx = p.X -(int)xx, dy = p.Y - (int)yy;
            return dx * dx + dy * dy;
        }
        public int DistanceFromEdgeSquared((int x, int y) p1, (int x, int y) p2, (int x, int y) p)
        {
            int A = p.x - p1.x;
            int B = p.y - p1.y;
            int C = p2.x - p1.x;
            int D = p2.y - p1.y;

            int dot = A * C + B * D;
            int len_sq = C * C + D * D;
            float param = dot / len_sq;
            
            if (param <= 0 || param >= 1) return int.MaxValue;

            int xx = p1.x + (int)(param * C);
            int yy = p1.y + (int)(param * D);
            int dx = p.x - xx, dy = p.y - yy;
            return dx * dx + dy * dy;
        }
        public int DistanceFromVertexSquared(Point p, Point v)
        {
            int dx = p.X - v.X;
            int dy = p.Y - v.Y;
            return dx * dx + dy * dy;
        }
        public int DistanceFromVertexSquared((int x, int y) p, (int x, int y) v)
        {
            int dx = p.x - v.x;
            int dy = p.y - v.y;
            return dx * dx + dy * dy;
        }

        public CGP1()
        {
            InitializeComponent();
            rec = new Rectangle(0, 0, radius * 2, radius * 2);
            pensEdge = new Pen[brushesVertex.Length];
            for(int i = 0; i < brushesVertex.Length; i++) pensEdge[i] = new Pen(brushesVertex[i], edgeThickness);

            polygons = new List<Polygon>();
            drawArea = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            canvas.Image = drawArea;
            Graphics g = Graphics.FromImage(drawArea);
            g.Clear(Color.White);
            g.Dispose();
            positionRelative = new (int x, int y)[2];

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

        private void SearchThroughPolygonsOnClick(Point position)
        {
            selectedVertex = null;
            selectedEdge = null;
            selectedPolygon = null;
            int dist = int.MaxValue, distShort = int.MaxValue;
            vertexIndex = -1;
            foreach (Polygon p in polygons)
            {
                for (int i = 0; i < p.vertices.Count; i++)
                {
                    if (DistanceFromVertexSquared(position, p.vertices[i].point) < toleranceRadius * radius * radius)
                    {
                        selectedVertex = p.vertices[i];
                        selectedPolygon = p;
                        break;
                    }
                    // If you tried to pick the second vertex in a polygon, this if would catch that and think you picked the edge instead
                    // it caused problems with picking edges over single vertices sometimes, this is why it had to be split into another for loop
                }

                if(selectedPolygon == null)
                    for (int i = 0; i < p.vertices.Count; i++)
                    {
                        dist = DistanceFromEdgeSquared(p.vertices[i].point, p.vertices[(i + 1) % p.vertices.Count].point, position);
                        distShort = Math.Min(dist, distShort);
                        if (dist < toleranceSquared)
                        {
                            selectedEdge = (p.vertices[i], p.vertices[(i + 1) % p.vertices.Count]);
                            selectedPolygon = p;
                            vertexIndex = i;
                            break;
                        }
                    }

                if (selectedPolygon != null) break;
            }

            buttonEdge.Text = $"{distShort}";
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            int dx, dy;
            Point position = new Point(e.X, e.Y);
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
                    case AppState.ready:
                        SearchThroughPolygonsOnClick(position);
                        if (selectedVertex != null && selectedPolygon.vertices.Count > 3)
                        {
                            selectedPolygon.vertices.Remove(selectedVertex);
                        }
                        else if (selectedEdge != null)
                        {
                            int x = (selectedEdge.Value.v1.point.X + selectedEdge.Value.v2.point.X) / 2;
                            int y = (selectedEdge.Value.v1.point.Y + selectedEdge.Value.v2.point.Y) / 2;
                            selectedPolygon.vertices.Insert(vertexIndex + 1, new Vertex(new Point(x, y), brushesColor.normal));
                        }
                        canvas.Invalidate();
                        break;
                    default:
                        break;
                }

                return;
            }


            if (e.Button == MouseButtons.Left)
            {
                switch (state)
                {
                    case AppState.newPolygonBegin:
                        {
                            tempPolygon = new Polygon();
                            tempPolygon.vertices = new List<Vertex>();
                            tempPolygon.vertices.Add(new Vertex(position, brushesColor.highlight));
                            //DrawVertex(v);
                            canvas.Invalidate();
                            state = AppState.newPolygonDrawing;
                            break;
                        }
                    case AppState.newPolygonDrawing:
                        {
                            dx = position.X - tempPolygon.vertices[0].point.X;
                            dy = position.Y - tempPolygon.vertices[0].point.Y;

                            // If most recent click is within the first vertex of the polygon, finish drawing polygon
                            if (dx * dx + dy * dy < 2 * radius * radius)
                            {
                                if (tempPolygon.vertices.Count > 2)
                                {
                                    tempPolygon.SetVertexBrush(brushesColor.normal);
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
                                        if (DistanceFromVertexSquared(position, v.point) < toleranceRadius * radius * radius) return;
                                    }
                                }
                                foreach (Vertex v in tempPolygon.vertices)
                                {
                                    if (DistanceFromVertexSquared(position, v.point) < toleranceRadius * radius * radius) return;
                                }

                                tempPolygon.vertices.Add(new Vertex(position, brushesColor.highlight));
                            }
                            canvas.Invalidate();
                            break;
                        }
                    case AppState.movePolygon:
                        SearchThroughPolygonsOnClick(position);
                        if (selectedPolygon == null)
                        {
                            buttonDebug.Text = "No vertex or edge selected";
                            return;
                        }
                        originalMousePosition = position;
                        isPressedLMB = true;
                        buttonLMB.Text = "LMB true";
                        break;
                    case AppState.ready:
                        SearchThroughPolygonsOnClick(position);

                        if (selectedPolygon == null)
                        {
                            buttonDebug.Text = "No vertex or edge selected";
                            return;
                        }

                        isPressedLMB = true;
                        buttonLMB.Text = "LMB true";

                        if(isPressedSpace == true)
                        {
                            selectedPolygon.SetVertexBrush(brushesColor.highlight);
                            state = AppState.movePolygon;
                        }else if (selectedVertex != null)
                        {
                            buttonDebug.Text = "Vertex selected";
                            selectedVertex.brush = brushesColor.highlight;
                            //positionRelative[0] = selectedVertex.point;
                        }
                        else
                            if (selectedEdge != null)
                        {
                            buttonDebug.Text = "Edge selected";
                            selectedEdge.Value.v1.brush = brushesColor.highlight;
                            selectedEdge.Value.v2.brush = brushesColor.highlight;
                            positionRelative[0] = (position.X - selectedEdge.Value.v1.point.X, position.Y - selectedEdge.Value.v1.point.Y);
                            positionRelative[1] = (position.X - selectedEdge.Value.v2.point.X, position.Y - selectedEdge.Value.v2.point.Y);
                        }
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
                    isPressedLMB = false;
                    if(state == AppState.movePolygon)
                    {
                        buttonDebug.Text = "Polygon released";
                        selectedPolygon.SetVertexBrush(brushesColor.normal);
                        selectedPolygon = null;
                        canvas.Invalidate();
                        buttonLMB.Text = "LMB false";
                    }
                    else if(selectedVertex != null)
                    {
                        buttonDebug.Text = "Vertex released";
                        selectedVertex.brush = brushesColor.normal;
                        selectedVertex = null;
                        canvas.Invalidate();
                        buttonLMB.Text = "LMB false";
                    }
                    else if(selectedEdge != null)
                    {
                        buttonDebug.Text = "Edge released";
                        selectedEdge.Value.v1.brush = brushesColor.normal;
                        selectedEdge.Value.v2.brush = brushesColor.normal;
                        selectedEdge = null;
                        canvas.Invalidate();
                        buttonLMB.Text = "LMB false";
                    }
                }

            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isPressedLMB == true)
            {
                int dx = e.X - originalMousePosition.X;
                int dy = e.Y - originalMousePosition.Y;
                if(state == AppState.movePolygon)
                {
                    foreach(Vertex v in selectedPolygon.vertices)
                    {
                        v.Move(dx, dy);
                    }
                    canvas.Invalidate();
                }
                else if(selectedVertex != null)
                {
                    buttonDebug.Text = "Vertex shmovin'";
                    selectedVertex.ChangePlacement(e.X, e.Y);
                    canvas.Invalidate();
                }
                else if(selectedEdge != null)
                {
                    buttonDebug.Text = "Edge shmovin'";
                    selectedEdge.Value.v1.ChangePlacement(e.X - positionRelative[0].x, e.Y - positionRelative[0].y);
                    selectedEdge.Value.v2.ChangePlacement(e.X - positionRelative[1].x, e.Y - positionRelative[1].y);
                    canvas.Invalidate();
                }
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

        private void CGP1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) isPressedSpace = true;
        }

        private void CGP1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) isPressedSpace = false;
        }

        private void buttonClearSpace_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                polygons.Clear();
                SetState_Ready();
            }
        }

        //public void DrawVertex(Vertex v)
        //{
        //    if (v == null) return;
        //    rec.Location = new System.Drawing.Point(v.point.X - radius, v.point.Y - radius);
        //    graph.FillEllipse(brushVertex, rec);
        //}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            canvas.Image.Dispose();
        }

        //public void DrawEdge(Vertex vp, Vertex vk)
        //{
        //    if (vp == null || vk == null) return;
        //    graph.DrawLine(penEdge, vp.point, vk.point);
        //}
        //public void DrawPolygon(Polygon p)
        //{
        //    if (p == null) return;
        //    foreach (Vertex v in p.vertices)
        //    {
        //        DrawVertex(v);
        //    }

        //    for (int i = 0; i < p.vertices.Count; i++)
        //    {
        //        DrawEdge(p.vertices[i], p.vertices[(i + 1) % p.vertices.Count]);
        //    }
        //}
        //public void DrawUnfinishedPolygon(Polygon p)
        //{
        //    if (p == null) return;

        //    foreach (Vertex v in tempPolygon.vertices)
        //    {
        //        DrawVertex(v);
        //    }

        //    if (tempPolygon.vertices.Count > 1)
        //        for (int i = 0; i < tempPolygon.vertices.Count - 1; i++)
        //        {
        //            DrawEdge(p.vertices[i], p.vertices[i + 1]);
        //        }
        //}

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            if (tempPolygon != null)
            {
                if (tempPolygon.vertices.Count > 1)
                    for (int i = 0; i < tempPolygon.vertices.Count - 1; i++)
                    {
                        e.Graphics.DrawLine(pensEdge[(int)tempPolygon.vertices[i].brush], tempPolygon.vertices[i].point, tempPolygon.vertices[i + 1].point);
                    }

                foreach (Vertex v in tempPolygon.vertices)
                {
                    e.Graphics.FillEllipse(brushesVertex[(int)v.brush], v.point.X - radius, v.point.Y - radius, 2 * radius, 2 * radius);
                }
            }

            if (polygons != null)
                foreach (Polygon p in polygons)
                {
                    for (int i = 0; i < p.vertices.Count; i++)
                    {
                        e.Graphics.DrawLine((p.vertices[i].brush == p.vertices[(i + 1) % p.vertices.Count].brush) ? pensEdge[(int)p.vertices[i].brush] : pensEdge[(int)brushesColor.normal], p.vertices[i].point, p.vertices[(i + 1) % p.vertices.Count].point);
                    }

                    foreach (Vertex v in p.vertices)
                    {
                        e.Graphics.FillEllipse(brushesVertex[(int)v.brush], v.point.X - radius, v.point.Y - radius, 2 * radius, 2 * radius);
                    }
                }
        }
    }
}