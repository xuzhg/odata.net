//---------------------------------------------------------------------
// <copyright file="EdmModelReferenceElementsJsonVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// The visitor for outputing &lt;edmx:referneced&gt; elements for referenced model.
    /// </summary>
    internal class EdmModelReferenceElementsJsonVisitor
    {
        public JsonWriter JsonWriter { get; }

        public EdmModelReferenceElementsJsonVisitor(JsonWriter jsonWriter)
        {
            JsonWriter = jsonWriter;
        }

        public void VisitEdmReferences(IEdmModel model)
        {
            IEnumerable<IEdmReference> references = model?.GetEdmReferences();
            if (references != null)
            {
                var edmReferences = references as IList<IEdmReference> ?? references.ToList();
                if (!edmReferences.Any())
                {
                    return;
                }

                JsonWriter.WritePropertyName("$Reference");
                JsonWriter.WriteStartObject();

                foreach (IEdmReference tmp in edmReferences)
                {
                    JsonWriter.WritePropertyName(tmp.Uri.ToString());
                    JsonWriter.WriteStartObject();

                    WriteIncludes(tmp.Includes);
                    WriteIncludeAnnotations(tmp.IncludeAnnotations);

                    JsonWriter.WriteEndObject();
                }

                JsonWriter.WriteEndObject();
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
                JsonWriter.WritePropertyName("$Include");
                JsonWriter.WriteStartArray(); // [

                foreach (var edmInclude in edmIncludes)
                {
                    JsonWriter.WriteStartObject(); // {
                    JsonWriter.WritePropertyName("$Namespace");
                    JsonWriter.WriteValue(edmInclude.Namespace);
                    JsonWriter.WritePropertyName("$Alias");
                    JsonWriter.WriteValue(edmInclude.Alias);
                    JsonWriter.WriteEndObject(); // }
                }

                JsonWriter.WriteEndArray();  // ]
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
                JsonWriter.WritePropertyName("$IncludeAnnotations");
                JsonWriter.WriteStartArray(); // [

                foreach (var edmAnnotation in edmAnnotations)
                {
                    JsonWriter.WriteStartObject(); // {
                    JsonWriter.WritePropertyName("$TermNamespace");
                    JsonWriter.WriteValue(edmAnnotation.TermNamespace);

                    if (!String.IsNullOrWhiteSpace(edmAnnotation.TargetNamespace))
                    {
                        JsonWriter.WritePropertyName("$TargetNamespace");
                        JsonWriter.WriteValue(edmAnnotation.TargetNamespace);
                    }

                    if (!String.IsNullOrWhiteSpace(edmAnnotation.Qualifier))
                    {
                        JsonWriter.WritePropertyName("$Qualifier");
                        JsonWriter.WriteValue(edmAnnotation.Qualifier);
                    }

                    JsonWriter.WriteEndObject(); // }
                }

                JsonWriter.WriteEndArray();  // ]
            }
        }
    }
}
