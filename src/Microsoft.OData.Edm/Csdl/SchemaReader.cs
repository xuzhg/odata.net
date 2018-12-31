//---------------------------------------------------------------------
// <copyright file="SchemaReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides Schema parsing services for EDM models.
    /// </summary>
    public static class SchemaReader
    {
        /// <summary>
        /// Returns an IEdmModel for the given Schema artifacts.
        /// </summary>
        /// <param name="readers">Collection of XmlReaders containing the Schema artifacts.</param>
        /// <param name="model">The model generated by parsing.</param>
        /// <param name="errors">Errors reported while parsing.</param>
        /// <returns>Success of the parse operation.</returns>
        public static bool TryParse(IEnumerable<XmlReader> readers, out IEdmModel model, out IEnumerable<EdmError> errors)
        {
            return TryParse(readers, Enumerable.Empty<IEdmModel>(), out model, out errors);
        }

        /// <summary>
        /// Returns an IEdmModel for the given Schema artifacts.
        /// </summary>
        /// <param name="readers">Collection of XmlReaders containing the Schema artifacts.</param>
        /// <param name="reference">Model to be references by the created model.</param>
        /// <param name="model">The model generated by parsing.</param>
        /// <param name="errors">Errors reported while parsing.</param>
        /// <returns>Success of the parse operation.</returns>
        public static bool TryParse(IEnumerable<XmlReader> readers, IEdmModel reference, out IEdmModel model, out IEnumerable<EdmError> errors)
        {
            return TryParse(readers, new IEdmModel[] { reference }, out model, out errors);
        }

        /// <summary>
        /// Returns an IEdmModel for the given Schema artifacts.
        /// </summary>
        /// <param name="readers">Collection of XmlReaders containing the Schema artifacts.</param>
        /// <param name="references">Models to be references by the created model.</param>
        /// <param name="model">The model generated by parsing.</param>
        /// <param name="errors">Errors reported while parsing.</param>
        /// <returns>Success of the parse operation.</returns>
        public static bool TryParse(IEnumerable<XmlReader> readers, IEnumerable<IEdmModel> references, out IEdmModel model, out IEnumerable<EdmError> errors)
        {
            return TryParse(readers, references, out model, out errors, true /*enableVocabularySupport*/);
        }

        /// <summary>
        /// Returns an IEdmModel for the given Schema artifacts.
        /// </summary>
        /// <param name="readers">Collection of XmlReaders containing the Schema artifacts.</param>
        /// <param name="references">Models to be references by the created model.</param>
        /// <param name="model">The model generated by parsing.</param>
        /// <param name="errors">Errors reported while parsing.</param>
        /// <param name="enableVocabularySupport">A value indicating enable/disable the built-in vocabulary supporting.</param>
        /// <returns>Success of the parse operation.</returns>
        public static bool TryParse(IEnumerable<XmlReader> readers, IEnumerable<IEdmModel> references, out IEdmModel model, out IEnumerable<EdmError> errors, bool enableVocabularySupport)
        {
            CsdlModel ast;
            if (CsdlParser.TryParse(readers, out ast, out errors))
            {
                IEnumerable<IEdmModel> refModels;
                if (enableVocabularySupport)
                {
                    refModels = references.Concat(VocabularyModelProvider.VocabularyModels);
                }
                else
                {
                    refModels = references;
                }
                CsdlSemanticsModel tmp = new CsdlSemanticsModel(ast, new CsdlSemanticsDirectValueAnnotationsManager(), refModels);
                model = tmp;
                return true;
            }

            model = null;
            return false;
        }
    }
}
