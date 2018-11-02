//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
    /// <summary>
    /// Represents the nested value of a resource.
    /// </summary>
    public sealed class ODataNestedResourceValue : ODataNestedValue
    {
        /// <summary>
        /// Create an instance of <see cref="ODataNestedResourceValue"/>.
        /// </summary>
        /// <param name="resource">The nested resource.</param>
        public ODataNestedResourceValue(ODataResourceBase resource)
        {
            if (resource == null)
            {
                throw Error.ArgumentNull(nameof(resource));
            }

            Resource = resource;
        }

        /// <summary>
        /// Gets the embeded resource.
        /// </summary>
        public ODataResourceBase Resource { get; }

        /// <summary>
        /// Gets/sets the nested items.
        /// </summary>
        public IList<ODataNestedItem> NestedItems { get; set; }

        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get
            {
                return Resource.GetInstanceAnnotations();
            }
            set
            {
                Resource.SetInstanceAnnotations(value);
            }
        }
    }
}