//---------------------------------------------------------------------
// <copyright file="CsdlReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Reader
{
    /// <summary>
    /// Provides CSDL parsing services for EDM models.
    /// </summary>
    internal class SchemaJsonReader
    {
        //private static readonly Dictionary<string, Action> EmptyParserLookup = new Dictionary<string, Action>();
    //    private readonly IDictionary<string, Action> schemaPropertyParserLookup;
        //private readonly Dictionary<string, Action> runtimeParserLookup;
        //private readonly Dictionary<string, Action> conceptualModelsParserLookup;
        //private readonly Dictionary<string, Action> dataServicesParserLookup;
      //  private readonly List<EdmError> errors;
   //     private readonly List<IEdmReference> edmReferences;
     //   private string entityContainer;

        ///// <summary>
        ///// Indicates where the document comes from.
        ///// </summary>
        //private string source;

    //   private IJsonReader jsonReader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">The XmlReader for current CSDL doc</param>
        /// <param name="referencedModelFunc">The function to load referenced model xml. If null, will stop loading the referenced model.</param>
        public SchemaJsonReader(IJsonReader reader, JsonReaderOptions options)
        {
      //      this.jsonReader = reader;
     //       this.errors = new List<EdmError>();
       //     this.edmReferences = new List<IEdmReference>();

            //// Setup the edmx parser.
            //this.schemaPropertyParserLookup = new Dictionary<string, Action>
            //{
            //    // $Alias
            //    { CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Alias, this.ParseAlias },

            //    // $Annotations
            //    { CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Annotations, this.ParseAnnotations }
            //};
        }

        public static CsdlSchema ParseSchemObject(IJsonReader jsonReader)
        {
            if (jsonReader == null)
            {
                throw new ArgumentNullException("jsonReader");
            }

            // Supports to read from Begin
            if (jsonReader.NodeType == JsonNodeType.None)
            {
                jsonReader.Read();
            }

            // Make sure the input is an object
            if (jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new Exception("");
            }

            // Pass the "{" tag.
            jsonReader.Read();

            while (jsonReader.NodeType != JsonNodeType.EndObject)
            {
                // Get the property name and move json reader to next token
                string propertyName = jsonReader.ReadPropertyName();

                // Now the Json reader point to the value.
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new Exception();
                }

                switch (propertyName)
                {
                    // "$Alias"
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Alias:
                        ParseAlias(jsonReader);
                        break;

                    // "$Annotations"
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Annotations:
                        ParseAnnotations(jsonReader);
                        break;

                    default:
                        if (propertyName[0] == '@')
                        {
                            // Annotation for the schema
                            ParseSchemaAnnotations(jsonReader);
                        }
                        else
                        {
                            // Schema members (entity type, complex type...)
                            ParseSchemaElements(propertyName, jsonReader);
                        }
                        break;
                }
            }

            // Consume the "}" tag.
            jsonReader.Read();

            return null;
        }

        private static void ParseAlias(IJsonReader jsonReader)
        {
            if (jsonReader.NodeType != JsonNodeType.PrimitiveValue)
            {
                throw new Exception();
            }

            string version = jsonReader.ReadStringValue();
            if (version != "4.0" && version != "4.01")
            {
                throw new Exception();
            }
        }

        private static void ParseAnnotations(IJsonReader jsonReader)
        {
            if (jsonReader.NodeType != JsonNodeType.PrimitiveValue)
            {
                throw new Exception();
            }
        }

        // The schema object MAY also contain annotations that apply to the schema itself.
        private static void ParseSchemaAnnotations(IJsonReader jsonReader)
        {
            // parse the "@Measures.ISOCurrency": {
            //            "$Path": "Currency"

        }

        private static void ParseSchemaElements(string name, IJsonReader jsonReader)
        {
            // The schema object MAY contain members representing entity types, complex types, enumeration types, type definitions, actions, functions, terms, and an entity container.
            // 1) An entity type is represented as a member of the schema object whose name is the unqualified name of the entity type and whose value is an object.
            // 2) A complex type is represented as a member of the schema object whose name is the unqualified name of the complex type and whose value is an object.
            // 3) An enumeration type is represented as a member of the schema object whose name is the unqualified name of the enumeration type and whose value is an object.
            // 4) A type definition is represented as a member of the schema object whose name is the unqualified name of the type definition and whose value is an object.
            // 5) An action is represented as a member of the schema object whose name is the unqualified name of the action and whose value is an array. The array contains one object per action overload.
            // 6) A function is represented as a member of the schema object whose name is the unqualified name of the function and whose value is an array.
            // 7) A term is represented as a member of the schema object whose name is the unqualified name of the term and whose value is an object.
            // 8) An entity container is represented as a member of the schema object whose name is the unqualified name of the entity container and whose value is an object.

            // Make sure the input is an object
            if (jsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new Exception("");
            }

            // Pass the "{" tag.
            jsonReader.Read();

            while (jsonReader.NodeType != JsonNodeType.EndObject)
            {
                // Get the property name and move json reader to next token
                string propertyName = jsonReader.ReadPropertyName();

                // Now the Json reader point to the value.
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new Exception();
                }

                switch (propertyName)
                {
                    // "$Alias"
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Attribute_Alias:
                        ParseAlias(jsonReader);
                        break;

                    // "$Annotations"
                    case CsdlConstants.Prefix_Dollar + CsdlConstants.Element_Annotations:
                        ParseAnnotations(jsonReader);
                        break;

                    default:
                        if (propertyName[0] == '@')
                        {
                            // Annotation for the schema
                            ParseSchemaAnnotations(jsonReader);
                        }
                        else
                        {
                            // Schema members (entity type, complex type...)
                            ParseSchemaElements(propertyName, jsonReader);
                        }
                        break;
                }
            }
            // Consume the "}" tag.
            jsonReader.Read();

           // return null;
        }

    }
}
