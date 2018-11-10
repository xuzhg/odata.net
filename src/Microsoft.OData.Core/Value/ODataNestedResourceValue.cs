//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
    public sealed class ODataResourceValue : ODataValue
    {
        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the nested properties belong to this resource.
        /// </summary>
        public IEnumerable<ODataProperty> Properties { get; set; }

        /// <summary>
        /// Collection of custom instance annotations.
        /// That's same as the instance annotations of the resource embeded in this class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return this.GetInstanceAnnotations(); }
            set { this.SetInstanceAnnotations(value); }
        }
    }

    /// <summary>
    /// Represents the value of a nested resource.
    /// It can have instance annotations.
    /// </summary>
    public sealed class ODataNestedResourceValue : ODataNestedValue
    {
        /// <summary>
        /// Create an instance of <see cref="ODataNestedResourceValue"/>.
        /// </summary>
        /// <param name="resource">The wrappered resource.</param>
        public ODataNestedResourceValue(ODataResourceBase resource)
        {
            if (resource == null)
            {
                throw Error.ArgumentNull(nameof(resource));
            }

            Resource = resource;
        }

        /// <summary>
        /// Gets the wrappered resource.
        /// </summary>
        public ODataResourceBase Resource { get; }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        public string TypeName
        {
            get { return Resource.TypeName; }
        }

        /// <summary>
        /// Gets or sets the nested properties belong to this resource.
        /// </summary>
        public IList<ODataNestedProperty> NestedProperties { get; set; }

        /// <summary>
        /// Collection of custom instance annotations.
        /// That's same as the instance annotations of the resource embeded in this class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return Resource.GetInstanceAnnotations(); }
            set { Resource.SetInstanceAnnotations(value); }
        }
    }
}