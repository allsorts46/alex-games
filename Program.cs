using System.Windows.Forms;
using System.Xml;
using Timer = System.Threading.Timer;

/*
 *  Score for staying alive longer
 *  Coloured bonus zones - stay inside for score multipliers
 * 
 * 
 * 
*/




namespace Numbers
	{
	internal class Program
		{
		static void Main(string[] args)
			{
			var MyNiceWindow = new LinesWindow();

			Application.Run(MyNiceWindow);
			}
		}



	public class Bouncer
		{
		public Point Where;
		private Int32 MoveyX;
		private Int32 MoveyY;
		private Pen Pen;

		public Bouncer(Pen pen, Size bounds)
			{
			var Choose = new Random().Next(4);

			if(Choose == 0)
				{
				Where = new Point(0, 0);
				MoveyX = 5;
				MoveyY = 5;
				}

			if (Choose == 1)
				{
				Where = new Point(bounds.Width, 0);
				MoveyX = -5;
				MoveyY = 5;
				}

			if (Choose == 2)
				{
				Where = new Point(0, bounds.Height);
				MoveyX = 5;
				MoveyY = -5;
				}
			
			if (Choose == 3)
				{
				Where = new Point(bounds.Width, bounds.Height);
				MoveyX = -5;
				MoveyY = -5;
				}

			Pen = pen;
			}

		public void Move(Size bounds)
			{
			Where = new Point(Where.X + MoveyX, Where.Y + MoveyY);

			if (Where.X > bounds.Width || Where.X <= 0)
				MoveyX = -MoveyX;

			if (Where.Y > bounds.Height || Where.Y <= 0)
				MoveyY = -MoveyY;

			}

		public void Draw(Graphics g, Size bounds)
			{
			LinesWindow.DrawPointingToAPlace(g, bounds, Where, 10, Pen);
			}

		}

	
	
	
	
	internal class LinesWindow : Form
		{
		private Pen[] NicePens;
		private Point Mousey;
		private Timer County;
		private List<Bouncer> Bouncers;

        public LinesWindow()
        {
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			ResizeRedraw = true;

			NicePens = new[]
				{
				new Pen(Color.Blue, 4),
				new Pen(Color.Red, 5),
				new Pen(Color.Green, 4),
				new Pen(Color.Yellow, 5),
				new Pen(Color.Purple, 5),
				new Pen(Color.Gold, 10)
				};

			County = new Timer(AhhhhTimer, null, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));

			Bouncers = new List<Bouncer>();
        }

		private void AhhhhTimer(object? state)
			{
			const Int32 CrashSize = 80;

			var Crashed = false;

			lock(Bouncers)
				{
				foreach (var bouncer in Bouncers)
					{
					bouncer.Move(ClientSize);

					var CrashBox = new Rectangle(bouncer.Where.X - CrashSize, bouncer.Where.Y - CrashSize, CrashSize * 2, CrashSize * 2);

					if (CrashBox.Contains(Mousey))
						Crashed = true;
					}
				}

			if(Crashed)
				AhhhhCrashed();

			Invalidate();
			}

		private void AhhhhCrashed()
			{
			System.Media.SystemSounds.Beep.Play();

			lock (Bouncers)
				Bouncers.Clear();
			}

		protected override void OnPaint(PaintEventArgs e)
			{
			base.OnPaint(e);

			var Bits = 100f;
			var XSpace = ClientSize.Width / Bits;
			var YSpace = ClientSize.Height / Bits;

			for (var i = 0; i < Bits; i++)
				{
				// Top left
				e.Graphics.DrawLine(NicePens[0], i * XSpace, 0, 0, ClientSize.Height - i * YSpace);

				// Top right
				e.Graphics.DrawLine(NicePens[1], ClientSize.Width - i * XSpace, 0, ClientSize.Width, Height - i * YSpace);

				// Bottom left
				e.Graphics.DrawLine(NicePens[2], i * XSpace, ClientSize.Height, 0, i * YSpace);

				// Bottom right
				e.Graphics.DrawLine(NicePens[3], Width - i * XSpace, ClientSize.Height, ClientSize.Width, i * YSpace);
				}


			lock (Bouncers)
				{
				foreach (var bouncer in Bouncers)
					bouncer.Draw(e.Graphics, ClientSize);
				}

			DrawPointingToAPlace(e.Graphics, ClientSize, Mousey, 10, NicePens[5]);
			}

		internal static void DrawPointingToAPlace(Graphics g, Size bounds, Point mousey, Int32 bits, Pen pen)
			{
			var XSpace = bounds.Width / bits;
			var YSpace = bounds.Height / bits;

			for (var x = 0; x < bounds.Width; x += (Int32)XSpace)
				{
				g.DrawLine(pen, x, 0, mousey.X, mousey.Y);
				g.DrawLine(pen, x, bounds.Height, mousey.X, mousey.Y);
				}

			for (var y = 0; y < bounds.Height; y += (Int32)YSpace)
				{
				g.DrawLine(pen, 0, y, mousey.X, mousey.Y);
				g.DrawLine(pen, bounds.Width, y, mousey.X, mousey.Y);
				}
			}

		protected override void OnMouseMove(MouseEventArgs e)
			{
			base.OnMouseMove(e);

			Mousey = e.Location;
			Invalidate();
			}

		protected override void OnMouseClick(MouseEventArgs e)
			{
			base.OnMouseClick(e);

			if (e.Button == MouseButtons.Left)
				{
				lock (Bouncers)
					Bouncers.Add(new Bouncer(Pens.Purple, ClientSize));
				}

			else if (e.Button == MouseButtons.Right)
				{
				lock (Bouncers)
					Bouncers.Clear();
				}
			}
		}
	}