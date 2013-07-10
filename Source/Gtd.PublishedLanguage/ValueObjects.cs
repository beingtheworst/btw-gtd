using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Gtd
{
    [Serializable]
    public sealed class TrustedSystemId : AbstractIdentity<long>
    {
        public const string TagValue = "trustedSystem";

        public TrustedSystemId(long id)
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

        public string ToStreamId()
        {
            return "system-" + Id;
        }

        public TrustedSystemId() { }
    }

    [DataContract(Namespace = "BTW2/GTD")]
    public struct RequestId
    {
        [DataMember(Order = 1)]
        public Guid Id { get; private set; }

        public bool IsEmpty { get { return Id == Guid.Empty; } }

        public static RequestId Empty = new RequestId(Guid.Empty);

        public RequestId(Guid id)
            : this()
        {
            Id = id;
        }

        public bool Equals(RequestId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RequestId && Equals((RequestId)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(RequestId left, RequestId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RequestId left, RequestId right)
        {
            return !left.Equals(right);
        }
    }

    [DataContract(Namespace = "BTW2/GTD")]
    public struct ThoughtId
    {
        [DataMember(Order = 1)]
        public Guid Id { get; private set; }

        public bool IsEmpty { get { return Id == Guid.Empty; } }

        public static ThoughtId Empty = new ThoughtId(Guid.Empty);

        public ThoughtId(Guid id)
            : this()
        {
            Id = id;
        }

        public bool Equals(ThoughtId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ThoughtId && Equals((ThoughtId)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ThoughtId left, ThoughtId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ThoughtId left, ThoughtId right)
        {
            return !left.Equals(right);
        }
    }

    [DataContract(Namespace = "BTW2/GTD")]
    public struct ProjectId
    {
        [DataMember(Order = 1)]
        public Guid Id { get; private set; }

        public bool IsEmpty { get { return Id == Guid.Empty; } }

        public static ProjectId Empty = new ProjectId(Guid.Empty);
        
        public ProjectId(Guid id) : this()
        {
            Id = id;
        }

        public bool Equals(ProjectId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ProjectId && Equals((ProjectId) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ProjectId left, ProjectId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProjectId left, ProjectId right)
        {
            return !left.Equals(right);
        }
    }

    [DataContract(Namespace = "BTW2/GTD")]
    public struct ActionId
    {
        [DataMember(Order = 1)]
        public Guid Id { get; private set; }
        public bool IsEmpty { get { return Id == Guid.Empty; } }
        public ActionId(Guid id) : this()
        {
            Id = id;
        }

        public bool Equals(ActionId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ActionId && Equals((ActionId) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public enum ProjectType
    {
        List,
        Sequential,
        Parallel
    }
}