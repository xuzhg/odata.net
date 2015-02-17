﻿//---------------------------------------------------------------------
// <copyright file="ServiceOperationRights.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;

    /// <summary>
    /// Used in place of Microsoft.OData.Service.Common.ServiceOperationRights
    /// </summary>
    [Flags]
    public enum ServiceOperationRights
    {
        /// <summary>Specifies no rights on this service operation.</summary>
        None = 0,

        /// <summary>Specifies the right to read one resource per request.</summary>
        ReadSingle = 1,

        /// <summary>Specifies the right to read multiple resources per request.</summary>
        ReadMultiple = 2,

        /// <summary>Specifies the right to read single or multiple resources in a single request.</summary>
        AllRead = ReadSingle | ReadMultiple,

        /// <summary>Specifies all rights to the service operation.</summary>
        All = AllRead,

        /// <summary>Actually use ServiceOperationRights, not EntitySetRights for the service operation.</summary>
        OverrideEntitySetRights = 4,
    }
}