/////////////////////////////////////////////////////////////////////////////
// <copyright file="OrderedContractResolver.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	/// <summary>
	/// Ordered contract resolver class.
	/// </summary>
	public class OrderedContractResolver : DefaultContractResolver
	{
		/// <summary>
		/// Create properties method.
		/// </summary>
		/// <param name="type">Property type.</param>
		/// <param name="memberSerialization">Member serialization object.</param>
		/// <returns>A list of properties.</returns>
		protected override IList<JsonProperty> CreateProperties(
			Type type, MemberSerialization memberSerialization)
		{
			IList<JsonProperty> properties =
				base.CreateProperties(type, memberSerialization);

			IOrderedEnumerable<JsonProperty> ordered =
				properties.OrderBy(p => p.PropertyName);

			properties = ordered.ToList();

			return properties;
		}
	}
}
