﻿//---------------------------------------------------------------------
// <copyright file="StringCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Collections.Specialized {

    using System.Diagnostics;
    using System.Collections;
    using System.Collections.Generic;

    /// <devdoc>
    ///    <para>Represents a collection of strings.</para>
    /// </devdoc>
    public class StringCollection : IList {
        private List<object> data = new List<object>();

        /// <devdoc>
        /// <para>Represents the entry at the specified index of the <see cref='System.Collections.Specialized.StringCollection'/>.</para>
        /// </devdoc>
        public string this[int index] {
            get {
                return ((string)data[index]);
            }
            set {
                data[index] = value;
            }
        }

        /// <devdoc>
        ///    <para>Gets the number of strings in the 
        ///    <see cref='System.Collections.Specialized.StringCollection'/> .</para>
        /// </devdoc>
        public int Count {
            get {
                return data.Count;
            }
        }

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}


        /// <devdoc>
        ///    <para>Adds a string with the specified value to the 
        ///    <see cref='System.Collections.Specialized.StringCollection'/> .</para>
        /// </devdoc>
        public int Add(string value) {
            int cnt = data.Count;
            data.Add(value);
            return cnt;
        }

        /// <devdoc>
        /// <para>Copies the elements of a string array to the end of the <see cref='System.Collections.Specialized.StringCollection'/>.</para>
        /// </devdoc>
        public void AddRange(string[] value) {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            data.AddRange(value);
        }

        /// <devdoc>
        ///    <para>Removes all the strings from the 
        ///    <see cref='System.Collections.Specialized.StringCollection'/> .</para>
        /// </devdoc>
        public void Clear() {
            data.Clear();
        }

        /// <devdoc>
        ///    <para>Gets a value indicating whether the 
        ///    <see cref='System.Collections.Specialized.StringCollection'/> contains a string with the specified 
        ///       value.</para>
        /// </devdoc>
        public bool Contains(string value) {
            return data.Contains(value);
        }

        /// <devdoc>
        /// <para>Copies the <see cref='System.Collections.Specialized.StringCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </devdoc>
        public void CopyTo(string[] array, int index) {
            data.CopyTo(array, index);
        }

        /// <devdoc>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the <see cref='System.Collections.Specialized.StringCollection'/> .</para>
        /// </devdoc>
        public StringEnumerator GetEnumerator() {
            return new StringEnumerator(this);
        }

        /// <devdoc>
        ///    <para>Returns the index of the first occurrence of a string in 
        ///       the <see cref='System.Collections.Specialized.StringCollection'/> .</para>
        /// </devdoc>
        public int IndexOf(string value) {
            return data.IndexOf(value);
        }

        /// <devdoc>
        /// <para>Inserts a string into the <see cref='System.Collections.Specialized.StringCollection'/> at the specified 
        ///    index.</para>
        /// </devdoc>
        public void Insert(int index, string value) {
            data.Insert(index, value);
        }

        /// <devdoc>
        /// <para>Gets a value indicating whether the <see cref='System.Collections.Specialized.StringCollection'/> is read-only.</para>
        /// </devdoc>
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <devdoc>
        ///    <para>Gets a value indicating whether access to the 
        ///    <see cref='System.Collections.Specialized.StringCollection'/> 
        ///    is synchronized (thread-safe).</para>
        /// </devdoc>
        public bool IsSynchronized {
            get {
                return false;
            }
        }

        /// <devdoc>
        ///    <para> Removes a specific string from the 
        ///    <see cref='System.Collections.Specialized.StringCollection'/> .</para>
        /// </devdoc>
        public void Remove(string value) {
            data.Remove(value);
        }

        /// <devdoc>
        /// <para>Removes the string at the specified index of the <see cref='System.Collections.Specialized.StringCollection'/>.</para>
        /// </devdoc>
        public void RemoveAt(int index) {
            data.RemoveAt(index);
        }

        /// <devdoc>
        /// <para>Gets an object that can be used to synchronize access to the <see cref='System.Collections.Specialized.StringCollection'/>.</para>
        /// </devdoc>
        public object SyncRoot {
            get {
                throw new NotImplementedException();
            }
        }

        object IList.this[int index] {
            get {
                return this[index];
            }
            set {
                this[index] = (string)value;
            }
        }

        int IList.Add(object value) {
            return Add((string)value);
        }

        bool IList.Contains(object value) {
            return Contains((string) value);
        }


        int IList.IndexOf(object value) {
            return IndexOf((string)value);
        }

        void IList.Insert(int index, object value) {
            Insert(index, (string)value);
        }

        void IList.Remove(object value) {
            Remove((string)value);
        }

        void ICollection.CopyTo(Array array, int index) {
            //data.CopyTo(array, index);
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return data.GetEnumerator();
        }
    }

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class StringEnumerator {
        private System.Collections.IEnumerator baseEnumerator;
        private System.Collections.IEnumerable temp;
        
        internal StringEnumerator(StringCollection mappings) {
            this.temp = (IEnumerable)(mappings);
            this.baseEnumerator = temp.GetEnumerator();
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Current {
            get {
                return (string)(baseEnumerator.Current);
            }
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool MoveNext() {
            return baseEnumerator.MoveNext();
        }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void Reset() {
            baseEnumerator.Reset();
        }
        
    }
}

