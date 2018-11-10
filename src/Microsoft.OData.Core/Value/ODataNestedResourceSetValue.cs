//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceSetValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData
{
    /// <summary>
    /// Represents the value of a nested resource set.
    /// It can't have instance annotations.
    /// </summary>
    public sealed class ODataNestedResourceSetValue : ODataNestedValue
    {
        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the resource items belong to this resource set.
        /// </summary>
        public IList<ODataNestedResourceValue> ResourceItems { get; set; }
    }
}
