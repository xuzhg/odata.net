﻿//---------------------------------------------------------------------
// <copyright file="TestFailedException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Common
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown whenever the test fails.
    /// </summary>
#if !SILVERLIGHT && !PORTABLELIB
    [Serializable]
#endif
    public abstract class TestFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the TestFailedException class without a message.
        /// </summary>
        protected TestFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestFailedException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        protected TestFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestFailedException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        protected TestFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

#if !SILVERLIGHT && !PORTABLELIB
        /// <summary>
        /// Initializes a new instance of the TestFailedException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected TestFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
