using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Animations
{
	/// <summary>
	/// Class inheriting <see cref="AnimatorBase"/> which animates nothing for itself,
	/// but still can be used as the controlling parent animator for other animators.
	/// </summary>
	public class DummyAnimator : AnimatorBase
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="container">Container the new instance should be added to.</param>
		public DummyAnimator(IContainer container) : base(container) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public DummyAnimator() {}

		#endregion

		#region Overridden from AnimatorBase

		/// <summary>
		/// Gets or sets the currently shown value.
		/// </summary>
		protected override object CurrentValueInternal
		{
			get { return null; }
			set {}
		}

		/// <summary>
		/// Gets or sets the starting value for the animation.
		/// </summary>
		public override object StartValue
		{
			get { return null; }
			set {}
		}

		/// <summary>
		/// Gets or sets the ending value for the animation.
		/// </summary>
		public override object EndValue
		{
			get { return null; }
			set {}
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
			return null;
		}

		#endregion
	}
}
