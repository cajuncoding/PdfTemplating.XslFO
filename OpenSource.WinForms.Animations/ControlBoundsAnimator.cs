using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Animations
{
	/// <summary>
	/// Class inheriting <see cref="AnimatorBase"/> to animate the
	/// <see cref="System.Windows.Forms.Control.Bounds"/> of a <see cref="Control"/>.
	/// </summary>
	public class ControlBoundsAnimator : AnimatorBase
	{
		#region Fields

		private const bool DEFAULT_ANIMATE = true;

		private Control _control;
		private Rectangle _startBounds;
		private Rectangle _endBounds;
		private bool _animateX = DEFAULT_ANIMATE;
		private bool _animateY = DEFAULT_ANIMATE;
		private bool _animateWidth = DEFAULT_ANIMATE;
		private bool _animateHeight = DEFAULT_ANIMATE;		

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="container">Container the new instance should be added to.</param>
		public ControlBoundsAnimator(IContainer container) : base(container) 
		{
			Initialize();
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ControlBoundsAnimator() 
		{
			Initialize();
		}

		private void Initialize()
		{
			_startBounds = DefaultStartBounds;
			_endBounds = DefaultEndBounds;
		}

		#endregion

		#region Public interface

		/// <summary>
		/// Gets or sets the starting bounds for the animation.
		/// </summary>
		[Browsable(true), Category("Appearance")]
		[Description("Gets or sets the starting bounds for the animation.")]
		public Rectangle StartBounds 
		{
			get { return _startBounds; }
			set 
			{
				if (_startBounds == value)
					return;

				_startBounds = value;
				CheckBounds();

				OnStartValueChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the ending bounds for the animation.
		/// </summary>
		[Browsable(true), Category("Appearance")]
		[Description("Gets or sets the ending bounds for the animation.")]
		public Rectangle EndBounds 
		{
			get { return _endBounds; }
			set 
			{
				if (_endBounds == value)
					return;

				_endBounds = value;
				CheckBounds();

				OnEndValueChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="System.Windows.Forms.Control.Left"/> of the 
		/// <see cref="Control"/> should be animated.
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(DEFAULT_ANIMATE)]
		[Description("Gets or sets whether the Left property of the control should be animated.")]
		public bool AnimateX
		{
			get { return _animateX; }
			set 
			{
				_animateX = value;
				CheckBounds();
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="System.Windows.Forms.Control.Top"/> of the 
		/// <see cref="Control"/> should be animated.
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(DEFAULT_ANIMATE)]
		[Description("Gets or sets whether the Top property of the control should be animated.")]
		public bool AnimateY 
		{
			get { return _animateY; }
			set 
			{
				_animateY = value;
				CheckBounds();
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="System.Windows.Forms.Control.Width"/> of the 
		/// <see cref="Control"/> should be animated.
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(DEFAULT_ANIMATE)]
		[Description("Gets or sets whether the Width property of the control should be animated.")]
		public bool AnimateWidth
		{
			get { return _animateWidth; }
			set 
			{
				_animateWidth = value;
				CheckBounds();
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="System.Windows.Forms.Control.Height"/> of the 
		/// <see cref="Control"/> should be animated.
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(DEFAULT_ANIMATE)]
		[Description("Gets or sets whether the Height property of the control should be animated.")]
		public bool AnimateHeight 
		{
			get { return _animateHeight; }
			set 
			{
				_animateHeight = value;
				CheckBounds();
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="Control"/> which 
		/// <see cref="System.Windows.Forms.Control.Bounds"/> should be animated.
		/// </summary>
		[Browsable(true), Category("Behavior")]
		[DefaultValue(null), RefreshProperties(RefreshProperties.Repaint)]
		[Description("Gets or sets which Control should be animated.")]
		public Control Control
		{
			get { return _control; }
			set
			{
				if (_control == value)
					return;

				if (_control != null)
				{
					_control.LocationChanged -= new EventHandler(OnCurrentValueChanged);
					_control.SizeChanged -= new EventHandler(OnCurrentValueChanged);
				}

				_control = value;

				if (_control != null)
				{
					_control.LocationChanged += new EventHandler(OnCurrentValueChanged);
					_control.SizeChanged += new EventHandler(OnCurrentValueChanged);
				}

				base.ResetValues();
			}
		}

		#endregion

		#region Protected Interface

		/// <summary>
		/// Gets the default value of the <see cref="StartBounds"/> property.
		/// </summary>
		protected virtual Rectangle DefaultStartBounds
		{
			get { return Rectangle.Empty; }
		}

		/// <summary>
		/// Gets the default value of the <see cref="EndBounds"/> property.
		/// </summary>
		protected virtual Rectangle DefaultEndBounds
		{
			get { return Rectangle.Empty; }
		}
		
		/// <summary>
		/// Indicates the designer whether <see cref="StartBounds"/> needs
		/// to be serialized.
		/// </summary>
		protected virtual bool ShouldSerializeStartBounds()
		{
			return _startBounds != DefaultStartBounds;
		}

		/// <summary>
		/// Indicates the designer whether <see cref="EndBounds"/> needs
		/// to be serialized.
		/// </summary>
		protected virtual bool ShouldSerializeEndBounds()
		{
			return _endBounds != DefaultEndBounds;
		}

		#endregion

		#region Overridden from AnimatorBase

		/// <summary>
		/// Gets or sets the currently shown value.
		/// </summary>
		protected override object CurrentValueInternal
		{
			get { return _control == null ? Rectangle.Empty : _control.Bounds; }
			set 
			{
				if (_control != null && _control.Bounds != (Rectangle)value)
					_control.Bounds = (Rectangle)value; 
			}
		}

		/// <summary>
		/// Gets or sets the starting value for the animation.
		/// </summary>
		public override object StartValue
		{
			get { return StartBounds; }
			set { StartBounds = (Rectangle)value; }
		}

		/// <summary>
		/// Gets or sets the ending value for the animation.
		/// </summary>
		public override object EndValue
		{
			get { return EndBounds; }
			set { EndBounds = (Rectangle)value; }
		}

		/// <summary>
		/// Calculates an interpolated value between <see cref="StartValue"/> and
		/// <see cref="EndValue"/> for a given step in %.
		/// Giving 0 will return the <see cref="StartValue"/>.
		/// Giving 100 will return the <see cref="EndValue"/>.
		/// </summary>
		/// <param name="step">Animation step in %</param>
		/// <returns>Interpolated value for the given step.</returns>
		protected override object GetValueForStep(double step)
		{
			return InterpolateRectangles(_startBounds, _endBounds, step);
		}

		/// <summary>
		/// Called whenever the current value changes. 
		/// </summary>
		/// <param name="sender">Sender of the notification.</param>
		/// <param name="e">Event arguments.</param>
		protected override void OnCurrentValueChanged(object sender, EventArgs e)
		{
			base.OnCurrentValueChanged(sender, e);
			CheckBounds();
		}

		#endregion

		#region Privates

		private void CheckBounds(ref Rectangle bounds)
		{
			if (_control != null)
			{
				if (!_animateX)
					bounds.X = _control.Bounds.X;
				if (!_animateY)
					bounds.Y = _control.Bounds.Y;
				if (!_animateWidth)
					bounds.Width = _control.Bounds.Width;
				if (!_animateHeight)
					bounds.Height = _control.Bounds.Height;
			}
		}

		private void CheckBounds()
		{
			CheckBounds(ref _startBounds);
			CheckBounds(ref _endBounds);
		}

		#endregion
	}
}
