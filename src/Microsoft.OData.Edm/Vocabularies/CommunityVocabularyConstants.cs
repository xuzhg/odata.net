//---------------------------------------------------------------------
// <copyright file="CommunityVocabularyConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Constant values for Community Vocabularies
    /// </summary>
    public static class CommunityVocabularyConstants
    {
        /// <summary>Org.OData.Community.V1.AlternateKeys </summary>
        public const string AlternateKeys = "Org.OData.Community.V1.AlternateKeys";

        /// <summary>Org.OData.Community.V1.AlternateKey.Key </summary>
        public const string AlternateKeyTypeKeyPropertyName = "Key";

        /// <summary>Org.OData.Community.V1.PropertyRef.Name </summary>
        public const string PropertyRefTypeNamePropertyName = "Name";

        /// <summary>Org.OData.Community.V1.PropertyRef.Alias </summary>
        public const string PropertyRefTypeAliasPropertyName = "Alias";

        /// <summary>Org.OData.Community.V1.AlternateKey </summary>
        internal const string AlternateKeyType = "Org.OData.Community.V1.AlternateKey";

        /// <summary>Org.OData.Community.V1.PropertyRef </summary>
        internal const string PropertyRefType = "Org.OData.Community.V1.PropertyRef";

        /// <summary>Org.OData.Community.V1 file suffix</summary>
        internal const string VocabularyUrlSuffix = "/Org.OData.Community.V1.xml";
    }
}
