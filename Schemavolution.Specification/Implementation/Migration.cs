using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

namespace Schemavolution.Specification.Implementation
{
    public abstract class Migration
    {
        private readonly Lazy<BigInteger> _sha256Hash;
        private readonly ImmutableList<Migration> _prerequisites;

        protected ImmutableList<Migration> Prerequisites => _prerequisites;

        protected Migration(ImmutableList<Migration> prerequisites)
        {
            _sha256Hash = new Lazy<BigInteger>(ComputeSha256Hash);
            _prerequisites = prerequisites;
        }

        public abstract IEnumerable<Migration> AllPrerequisites { get; }
        public abstract string[] GenerateSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph);
        public abstract string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected, IGraphVisitor graph);
        internal abstract MigrationMemento GetMemento();
        protected abstract BigInteger ComputeSha256Hash();

        internal virtual void AddToParent()
        {
        }

        internal BigInteger Sha256Hash => _sha256Hash.Value;

        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
                return Equals((Migration)obj);
            return base.Equals(obj);
        }

        public bool Equals(Migration other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return other.Sha256Hash == this.Sha256Hash;
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(Sha256Hash.ToByteArray(), 0);
        }
    }
}
