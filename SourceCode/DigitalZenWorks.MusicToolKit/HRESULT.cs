/////////////////////////////////////////////////////////////////////////////
// <copyright file="HRESULT.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit;

#pragma warning disable CA1028
#pragma warning disable CA1707
/// <summary>
/// COM HRESULT codes enumeration.
/// </summary>
public enum HRESULT : uint
{
	/// <summary>
	/// Sucess.
	/// </summary>
	S_OK = 0x0000,

	/// <summary>
	/// False.
	/// </summary>
	S_FALSE = 0x0001,

	/// <summary>
	/// Invalid argument.
	/// </summary>
	E_INVALIDARG = 0x80070057,

	/// <summary>
	/// Out of memory.
	/// </summary>
	E_OUTOFMEMORY = 0x8007000E,

	/// <summary>
	/// iTunes user canceled.
	/// </summary>
	ITUNES_E_USERCANCEL = 0xA0040201,

	/// <summary>
	/// iTunes object deleted.
	/// </summary>
	ITUNES_E_OBJECTDELETED = 0xA0040202,

	/// <summary>
	/// iTunes object locked.
	/// </summary>
	ITUNES_E_OBJECTLOCKED = 0xA0040203,

	/// <summary>
	/// iTunes conversion in progress.
	/// </summary>
	ITUNES_E_CONVERSIONINPROGRESS = 0xA0040204,

	/// <summary>
	/// iTunes music store disabled.
	/// </summary>
	ITUNES_E_MUSICSTOREDISABLED = 0xA0040205,

	/// <summary>
	/// iTunes object exists.
	/// </summary>
	ITUNES_E_OBJECTEXISTS = 0xA0040206,

	/// <summary>
	/// iTunes podcasts disabled.
	/// </summary>
	ITUNES_E_PODCASTSDISABLED = 0xA0040207
}
#pragma warning restore CA1028

