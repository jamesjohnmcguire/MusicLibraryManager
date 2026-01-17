/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioSettings.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Tests;

/// <summary>
/// Audio settings structure.
/// </summary>
internal struct AudioSettings
{
	/// <summary>
	/// The duration in seconds.
	/// </summary>
	public double Duration;

	/// <summary>
	/// The frequency in hertz.
	/// </summary>
	public int Frequency;

	/// <summary>
	/// Initializes a new instance of the <see cref="AudioSettings"/> struct.
	/// </summary>
	public AudioSettings()
	{
		Duration = 1.0;
		Frequency = 440;
	}
}
