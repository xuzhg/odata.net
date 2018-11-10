//---------------------------------------------------------------------
// <copyright file="ODataNestedValueUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
    /// <summary>
    /// Extension class with utility methods to read <see cref="ODataNestedValue"/> from <see cref="ODataReader"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils")]
    public static class ODataNestedValueUtils
    {
        /// <summary>
        /// Read and return the <see cref="ODataNestedValue"/>.
        /// </summary>
        /// <param name="reader">The OData reader.</param>
        /// <returns>The <see cref="ODataNestedValue"/>.</returns>
        public static ODataNestedValue ReadNestedValue(this ODataReader reader)
        {
            if (reader == null)
            {
                throw Error.ArgumentNull(nameof(reader));
            }

            ODataNestedValue topLevelItem = null;
            Stack<object> itemsStack = new Stack<object>();

            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceStart: // resource start
                        ReadResource(reader.Item as ODataResourceBase, itemsStack, ref topLevelItem);
                        break;

                    case ODataReaderState.NestedResourceInfoStart:
                        ReadNestedResourceInfo(reader.Item as ODataNestedResourceInfo, itemsStack);
                        break;

                    case ODataReaderState.ResourceSetStart: // resource set
                        ReadResourceSet(reader.Item as ODataResourceSetBase, itemsStack, ref topLevelItem);
                        break;

                    case ODataReaderState.ResourceEnd:
                    case ODataReaderState.ResourceSetEnd:
                    case ODataReaderState.NestedResourceInfoEnd:
                        Debug.Assert(itemsStack.Count > 0);
                        itemsStack.Pop();
                        break;

                    default:
                        break;
                }
            }

            Debug.Assert(itemsStack.Count == 0);
            return topLevelItem;
        }

        private static void ReadResource(ODataResourceBase resource, Stack<object> itemsStack, ref ODataNestedValue topLevelValue)
        {
            // resource maybe null if it's not top-level resource.
            ODataNestedResourceValue resourceValue = null;
            if (resource != null)
            {
                resourceValue = new ODataNestedResourceValue(resource);
            }

            if (itemsStack.Count == 0)
            {
                Debug.Assert(resource != null, "The top-level resource can never be null.");
                topLevelValue = resourceValue;
            }
            else
            {
                // Non top level resource (it maybe null) should be either:
                // 1. an element of an resource set
                // 2. the child of the nested resource info
                object parentItem = itemsStack.Peek();
                ODataNestedResourceSetValue parentResourceSet = parentItem as ODataNestedResourceSetValue;
                if (parentResourceSet != null)
                {
                    parentResourceSet.Add(resourceValue);
                }
                else
                {
                    ODataNestedProperty parentNestedProperty = parentItem as ODataNestedProperty;
                    Debug.Assert(parentNestedProperty != null &&
                        parentNestedProperty.NestedResourceInfo != null &&
                        parentNestedProperty.NestedResourceInfo.IsCollection == false);
                    parentNestedProperty.Value = resourceValue;
                }
            }

            itemsStack.Push(resourceValue);
        }

        private static void ReadResourceSet(ODataResourceSetBase resourceSet, Stack<object> itemsStack, ref ODataNestedValue topLevelValue)
        {
            Debug.Assert(resourceSet != null, "ResourceSet should never be null.");

            ODataNestedResourceSetValue resourceSetValue = new ODataNestedResourceSetValue { TypeName = resourceSet.TypeName };

            if (itemsStack.Count > 0)
            {
                // a nested resource set should belong to a nested resource item
                ODataNestedProperty parentNestedProperty = itemsStack.Peek() as ODataNestedProperty;
                Debug.Assert(parentNestedProperty != null && parentNestedProperty.Value == null && parentNestedProperty.NestedResourceInfo.IsCollection == true);

                parentNestedProperty.Value = resourceSetValue;
            }
            else
            {
                topLevelValue = resourceSetValue;
            }

            itemsStack.Push(resourceSetValue);
        }

        private static void ReadNestedResourceInfo(ODataNestedResourceInfo nestedResourceInfo, Stack<object> itemsStack)
        {
            Debug.Assert(nestedResourceInfo != null, "nested resource info should never be null.");

            ODataNestedProperty nestedProperty = new ODataNestedProperty(nestedResourceInfo);

            // nested resource info should embeded in the resource
            ODataNestedResourceValue parentResource = itemsStack.Peek() as ODataNestedResourceValue;
            Debug.Assert(parentResource != null, "nested resource info should belong to a resource");
            parentResource.Add(nestedProperty);

            itemsStack.Push(nestedProperty);
        }

        private static void Add(this ODataNestedResourceSetValue setValue, ODataNestedResourceValue item)
        {
            if (setValue.ResourceItems == null)
            {
                setValue.ResourceItems = new List<ODataNestedResourceValue>();
            }

            setValue.ResourceItems.Add(item);
        }

        private static void Add(this ODataNestedResourceValue sourceValue, ODataNestedProperty property)
        {
            if (sourceValue.NestedProperties == null)
            {
                sourceValue.NestedProperties = new List<ODataNestedProperty>();
            }

            sourceValue.NestedProperties.Add(property);
        }
    }
}