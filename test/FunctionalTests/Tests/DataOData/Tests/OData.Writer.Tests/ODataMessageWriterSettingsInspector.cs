﻿//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettingsInspector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    #region Namespaces
    using System;
    using System.Xml;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    // These tests and helpers are disabled on Silverlight and Phone because they  
    // use private reflection not available on Silverlight and Phone
#if !SILVERLIGHT && !WINDOWS_PHONE
    internal static class ODataMessageWriterSettingsInspector
    {
        public static object GetWriterBehavior(this ODataMessageWriterSettings settings)
        {
            return ReflectionUtils.GetProperty(settings, "WriterBehavior");
        }

        public static string GetAcceptableMediaTypes(this ODataMessageWriterSettings settings)
        {
            return (string)ReflectionUtils.GetProperty(settings, "AcceptableMediaTypes");
        }

        public static string GetAcceptableCharsets(this ODataMessageWriterSettings settings)
        {
            return (string)ReflectionUtils.GetProperty(settings, "AcceptableCharsets");
        }

        public static ODataFormat GetFormat(this ODataMessageWriterSettings settings)
        {
            return (ODataFormat)ReflectionUtils.GetProperty(settings, "Format");
        }

        public static Func<ODataEntry, XmlWriter, XmlWriter> GetAtomStartEntryXmlCustomizationCallback(this ODataMessageWriterSettings settings)
        {
            return (Func<ODataEntry, XmlWriter, XmlWriter>)ReflectionUtils.GetProperty(settings, "AtomStartEntryXmlCustomizationCallback");
        }

        public static Action<ODataEntry, XmlWriter, XmlWriter> GetAtomEndEntryXmlCustomizationCallback(this ODataMessageWriterSettings settings)
        {
            return (Action<ODataEntry, XmlWriter, XmlWriter>)ReflectionUtils.GetProperty(settings, "AtomEndEntryXmlCustomizationCallback");
        }
    }
#endif
}
