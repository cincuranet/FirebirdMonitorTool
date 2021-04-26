using System;
using FirebirdMonitorTool.Procedure;
using NUnit.Framework;

namespace FirebirdMonitorTool.Tests
{
    public sealed class ParserProcedureTests : ParserTestsBase
    {
        [Test]
        public void ProcedureStart()
        {
            var header = "2021-03-31T19:47:26.3510 (3148:000000007ED424C0) EXECUTE_PROCEDURE_START";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_252809, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49673)
	E:\www\xxx.com\:3456
		(TRA_216114, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)

Procedure SET_VALUE:
param0 = varchar(800), ""INTERFACE""
param1 = varchar(800), ""BRANDING""
param2 = smallint, ""1""
param3 = bigint, ""<NULL>""
param4 = bigint, ""<NULL>""";
            var result = Parse<IProcedureStart>(header, message);
            Assert.AreEqual("SET_VALUE", result.ProcedureName);
            Assert.AreEqual(@"param0 = varchar(800), ""INTERFACE""
param1 = varchar(800), ""BRANDING""
param2 = smallint, ""1""
param3 = bigint, ""<NULL>""
param4 = bigint, ""<NULL>""", result.Params);
        }

        [Test]
        public void ProcedureFinishWithTableCounts()
        {
            var header = "2021-03-31T19:47:26.8080 (3148:000000007ED424C0) EXECUTE_PROCEDURE_FINISH";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_476403, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49677)
	E:\www\xxx.com\:12480
		(TRA_1428665, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)

Procedure SYNC_MARK_LAST_SYNC:
param0 = bigint, ""1""
param1 = timestamp, ""2021-03-31T17:47:24.3861""

      0 ms, 49 fetch(es), 4 mark(s)

Table                             Natural     Index    Update    Insert    Delete   Backout     Purge   Expunge
***************************************************************************************************************
RDB$INDICES                                       8                                                            
T_FUNBOO                                2                                                                      
T_NODE                                            1         1                                                  ";
            var result = Parse<IProcedureEnd>(header, message);
            Assert.AreEqual("SYNC_MARK_LAST_SYNC", result.ProcedureName);
            Assert.AreEqual(@"param0 = bigint, ""1""
param1 = timestamp, ""2021-03-31T17:47:24.3861""", result.Params);
            Assert.AreEqual(TimeSpan.FromMilliseconds(0), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(49, result.Fetches);
            Assert.AreEqual(4, result.Marks);
            Assert.AreEqual(3, result.TableCounts.Count);
            Assert.AreEqual("RDB$INDICES", result.TableCounts[0].Name);
            Assert.AreEqual(null, result.TableCounts[0].Natural);
            Assert.AreEqual(8, result.TableCounts[0].Index);
            Assert.AreEqual(null, result.TableCounts[0].Update);
            Assert.AreEqual(null, result.TableCounts[0].Insert);
            Assert.AreEqual(null, result.TableCounts[0].Delete);
            Assert.AreEqual(null, result.TableCounts[0].Backout);
            Assert.AreEqual(null, result.TableCounts[0].Purge);
            Assert.AreEqual(null, result.TableCounts[0].Expunge);
            Assert.AreEqual("T_FUNBOO", result.TableCounts[1].Name);
            Assert.AreEqual(2, result.TableCounts[1].Natural);
            Assert.AreEqual(null, result.TableCounts[1].Index);
            Assert.AreEqual(null, result.TableCounts[1].Update);
            Assert.AreEqual(null, result.TableCounts[1].Insert);
            Assert.AreEqual(null, result.TableCounts[1].Delete);
            Assert.AreEqual(null, result.TableCounts[1].Backout);
            Assert.AreEqual(null, result.TableCounts[1].Purge);
            Assert.AreEqual(null, result.TableCounts[1].Expunge);
            Assert.AreEqual("T_NODE", result.TableCounts[2].Name);
            Assert.AreEqual(null, result.TableCounts[2].Natural);
            Assert.AreEqual(1, result.TableCounts[2].Index);
            Assert.AreEqual(1, result.TableCounts[2].Update);
            Assert.AreEqual(null, result.TableCounts[2].Insert);
            Assert.AreEqual(null, result.TableCounts[2].Delete);
            Assert.AreEqual(null, result.TableCounts[2].Backout);
            Assert.AreEqual(null, result.TableCounts[2].Purge);
            Assert.AreEqual(null, result.TableCounts[2].Expunge);
        }

        [Test]
        public void ProcedureStartNoInputParams()
        {
            var header = "2021-03-31T19:47:25.4230 (3148:000000007ED424C0) EXECUTE_PROCEDURE_START";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_423063, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49656)
	E:\www\xxx.com\:12480
		(TRA_1264236, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)

Procedure INT_VERSIONING_START:";
            var result = Parse<IProcedureStart>(header, message);
            Assert.AreEqual("INT_VERSIONING_START", result.ProcedureName);
            Assert.AreEqual(null, result.Params);
        }

        [Test]
        public void ProcedureFinishNoInputParamsWithTableCounts()
        {
            var header = "2021-03-31T19:47:26.8070 (3148:000000007ED424C0) EXECUTE_PROCEDURE_FINISH";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_476403, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49677)
	E:\www\xxx.com\:12480
		(TRA_1428665, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)

Procedure INT_SYNC_NODES:
      1 ms, 2 fetch(es)

Table                             Natural     Index    Update    Insert    Delete   Backout     Purge   Expunge
***************************************************************************************************************
T_FUNBOO                                1                                                                      ";
            var result = Parse<IProcedureEnd>(header, message);
            Assert.AreEqual("INT_SYNC_NODES", result.ProcedureName);
            Assert.AreEqual(null, result.Params);
            Assert.AreEqual(TimeSpan.FromMilliseconds(1), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(2, result.Fetches);
            Assert.AreEqual(null, result.Marks);
            Assert.AreEqual(1, result.TableCounts.Count);
            Assert.AreEqual("T_FUNBOO", result.TableCounts[0].Name);
            Assert.AreEqual(1, result.TableCounts[0].Natural);
            Assert.AreEqual(null, result.TableCounts[0].Index);
            Assert.AreEqual(null, result.TableCounts[0].Update);
            Assert.AreEqual(null, result.TableCounts[0].Insert);
            Assert.AreEqual(null, result.TableCounts[0].Delete);
            Assert.AreEqual(null, result.TableCounts[0].Backout);
            Assert.AreEqual(null, result.TableCounts[0].Purge);
            Assert.AreEqual(null, result.TableCounts[0].Expunge);
        }

        [Test]
        public void ProcedureFinishNoInputParams()
        {
            var header = "2021-03-31T19:47:27.8160 (3148:000000007ED41EC0) EXECUTE_PROCEDURE_FINISH";
            var message = @"	E:\DB\XXX\XXX.FDB (ATT_127024, CLIENT:NONE, UTF8, TCPv4:127.0.0.1/49693)
	E:\www\xxx.com\:12480
		(TRA_96946, READ_COMMITTED | REC_VERSION | NOWAIT | READ_WRITE)

Procedure INT_VERSIONING_START:
      0 ms";
            var result = Parse<IProcedureEnd>(header, message);
            Assert.AreEqual("INT_VERSIONING_START", result.ProcedureName);
            Assert.AreEqual(null, result.Params);
            Assert.AreEqual(TimeSpan.FromMilliseconds(0), result.ElapsedTime);
            Assert.AreEqual(null, result.Reads);
            Assert.AreEqual(null, result.Writes);
            Assert.AreEqual(null, result.Fetches);
            Assert.AreEqual(null, result.Marks);
            Assert.AreEqual(null, result.TableCounts);
        }
    }
}
