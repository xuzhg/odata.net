﻿//---------------------------------------------------------------------
// <copyright file="CodeLayerBuilderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Test.Astoria;

namespace System.Data.Test.Astoria.ReflectionProvider
{
    public abstract class CodeLayerBuilderBase : IDisposable
    {
        private Workspace _workspace;
        private WorkspaceLanguage _language;
        protected CSharpCodeLanguageHelper _codeLanguageHelper = null;
        private StreamWriter _writer;

        private string _codeFilePath;

        protected internal CodeLayerBuilderBase(Workspace workspace, WorkspaceLanguage language, string codefilePath)
        {
            _workspace = workspace;
            _language = language;
            _codeFilePath = codefilePath;

            _writer = new StreamWriter(_codeFilePath);

            _codeLanguageHelper = new CSharpCodeLanguageHelper(_writer);
        }

        public Workspace Workspace { get { return _workspace; } set { _workspace = value; } }
        public WorkspaceLanguage Language { get { return _language; } set { _language = value; } }
        public CSharpCodeLanguageHelper CodeBuilder { get { return _codeLanguageHelper; } set { _codeLanguageHelper = value; } }
        protected StreamWriter Writer { get { return _writer; } }

        public abstract void Build();

        protected void WriteFileHeader()
        {
            CodeBuilder.WriteCommentLine("------------------------------------------------------------------------------");
            CodeBuilder.WriteCommentLine(" <auto-generated>");
            CodeBuilder.WriteCommentLine("     This code was generated by a tool.");
            CodeBuilder.WriteCommentLine("");
            CodeBuilder.WriteCommentLine("     Changes to this file may cause incorrect behavior and will be lost if");
            CodeBuilder.WriteCommentLine("     the code is regenerated.");
            CodeBuilder.WriteCommentLine(" </auto-generated>");
            CodeBuilder.WriteCommentLine("------------------------------------------------------------------------------");
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_codeLanguageHelper != null)
            {
                _codeLanguageHelper.Dispose();
                _codeLanguageHelper = null;
            }
        }

        #endregion
    }
}
