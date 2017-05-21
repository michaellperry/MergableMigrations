using System;
using System.Numerics;

namespace MergableMigrations.Specification.Implementation
{
    public abstract class Migration
    {
        public abstract string[] GenerateSql(MigrationHistoryBuilder migrationsAffected);
        internal abstract MigrationMemento GetMemento();
        internal abstract BigInteger Sha256Hash();

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
            return other.Sha256Hash() == this.Sha256Hash();
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(Sha256Hash().ToByteArray(), 0);
        }
    }
}
