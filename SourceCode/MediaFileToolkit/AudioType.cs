/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioType.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFileToolkit;

/// <summary>
/// Specifies the type of audio encoding based on compression and quality
/// characteristics.
/// </summary>
/// <remarks>Use this enumeration to indicate whether an audio file or stream
/// uses lossy compression, lossless compression, or if the encoding type is
/// unknown. Lossy formats typically reduce file size at the expense of some
/// audio quality, while lossless formats preserve the original audio data. The
/// Unknown value can be used when the encoding type cannot be determined.
/// </remarks>
public enum AudioType
{
	/// <summary>
	/// Specifies that the operation or process may result in a loss of data
	/// or precision.
	/// </summary>
	Lossy,

	/// <summary>
	/// Gets or sets a value indicating whether lossless compression
	/// is enabled.
	/// </summary>
	Lossless,

	/// <summary>
	/// Represents an unknown or unspecified value.
	/// </summary>
	Unknown
}
