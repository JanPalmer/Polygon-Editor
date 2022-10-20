using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PRO1.Entities;

namespace PRO1
{
    //Co działa:
    //    Dodawanie nowego wielokąta
    //    Przesuwanie wierzchołka (LPM na wierzchołku)
    //    Dodawanie wierzchołka w środku wybranej krawędzi (PPM na krawędzi)
    //    Przesuwanie krawędzi (LPM na krawędzi)
    //    Przesuwanie całego wielokąta (Space + LPM na dowolną część wielokąta)
    //    Usuwanie wielokąta (Space + PPM na dowolną część wielokąta)
    //Nie działa:
    //Nie zaimplementowane:
    //    Algorytm Bresenhama
    //    Przełączanie się między algorytmami
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
        public Pen[] pensEdge;
        public Graphics graph;
        public Bitmap drawArea;
        public AppState state;
        public Polygon tempPolygon;
        public List<Polygon> polygons;

        public Vertex selectedVertex = null;
        public Edge selectedEdge = null;
        public Polygon selectedPolygon = null;
        public bool isPressedLMB = false;

        public bool useBresenham;

        public Point originalMousePosition;
        public int vertexIndex;

        public bool isPressedSpace = false;

        //public int EdgeLenSquared(Vertex v1, Vertex v2)
        //{
        //    int x = v1.point.X - v2.point.X;
        //    int y = v1.point.Y - v2.point.Y;
        //    return x * x + y * y;
        //}
        //public int EdgeLenSquared(Point p1, Point p2)
        //{
        //    int x = p1.X - p2.X;
        //    int y = p1.Y - p2.Y;
        //    return x * x + y * y;
        //}
        //public int EdgeLenSquared((int X, int Y) p1, (int X, int Y) p2)
        //{
        //    int x = p1.X - p2.X;
        //    int y = p1.Y - p2.Y;
        //    return x * x + y * y;
        //}
        //public int DistanceFromEdgeSquared((int x, int y) p1, (int x, int y) p2, (int x, int y) p)
        //{
        //    int A = p.x - p1.x;
        //    int B = p.y - p1.y;
        //    int C = p2.x - p1.x;
        //    int D = p2.y - p1.y;

        //    int dot = A * C + B * D;
        //    int len_sq = C * C + D * D;
        //    float param = dot / len_sq;
            
        //    if (param <= 0 || param >= 1) return int.MaxValue;

        //    int xx = p1.x + (int)(param * C);
        //    int yy = p1.y + (int)(param * D);
        //    int dx = p.x - xx, dy = p.y - yy;
        //    return dx * dx + dy * dy;
        //}
        //public int DistanceFromVertexSquared((int x, int y) p, (int x, int y) v)
        //{
        //    int dx = p.x - v.x;
        //    int dy = p.y - v.y;
        //    return dx * dx + dy * dy;
        //}

        public CGP1()
        {
            InitializeComponent();
            rec = new Rectangle(0, 0, radius * 2, radius * 2);
            pensEdge = new Pen[brushesVertex.Length];
            for(int i = 0; i < brushesVertex.Length; i++) pensEdge[i] = new Pen(brushesVertex[i], edgeThickness);

            useBresenham = radioButtonBresenham.Checked;
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

        private void buttonFixedRelation_SetState_AddRelation()
        {
            buttonFixedRelation.BackColor = SystemColors.GradientActiveCaption;
            buttonFixedRelation.Text = "Cancel Adding Relation";
        }

        private void buttonFixedRelation_SetState_Ready()
        {
            buttonFixedRelation.BackColor = SystemColors.Control;
            buttonFixedRelation.Text = "Add Fixed Length Relation";
        }

        private void SetState_Ready()
        {
            tempPolygon = null;
            canvas.Invalidate();
            state = AppState.ready;
            buttonNewPolygon_SetState_Ready();
            buttonFixedRelation_SetState_Ready();
        }
        private void SetState_newPolygon()
        {
            state = AppState.newPolygonBegin;
            buttonNewPolygon_SetState_newPolygon();
        }
        private void SetState_addRelationFixedLength()
        {
            state = AppState.RelationFixedLength;
            buttonFixedRelation_SetState_AddRelation();
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
                foreach(Vertex v in p.vertices)
                {
                    if (DistanceFromVertexSquared(position, v.point) < toleranceRadius * radius * radius)
                    {
                        selectedVertex = v;
                        selectedPolygon = p;
                        break;
                    }
                }

                if (selectedPolygon == null)
                {
                    foreach(Edge e in p.edges)
                    {
                        dist = DistanceFromEdgeSquared(e.v1.point, e.v2.point, position);
                        distShort = Math.Min(dist, distShort);
                        if (dist < toleranceSquared)
                        {
                            selectedEdge = e;
                            selectedPolygon = p;
                            break;
                        }
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
                        tempPolygon.edges[tempPolygon.edges.Count - 1].DiscardRelation();
                        tempPolygon.edges.RemoveAt(tempPolygon.edges.Count - 1);
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
                    case AppState.RelationFixedLength:
                        SearchThroughPolygonsOnClick(position);
                        if (selectedEdge == null)
                        {
                            CGP1.ActiveForm.Text = "No edge selected for a fixed length relation";
                            //buttonDebug.Text = 
                            SetState_Ready();
                            return;
                        }
                        if (selectedEdge.relation != null)
                        {
                            selectedEdge.DiscardRelation();
                            canvas.Invalidate();
                            return;
                        }

                        SetState_Ready();
                        break;
                    case AppState.ready:
                        SearchThroughPolygonsOnClick(position);
                        if (isPressedSpace == true && selectedPolygon != null)
                        {
                            polygons.Remove(selectedPolygon);
                            canvas.Invalidate();
                            return;
                        }

                        if (selectedVertex != null && selectedPolygon.vertices.Count > 3)
                        {
                            List<Vertex> vertices = selectedVertex.GetNeighbors();
                            Edge nuEdge = new Edge(vertices[0], vertices[1], selectedPolygon);
                            foreach (Edge edge in selectedVertex.GetEdges())
                            {
                                edge.DiscardRelation();
                                selectedPolygon.edges.Remove(edge);
                            }
                            selectedPolygon.vertices.Remove(selectedVertex);
                            selectedPolygon.edges.Add(nuEdge);
                        }
                        else if (selectedEdge != null)
                        {
                            int x = (selectedEdge.v1.point.X + selectedEdge.v2.point.X) / 2;
                            int y = (selectedEdge.v1.point.Y + selectedEdge.v2.point.Y) / 2;
                            Vertex nuVertex = new Vertex(new Point(x, y), selectedPolygon, brushesColor.normal);

                            // Add the new Vertex and two new Edges to connect it to its neighbors
                            selectedPolygon.edges.Add(new Edge(selectedEdge.v1, nuVertex, selectedPolygon));
                            selectedPolygon.edges.Add(new Edge(selectedEdge.v2, nuVertex, selectedPolygon));
                            selectedPolygon.vertices.Add(nuVertex);

                            // Discard the edge connecting new Vertex's neighbors
                            selectedEdge.DiscardRelation();
                            selectedPolygon.edges.Remove(selectedEdge);
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
                            tempPolygon.edges = new List<Edge>();
                            tempPolygon.vertices.Add(new Vertex(position, tempPolygon, brushesColor.highlight));
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
                                    Edge nuEdge = new Edge(tempPolygon.vertices.Last(), tempPolygon.vertices.First(), tempPolygon, brushesColor.normal);
                                    tempPolygon.edges.Add(nuEdge);
                                    tempPolygon.SetVertexBrush(brushesColor.normal);
                                    tempPolygon.SetEdgeBrush(brushesColor.normal);
                                    polygons.Add(tempPolygon);
                                    SetState_Ready();
                                }
                            }
                            else
                            {
                                SearchThroughPolygonsOnClick(position);
                                if (selectedPolygon != null) return;

                                Vertex nuVertex = new Vertex(position, tempPolygon, brushesColor.highlight);
                                Edge nuEdge = new Edge(tempPolygon.vertices.Last(), nuVertex, tempPolygon, brushesColor.highlight);
                                tempPolygon.vertices.Add(nuVertex);
                                tempPolygon.edges.Add(nuEdge);
                            }
                            canvas.Invalidate();
                            break;
                        }
                    case AppState.RelationFixedLength:
                        SearchThroughPolygonsOnClick(position);
                        if (selectedEdge == null)
                        {
                            CGP1.ActiveForm.Text = "No edge selected for a fixed length relation";
                            //buttonDebug.Text = 
                            SetState_Ready();
                            return;
                        }
                        if(selectedEdge.relation != null)
                        {
                            selectedEdge.DiscardRelation();                  
                        }
                        else
                        {
                            RelationFixedLength relation = new RelationFixedLength(selectedEdge);
                            selectedEdge.relation = relation;
                        }

                        canvas.Invalidate();
                        break;
                    case AppState.ready:
                        SearchThroughPolygonsOnClick(position);

                        if (selectedPolygon == null)
                        {
                            buttonDebug.Text = "No vertex or edge selected";
                            return;
                        }

                        isPressedLMB = true;
                        //buttonLMB.Text = "LMB true";
                        originalMousePosition = position;

                        if (isPressedSpace == true)
                        {
                            selectedPolygon.SetVertexBrush(brushesColor.highlight);
                            selectedPolygon.SetEdgeBrush(brushesColor.highlight);
                            state = AppState.movePolygon;
                        }else if (selectedVertex != null)
                        {
                            buttonDebug.Text = "Vertex selected";
                            selectedVertex.SetColor(brushesColor.highlight);
                        }
                        else
                            if (selectedEdge != null)
                        {
                            buttonDebug.Text = "Edge selected";
                            selectedEdge.SetColor(brushesColor.highlight);
                            selectedEdge.v1.SetColor(brushesColor.highlight);
                            selectedEdge.v2.SetColor(brushesColor.highlight);
                        }
                        break;
                    default:
                        break;
                }

                return;
            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isPressedLMB == true)
            {
                int dx = e.X - originalMousePosition.X;
                int dy = e.Y - originalMousePosition.Y;
                if (state == AppState.movePolygon)
                {
                    foreach (Vertex v in selectedPolygon.vertices)
                    {
                        v.Move(dx, dy);
                    }
                }
                else if (selectedVertex != null)
                {
                    buttonDebug.Text = "Vertex shmovin'";
                    selectedVertex.Move(dx, dy);
                    selectedVertex.EnforceRelation();
                }
                else if (selectedEdge != null)
                {
                    buttonDebug.Text = "Edge shmovin'";
                    selectedEdge.Move(dx, dy);
                    selectedEdge.EnforceRelation();
                }

                canvas.Invalidate();
                originalMousePosition = e.Location;
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
                        selectedPolygon.SetEdgeBrush(brushesColor.normal);
                        selectedPolygon = null;
                        state = AppState.ready;
                        canvas.Invalidate();
                        //buttonLMB.Text = "LMB false";
                    }
                    else if(selectedVertex != null)
                    {
                        buttonDebug.Text = "Vertex released";
                        selectedVertex.SetColor(brushesColor.normal);
                        selectedPolygon.inBFS = false;
                        selectedVertex.EnforceRelation();
                        selectedVertex = null;
                        canvas.Invalidate();
                        //buttonLMB.Text = "LMB false";
                    }
                    else if(selectedEdge != null)
                    {
                        buttonDebug.Text = "Edge released";
                        selectedEdge.SetColor(brushesColor.normal);
                        selectedEdge.v1.SetColor(brushesColor.normal);
                        selectedEdge.v2.SetColor(brushesColor.normal);
                        selectedPolygon.inBFS = false;
                        selectedEdge.EnforceRelation();
                        selectedEdge = null;
                        canvas.Invalidate();
                        //buttonLMB.Text = "LMB false";
                    }
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
            if (e.KeyCode == Keys.Space)
            {
                //state = AppState.movePolygon;
                isPressedSpace = true;
                buttonSpace.Text = "Space pressed";
            }
        }

        private void CGP1_KeyUp(object sender, KeyEventArgs e)
        {
            //buttonSpace.Text = $"{e.KeyCode}";
            if (e.KeyCode == Keys.Space)
            {
                //state = AppState.ready;
                isPressedSpace = false;
                buttonSpace.Text = "Space depressed";
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

        private void radioButtonBresenham_CheckedChanged(object sender, EventArgs e)
        {
            useBresenham = !useBresenham;
            if (useBresenham)
            {
                buttonDebug.Text = "Using Bresenham";
            }
            else
            {
                buttonDebug.Text = "Using Build-in";
            }
            canvas.Invalidate();
        }

        private void buttonFixedRelation_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (state == AppState.ready)
                {
                    SetState_addRelationFixedLength();
                }
                else
                {
                    SetState_Ready();
                }
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

        public void DrawPoint(Graphics g, brushesColor color, Point p)
        {
            //if(p.X > 0 && p.Y > 0 && p.X < canvas.Image.Width && p.Y < canvas.Image.Height)
            g.FillEllipse(brushesVertex[(int)color], p.X, p.Y, edgeThickness, edgeThickness);
        }

        public void DrawLineBresenham(Graphics g, Point p, Point k, brushesColor color)
        {
            buttonEdge.Text = $"Drawing {p.X},{p.Y} to {k.X},{k.Y}";
            int w = k.X - p.X;
            int h = k.Y - p.Y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;

            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            Point cursor = new Point(p.X, p.Y);

            if(longest <= shortest)
            {
                (longest, shortest) = (shortest, longest);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for(int i = 0; i <= longest; i++)
            {
                buttonDebug.Text = $"{i} out of {longest}";
                DrawPoint(g, color, cursor);
                numerator += shortest;
                if(numerator >= longest)
                {
                    numerator -= longest;
                    cursor.X += dx1;
                    cursor.Y += dy1;
                }
                else
                {
                    cursor.X += dx2;
                    cursor.Y += dy2;
                }
            }
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            brushesColor edgeColor;

            if (tempPolygon != null)
            {
                //if (tempPolygon.vertices.Count > 1)
                //    for (int i = 0; i < tempPolygon.vertices.Count - 1; i++)
                //    {
                //        edgeColor = tempPolygon.vertices[i].brush;
                //        if (useBresenham)
                //        {
                //            DrawLineBresenham(e.Graphics, tempPolygon.vertices[i].point, tempPolygon.vertices[i + 1].point, edgeColor);
                //        }
                //        else
                //        {
                //            e.Graphics.DrawLine(pensEdge[(int)edgeColor], tempPolygon.vertices[i].point, tempPolygon.vertices[i + 1].point);
                //        }
                //    }

                if (tempPolygon.vertices.Count > 1)
                {
                    foreach(Edge edge in tempPolygon.edges)
                    {
                        if (useBresenham)
                            DrawLineBresenham(e.Graphics, edge.v1.point, edge.v2.point, edge.color);
                        else
                            e.Graphics.DrawLine(pensEdge[(int)edge.color], edge.v1.point, edge.v2.point);
                    }
                }

                    foreach (Vertex v in tempPolygon.vertices)
                {
                    e.Graphics.FillEllipse(brushesVertex[(int)v.brush], v.point.X - radius, v.point.Y - radius, 2 * radius, 2 * radius);
                }
            }

            if (polygons != null)
                foreach (Polygon p in polygons)
                {
                    //for (int i = 0; i < p.vertices.Count; i++)
                    //{
                    //    edgeColor = (p.vertices[i].brush == p.vertices[(i + 1) % p.vertices.Count].brush) ? p.vertices[i].brush : brushesColor.normal;
                    //    if (useBresenham)
                    //    {
                    //        DrawLineBresenham(e.Graphics, p.vertices[i].point, p.vertices[(i + 1) % p.vertices.Count].point, edgeColor);
                    //    }
                    //    else
                    //    {
                    //        e.Graphics.DrawLine(pensEdge[(int)edgeColor], p.vertices[i].point, p.vertices[(i + 1) % p.vertices.Count].point);
                    //    }

                    //    //e.Graphics.DrawLine((p.vertices[i].brush == p.vertices[(i + 1) % p.vertices.Count].brush) ? pensEdge[(int)p.vertices[i].brush] : pensEdge[(int)brushesColor.normal], p.vertices[i].point, p.vertices[(i + 1) % p.vertices.Count].point);
                    //}

                    foreach (Edge edge in p.edges)
                    {
                        if (useBresenham)
                            DrawLineBresenham(e.Graphics, edge.v1.point, edge.v2.point, edge.color);
                        else
                            e.Graphics.DrawLine(pensEdge[(int)edge.color], edge.v1.point, edge.v2.point);
                    }

                    foreach (Vertex v in p.vertices)
                    {
                        e.Graphics.FillEllipse(brushesVertex[(int)v.brush], v.point.X - radius, v.point.Y - radius, 2 * radius, 2 * radius);
                    }
                }
        }
    }
}