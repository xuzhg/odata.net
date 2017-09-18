//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal class EdmModelCsdlJsonWriter : EdmModelCsdlWriter
    {
        public JsonWriter JsonWriter { get; }

        public EdmModelCsdlJsonWriter(IEdmModel model, IEnumerable<EdmSchema> schemas, JsonWriter jsonWriter, Version edmxVersion)
            : base(schemas, new EdmModelCsdlSchemaJsonWriter(model, jsonWriter, edmxVersion))
        {
            EdmUtil.CheckArgumentNull(jsonWriter, "jsonWriter");

            JsonWriter = jsonWriter;
        }

        public override void WriteODataCsdl()
        {
            WriteStart();
            WriteEntityContainer();
            WriteReferenceElements();
            WriteSchemas();
            WriteEnd();
        }

        public override void WriteEfCsdl()
        {
            throw new NotImplementedException("JSON Metadata for EF is not supported.");
        }

        private void WriteStart()
        {
            JsonWriter.WriteStartObject(); // {
            JsonWriter.WritePropertyName("$" + CsdlConstants.Attribute_Version);
            JsonWriter.WriteValue(EdmxVersion.ToString());
        }

        private void WriteEntityContainer()
        {
            JsonWriter.WritePropertyName("$" + CsdlConstants.Element_EntityContainer);
            JsonWriter.WriteValue(Model.EntityContainer.FullName());
        }

        /*
        private void WriteSchemasJson()
        {
            Version edmVersion = Model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest;
            foreach (EdmSchema schema in Schemas)
            {
                var visitor = new SchemaSerializationJsonVisitor(Model, JsonWriter, edmVersion);
                visitor.VisitEdmSchema(schema, Model.GetNamespacePrefixMappings());
            }

            if (edmVersion == null)
            {
                
            }
        }*/

        private void WriteEnd()
        {
            JsonWriter.WriteEndObject();
        }
    }
}
