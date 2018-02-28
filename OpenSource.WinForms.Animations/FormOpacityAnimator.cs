using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Animations
{
	/// <summary>
	/// Class inheriting <see cref="AnimatorBase"/> to animate the
	/// <see cref="System.Windows.Forms.Form.Opacity"/> of a <see cref="Form"/>.
	/// </summary>
	public class FormOpacityAnimator : AnimatorBase
	{
		#region Fields

		private const double DEFAULT_OPACITY = 1.0;

		private Form _form;
		private double _startOpacity = DEFAULT_OPACITY;
		private double _endOpacity = DEFAULT_OPACITY;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="container">Container the new instance should be added to.</param>
		public FormOpacityAnimator(IContainer container) : base(container) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public FormOpacityAnimator() {}

		#endregion

		#region Public interface

		/// <summary>
		/// Gets or sets the starting opacity for the animation.
		/// </summary>
		[Category("Appearance"), DefaultValue(DEFAULT_OPACITY)]
		[Browsable(true), TypeConverter(typeof(OpacityConverter))]
		[Description("Gets or sets the starting opacity for the animation.")]
		public double StartOpacity 
		{
			get { return _startOpacity; }
			set 
			{
				if (_startOpacity == value)
					return;

				_startOpacity = value;

				OnStartValueChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the ending opacity for the animation.
		/// </summary>
		[Category("Appearance"), DefaultValue(DEFAULT_OPACITY)]
		[Browsable(true), TypeConverter(typeof(OpacityConverter))]
		[Description("Gets or sets the ending opacity for the animation.")]
		public double EndOpacity 
		{
			get { return _endOpacity; }
			set 
			{
				if (_endOpacity == value)
					return;

				_endOpacity = value;

				OnEndValueChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="Form"/> which 
		/// <see cref="System.Windows.Forms.Form.Opacity"/> should be animated.
		/// </summary>
		[Browsable(true), DefaultValue(null), Category("Behavior")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Description("Gets or sets which Form should be animated.")]
		public Form Form
		{
			get { return _form; }
			set
			{
				if (_form == value)
					return;

				_form = value;

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
			get { return _form == null ? 0.0 : _form.Opacity; }
			set 
			{
				if (_form != null)
					_form.Opacity = (double)value; 
			}
		}

		/// <summary>
		/// Gets or sets the starting value for the animation.
		/// </summary>
		public override object StartValue
		{
			get { return StartOpacity; }
			set { StartOpacity = (double)value; }
		}

		/// <summary>
		/// Gets or sets the ending value for the animation.
		/// </summary>
		public override object EndValue
		{
			get { return EndOpacity; }
			set { EndOpacity = (double)value; }
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
			return InterpolateDoubleValues(_startOpacity, _endOpacity, step);
		}

		#endregion
	}
}
