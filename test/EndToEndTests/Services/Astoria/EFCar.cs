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
    
    public partial class EFCar
    {
        public EFCar()
        {
            this.EFPersons = new HashSet<EFPerson>();
        }
    
        public int VIN { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<EFPerson> EFPersons { get; set; }
    }
}
