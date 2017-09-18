//---------------------------------------------------------------------
// <copyright file="EdmModelReferenceElementsVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// The visitor for outputing &lt;edmx:referneced&gt; elements for referenced model.
    /// </summary>
    internal class EdmModelReferenceElementsVisitor
    {
        private readonly EdmModelCsdlSchemaWriter _schemaWriter;

        internal EdmModelReferenceElementsVisitor(EdmModelCsdlSchemaWriter schemaWriter)
        {
            _schemaWriter = schemaWriter;
        }

        internal void VisitEdmReferences(IEdmModel model)
        {
            IEnumerable<IEdmReference> references = model?.GetEdmReferences();
            if (references == null)
            {
                return;
            }

            var edmReferences = references as IList<IEdmReference> ?? references.ToList();
            if (edmReferences.Any())
            {
                _schemaWriter.WriteReferencesStart(edmReferences);

                foreach (IEdmReference reference in edmReferences)
                {
                    _schemaWriter.WriteReferenceElementStart(reference);

                    WriteIncludes(reference.Includes);
                    WriteIncludeAnnotations(reference.IncludeAnnotations);

                    _schemaWriter.WriteReferenceElementEnd(reference);
                }

                _schemaWriter.WriteReferencesEnd(edmReferences);
            }
        }

        private void WriteIncludes(IEnumerable<IEdmInclude> includes)
        {
            if (includes == null)
            {
                return;
            }

            var edmIncludes = includes.ToList();
            if (edmIncludes.Any())
            {
                _schemaWriter.WriteReferenceIncludesStart(edmIncludes);

                foreach (IEdmInclude include in edmIncludes)
                {
                    _schemaWriter.WriteIncludeElement(include);
                }

                _schemaWriter.WriteReferenceIncludesEnd(edmIncludes);
            }
        }

        private void WriteIncludeAnnotations(IEnumerable<IEdmIncludeAnnotations> annotations)
        {
            if (annotations == null)
            {
                return;
            }

            var edmAnnotations = annotations.ToList();
            if (edmAnnotations.Any())
            {
                _schemaWriter.WriteReferenceIncludeAnnotationsStart(edmAnnotations);

                foreach (IEdmIncludeAnnotations includeAnnotations in edmAnnotations)
                {
                    _schemaWriter.WriteIncludeAnnotationsElement(includeAnnotations);
                }

                _schemaWriter.WriteReferenceIncludeAnnotationsEnd(edmAnnotations);
            }
        }
    }
}
