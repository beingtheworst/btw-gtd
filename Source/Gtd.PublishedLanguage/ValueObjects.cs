using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Gtd
{
    [Serializable]
    public sealed class TenantId : AbstractIdentity<long>
    {
        public const string TagValue = "tenant";

        public TenantId(long id)
        {
            Contract.Requires(id > 0);
            Id = id;
        }

        public override string GetTag()
        {
            return TagValue;
        }


        [DataMember(Order = 1)]
        public override long Id { get; protected set; }

        public TenantId() { }
    }


}