//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceSetValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
    /// <summary>
    /// Represents the nested value of a resource set.
    /// </summary>
    public sealed class ODataNestedResourceSetValue : ODataNestedValue
    {
        /// <summary>
        /// Create an instance of <see cref="ODataNestedResourceSetValue"/>.
        /// </summary>
        /// <param name="resourceSet">The nested resource set.</param>
        public ODataNestedResourceSetValue(ODataResourceSetBase resourceSet)
        {
            if (resourceSet == null)
            {
                throw Error.ArgumentNull(nameof(resourceSet));
            }

            ResourceSet = resourceSet;
        }

        /// <summary>
        /// Gets the nested resource set.
        /// </summary>
        public ODataResourceSetBase ResourceSet { get; }

        /// <summary>
        /// Gets/set the nested resources.
        /// </summary>
        public IList<ODataNestedResourceValue> NestedResources { get; set; }

        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get
            {
                return ResourceSet.GetInstanceAnnotations();
            }
            set
            {
                ResourceSet.SetInstanceAnnotations(value);
            }
        }
    }
}
