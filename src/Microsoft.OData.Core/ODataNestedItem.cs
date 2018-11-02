//---------------------------------------------------------------------
// <copyright file="ODataNestedItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
    /// <summary>
    /// Represents the nested resource or resource set item.
    /// </summary>
    public sealed class ODataNestedItem
    {
        /// <summary>
        /// Create an instance of <see cref="ODataNestedItem"/>.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info.</param>
        public ODataNestedItem(ODataNestedResourceInfo nestedResourceInfo)
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
        /// Gets/set the nested resource value.
        /// </summary>
        public ODataNestedValue NestedValue { get; set; }

        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get
            {
                return NestedResourceInfo.GetInstanceAnnotations();
            }
            set
            {
                NestedResourceInfo.SetInstanceAnnotations(value);
            }
        }
    }
}