/////////////////////////////////////////////////////////////////////////////
// <copyright file="ChromaprintContext.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// The chromaprint context.
	/// </summary>
	public struct ChromaprintContext : IEquatable<ChromaprintContext>
	{
#pragma warning disable CA1051
#pragma warning disable CA1720
		/// <summary>
		/// The struct pointer.
		/// </summary>
		public IntPtr Pointer;
#pragma warning restore CA1720
#pragma warning restore CA1051

		/// <summary>
		/// Equals override operator.
		/// </summary>
		/// <param name="left">The left side value.</param>
		/// <param name="right">The right side value.</param>
		/// <returns>A value indicating whether the objects are the same
		/// or not.</returns>
		public static bool operator ==(
			ChromaprintContext left, ChromaprintContext right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Not equals override operator.
		/// </summary>
		/// <param name="left">The left side value.</param>
		/// <param name="right">The right side value.</param>
		/// <returns>A value indicating whether the objects are not the same
		/// or so.</returns>
		public static bool operator !=(
			ChromaprintContext left, ChromaprintContext right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Equals override.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>A value indicating whether the object is the same
		/// or not.</returns>
		public override bool Equals(object obj)
		{
			bool equity = false;
			if (obj != null)
			{
				Type thisType = this.GetType();
				Type objType = obj.GetType();

				if (thisType.Equals(objType))
				{
					ChromaprintContext context = (ChromaprintContext)obj;

					equity = Equals(context);
				}
			}

			return equity;
		}

		/// <summary>
		/// Equals override.
		/// </summary>
		/// <param name="other">The other object to compare.</param>
		/// <returns>A value indicating whether the object is the same
		/// or not.</returns>
		public bool Equals(ChromaprintContext other)
		{
			bool equity = false;

			if (Pointer == other.Pointer)
			{
				equity = true;
			}

			return equity;
		}

		/// <summary>
		/// Get hash code.
		/// </summary>
		/// <returns>The hash code value.</returns>
		public override int GetHashCode()
		{
			return ((int)Pointer << 2) ^ 31;
		}
	}
}
