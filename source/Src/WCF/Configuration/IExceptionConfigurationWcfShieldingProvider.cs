﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using EnterpriseLibrary.Common.Configuration.Fluent;

namespace EnterpriseLibrary.Common.Configuration.Fluent
{
    /// <summary>
    /// This interfaces supports the fluent configuration of exception shielding.
    /// </summary>
    public interface IExceptionConfigurationWcfShieldingProvider : IExceptionConfigurationForExceptionTypeOrPostHandling
    {
        /// <summary>
        /// Maps a property from the exception to the fault contract.
        /// </summary>
        /// <param name="name">Fault contract property to map to</param>
        /// <param name="source">Source property to map from.</param>
        /// <returns></returns>
        IExceptionConfigurationWcfShieldingProvider MapProperty(string name, string source);
    }
}
