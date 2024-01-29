using static SqlParser.Ast.ResetConfig;

namespace SqlParser.Ast
{
    public abstract class RoleOption : IWriteSql
    {
        public class BypassRls(bool Bypass) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.Write(Bypass ? "BYPASSRLS" : "NOBYPASSRLS");
            }
        }

        public class ConnectionLimit(Expression Expression) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.WriteSql($"CONNECTION LIMIT {Expression}");
            }
        }

        public class CreateDb(bool Create) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.Write(Create ? "CREATEDB" : "NOCREATEDB");
            }
        }

        public class CreateRole(bool Create) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.Write(Create ? "CREATEROLE" : "NOCREATEROLE");
            }
        }

        public class Inherit(bool InheritValue) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.Write(InheritValue ? "INHERIT" : "NOINHERIT");
            }
        }

        public class Login(bool LoginValue) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.Write(LoginValue ? "LOGIN" : "NOLOGIN");
            }
        }

        public class PasswordOption(Password Password) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                switch (Password)
                {
                    case Password.ValidPassword v:
                        writer.WriteSql($"PASSWORD {v.Expression}");
                        break;
                    case Password.NullPassword:
                        writer.Write("PASSWORD NULL");
                        break;
                }
            }
        }

        public class Replication(bool Replicate) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.Write(Replicate ? "REPLICATION" : "NOREPLICATION");
            }
        }
   
        public class SuperUser(bool IsSuperUser) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.Write(IsSuperUser ? "SUPERUSER" : "NOSUPERUSER");
            }
        }

        public class ValidUntil(Expression Expression) : RoleOption
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.WriteSql($"VALID UNTIL {Expression}");
            }
        }
        public abstract void ToSql(SqlTextWriter writer);
    }

    public abstract class SetConfigValue
    {
        public class Default : SetConfigValue;
        public class FromCurrent : SetConfigValue;
        public class Value(Expression Expression) : SetConfigValue;
    }

    public abstract class ResetConfig
    {
        public class All : ResetConfig;
        public class ConfigName(ObjectName Name) : ResetConfig;
    }

    public abstract class AlterRoleOperation : IWriteSql
    {
        public class RenameRole(Ident RoleName) : AlterRoleOperation
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.WriteSql($"RENAME TO {RoleName}");
            }
        }

        public class AddMember(Ident MemberName) : AlterRoleOperation
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.WriteSql($"ADD MEMBER {MemberName}");
            }
        }

        public class DropMember(Ident MemberName) : AlterRoleOperation
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.WriteSql($"DROP MEMBER {MemberName}");
            }
        }

        public class WithOptions(Sequence<RoleOption> Options) : AlterRoleOperation
        {
            public override void ToSql(SqlTextWriter writer)
            {
                writer.Write("WITH ");
                writer.WriteDelimited(Options, " ");
            }
        }

        public class Set(ObjectName ConfigName, SetConfigValue ConfigValue, ObjectName? InDatabase) : AlterRoleOperation
        {
            public override void ToSql(SqlTextWriter writer)
            {
                if (InDatabase != null)
                {
                    writer.WriteSql($"IN DATABASE {InDatabase} ");
                }

                switch (ConfigValue)
                {
                    case SetConfigValue.Default:
                        writer.WriteSql($"SET {ConfigName} TO DEFAULT");
                        break;
                    case SetConfigValue.FromCurrent:
                        writer.WriteSql($"SET {ConfigName} FROM CURRENT");
                        break;
                    case SetConfigValue.Value value:
                        writer.WriteSql($"SET {ConfigName} TO {value.Expression}");
                        break;
                }
            }
        }

        public class Reset(ResetConfig ConfigName, ObjectName? InDatabase) : AlterRoleOperation
        {
            public override void ToSql(SqlTextWriter writer)
            {
                if (InDatabase != null)
                {
                    writer.WriteSql($"IN DATABASE {InDatabase} ");
                }

                switch (ConfigName)
                {
                    case All:
                        writer.Write("RESET ALL");
                        break;

                    case ConfigName c:
                        writer.WriteSql($"RESET {c.Name}");
                        break;
                }
            }
        }

        public abstract void ToSql(SqlTextWriter writer);
    }
}