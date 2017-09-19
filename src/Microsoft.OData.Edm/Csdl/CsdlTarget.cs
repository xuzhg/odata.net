﻿//---------------------------------------------------------------------
// <copyright file="CsdlTarget.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Specifies what target of a CSDL doc.
    /// </summary>
    public enum CsdlTarget
    {
        /// <summary>
        /// The target is Entity Framework.
        /// </summary>
        EntityFramework,

        /// <summary>
        /// The target is OData XML Metadata.
        /// </summary>
        OData,

        /// <summary>
        /// The target is OData JSON Metadata.
        /// </summary>
        ODataJson,

        /// <summary>
        /// The target is One API metadata.
        /// </summary>
        OneApi
    }
}
