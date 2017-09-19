//---------------------------------------------------------------------
// <copyright file="CsdlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Json;
using JsonWriter = Newtonsoft.Json.JsonWriter;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides CSDL serialization services (XML or JSON) for EDM models.
    /// </summary>
    [CLSCompliant(false)]
    public static class CsdlWriter
    {
        /// <summary>
        /// Outputs a CSDL artifact to the provided XmlWriter.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writer">XmlWriter the generated CSDL will be written to.</param>
        /// <param name="target">Target implementation of the CSDL being generated.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteCsdl(IEdmModel model, XmlWriter writer, CsdlTarget target, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writer, "writer");

            return TryWriteCsdl(model, writer, null, target, out errors);
        }

        /// <summary>
        /// Outputs a CSDL artifact to the provided JSON Writer.
        /// </summary>
        /// <param name="model">Model to be written.</param>
        /// <param name="writer">JSON Writer the generated CSDL will be written to.</param>
        /// <param name="errors">Errors that prevented successful serialization, or no errors if serialization was successful. </param>
        /// <returns>A value indicating whether serialization was successful.</returns>
        public static bool TryWriteCsdl(IEdmModel model, JsonWriter writer, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(writer, "writer");

            return TryWriteCsdl(model, null, writer, CsdlTarget.OData, out errors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="writer"></param>
        /// <param name="target"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static bool TryWriteCsdl(IEdmModel model, Stream stream, CsdlTarget target,
            out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(model, "model");
          //  EdmUtil.CheckArgumentNull(writer, "writer");

            switch (target)
            {
                case CsdlTarget.EntityFramework:
                case CsdlTarget.OData:
                    XmlWriter xmlWriter = XmlWriter.Create(stream);
                    return TryWriteCsdl(model, xmlWriter, target, out errors);
                case CsdlTarget.Json:
                    break;
                case CsdlTarget.Swagger:
                    break;
            }
            errors = null;
            return false;
        }

        private static bool TryWriteCsdl(IEdmModel model, XmlWriter xmlWriter, JsonWriter jsonWriter, CsdlTarget target, out IEnumerable<EdmError> errors)
        {
            errors = model.GetSerializationErrors();
            if (errors.FirstOrDefault() != null)
            {
                return false;
            }

            Version edmxVersion = model.GetEdmxVersion();
            if (edmxVersion != null)
            {
                if (!CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion))
                {
                    errors = new [] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmxVersion, Strings.Serializer_UnknownEdmxVersion) };
                    return false;
                }
            }
            else if (!CsdlConstants.EdmToEdmxVersions.TryGetValue(model.GetEdmVersion() ?? EdmConstants.EdmVersionLatest, out edmxVersion))
            {
                errors = new [] { new EdmError(new CsdlLocation(0, 0), EdmErrorCode.UnknownEdmVersion, Strings.Serializer_UnknownEdmVersion) };
                return false;
            }

            edmxVersion = edmxVersion ?? EdmConstants.EdmVersionLatest;

            IEnumerable<EdmSchema> schemas = new EdmModelSchemaSeparationSerializationVisitor(model).GetSchemas();

            EdmModelCsdlWriter csdlWriter;
            if (jsonWriter != null)
            {
                csdlWriter = new EdmModelCsdlJsonWriter(model, schemas, jsonWriter, edmxVersion);
            }
            else
            {
                csdlWriter = new EdmModelCsdlXmlWriter(model, schemas, xmlWriter, edmxVersion);
            }
            csdlWriter.WriteCsdl(target);

            errors = Enumerable.Empty<EdmError>();
            return true;
        }
    }
}
