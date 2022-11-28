using Microsoft.SmallBasic.Library.Internal;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Microsoft.SmallBasic.Library
{
	/// <summary>
	/// The Turtle provides Logo-like functionality to draw shapes by manipulating the properties of a pen and drawing primitives.
	/// </summary>
	[SmallBasicType]
	public static class Turtle
	{
		private const string _turtleName = "_turtle";

		private static bool _initialized = false;

		private static bool _isVisible = true;

		private static double _currentX = 320.0;

		private static double _currentY = 240.0;

		private static FrameworkElement _turtle;

		private static int _speed = 5;

		private static double _angle = 0.0;

		private static RotateTransform _rotateTransform;

		private static bool _penDown = true;

		private static bool _toShow = false;

		/// <summary>
		/// Specifies how fast the turtle should move.  Valid values are 1 to 10.  If Speed is set to 10, the turtle moves and rotates instantly.
		/// </summary>
		public static Primitive Speed
		{
			get
			{
				return _speed;
			}
			set
			{
				_speed = value;
				if (_speed < 1)
				{
					_speed = 1;
				}
				else if (_speed > 10)
				{
					_speed = 10;
				}
			}
		}

		/// <summary>
		/// Gets or sets the current angle of the turtle.  While setting, this will turn the turtle instantly to the new angle.
		/// </summary>
		public static Primitive Angle
		{
			get
			{
				return _angle;
			}
			set
			{
				_angle = value;
				if (_rotateTransform != null)
				{
					GraphicsWindow.Invoke(delegate
					{
						DoubleAnimation animation = new DoubleAnimation
						{
							To = _angle,
							Duration = new Duration(TimeSpan.FromMilliseconds(0.0))
						};
						_rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
					});
				}
			}
		}

		/// <summary>
		/// Gets or sets the X location of the Turtle.  While setting, this will move the turtle instantly to the new location.
		/// </summary>
		public static Primitive X
		{
			get
			{
				return _currentX;
			}
			set
			{
				_currentX = value;
				Shapes.Move("_turtle", _currentX, _currentY);
			}
		}

		/// <summary>
		/// Gets or sets the Y location of the Turtle.  While setting, this will move the turtle instantly to the new location.
		/// </summary>
		public static Primitive Y
		{
			get
			{
				return _currentY;
			}
			set
			{
				_currentY = value;
				Shapes.Move("_turtle", _currentX, _currentY);
			}
		}

		/// <summary>
		/// Shows the Turtle to enable interactions with it.
		/// </summary>
		public static void Show()
		{
			_isVisible = true;
			_toShow = true;
			VerifyAccess();
			_toShow = false;
		}

		/// <summary>
		/// Hides the Turtle and disables interactions with it.
		/// </summary>
		public static void Hide()
		{
			if (_isVisible)
			{
				GraphicsWindow.VerifyAccess();
				GraphicsWindow.Invoke(delegate
				{
					Shapes.Remove("_turtle");
				});
				_isVisible = false;
			}
		}

		/// <summary>
		/// Sets the pen down to enable the turtle to draw as it moves.
		/// </summary>
		public static void PenDown()
		{
			_penDown = true;
		}

		/// <summary>
		/// Lifts the pen up to stop drawing as the turtle moves.
		/// </summary>
		public static void PenUp()
		{
			_penDown = false;
		}

		/// <summary>
		/// Moves the turtle to a specified distance.  If the pen is down, it will draw a line as it moves.
		/// </summary>
		/// <param name="distance">
		/// The distance to move the turtle.
		/// </param>
		public static void Move(Primitive distance)
		{
			VerifyAccess();
			double animateTime = Math.Abs((double)distance.GetAsDecimal() * 320.0 / (double)(_speed * _speed));
			if (_speed == 10)
			{
				animateTime = 5.0;
			}
			double num = _angle / 180.0 * System.Math.PI;
			double newY = _currentY - distance * System.Math.Cos(num);
			double newX = _currentX + distance * System.Math.Sin(num);
			Shapes.Animate("_turtle", newX, newY, animateTime);
			if (_penDown)
			{
				GraphicsWindow.Invoke(delegate
				{
					string name = Shapes.GenerateNewName("_turtleLine");
					Line line = new Line
					{
						Name = name,
						X1 = _currentX,
						Y1 = _currentY,
						Stroke = GraphicsWindow._pen.Brush,
						StrokeThickness = GraphicsWindow._pen.Thickness
					};
					GraphicsWindow.AddShape(name, line);
					DoubleAnimation animation = new DoubleAnimation
					{
						From = _currentX,
						To = newX,
						Duration = new Duration(TimeSpan.FromMilliseconds(animateTime))
					};
					DoubleAnimation animation2 = new DoubleAnimation
					{
						From = _currentY,
						To = newY,
						Duration = new Duration(TimeSpan.FromMilliseconds(animateTime))
					};
					line.BeginAnimation(Line.X2Property, animation);
					line.BeginAnimation(Line.Y2Property, animation2);
				});
			}
			_currentX = newX;
			_currentY = newY;
			WaitForReturn(animateTime);
		}

		/// <summary>
		/// Turns and moves the turtle to the specified location.  If the pen is down, it will draw a line as it moves.
		/// </summary>
		/// <param name="x">
		/// The x co-ordinate of the destination point.
		/// </param>
		/// <param name="y">
		/// The y co-ordinate of the destination point.
		/// </param>
		public static void MoveTo(Primitive x, Primitive y)
		{
			double num = (x - X) * (x - X) + (y - Y) * (y - Y);
			if (num != 0.0)
			{
				double num2 = System.Math.Sqrt(num);
				double num3 = Y - y;
				double num4 = System.Math.Acos(num3 / num2) * 180.0 / System.Math.PI;
				if ((bool)(x < X))
				{
					num4 = 360.0 - num4;
				}
				double num5 = num4 - (double)((int)Angle % 360);
				if (num5 > 180.0)
				{
					num5 -= 360.0;
				}
				Turn(num5);
				Move(num2);
			}
		}

		/// <summary>
		/// Turns the turtle by the specified angle.  Angle is in degrees and can be either positive or negative.  If the angle is positive, the turtle turns to its right.  If it is negative, the turtle turns to its left.
		/// </summary>
		/// <param name="angle">
		/// The angle to turn the turtle.
		/// </param>
		public static void Turn(Primitive angle)
		{
			VerifyAccess();
			double animateTime = Math.Abs((double)angle.GetAsDecimal() * 200.0 / (double)(_speed * _speed));
			if (_speed == 10)
			{
				animateTime = 5.0;
			}
			_angle += angle;
			GraphicsWindow.Invoke(delegate
			{
				DoubleAnimation animation = new DoubleAnimation
				{
					To = _angle,
					Duration = new Duration(TimeSpan.FromMilliseconds(animateTime))
				};
				_rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
			});
			WaitForReturn(animateTime);
		}

		/// <summary>
		/// Turns the turtle 90 degrees to the right.
		/// </summary>
		public static void TurnRight()
		{
			Turn(90);
		}

		/// <summary>
		/// Turns the turtle 90 degrees to the left.
		/// </summary>
		public static void TurnLeft()
		{
			Turn(-90);
		}

		private static void VerifyAccess()
		{
			GraphicsWindow.VerifyAccess();
			if (_initialized && !_toShow)
			{
				return;
			}
			_initialized = true;
			if (!_isVisible)
			{
				return;
			}
			GraphicsWindow.Invoke(delegate
			{
				if (_turtle == null)
				{
					ImageSource source = new BitmapImage(SmallBasicApplication.GetResourceUri("Turtle.png"));
					_turtle = new Image
					{
						Source = source,
						Margin = new Thickness(-8.0, -8.0, 0.0, 0.0),
						Height = 16.0,
						Width = 16.0
					};
					Panel.SetZIndex(_turtle, 1000000);
					_rotateTransform = new RotateTransform
					{
						Angle = _angle,
						CenterX = 8.0,
						CenterY = 8.0
					};
					_turtle.RenderTransform = _rotateTransform;
					_turtle.Width = 16.0;
					_turtle.Height = 16.0;
				}
				Canvas.SetLeft(_turtle, _currentX);
				Canvas.SetTop(_turtle, _currentY);
				GraphicsWindow.AddShape("_turtle", _turtle);
			});
		}

		private static void WaitForReturn(double time)
		{
			if (SmallBasicApplication.HasShutdown)
			{
				return;
			}
			AutoResetEvent evt = new AutoResetEvent(initialState: false);
			SmallBasicApplication.Invoke(delegate
			{
				DispatcherTimer dt = new DispatcherTimer();
				dt.Interval = TimeSpan.FromMilliseconds(time);
				dt.Tick += delegate
				{
					evt.Set();
					dt.Stop();
				};
				dt.Start();
			});
			int millisecondsTimeout = 100;
			if (SmallBasicApplication.Dispatcher.CheckAccess())
			{
				millisecondsTimeout = 10;
			}
			while (!evt.WaitOne(millisecondsTimeout) && !SmallBasicApplication.HasShutdown)
			{
				SmallBasicApplication.ClearDispatcherQueue();
			}
		}
	}
}
