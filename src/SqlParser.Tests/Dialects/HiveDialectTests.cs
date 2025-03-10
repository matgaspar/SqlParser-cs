﻿// ReSharper disable StringLiteralTypo
using SqlParser.Ast;
using SqlParser.Dialects;
using static SqlParser.Ast.Expression;

namespace SqlParser.Tests.Dialects
{
    public class HiveDialectTests : ParserTestBase
    {
        public HiveDialectTests()
        {
            DefaultDialects = new[] { new HiveDialect() };
        }

        [Fact]
        public void Parse_Table_Create()
        {
            VerifiedOnlySelect("SELECT 'single', \"double\"");
            VerifiedStatement("CREATE TABLE IF NOT EXISTS db.table (a BIGINT, b STRING, c TIMESTAMP) PARTITIONED BY (d STRING, e TIMESTAMP) STORED AS ORC LOCATION 's3://...' TBLPROPERTIES (\"prop\" = \"2\", \"asdf\" = '1234', 'asdf' = \"1234\", \"asdf\" = 2)");
        }

        [Fact]
        public void Parse_Insert_Overwrite()
        {
            VerifiedStatement("INSERT OVERWRITE TABLE db.new_table PARTITION (a = '1', b) SELECT a, b, c FROM db.table");
        }

        [Fact]
        public void Parse_Truncate()
        {
            VerifiedStatement("TRUNCATE TABLE db.table");
        }

        [Fact]
        public void Parse_Analyze()
        {
            // ReSharper disable once StringLiteralTypo
            VerifiedStatement("ANALYZE TABLE db.table_name PARTITION (a = '1234', b) COMPUTE STATISTICS NOSCAN CACHE METADATA");
        }


        [Fact]
        public void Parse_Analyze_For_Columns()
        {
            VerifiedStatement("ANALYZE TABLE db.table_name PARTITION (a = '1234', b) COMPUTE STATISTICS FOR COLUMNS");
        }

        [Fact]
        public void Parse_Msck()
        {
            VerifiedStatement("MSCK REPAIR TABLE db.table_name ADD PARTITIONS");
            VerifiedStatement("MSCK REPAIR TABLE db.table_name");
        }

        [Fact]
        public void Parse_Set()
        {
            VerifiedStatement("SET HIVEVAR:name = a, b, c_d");
        }


        [Fact]
        public void Parse_Spaceship()
        {
            VerifiedStatement("SELECT * FROM db.table WHERE a <=> b");
        }

        [Fact]
        public void Parse_With_Cte()
        {
            VerifiedStatement("WITH a AS (SELECT * FROM b) INSERT INTO TABLE db.table_table PARTITION (a) SELECT * FROM b");
        }

        [Fact]
        public void Drop_Table_Purge()
        {
            VerifiedStatement("DROP TABLE db.table_name PURGE");
        }

        [Fact]
        public void Create_Table_Like()
        {
            VerifiedStatement("CREATE TABLE db.table_name LIKE db.other_table");
        }

        [Fact]
        public void Test_Identifier()
        {
            VerifiedStatement("SELECT a AS 3_barrr_asdf FROM db.table_name");
        }

        [Fact]
        public void Test_Alter_Partition()
        {
            VerifiedStatement("ALTER TABLE db.table PARTITION (a = 2) RENAME TO PARTITION (a = 1)");
        }

        [Fact]
        public void Test_Add_Partition()
        {
            VerifiedStatement("ALTER TABLE db.table ADD IF NOT EXISTS PARTITION (a = 'asdf', b = 2)");
        }

        [Fact]
        public void Test_Drop_Partition()
        {
            VerifiedStatement("ALTER TABLE db.table DROP PARTITION (a = 1)");
        }

        [Fact]
        public void Test_Drop_If_Exists()
        {
            VerifiedStatement("ALTER TABLE db.table DROP IF EXISTS PARTITION (a = 'b', c = 'd')");
        }

        [Fact]
        public void Test_Cluster_By()
        {
            VerifiedStatement("SELECT a FROM db.table CLUSTER BY a, b");
        }

        [Fact]
        public void Test_Distribute_By()
        {
            VerifiedStatement("SELECT a FROM db.table DISTRIBUTE BY a, b");
        }

        [Fact]
        public void No_Join_Condition()
        {
            VerifiedStatement("SELECT a, b FROM db.table_name JOIN a");
        }

        [Fact]
        public void Columns_After_Partition()
        {
            VerifiedStatement("INSERT INTO db.table_name PARTITION (a, b) (c, d) SELECT a, b, c, d FROM db.table");
        }

        [Fact]
        // ReSharper disable once IdentifierTypo
        public void Long_Numerics()
        {
            VerifiedStatement("SELECT MIN(MIN(10, 5), 1L) AS a");
        }

        [Fact]
        public void Decimal_Precision()
        {
            VerifiedStatement("SELECT CAST(a AS DECIMAL(18,2)) FROM db.table");
        }

        [Fact]
        public void Create_Temp_Table()
        {
            var query = "CREATE TEMPORARY TABLE db.table (a INT NOT NULL)";
            var query2 = "CREATE TEMP TABLE db.table (a INT NOT NULL)";

            VerifiedStatement(query);
            OneStatementParsesTo(query2, query);
        }

        [Fact]
        public void Create_Local_Directory()
        {
            VerifiedStatement("INSERT OVERWRITE LOCAL DIRECTORY '/home/blah' STORED AS TEXTFILE SELECT * FROM db.table");
        }

        [Fact]
        public void Lateral_View()
        {
            VerifiedStatement("SELECT a FROM db.table LATERAL VIEW explode(a) t AS j, P LATERAL VIEW OUTER explode(a) t AS a, b WHERE a = 1");
        }

        [Fact]
        public void Sort_By()
        {
            VerifiedStatement("SELECT * FROM db.table SORT BY a");
        }

        [Fact]
        public void Rename_Table()
        {
            VerifiedStatement("ALTER TABLE db.table_name RENAME TO db.table_2");
        }

        [Fact]
        public void Map_Access()
        {
            VerifiedStatement("SELECT a.b[\"asdf\"] FROM db.table WHERE a = 2");
        }

        [Fact]
        public void From_Cte()
        {
            VerifiedStatement("WITH cte AS (SELECT * FROM a.b) FROM cte INSERT INTO TABLE a.b PARTITION (a) SELECT *");
        }

        [Fact]
        public void Set_Statement_With_Minus()
        {
            var variable = VerifiedStatement<Statement.SetVariable>("SET hive.tez.java.opts = -Xmx4g");

            var expected = new Statement.SetVariable(false, false,
                new ObjectName(new List<Ident> { "hive", "tez", "java", "opts" }),
                new[]
                {
                    new UnaryOp(new Identifier("Xmx4g"), UnaryOperator.Minus)
                });

            Assert.Equal(expected, variable);

            var ex = Assert.Throws<ParserException>(() => ParseSqlStatements("SET hive.tez.java.opts = -"));
            Assert.Equal("Expected variable value, found EOF", ex.Message);
        }

        [Fact]
        public void Parse_Create_Function()
        {
            var sql = "CREATE TEMPORARY FUNCTION mydb.myfunc AS 'org.random.class.Name' USING JAR 'hdfs://somewhere.com:8020/very/far'";
            var create = VerifiedStatement<Statement.CreateFunction>(sql);

            Assert.True(create.Temporary);
            Assert.Equal("mydb.myfunc", create.Name);
            var fnBody = new CreateFunctionBody
            {
                As = new FunctionDefinition.SingleQuotedDef("org.random.class.Name"),
                Using = new CreateFunctionUsing.Jar("hdfs://somewhere.com:8020/very/far")
            };
            Assert.Equal(fnBody, create.Parameters);

            var ex = Assert.Throws<ParserException>(() =>
                ParseSqlStatements("CREATE TEMPORARY FUNCTION mydb.myfunc AS 'org.random.class.Name' USING JAR"));
            Assert.Equal("Expected literal string, found EOF", ex.Message);

            DefaultDialects = new[] { new GenericDialect() };

            ex = Assert.Throws<ParserException>(() => ParseSqlStatements(sql));
            Assert.Equal("Expected an object type after CREATE, found FUNCTION, Line: 1, Col: 18", ex.Message);
        }

        [Fact]
        public void Filter_During_Aggregation()
        {
            var sql = """
                SELECT
                 ARRAY_AGG(name) FILTER (WHERE name IS NOT NULL),
                 ARRAY_AGG(name) FILTER (WHERE name LIKE 'a%')
                 FROM region
                """;
            VerifiedStatement(sql);
        }

        [Fact]
        public void Filter_During_Aggregation_Aliased()
        {
            var sql = """
                SELECT
                 ARRAY_AGG(name) FILTER (WHERE name IS NOT NULL) AS agg1,
                 ARRAY_AGG(name) FILTER (WHERE name LIKE 'a%') AS agg2
                 FROM region
                """;
            VerifiedStatement(sql);
        }

        [Fact]
        public void Filter_As_Alias()
        {
            OneStatementParsesTo("SELECT name filter FROM region", "SELECT name AS filter FROM region");
        }

        [Fact]
        public void Parse_Delimited_Identifiers()
        {
            // check that quoted identifiers in any position remain quoted after serialization
            var select =
                VerifiedOnlySelect(
                    "SELECT \"alias\".\"bar baz\", \"myfun\"(), \"simple id\" AS \"column alias\" FROM \"a table\" AS \"alias\"");

            var table = (TableFactor.Table)select.From!.Single().Relation!;

            Assert.Equal(new Ident[] { new("a table", Symbols.DoubleQuote) }, table.Name.Values);
            Assert.Equal(new Ident("alias", Symbols.DoubleQuote), table.Alias!.Name);
            Assert.Equal(3, select.Projection.Count);

            Assert.Equal(new CompoundIdentifier(new Ident[]
            {
                new("alias", Symbols.DoubleQuote),
                new("bar baz", Symbols.DoubleQuote),

            }), select.Projection[0].AsExpr());

            Assert.Equal(new Function(new ObjectName(new Ident("myfun", Symbols.DoubleQuote))),
                select.Projection[1].AsExpr());

            var withAlias = (SelectItem.ExpressionWithAlias)select.Projection[2];
            Assert.Equal(new Identifier(new Ident("simple id", Symbols.DoubleQuote)), withAlias.Expression);
            Assert.Equal(new Ident("column alias", Symbols.DoubleQuote), withAlias.Alias);
            VerifiedStatement("CREATE TABLE \"foo\" (\"bar\" \"int\")");
            VerifiedStatement("ALTER TABLE foo ADD CONSTRAINT \"bar\" PRIMARY KEY (baz)");
        }

        [Fact]
        public void Parse_Like()
        {
            Test(false);
            Test(true);

            void Test(bool negated)
            {
                var negation = negated ? "NOT " : null;

                var select = VerifiedOnlySelect($"SELECT * FROM customers WHERE name {negation}LIKE '%a'");
                var expected = new Like(new Identifier("name"), negated,
                    new LiteralValue(new Value.SingleQuotedString("%a")));
                Assert.Equal(expected, select.Selection);

                select = VerifiedOnlySelect($"SELECT * FROM customers WHERE name {negation}LIKE '%a' ESCAPE '\\'");
                expected = new Like(new Identifier("name"), negated,
                    new LiteralValue(new Value.SingleQuotedString("%a")), Symbols.Backslash);
                Assert.Equal(expected, select.Selection);

                // This statement tests that LIKE and NOT LIKE have the same precedence.
                select = VerifiedOnlySelect($"SELECT * FROM customers WHERE name {negation}LIKE '%a' IS NULL");
                var isNull = new IsNull(new Like(new Identifier("name"), negated,
                    new LiteralValue(new Value.SingleQuotedString("%a"))));
                Assert.Equal(isNull, select.Selection);
            }
        }

        [Fact]
        public void Parse_Similar_To()
        {
            Test(false);
            Test(true);

            void Test(bool negated)
            {
                var negation = negated ? "NOT " : null;

                var select = VerifiedOnlySelect($"SELECT * FROM customers WHERE name {negation}SIMILAR TO '%a'");
                var expected = new SimilarTo(new Identifier("name"), negated, new LiteralValue(new Value.SingleQuotedString("%a")));
                Assert.Equal(expected, select.Selection);

                select = VerifiedOnlySelect($"SELECT * FROM customers WHERE name {negation}SIMILAR TO '%a' ESCAPE '\\'");
                expected = new SimilarTo(new Identifier("name"), negated, new LiteralValue(new Value.SingleQuotedString("%a")), Symbols.Backslash);
                Assert.Equal(expected, select.Selection);

                // This statement tests that LIKE and NOT LIKE have the same precedence.
                select = VerifiedOnlySelect($"SELECT * FROM customers WHERE name {negation}SIMILAR TO '%a' ESCAPE '\\' IS NULL");
                var isNull = new IsNull(new SimilarTo(new Identifier("name"), negated, new LiteralValue(new Value.SingleQuotedString("%a")), Symbols.Backslash));
                Assert.Equal(isNull, select.Selection);
            }
        }

        [Fact]
        public void Parse_Describe()
        {
            VerifiedStatement("DESCRIBE namespace.`table`", new Dialect[] { new HiveDialect(), new GenericDialect() });
        }

        [Fact]
        public void Test_Add_Multiple_Partitions()
        {
            const string sql = "ALTER TABLE db.table ADD IF NOT EXISTS PARTITION (`a` = 'asdf', `b` = 2) PARTITION (`a` = 'asdh', `b` = 3)";
            VerifiedStatement(sql);
        }
    }
}
