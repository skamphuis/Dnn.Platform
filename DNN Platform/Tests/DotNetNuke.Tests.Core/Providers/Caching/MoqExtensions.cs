﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using DotNetNuke.Services.Cache;
using DotNetNuke.Tests.Utilities;

using Moq.Language.Flow;

namespace DotNetNuke.Tests.Providers.Caching
{
    public static class MoqExtensions
    {
        public static IReturnsResult<CachingProvider> ReturnsValidValue(this ISetup<CachingProvider, object> ret)
        {
            return ret.Returns(() => Constants.CACHEING_ValidValue);
        }
    }
}
