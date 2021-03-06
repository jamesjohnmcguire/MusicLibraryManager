﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="Chain.cs" company="Digital Zen Works">
// Copyright © 2019 - 2021 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUtility
{
	public enum Chain
	{
		/// <summary>
		/// No chaining.
		/// </summary>
		None,

		/// <summary>
		/// Chain by anding.
		/// </summary>
		And,

		/// <summary>
		/// Chain by oring.
		/// </summary>
		Or,

		/// <summary>
		/// Chain by xoring.
		/// </summary>
		Xor
	}
}
