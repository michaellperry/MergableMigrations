using System;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    public abstract class Migration
    {
        private Lazy<BigInteger> _sha256Hash;

        protected Migration()
        {
            _sha256Hash = new Lazy<BigInteger>(ComputeSha256Hash);
        }

        public abstract string[] GenerateSql(MigrationHistoryBuilder migrationsAffected);
        public abstract string[] GenerateRollbackSql(MigrationHistoryBuilder migrationsAffected);
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
