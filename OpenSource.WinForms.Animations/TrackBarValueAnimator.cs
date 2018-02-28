using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Animations
{
	/// <summary>
	/// Class inheriting <see cref="AnimatorBase"/> to animate the
	/// <see cref="System.Windows.Forms.TrackBar.Value"/> of a <see cref="TrackBar"/>.
	/// </summary>
	public class TrackBarValueAnimator : AnimatorBase
	{
		#region Fields

		private const int DEFAULT_VALUE = 0;

		private TrackBar _trackBar;
		private int _startValue = DEFAULT_VALUE;
		private int _endValue = DEFAULT_VALUE;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="container">Container the new instance should be added to.</param>
		public TrackBarValueAnimator(IContainer container) : base(container) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public TrackBarValueAnimator() {}

		#endregion

		#region Public interface

		/// <summary>
		/// Gets or sets the starting Value for the animation.
		/// </summary>
		[Browsable(true), DefaultValue(DEFAULT_VALUE), Category("Appearance")]
		[Description("Gets or sets the starting Value for the animation.")]
		public int StartTrackBarValue 
		{
			get { return _startValue; }
			set 
			{
				if (_startValue == value)
					return;

				_startValue = value;

				CheckValues();

				OnStartValueChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the ending Value for the animation.
		/// </summary>
		[Browsable(true), DefaultValue(DEFAULT_VALUE), Category("Appearance")]
		[Description("Gets or sets the ending Value for the animation.")]
		public int EndTrackBarValue 
		{
			get { return _endValue; }
			set 
			{
				if (_endValue == value)
					return;

				_endValue = value;

				CheckValues();

				OnEndValueChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="TrackBar"/> which 
		/// <see cref="System.Windows.Forms.TrackBar.Value"/> should be animated.
		/// </summary>
		[Browsable(true), DefaultValue(null), Category("Behavior")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Description("Gets or sets which TrackBar should be animated.")]
		public TrackBar TrackBar
		{
			get { return _trackBar; }
			set
			{
				if (_trackBar == value)
					return;

				if (_trackBar != null)
					_trackBar.ValueChanged -= new EventHandler(OnCurrentValueChanged);				

				_trackBar = value;

				if (_trackBar != null)
					_trackBar.ValueChanged += new EventHandler(OnCurrentValueChanged);

				base.ResetValues();
			}
		}

		#endregion

		#region Overridden from AnimatorBase

		/// <summary>
		/// Gets or sets the currently shown value.
		/// </summary>
		protected override object CurrentValueInternal
		{
			get { return _trackBar == null ? 0 : _trackBar.Value; }
			set 
			{
				if (_trackBar != null)
					_trackBar.Value = (int)value; 
			}
		}

		/// <summary>
		/// Gets or sets the starting value for the animation.
		/// </summary>
		public override object StartValue
		{
			get { return StartTrackBarValue; }
			set { StartTrackBarValue = (int)value; }
		}

		/// <summary>
		/// Gets or sets the ending value for the animation.
		/// </summary>
		public override object EndValue
		{
			get { return EndTrackBarValue; }
			set { EndTrackBarValue = (int)value; }
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
			return CheckValue(InterpolateIntegerValues(_startValue, _endValue, step));
		}

		/// <summary>
		/// Called whenever the current value changes. 
		/// </summary>
		/// <param name="sender">Sender of the notification.</param>
		/// <param name="e">Event arguments.</param>
		protected override void OnCurrentValueChanged(object sender, EventArgs e)
		{
			base.OnCurrentValueChanged(sender, e);
			CheckValues();
		}

		#endregion

		#region Privates

		private void CheckValues()
		{
			_startValue = CheckValue(_startValue);
			_endValue = CheckValue(_endValue);
		}

		private int CheckValue(int value)
		{
			if (_trackBar == null)
				return value;

			if (value < TrackBar.Minimum)
				value = TrackBar.Minimum;
			else if (value > TrackBar.Maximum)
				value = TrackBar.Maximum;

			return value;
		}

		#endregion
	}
}
