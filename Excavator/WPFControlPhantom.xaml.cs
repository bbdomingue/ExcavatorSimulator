using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;


using SamSeifert.GlobalEvents;


namespace Excavator
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class WPFControlPhantom : UserControl
    {
        public float[] angles = new float[4];

        const float scaleToScreen = 2 / 5.5f;

        const float arm0Length = ControlPhantom.arm0LengthInches * scaleToScreen;
        const float arm1Length = ControlPhantom.arm1LengthInches * scaleToScreen;
        const float arm2Length = ControlPhantom.arm2LengthInches * scaleToScreen;
        const float arm3Length = ControlPhantom.arm3LengthInches * scaleToScreen;

        const float armWidth = 0.3f;
        const float armDepth = 0.5f;
        const float jointRad = 0.35f;

        const float handleDim = 0.5f;

        const float baseCylHeight = 0.5f;
        const float baseCylRad = 2.0f;

        const int circ_count = 8; // count for half circle


        private Color _Color = Colors.Black;
        private int _IntFrameCount = 0;

        private ModelVisual3D _ModelVisual3D_P0 = new ModelVisual3D();
        private ModelVisual3D _ModelVisual3D_P1 = new ModelVisual3D();
        private ModelVisual3D _ModelVisual3D_P2 = new ModelVisual3D();
        private ModelVisual3D _ModelVisual3D_P3 = new ModelVisual3D();

        internal int get_IntFrameCount()
        {
            int temp = this._IntFrameCount;
            this._IntFrameCount = 0;
            return temp;
        }

        public WPFControlPhantom()
        {
            InitializeComponent();

//            CompositionTarget.Rendering += (sender, eventargs) => { this._IntFrameCount++; };
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            Point3D p0, p1, p2, p3;

            this._Color = Colors.DarkGray;
            Model3DGroup surface = new Model3DGroup();
            const int dim = 3;
            p0 = new Point3D(-dim, 0, -dim);
            p1 = new Point3D(dim, 0, -dim);
            p2 = new Point3D(-dim, 0, dim / 2);
            p3 = new Point3D(dim, 0, dim / 2);
            createTriangle(p2, p1, p0, surface);
            createTriangle(p1, p2, p3, surface);
            ModelVisual3D m1 = new ModelVisual3D();
            m1.Content = surface;
            this.mainViewport.Children.Add(m1);

            this._ModelVisual3D_P0.Content = this.createArm0();
            this._ModelVisual3D_P1.Content = this.createArm1();
            this._ModelVisual3D_P2.Content = this.createArm2();
            this._ModelVisual3D_P3.Content = this.createArm3();

            this._ModelVisual3D_P2.Children.Add(this._ModelVisual3D_P3);
            this._ModelVisual3D_P1.Children.Add(this._ModelVisual3D_P2);
            this._ModelVisual3D_P0.Children.Add(this._ModelVisual3D_P1);
            this.mainViewport.Children.Add(_ModelVisual3D_P0);

            this.addListeners();
            this.panToAngle();
        }


        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this._IntFrameCount++;

            this._ModelVisual3D_P0.Transform = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0, 1, 0), angles[0]),
                new Point3D(0, 0, 0));

            this._ModelVisual3D_P1.Transform = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(1, 0, 0), angles[1]),
                new Point3D(0, arm0Length, 0));

            this._ModelVisual3D_P2.Transform = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(1, 0, 0), angles[2]),
                new Point3D(0, arm0Length + arm1Length, 0));

            this._ModelVisual3D_P3.Transform = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(1, 0, 0), angles[3]),
                new Point3D(0, arm0Length + arm1Length + arm2Length, 0));
        }



























        private Model3DGroup createArm0()
        {
            this._Color = Colors.LightGreen;
            Model3DGroup _Model3DGroup = new Model3DGroup();

            Point3D p0, p1, p2, p3, p4, p5, p6, p7;
            Vector3D p0V;

            double sh = baseCylHeight;
            Vector3D s = new Vector3D(armWidth * 0.99, arm0Length, armDepth);
            p0 = new Point3D(-s.X / 2, sh, -s.Z / 2);
            p1 = new Point3D(s.X / 2, sh, -s.Z / 2);
            p2 = new Point3D(s.X / 2, s.Y, -s.Z / 2);
            p3 = new Point3D(-s.X / 2, s.Y, -s.Z / 2);
            p4 = new Point3D(-s.X / 2, sh, s.Z / 2);
            p5 = new Point3D(s.X / 2, sh, s.Z / 2);
            p6 = new Point3D(s.X / 2, s.Y, s.Z / 2);
            p7 = new Point3D(-s.X / 2, s.Y, s.Z / 2);
            createTriangle(p6, p2, p3, _Model3DGroup);
            createTriangle(p7, p6, p3, _Model3DGroup);
            createTriangle(p5, p1, p2, _Model3DGroup);
            createTriangle(p6, p5, p2, _Model3DGroup);
            createTriangle(p4, p0, p1, _Model3DGroup);
            createTriangle(p5, p4, p1, _Model3DGroup);
            createTriangle(p7, p3, p0, _Model3DGroup);
            createTriangle(p4, p7, p0, _Model3DGroup);
            createTriangle(p5, p6, p7, _Model3DGroup);
            createTriangle(p4, p5, p7, _Model3DGroup);
            createTriangle(p0, p3, p2, _Model3DGroup);
            createTriangle(p1, p0, p2, _Model3DGroup);

            p0 = new Point3D(0, baseCylHeight, 0);
            for (int i = 0; i < circ_count * 2; i++)
            {
                double ang0 = i * Math.PI / circ_count;
                double ang1 = (i + 1) * Math.PI / circ_count;
                double r = i < circ_count ? baseCylRad : baseCylRad / 3;

                p1 = new Point3D(-r * Math.Cos(ang1), baseCylHeight, -r * Math.Sin(ang1));
                p2 = new Point3D(-r * Math.Cos(ang0), baseCylHeight, -r * Math.Sin(ang0));
                createTriangle(p0, p1, p2, _Model3DGroup);

                p3 = new Point3D(p2.X, 0, p2.Z);
                p4 = new Point3D(p1.X, 0, p1.Z);

                createTriangle(p3, p2, p1, _Model3DGroup);
                createTriangle(p3, p1, p4, _Model3DGroup);
            }
            p0 = new Point3D(-baseCylRad, baseCylHeight, 0);
            p1 = new Point3D(baseCylRad, baseCylHeight, 0);
            p2 = new Point3D(-baseCylRad, 0, 0);
            p3 = new Point3D(baseCylRad, 0, 0);
            createTriangle(p2, p1, p0, _Model3DGroup);
            createTriangle(p1, p2, p3, _Model3DGroup);


            this._Color = Colors.Yellow;
            p0 = new Point3D(-armWidth / 2, s.Y, 0);
            p3 = new Point3D(-p0.X, p0.Y, p0.Z);
            p0V = new Vector3D(p0.X, p0.Y, p0.Z);
            for (int i = 0; i < circ_count * 2; i++)
            {
                double ang0 = i * Math.PI / circ_count;
                double ang1 = (i + 1) * Math.PI / circ_count;
                double r = jointRad;

                p1 = new Point3D(0, r * Math.Sin(ang1), -r * Math.Cos(ang1));
                p2 = new Point3D(0, r * Math.Sin(ang0), -r * Math.Cos(ang0));

                p1 = Point3D.Add(p1, p0V);
                p2 = Point3D.Add(p2, p0V);

                createTriangle(p0, p1, p2, _Model3DGroup);

                p4 = new Point3D(-p2.X, p2.Y, p2.Z);
                p5 = new Point3D(-p1.X, p1.Y, p1.Z);

                createTriangle(p3, p4, p5, _Model3DGroup);

                createTriangle(p1, p4, p2, _Model3DGroup);
                createTriangle(p4, p1, p5, _Model3DGroup);
            }

            return _Model3DGroup;
        }

        private Model3DGroup createArm1()
        {
            this._Color = Colors.Salmon;
            Model3DGroup _Model3DGroup = new Model3DGroup();

            Point3D p0, p1, p2, p3, p4, p5, p6, p7;
            Vector3D p0V;

            double sh = arm0Length;
            Vector3D s = new Vector3D(armWidth * 0.99, sh + arm1Length, armDepth);
            p0 = new Point3D(-s.X / 2, sh, -s.Z / 2);
            p1 = new Point3D(s.X / 2, sh, -s.Z / 2);
            p2 = new Point3D(s.X / 2, s.Y, -s.Z / 2);
            p3 = new Point3D(-s.X / 2, s.Y, -s.Z / 2);
            p4 = new Point3D(-s.X / 2, sh, s.Z / 2);
            p5 = new Point3D(s.X / 2, sh, s.Z / 2);
            p6 = new Point3D(s.X / 2, s.Y, s.Z / 2);
            p7 = new Point3D(-s.X / 2, s.Y, s.Z / 2);
            createTriangle(p6, p2, p3, _Model3DGroup);
            createTriangle(p7, p6, p3, _Model3DGroup);
            createTriangle(p5, p1, p2, _Model3DGroup);
            createTriangle(p6, p5, p2, _Model3DGroup);
            createTriangle(p4, p0, p1, _Model3DGroup);
            createTriangle(p5, p4, p1, _Model3DGroup);
            createTriangle(p7, p3, p0, _Model3DGroup);
            createTriangle(p4, p7, p0, _Model3DGroup);
            createTriangle(p5, p6, p7, _Model3DGroup);
            createTriangle(p4, p5, p7, _Model3DGroup);
            createTriangle(p0, p3, p2, _Model3DGroup);
            createTriangle(p1, p0, p2, _Model3DGroup);

            this._Color = Colors.Yellow;
            p0 = new Point3D(-armWidth / 2, s.Y, 0);
            p3 = new Point3D(-p0.X, p0.Y, p0.Z);
            p0V = new Vector3D(p0.X, p0.Y, p0.Z);
            for (int i = 0; i < circ_count * 2; i++)
            {
                double ang0 = i * Math.PI / circ_count;
                double ang1 = (i + 1) * Math.PI / circ_count;
                double r = jointRad;

                p1 = new Point3D(0, r * Math.Sin(ang1), -r * Math.Cos(ang1));
                p2 = new Point3D(0, r * Math.Sin(ang0), -r * Math.Cos(ang0));

                p1 = Point3D.Add(p1, p0V);
                p2 = Point3D.Add(p2, p0V);

                createTriangle(p0, p1, p2, _Model3DGroup);

                p4 = new Point3D(-p2.X, p2.Y, p2.Z);
                p5 = new Point3D(-p1.X, p1.Y, p1.Z);

                createTriangle(p3, p4, p5, _Model3DGroup);

                createTriangle(p1, p4, p2, _Model3DGroup);
                createTriangle(p4, p1, p5, _Model3DGroup);
            }

            return _Model3DGroup;
        }

        private Model3DGroup createArm2()
        {
            this._Color = Colors.Blue;
            Model3DGroup _Model3DGroup = new Model3DGroup();

            Point3D p0, p1, p2, p3, p4, p5, p6, p7;

            double sh = arm0Length + arm1Length;
            Vector3D s = new Vector3D(armWidth * 0.99, sh + arm2Length, armDepth);
            p0 = new Point3D(-s.X / 2, sh, -s.Z / 2);
            p1 = new Point3D(s.X / 2, sh, -s.Z / 2);
            p2 = new Point3D(s.X / 2, s.Y, -s.Z / 2);
            p3 = new Point3D(-s.X / 2, s.Y, -s.Z / 2);
            p4 = new Point3D(-s.X / 2, sh, s.Z / 2);
            p5 = new Point3D(s.X / 2, sh, s.Z / 2);
            p6 = new Point3D(s.X / 2, s.Y, s.Z / 2);
            p7 = new Point3D(-s.X / 2, s.Y, s.Z / 2);
            createTriangle(p6, p2, p3, _Model3DGroup);
            createTriangle(p7, p6, p3, _Model3DGroup);
            createTriangle(p5, p1, p2, _Model3DGroup);
            createTriangle(p6, p5, p2, _Model3DGroup);
            createTriangle(p4, p0, p1, _Model3DGroup);
            createTriangle(p5, p4, p1, _Model3DGroup);
            createTriangle(p7, p3, p0, _Model3DGroup);
            createTriangle(p4, p7, p0, _Model3DGroup);
            createTriangle(p5, p6, p7, _Model3DGroup);
            createTriangle(p4, p5, p7, _Model3DGroup);
            createTriangle(p0, p3, p2, _Model3DGroup);
            createTriangle(p1, p0, p2, _Model3DGroup);

            this._Color = Colors.Yellow;
            this.createSphere(jointRad, new Point3D(0, s.Y, 0), _Model3DGroup);

            return _Model3DGroup;
        }

        public Model3DGroup createArm3()
        {
            this._Color = Colors.Orange;
            Model3DGroup _Model3DGroup = new Model3DGroup();

            Point3D p0, p1, p2, p3, p4, p5, p6, p7;

            Vector3D offset = new Vector3D(jointRad, arm0Length + arm1Length + arm2Length, 0);

            Vector3D s = new Vector3D(arm3Length, handleDim, handleDim);
            p0 = new Point3D(0, -s.Y / 2, -s.Z / 2);
            p1 = new Point3D(s.X, -s.Y / 2, -s.Z / 2);
            p2 = new Point3D(s.X, s.Y / 2, -s.Z / 2);
            p3 = new Point3D(0, s.Y / 2, -s.Z / 2);
            p4 = new Point3D(0, -s.Y / 2, s.Z / 2);
            p5 = new Point3D(s.X, -s.Y / 2, s.Z / 2);
            p6 = new Point3D(s.X, s.Y / 2, s.Z / 2);
            p7 = new Point3D(0, s.Y / 2, s.Z / 2);

            p0 = Point3D.Add(p0, offset);
            p1 = Point3D.Add(p1, offset);
            p2 = Point3D.Add(p2, offset);
            p3 = Point3D.Add(p3, offset);
            p4 = Point3D.Add(p4, offset);
            p5 = Point3D.Add(p5, offset);
            p6 = Point3D.Add(p6, offset);
            p7 = Point3D.Add(p7, offset);

            createTriangle(p6, p2, p3, _Model3DGroup);
            createTriangle(p7, p6, p3, _Model3DGroup);
            createTriangle(p5, p1, p2, _Model3DGroup);
            createTriangle(p6, p5, p2, _Model3DGroup);
            createTriangle(p4, p0, p1, _Model3DGroup);
            createTriangle(p5, p4, p1, _Model3DGroup);
            createTriangle(p7, p3, p0, _Model3DGroup);
            createTriangle(p4, p7, p0, _Model3DGroup);
            createTriangle(p5, p6, p7, _Model3DGroup);
            createTriangle(p4, p5, p7, _Model3DGroup);
            createTriangle(p0, p3, p2, _Model3DGroup);
            createTriangle(p1, p0, p2, _Model3DGroup);

            return _Model3DGroup;
        }























        private void createTriangle(Point3D p0, Point3D p1, Point3D p2, Model3DGroup m)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            Vector3D normal = CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            Material material = new DiffuseMaterial(new SolidColorBrush(this._Color));
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            m.Children.Add(model);
        }

        private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        private void createSphere(double radius, Point3D center, Model3DGroup m)
        {
            Point3D p0, p1, p2, p3;

            int lx, ly;
            int x2, y2, x, y;

            Vector3D[][] grid = this.getGrid(radius, out lx, out ly);

            for (y = 0, y2 = 1; y < ly - 1; y++, y2++)
            {
                for (x = 0; x < lx; x++)
                {
                    x2 = (x + 1) % lx;

                    p0 = Point3D.Add(center, grid[x][y]);
                    p1 = Point3D.Add(center, grid[x][y2]);
                    p2 = Point3D.Add(center, grid[x2][y2]);
                    p3 = Point3D.Add(center, grid[x2][y]);

                    createTriangle(p0, p2, p3, m);
                    
                    createTriangle(p2, p0, p1, m);
                }
            }
        }

        private Vector3D[][] getGrid(double radius, out int thetaCount, out int phiCount)
        {
            thetaCount = 24;
            int thetaInc = 360 / thetaCount;
            int theta;

            phiCount = 12;
            float phiInc = 180.0f / phiCount;
            float phi;

            phiCount++;

            Vector3D[][] grid = new Vector3D[thetaCount][];

            int x, y;
            for (x = 0, theta = 0; x < thetaCount; x++, theta += thetaInc)
            {
                grid[x] = new Vector3D[phiCount];

                for (y = 0, phi = 0; y < phiCount; y++, phi += phiInc)
                {
                    double vY = Math.Cos(StaticMethods.toRadiansD(phi));
                    double planarDistance = Math.Sin(StaticMethods.toRadiansD(phi));

                    double vZ = Math.Cos(StaticMethods.toRadiansD(theta));
                    double vX = Math.Sin(StaticMethods.toRadiansD(theta));


                    grid[x][y] = new Vector3D(
                        vX * radius * planarDistance,
                        vY * radius,
                        vZ * radius * planarDistance);

                }
            }

            return grid;
        }












































        private bool mouseDown = false;

        private void g_LMouseDown(object sender, EventArgs e)
        {
            Point position = this.PointToScreen(new Point());

            System.Drawing.Point p = System.Windows.Forms.Control.MousePosition;

            p.X -= (int)position.X;
            p.Y -= (int)position.Y;

            this.mouseDown = 
                p.X > 0 && p.X < this.ActualWidth &&
                p.Y > 0 && p.Y < this.ActualHeight;
        }

        private void g_LMouseDrag(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!this.mouseDown) return;

            panH = Math.Min(Math.PI, Math.Max(-Math.PI, panH + e.X / 100.0));
            panV = Math.Min(Math.PI / 2, Math.Max(0, panV + e.Y / 100.0));

            this.panToAngle();
        }
        
        private double panH = - Math.PI / 4;
        private double panV = Math.PI / 4;

        private void panToAngle()
        {
            if (this.mainViewport == null) return;

            PerspectiveCamera c = this.mainViewport.Camera as PerspectiveCamera;

            if (c == null) return;

            const int distance = 7;

            c.FarPlaneDistance = 1000;
            c.LookDirection = new Vector3D(
                Math.Sin(panH) * Math.Cos(panV),
                -Math.Sin(panV),
                -Math.Cos(panH) * Math.Cos(panV));
            c.UpDirection = new Vector3D(0, 1, 0);
            c.NearPlaneDistance = 1;
            c.Position = new Point3D(
                -distance * Math.Sin(panH) * Math.Cos(panV),
                distance * Math.Sin(panV),
                distance * Math.Cos(panH) * Math.Cos(panV));
            c.FieldOfView = 70;
        }

        private void addListeners()
        {
            GlobalEventHandler.LMouseDown += new EventHandler(this.g_LMouseDown);
            GlobalEventHandler.LMouseDrag += new System.Windows.Forms.MouseEventHandler(this.g_LMouseDrag);
        }

        private void removeListeners()
        {
            GlobalEventHandler.LMouseDown -= new EventHandler(this.g_LMouseDown);
            GlobalEventHandler.LMouseDrag -= new System.Windows.Forms.MouseEventHandler(this.g_LMouseDrag);
        }

        internal void Deconstruct()
        {
            this.removeListeners();
        }
    }

}
