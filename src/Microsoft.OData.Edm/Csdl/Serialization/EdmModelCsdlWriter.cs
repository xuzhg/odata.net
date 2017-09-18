//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal abstract class EdmModelCsdlWriter
    {
        protected IEnumerable<EdmSchema> Schemas { get; }

        protected IEdmModel Model => SchemaWriter.Model;

        protected Version EdmxVersion => SchemaWriter.Version;

        protected EdmModelCsdlSchemaWriter SchemaWriter { get; }

        protected EdmModelCsdlWriter(IEnumerable<EdmSchema> schemas, EdmModelCsdlSchemaWriter schemaWriter)
        {
            EdmUtil.CheckArgumentNull(schemaWriter, "schemaWriter");

            Schemas = schemas;
            SchemaWriter = schemaWriter;
        }

        public void WriteCsdl(CsdlTarget target)
        {
            switch (target)
            {
                case CsdlTarget.EntityFramework:
                    WriteEfCsdl();
                    break;
                case CsdlTarget.OData:
                    WriteODataCsdl();
                    break;
                default:
                    throw new InvalidOperationException(Strings.UnknownEnumVal_CsdlTarget(target.ToString()));
            }
        }

        public abstract void WriteODataCsdl();

        public abstract void WriteEfCsdl();

        protected virtual void WriteReferenceElements()
        {
            EdmModelReferenceElementsVisitor visitor = new EdmModelReferenceElementsVisitor(SchemaWriter);
            visitor.VisitEdmReferences(Model);
        }

        protected void WriteSchemas()
        {
            // TODO: for referenced model - write alias as is, instead of writing its namespace.
            foreach (EdmSchema schema in Schemas)
            {
                var visitor = new EdmModelCsdlSerializationVisitor(Model, SchemaWriter);
                visitor.VisitEdmSchema(schema, Model.GetNamespacePrefixMappings());
            }
        }
    }
}
