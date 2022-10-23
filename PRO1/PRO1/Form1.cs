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
    //    Algorytm Bresenhama
    //    Przełączanie się między algorytmami
    //    Relacja stałej długości krawędzi - trzęsie się, co jest wynikiem opóźnień w obliczeniach przy przesuwaniu.
    //    SPRAWDZIĆ JESZCZE RAZ ALGORYTM DOSUWANIA WIERZCHOŁKA ORAZ MOŻE ZOPTYMALIZOWAĆ METODĘ Vertex.GetEdges()
    //    TRZĘSIE SIĘ PRZY BARDZIEJ GWAŁTOWNYCH RUCHACH
    //Nie działa:
    //Nie zaimplementowane:
    //    Ograniczenia
    //    Predefiniowana scena
    //Dodatkowo dodane:
    //    Usuwanie wierzchołków w trakcie tworzenia wielokąta poprzez wciśnięcie PPM
    //    Podświetlanie wierzchołków i krawędzi różnorakie

    public partial class CGP1 : Form
    {
        public enum AppState
        {
            ready,
            newPolygonBegin,
            newPolygonDrawing,
            movePolygon,
            RelationFixedLength,
            RelationPerpendicular
        }

        public const int radius = 10, edgeThickness = 4, toleranceSquared = 100, toleranceRadius = 5, textOffset = 10;

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

        public bool isPressedSpace = false;

        public Edge perpendicularPretendent;

        public ToolTip tooltip;

        public void InitializeToolTip(ToolTip ttp)
        {
            string bttnHelp = "Default Mode controls:\n" +
                "LMB on vertex/edge - move vertex/edge\n" +
                "RMB on vertex - delete vertex (can't bring down a polygon beneath 3 vertices)\n" +
                "RMB on edge - insert a new vertex in the middle of the edge\n" +
                "Hold SPACE + LMB on vertex/edge - move whole polygon\n" +
                "Hold SPACE + RMB on vertex/edge - delete whole polygon\n" +
                "If you have added any relations, while moving single vertices or edges you might\n" +
                " see edges with relations trying to uphold the constraints brought about by the relations.\n" +
                "Current edge drawing algorithm:\n" +
                "Using the radio buttons in the lower right corner, you can choose which\n" +
                "algorithm should the app use to draw edges of your polygons.\n" +
                "The default drawing algorithm is the version provided by Windows Forms.";
            ttp.SetToolTip(buttonHelp, bttnHelp);
            string bttnNewPolygon =
                "Click to enter Polygon Creation Mode.\n" +
                "LMB - Insert new vertex\n" +
                "RMB - Delete most recent vertex\n" +
                "Finish a polygon by clicking on the starting vertex\n" +
                "You can't insert a vertex too close to an already existing one\n" +
                "Exit Mode by finishing a polygon or clicking this button again.";
            ttp.SetToolTip(buttonNewPolygon, bttnNewPolygon);
            string bttnFixedLengthRel =
                "Click to enter Fixed Length Relation Mode\n" +
                "A Fixed Length Relation will try to keep an edge the same length while changing position of vertices or edges\n" +
                "An edge has a Fixed Length Relation if it is GREEN\n" +
                "LMB on normal edge - Add a Fixed Length Relation\n" +
                "LMB or RMB on edge with a Relation - Delete the Relation\n" +
                "Exit Mode by clicking on empty space in the drawing field or clicking this button again";
            ttp.SetToolTip(buttonFixedRelation, bttnFixedLengthRel);
            string bttnPerpendicularityRel = "Click to enter Perpendicular Relation Mode\n" +
                "A Perpendicular Relation will try to keep a pair of edges as perpendicular to each as possible\n" +
                "Edges belong to the same Perpendicularity Relation if the are both RED and have the same INDEX\n" +
                "LMB on 2 different edges - create a Perpendicular Relation between them\n" +
                "LMB or RMB on an edge with a Relation - Delete the Relation\n" +
                "Exit Mode by adding a Relation, clicking on empty space or clicking this button again";
            ttp.SetToolTip(buttonPerpendicularRelation, bttnPerpendicularityRel);
            string bttnClearField = "Clears the whole drawing field, deleting every polygon";
            ttp.SetToolTip(buttonClearSpace, bttnClearField);
        }

        public CGP1()
        {
            InitializeComponent();
            pensEdge = new Pen[brushesVertex.Length];
            for(int i = 0; i < brushesVertex.Length; i++) pensEdge[i] = new Pen(brushesVertex[i], edgeThickness);

            useBresenham = radioButtonBresenham.Checked;
            polygons = new List<Polygon>();
            drawArea = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            canvas.Image = drawArea;
            Graphics g = Graphics.FromImage(drawArea);
            g.Clear(Color.White);
            g.Dispose();

            tooltip = new ToolTip();
            InitializeToolTip(tooltip);

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

        private void buttonPerpendicularRelation_SetState_AddRelation()
        {
            buttonPerpendicularRelation.BackColor = SystemColors.GradientActiveCaption;
            buttonPerpendicularRelation.Text = "Cancel Adding Relation";
        }
        private void buttonPerpendicularRelation_SetState_Ready()
        {
            buttonPerpendicularRelation.BackColor = SystemColors.Control;
            buttonPerpendicularRelation.Text = "Add Perpendicularity Relation";
        }

        private void SetState_Ready()
        {
            tempPolygon = null;
            DrawFrame();
            state = AppState.ready;
            buttonNewPolygon_SetState_Ready();
            buttonFixedRelation_SetState_Ready();
            buttonPerpendicularRelation_SetState_Ready();
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
        private void SetState_addRelationPerpendicular()
        {
            state = AppState.RelationPerpendicular;
            buttonPerpendicularRelation_SetState_AddRelation();
        }

        private void SearchThroughPolygonsOnClick(Point position)
        {
            selectedVertex = null;
            selectedEdge = null;
            selectedPolygon = null;
            int dist = int.MaxValue;
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
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
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
                            DrawFrame();
                        }
                        break;
                    case AppState.RelationFixedLength:
                        SearchThroughPolygonsOnClick(position);
                        if (selectedEdge == null)
                        {
                            CGP1.ActiveForm.Text = "No edge selected for a fixed length relation";
                            SetState_Ready();
                            return;
                        }
                        if (selectedEdge.relation != null && selectedEdge.relation.GetType() == typeof(RelationFixedLength))
                        {
                            selectedEdge.DiscardRelation();
                            DrawFrame();
                            return;
                        }

                        SetState_Ready();
                        break;
                    case AppState.RelationPerpendicular:
                        SearchThroughPolygonsOnClick(position);
                        if(selectedEdge == null)
                        {
                            CGP1.ActiveForm.Text = "No edge selected for a perpendicularity relation";
                            SetState_Ready();
                            return;
                        }
                        if (selectedEdge.relation != null && selectedEdge.relation.GetType() == typeof(RelationPerpendicular))
                        {
                            selectedEdge.DiscardRelation();
                            DrawFrame();
                            return;
                        }
                        break;
                    case AppState.ready:
                        SearchThroughPolygonsOnClick(position);
                        if (isPressedSpace == true && selectedPolygon != null)
                        {
                            // Remove whole polygon
                            polygons.Remove(selectedPolygon);
                            DrawFrame();
                            return;
                        }

                        if (selectedVertex != null && selectedPolygon.vertices.Count > 3)
                        {
                            // Remove clicked vertex
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
                            selectedPolygon.edges.Add(new Edge(nuVertex, selectedEdge.v2, selectedPolygon));
                            selectedPolygon.vertices.Add(nuVertex);

                            // Discard the edge connecting new Vertex's neighbors
                            selectedEdge.DiscardRelation();
                            selectedPolygon.edges.Remove(selectedEdge);
                        }
                        DrawFrame();
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
                            // Begin drawing new Polygon
                            tempPolygon = new Polygon();
                            tempPolygon.vertices = new List<Vertex>();
                            tempPolygon.edges = new List<Edge>();
                            tempPolygon.vertices.Add(new Vertex(position, tempPolygon, brushesColor.highlight));
                            DrawFrame();
                            state = AppState.newPolygonDrawing;
                            break;
                        }
                    case AppState.newPolygonDrawing:
                        {
                            // Drawing new Polygon
                            int dx = position.X - tempPolygon.vertices[0].point.X;
                            int dy = position.Y - tempPolygon.vertices[0].point.Y;

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
                            DrawFrame();
                            //canvas.Invalidate();
                            break;
                        }
                    case AppState.RelationFixedLength:
                        SearchThroughPolygonsOnClick(position);
                        if (selectedEdge == null)
                        {
                            CGP1.ActiveForm.Text = "No edge selected for a fixed length relation";
                            SetState_Ready();
                            return;
                        }
                        if(selectedEdge.relation != null && selectedEdge.relation.GetType() == typeof(RelationFixedLength))
                        {
                            selectedEdge.DiscardRelation();                  
                        }
                        else
                        {
                            RelationFixedLength relation = new RelationFixedLength(selectedEdge);
                            selectedEdge.relation = relation;
                        }
                        DrawFrame();
                        break;
                    case AppState.RelationPerpendicular:
                        SearchThroughPolygonsOnClick(position);
                        if (selectedEdge == null)
                        {
                            // No edge selected
                            CGP1.ActiveForm.Text = "No edge selected for a perpendicularity relation";
                            if (perpendicularPretendent != null)
                            {
                                perpendicularPretendent.SetColor(brushesColor.normal);
                                perpendicularPretendent = null;
                            }
                            SetState_Ready();
                            return;
                        }
                        if (selectedEdge.relation != null && selectedEdge.relation.GetType() == typeof(RelationPerpendicular))
                        {
                            // Selected an edge with Perpendicular Relation - discarding Relation
                            selectedEdge.DiscardRelation();
                            if(perpendicularPretendent != null)
                            {
                                perpendicularPretendent.SetColor(brushesColor.normal);
                                perpendicularPretendent = null;
                            }
                        }
                        else
                        {
                            // Selected an edge without any Relations
                            if (perpendicularPretendent == null)
                            {
                                // Selected first edge
                                perpendicularPretendent = selectedEdge;
                                perpendicularPretendent.SetColor(brushesColor.RelPerpendicular);
                            }
                            else
                            {
                                // Selected second edge
                                RelationPerpendicular rel = new RelationPerpendicular(selectedEdge, perpendicularPretendent);
                                selectedEdge.relation = rel;
                                perpendicularPretendent.relation = rel;
                                foreach (Polygon p in polygons) p.visited = false;
                                perpendicularPretendent.EnforceRelation(null);
                                perpendicularPretendent = null;
                                SetState_Ready();
                            }
                        }
                        DrawFrame();
                        break;
                    case AppState.ready:
                        SearchThroughPolygonsOnClick(position);

                        if (selectedPolygon == null)
                        {
                            // No Polygon selected
                            CGP1.ActiveForm.Text = "No vertex or edge selected";
                            return;
                        }

                        isPressedLMB = true;
                        originalMousePosition = position;

                        if (isPressedSpace == true)
                        {
                            // Move whole polygon
                            selectedPolygon.SetVertexBrush(brushesColor.highlight);
                            selectedPolygon.SetEdgeBrush(brushesColor.highlight);
                            state = AppState.movePolygon;
                        }else if (selectedVertex != null)
                        {
                            // Move single vertex
                            CGP1.ActiveForm.Text = "Vertex selected";
                            selectedVertex.SetColor(brushesColor.highlight);
                        }
                        else
                            if (selectedEdge != null)
                        {
                            // Move an edge
                            CGP1.ActiveForm.Text = "Edge selected";
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

                foreach (Polygon p in polygons) p.visited = false;
                if (state == AppState.movePolygon)
                {
                    foreach (Vertex v in selectedPolygon.vertices)
                    {
                        v.Move(dx, dy);
                    }
                }
                else if (selectedVertex != null)
                {
                    CGP1.ActiveForm.Text = "Moving Vertex";
                    selectedVertex.Move(dx, dy);
                    selectedVertex.EnforceRelation(null);
                }
                else if (selectedEdge != null)
                {
                    CGP1.ActiveForm.Text = "Moving Edge";
                    selectedEdge.Move(dx, dy);
                    selectedEdge.EnforceRelation(null);
                }

                DrawFrame();
                originalMousePosition = e.Location;
            }
        }
        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isPressedLMB == true)
                {
                    foreach (Polygon p in polygons) p.visited = false;
                    isPressedLMB = false;
                    if(state == AppState.movePolygon)
                    {
                        CGP1.ActiveForm.Text = "Polygon released";
                        selectedPolygon.SetVertexBrush(brushesColor.normal);
                        selectedPolygon.SetEdgeBrush(brushesColor.normal);
                        selectedPolygon = null;
                        state = AppState.ready;
                    }
                    else if(selectedVertex != null)
                    {
                        CGP1.ActiveForm.Text = "Vertex released";
                        selectedVertex.SetColor(brushesColor.normal);
                        selectedVertex.EnforceRelation(null);
                        selectedVertex = null;
                    }
                    else if(selectedEdge != null)
                    {
                        CGP1.ActiveForm.Text = "Edge released";
                        selectedEdge.SetColor(brushesColor.normal);
                        selectedEdge.v1.SetColor(brushesColor.normal);
                        selectedEdge.v2.SetColor(brushesColor.normal);
                        selectedEdge.EnforceRelation(null);
                        selectedEdge = null;
                    }

                    DrawFrame();
                }

            }
        }

        private void buttonNewPolygon_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                if(state == AppState.ready)
                    SetState_newPolygon();
                else
                    SetState_Ready();
        }

        private void CGP1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                isPressedSpace = true;
        }

        private void CGP1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)          
                isPressedSpace = false;
        }

        private void buttonClearSpace_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                polygons.Clear();
                RelationPerpendicular.count = 0;
                SetState_Ready();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            canvas.Image.Dispose();
        }

        private void radioButtonBresenham_CheckedChanged(object sender, EventArgs e)
        {
            useBresenham = !useBresenham;
            if (useBresenham)
            {
                CGP1.ActiveForm.Text = "Using Bresenham";
            }
            else
            {
                Graphics g = Graphics.FromImage(drawArea);
                g.Clear(Color.White);
                CGP1.ActiveForm.Text = "Using Build-in";
            }
            DrawFrame();
        }

        private void buttonFixedRelation_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (state == AppState.ready)
                    SetState_addRelationFixedLength();
                else
                    SetState_Ready();
        }

        private void buttonPerpendicularRelation_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (state == AppState.ready)
                    SetState_addRelationPerpendicular();
                else
                    SetState_Ready();
        }

        public void AddPerpendicularityRelationIndex(Graphics g, Edge edge, RelationPerpendicular rel)
        {
            int x = edge.v1.point.X + (edge.v2.point.X - edge.v1.point.X) / 2 + textOffset;
            int y = edge.v1.point.Y + (edge.v2.point.Y - edge.v1.point.Y) / 2 + textOffset;
            Point p = new Point(x, y);
            g.DrawString(rel.id.ToString(), DefaultFont, brushesVertex[0], p);
        }
        public void DrawEdges(Graphics g, Polygon polygon)
        {
            if (polygon.edges.Count <= 0) return;
            foreach (Edge edge in polygon.edges)
            {
                // If an edge is part of a Perpendicular Relation, add its relation's index next to it
                if (edge.relation != null && edge.relation.GetType() == typeof(RelationPerpendicular))
                    AddPerpendicularityRelationIndex(g, edge, (RelationPerpendicular)edge.relation);
                g.DrawLine(pensEdge[(int)edge.color], edge.v1.point, edge.v2.point);
            }
        }
        public void DrawEdgesBresenham(Graphics g, Polygon polygon)
        {
            if (polygon.edges.Count <= 0) return;
            foreach (Edge edge in polygon.edges)
            {
                if (edge.relation != null && edge.relation.GetType() == typeof(RelationPerpendicular))
                    AddPerpendicularityRelationIndex(graph, edge, (RelationPerpendicular)edge.relation);
                DrawLineBresenham(graph, edge.v1.point, edge.v2.point, GetColorFromBrush(edge.color));
            }
        }
        public void DrawVertices(Graphics g, Polygon polygon)
        {
            foreach (Vertex v in polygon.vertices)
            {
                g.FillEllipse(brushesVertex[(int)v.brush], v.point.X - radius, v.point.Y - radius, 2 * radius, 2 * radius);
            }
        }
        public void DrawPolygon(Graphics g, Polygon polygon)
        {
            if (polygon == null) return;

            // If Bresenham's algorithm has been chosen for drawing edges, edges will be drawn in the DrawFrame() function before PictureBox.Refresh()
            if (!useBresenham)  DrawEdges(g, polygon);
            DrawVertices(g, polygon);
        }
        public void DrawPoint(Graphics g, Color color, Point p)
        {
            if(p.X > 0 && p.Y > 0 && p.X < canvas.Image.Width - 1 && p.Y < canvas.Image.Height - 1)
            {
                for(int y = -1; y <= 1; y++)
                    for(int x = -1; x <= 1; x++)
                    {
                        if ((x ^ y) != 0 || (x == 0 && y == 0))
                        {
                            drawArea.SetPixel(p.X + x, p.Y + y, color);
                        }
                    }
            }
        }
        public void DrawLineBresenham(Graphics g, Point p, Point k, Color color)
        {
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
            //PictureBox's Paint event
            
            DrawPolygon(e.Graphics, tempPolygon);

            if (polygons != null)
                foreach (Polygon p in polygons)
                {
                    DrawPolygon(e.Graphics, p);
                }
        }

        public void DrawFrame()
        {
            // If Bresenham's algorithm is selected for drawing edges, draw edges with it,
            // then Refresh the PictureBox to launch its Paint event to paint vertices (and edges, if Bresenham was not selected)
            // Due to how Winforms' drawing works, drawing on a bitmap has to be done before Refresh(),
            // while functions like DrawLine and FillElipse can be done inside the PictureBox_Paint event
            if (useBresenham)
            {
                Graphics g = Graphics.FromImage(drawArea);
                g.Clear(Color.White);
                DrawEdgesBresenham(g, tempPolygon);

                if (polygons != null)
                    foreach (Polygon p in polygons)
                    {
                        DrawEdgesBresenham(g, p);
                    }
            }

            canvas.Refresh();
        }
    }
}