//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceInfoValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
    /// <summary>
    /// Represents the value of a nested resource info.
    /// </summary>
    public sealed class ODataNestedResourceInfoValue : ODataNestedValue
    {
        /// <summary>
        /// Create an instance of <see cref="ODataNestedResourceInfoValue"/>.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info.</param>
        public ODataNestedResourceInfoValue(ODataNestedResourceInfo nestedResourceInfo)
        {
            if (nestedResourceInfo == null)
            {
                throw Error.ArgumentNull(nameof(nestedResourceInfo));
            }

            NestedResourceInfo = nestedResourceInfo;
        }

        /// <summary>
        /// Gets the nested resource info.
        /// </summary>
        public ODataNestedResourceInfo NestedResourceInfo { get; }

        /// <summary>
        /// Gets or set the nested value.
        /// </summary>
        public ODataNestedValue NestedValue { get; set; }
    }
}