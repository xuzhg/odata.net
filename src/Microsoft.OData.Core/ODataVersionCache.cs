//---------------------------------------------------------------------
// <copyright file="ODataVersionCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Simple ODataVersion specific cache.
    /// </summary>
    /// <typeparam name="T">The type of the item being cached.</typeparam>
    internal sealed class ODataVersionCache<T>
    {
        /// <summary>
        /// Lazy constructing T for ODataVersion.V4.
        /// </summary>
        private readonly SimpleLazy<T> v3;

        /// <summary>
        /// Constructs an instance of the ODataVersionCache.
        /// </summary>
        /// <param name="factory">The method to call to create a new instance of <typeparamref name="T"/> for a given ODataVersion.</param>
        internal ODataVersionCache(Func<ODataVersion, T> factory)
        {
            Debug.Assert(factory != null, "factory != null");

            this.v3 = new SimpleLazy<T>(() => factory(ODataVersion.V4), true /*isThreadSafe*/);
        }

        /// <summary>
        /// Indexer to get the cached item when given the ODataVersion.
        /// </summary>
        /// <param name="version">The ODataVersion to look up.</param>
        /// <returns>The cached item.</returns>
        internal T this[ODataVersion version]
        {
            get
            {
                switch (version)
                {
                    case ODataVersion.V4:
                        return this.v3.Value;

                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataVersionCache_UnknownVersion));
                }
            }
        }
    }
}