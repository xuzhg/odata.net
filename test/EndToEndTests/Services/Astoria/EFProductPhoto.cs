//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.Astoria
{
    using System;
    using System.Collections.Generic;
    
    public partial class EFProductPhoto
    {
        public int ProductId { get; set; }
        public int PhotoId { get; set; }
        public byte[] Photo { get; set; }
    
        public virtual EFProduct EFProduct { get; set; }
    }
}
