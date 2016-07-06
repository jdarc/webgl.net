using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WebGL;

namespace Demo.THREE
{
    public partial class BaseForm : Form
    {
        protected readonly HTMLCanvasElement canvas;
        protected readonly WebGLRenderingContext context;
        protected bool rendering;

        public BaseForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            var workingArea = Screen.PrimaryScreen.WorkingArea;
            Size = new Size((workingArea.Width * 98) / 100, (workingArea.Height * 98) / 100);

            canvas = new HTMLCanvasElement(this);
            context = (WebGLRenderingContext)canvas.getContext("webgl");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            var name = GetType().Name;
            Text = Regex.Replace(name.Substring(0, name.Length - 4), "([a-z])([A-Z])", "$1 $2");

            initialize();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else
            {
                onKeyDown(e);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            onMouseClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            onMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            onMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            onMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            onWindowResize(e);
        }

        protected double aspectRatio
        {
            get { return ClientSize.Width / (double)ClientSize.Height; }
        }

        protected PointF windowHalf
        {
            get { return new PointF(ClientSize.Width * 0.5f, ClientSize.Height * 0.5f); }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            context.Dispose();
            base.OnFormClosing(e);
        }

        protected virtual void onWindowResize(EventArgs eventArgs)
        {
        }

        protected virtual void onKeyDown(KeyEventArgs keyEventArgs)
        {
        }

        protected virtual void onMouseUp(MouseEventArgs mouseEventArgs)
        {
        }

        protected virtual void onMouseDown(MouseEventArgs mouseEventArgs)
        {
        }

        protected virtual void onMouseMove(MouseEventArgs mouseEventArgs)
        {
        }

        protected virtual void onMouseClick(MouseEventArgs e)
        {
        }

        protected virtual void initialize()
        {
        }

        protected virtual void update()
        {
        }

        protected virtual void render()
        {
        }

        private void tick(object sender, EventArgs e)
        {
            if (rendering)
            {
                return;
            }
            rendering = true;
            try
            {
                context.makeCurrent();
                update();
                render();
                context.swapBuffers();
            }
            catch (Exception ex)
            {
                JSConsole.error(ex.Source);
                JSConsole.error(ex.Message);
                JSConsole.error(ex.StackTrace);
                Application.Exit();
            }
            rendering = false;
        }
    }
}