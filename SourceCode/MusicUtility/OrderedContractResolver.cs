/////////////////////////////////////////////////////////////////////////////
// <copyright file="OrderedContractResolver.cs" company="Digital Zen Works">
// Copyright © 2019 - 2021 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicUtility
{
	public class OrderedContractResolver : DefaultContractResolver
	{
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
