//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlXmlWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal class EdmModelCsdlXmlWriter : EdmModelCsdlWriter
    {
        public XmlWriter XmlWriter { get; }

        public string EdmxNamespace { get; }

        public EdmModelCsdlXmlWriter(IEdmModel model, IEnumerable<EdmSchema> schemas, XmlWriter xmlWriter, Version edmxVersion)
            : base(schemas, new EdmModelCsdlSchemaXmlWriter(model, xmlWriter, edmxVersion))
        {
            EdmUtil.CheckArgumentNull(xmlWriter, "xmlWriter");

            XmlWriter = xmlWriter;

            Debug.Assert(CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion), "CsdlConstants.SupportedEdmxVersions.ContainsKey(edmxVersion)");
            this.EdmxNamespace = CsdlConstants.SupportedEdmxVersions[edmxVersion];
        }

        public override void WriteEfCsdl()
        {
            WriteEdmxElement();
            WriteRuntimeElement();
            WriteConceptualModelsElement();
            WriteSchemas();
            EndElement(); // </ConceptualModels>
            EndElement(); // </Runtime>
            EndElement(); // </Edmx>
        }

        public override void WriteODataCsdl()
        {
            WriteEdmxElement();
            WriteReferenceElements();
            WriteDataServicesElement();
            WriteSchemas();
            EndElement(); // </DataServices>
            EndElement(); // </Edmx>
        }

        private void WriteEdmxElement()
        {
            XmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Edmx, EdmxNamespace);
            XmlWriter.WriteAttributeString(CsdlConstants.Attribute_Version, EdmxVersion.ToString());
        }

        private void WriteRuntimeElement()
        {
            XmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_Runtime, EdmxNamespace);
        }

        private void WriteConceptualModelsElement()
        {
            XmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_ConceptualModels, EdmxNamespace);
        }

        private void WriteDataServicesElement()
        {
            XmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_DataServices, EdmxNamespace);
        }

        private void EndElement()
        {
            XmlWriter.WriteEndElement();
        }
    }
}
