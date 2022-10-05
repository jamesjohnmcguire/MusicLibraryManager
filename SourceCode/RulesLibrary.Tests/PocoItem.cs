/////////////////////////////////////////////////////////////////////////////
// <copyright file="PocoItem.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesLibrary.Tests
{
	/// <summary>
	/// Plain old class object item.
	/// </summary>
	public class PocoItem
	{
		/// <summary>
		/// Gets or sets the property 1.
		/// </summary>
		/// <value>The property 1.</value>
		public string Property1 { get; set; }

		/// <summary>
		/// Gets or sets the PropertySet.
		/// </summary>
		/// <value>The artists.</value>
		public virtual string[] PropertySet { get; set; }

		/// <summary>
		/// Gets or sets the PropertySet2.
		/// </summary>
		/// <value>The artists.</value>
		public virtual string[] PropertySet2 { get; set; }
	}
}
