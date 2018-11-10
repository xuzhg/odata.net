//---------------------------------------------------------------------
// <copyright file="ODataNestedProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a single nested property of a resource.
    /// It can have instance annotations.
    /// </summary>
    public sealed class ODataNestedProperty : ODataAnnotatable
    {
        /// <summary>
        /// Create an instance of <see cref="ODataNestedProperty"/>.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info.</param>
        public ODataNestedProperty(ODataNestedResourceInfo nestedResourceInfo)
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
        /// Gets the nested property name.
        /// </summary>
        public string Name
        {
            get
            {
                return NestedResourceInfo.Name;
            }
        }

        /// <summary>
        ///  Gets or sets the property value.
        /// </summary>
        public ODataNestedValue Value { get; set; }

        /// <summary>
        /// Collection of custom instance annotations of this nested property.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return this.GetInstanceAnnotations(); }
            set { this.SetInstanceAnnotations(value); }
        }
    }
}
