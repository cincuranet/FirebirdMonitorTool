using System;
using System.IO;
using System.Linq;
using FirebirdMonitorTool.Interfaces;
using FirebirdMonitorTool.Interfaces.Attachment;
using FirebirdMonitorTool.Interfaces.Statement;
using FirebirdMonitorTool.Interfaces.Transaction;
using NUnit.Framework;

namespace FirebirdMonitorTool.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    [Category("Parser")]
    public class ParserTests : AbstractParserTests
    {
        [Test]
        public void RawTraceData_AttachDatabase_Start()
        {
            const string Message = @"    GoOnline (ATT_108, PPWLOCAL\DVDWOUWE:NONE, NONE, XNET:SATIRIX)
    D:\Firebird\Firebird-2.5.2.26539-0_x64-3050\bin\isql.exe:8260 ";
            ICommand rawTraceData = new MockTraceData(AttachDatabase, Message);
            IParser parser = new Parser.Parser();
            var attachDatabaseCommand = parser.Parse(rawTraceData) as IAttachmentStart;
            Assert.IsNotNull(attachDatabaseCommand);
            Assert.AreEqual("GoOnline", attachDatabaseCommand.DatabaseName);
            Assert.AreEqual(108L, attachDatabaseCommand.ConnectionId);
            Assert.AreEqual(@"PPWLOCAL\DVDWOUWE", attachDatabaseCommand.User);
            Assert.AreEqual("NONE", attachDatabaseCommand.Role);
            Assert.AreEqual("NONE", attachDatabaseCommand.CharacterSet);
            Assert.AreEqual("XNET", attachDatabaseCommand.RemoteProtocol);
            Assert.AreEqual("SATIRIX", attachDatabaseCommand.RemoteAddress);
            Assert.AreEqual(@"D:\Firebird\Firebird-2.5.2.26539-0_x64-3050\bin\isql.exe", attachDatabaseCommand.RemoteProcessName);
            Assert.AreEqual(8260L, attachDatabaseCommand.RemoteProcessId);
        }

        [Test]
        public void RawTraceData_DetachDatabase()
        {
            const string Message = @"    GoOnline (ATT_107, PPWLOCAL\DVDWOUWE:NONE, NONE, XNET:SATIRIX)
    D:\Firebird\Firebird-2.5.2.26539-0_x64-3050\bin\isql.exe:8688 ";
            ICommand rawTraceData = new MockTraceData(DetachDatabase, Message);
            IParser parser = new Parser.Parser();
            var detachAttacmentCommand = parser.Parse(rawTraceData) as IAttachmentEnd;
            Assert.IsNotNull(detachAttacmentCommand);
            Assert.AreEqual("GoOnline", detachAttacmentCommand.DatabaseName);
            Assert.AreEqual(107L, detachAttacmentCommand.ConnectionId);
            Assert.AreEqual(@"PPWLOCAL\DVDWOUWE", detachAttacmentCommand.User);
            Assert.AreEqual("NONE", detachAttacmentCommand.Role);
            Assert.AreEqual("NONE", detachAttacmentCommand.CharacterSet);
            Assert.AreEqual("XNET", detachAttacmentCommand.RemoteProtocol);
            Assert.AreEqual("SATIRIX", detachAttacmentCommand.RemoteAddress);
            Assert.AreEqual(@"D:\Firebird\Firebird-2.5.2.26539-0_x64-3050\bin\isql.exe", detachAttacmentCommand.RemoteProcessName);
            Assert.AreEqual(8688L, detachAttacmentCommand.RemoteProcessId);
        }

        [Test]
        public void RawTraceData_StartTransaction_Concurreny()
        {
            const string Message = @"    GoOnline (ATT_108, PPWLOCAL\DVDWOUWE:NONE, NONE, XNET:SATIRIX)
    D:\Firebird\Firebird-2.5.2.26539-0_x64-3050\bin\isql.exe:8260
        (TRA_7437, CONCURRENCY | WAIT | READ_WRITE) ";
            ICommand rawTraceData = new MockTraceData(StartTransaction, Message);
            IParser parser = new Parser.Parser();
            var startTransactionCommand = parser.Parse(rawTraceData) as ITransactionStart;
            Assert.IsNotNull(startTransactionCommand);
            Assert.AreEqual(7437L, startTransactionCommand.TransactionId);
            Assert.AreEqual(108L, startTransactionCommand.ConnectionId);
            Assert.AreEqual("CONCURRENCY", startTransactionCommand.IsolationMode);
            Assert.AreEqual(null, startTransactionCommand.RecordVersion);
            Assert.AreEqual(true, startTransactionCommand.Wait);
            Assert.AreEqual(null, startTransactionCommand.WaitTime);
            Assert.AreEqual(false, startTransactionCommand.ReadOnly);
        }

        [Test]
        public void RawTraceData_StartTransaction_ReadCommitted_NoRecVersion()
        {
            const string Message = @"    GoOnline (ATT_108, PPWLOCAL\DVDWOUWE:NONE, NONE, XNET:SATIRIX)
    D:\Firebird\Firebird-2.5.2.26539-0_x64-3050\bin\isql.exe:8260
        (TRA_7438, READ_COMMITTED | NO_REC_VERSION | WAIT | READ_WRITE) ";
            ICommand rawTraceData = new MockTraceData(StartTransaction, Message);
            IParser parser = new Parser.Parser();
            var startTransactionCommand = parser.Parse(rawTraceData) as ITransactionStart;
            Assert.IsNotNull(startTransactionCommand);
            Assert.AreEqual(7438L, startTransactionCommand.TransactionId);
            Assert.AreEqual(108L, startTransactionCommand.ConnectionId);
            Assert.AreEqual("READ_COMMITTED", startTransactionCommand.IsolationMode);
            Assert.AreEqual(false, startTransactionCommand.RecordVersion);
            Assert.AreEqual(true, startTransactionCommand.Wait);
            Assert.AreEqual(null, startTransactionCommand.WaitTime);
            Assert.AreEqual(false, startTransactionCommand.ReadOnly);
        }

        [Test]
        public void RawTraceData_EndTransaction_Commit()
        {
            const string Message = @"    GoOnline (ATT_108, PPWLOCAL\DVDWOUWE:NONE, NONE, XNET:SATIRIX)
    D:\Firebird\Firebird-2.5.2.26539-0_x64-3050\bin\isql.exe:8260
        (TRA_7437, CONCURRENCY | WAIT | READ_WRITE)
     35 ms, 1 read(s), 1 write(s), 1 fetch(es), 1 mark(s) ";
            ICommand rawTraceData = new MockTraceData(CommitTransaction, Message);
            IParser parser = new Parser.Parser();
            var endTransactionCommand = parser.Parse(rawTraceData) as ITransactionEnd;
            Assert.IsNotNull(endTransactionCommand);
            Assert.AreEqual(35, endTransactionCommand.ElapsedTime.TotalMilliseconds);
            Assert.AreEqual(1L, endTransactionCommand.Reads);
            Assert.AreEqual(1L, endTransactionCommand.Writes);
            Assert.AreEqual(1L, endTransactionCommand.Fetches);
            Assert.AreEqual(1L, endTransactionCommand.Marks);
        }

        [Test]
        public void RawTraceData_Statement_Prepare_Plan_NoParams_NoTableCount()
        {
            var message = File.ReadAllText(@"Messages\RawTraceData_Statement_Prepare_Plan_NoParams_NoTableCount.txt");
            ICommand rawTraceData = new MockTraceData(PrepareStatement, message);
            IParser parser = new Parser.Parser();
            var prepareStatementCommand = parser.Parse(rawTraceData) as IStatementPrepare;
            Assert.IsNotNull(prepareStatementCommand);
            Assert.AreEqual(34L, prepareStatementCommand.StatementId);
            Assert.AreEqual("select * from rdb$database", prepareStatementCommand.Text);
            Assert.AreEqual("PLAN (RDB$DATABASE NATURAL)", prepareStatementCommand.Plan);
            Assert.AreEqual(7, prepareStatementCommand.ElapsedTime.TotalMilliseconds);
        }

        [Test]
        public void RawTraceData_Statement_Start_Plan_NoParams_TableCounts()
        {
            var message = File.ReadAllText(@"Messages\RawTraceData_Statement_Start_Plan_NoParams_TableCounts.txt");
            ICommand rawTraceData = new MockTraceData(ExecuteStatementFinish, message);
            IParser parser = new Parser.Parser();
            var finishStatementCommand = parser.Parse(rawTraceData) as IStatementFinish;
            Assert.IsNotNull(finishStatementCommand);
            Assert.AreEqual(37L, finishStatementCommand.StatementId);
            Assert.AreEqual(@"select cast(1 as integer), cast(d.rdb$character_set_name as varchar(64)), cast(c.rdb$default_collate_name as varchar(64))
 from rdb$database d
 left join rdb$character_sets c ON (d.rdb$character_set_name = c.rdb$character_set_name)
 union
 select cast(2 as integer),  cast(rdb$field_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relation_fields
 where (rdb$relation_name = 'RDB$FIELDS') and (rdb$field_name = 'RDB$FIELD_PRECISION')
 union
 select cast(3 as integer), cast(rdb$relation_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relations
 where (rdb$relation_name = 'IBE$VERSION_HISTORY')
 union
 select cast(4 as integer), cast(rdb$relation_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relations
 where (rdb$relation_name = 'IBE$PARAMS_HISTORY')
 union
 select cast(5 as integer), cast(rdb$relation_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relations
 where (rdb$relation_name = 'IBE$PROJECT')
 union
 select cast(6 as integer),  cast(rdb$field_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relation_fields
 where (rdb$relation_name = 'RDB$RELATIONS') and (rdb$field_name = 'RDB$RELATION_TYPE')
 union
 select cast(7 as integer), cast(rdb$relation_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relations
 where (rdb$relation_name = 'IBE$SCRIPTS')
 union
 select cast(8 as integer),  cast(rdb$field_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relation_fields
 where (rdb$relation_name = 'RDB$GENERATORS') and (rdb$field_name = 'RDB$DESCRIPTION')
 union
 select cast(9 as integer),  cast(rdb$field_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relation_fields
 where (rdb$relation_name = 'RDB$ROLES') and (rdb$field_name = 'RDB$DESCRIPTION')
 union
 select cast(10 as integer),  cast(rdb$field_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relation_fields
 where (rdb$relation_name = 'RDB$PROCEDURE_PARAMETERS') and (rdb$field_name = 'RDB$PARAMETER_MECHANISM')
 union
 select cast(11 as integer),  cast(rdb$field_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relation_fields
 where (rdb$relation_name = 'RDB$PROCEDURE_PARAMETERS') and (rdb$field_name = 'RDB$FIELD_NAME')
 union
 select cast(12 as integer),  cast(rdb$relation_name as varchar(64)), cast(' ' as varchar(64)) from rdb$relations
 where (rdb$relation_name = 'RDB$ENCRYPTIONS')
 union
 select cast(16 as integer),  cast(rdb$character_set_name as varchar(64)), cast(' ' as varchar(64)) from rdb$character_sets
 where (rdb$character_set_name = 'UTF8')".Replace(Environment.NewLine, "\r"), finishStatementCommand.Text);
            Assert.AreEqual("PLAN JOIN (D NATURAL, C INDEX (RDB$INDEX_19)) PLAN (RDB$RELATION_FIELDS INDEX (RDB$INDEX_15)) PLAN (RDB$RELATIONS INDEX (RDB$INDEX_0)) PLAN (RDB$RELATIONS INDEX (RDB$INDEX_0)) PLAN (RDB$RELATIONS INDEX (RDB$INDEX_0)) PLAN (RDB$RELATION_FIELDS INDEX (RDB$INDEX_15)) PLAN (RDB$RELATIONS INDEX (RDB$INDEX_0)) PLAN (RDB$RELATION_FIELDS INDEX (RDB$INDEX_15)) PLAN (RDB$RELATION_FIELDS INDEX (RDB$INDEX_15)) PLAN (RDB$RELATION_FIELDS INDEX (RDB$INDEX_15)) PLAN (RDB$RELATION_FIELDS INDEX (RDB$INDEX_15)) PLAN (RDB$RELATIONS INDEX (RDB$INDEX_0)) PLAN (RDB$CHARACTER_SETS INDEX (RDB$INDEX_19))", finishStatementCommand.Plan);
            Assert.IsNotNull(finishStatementCommand.Params);
            Assert.AreEqual(0, finishStatementCommand.Params.Count());
            Assert.IsNotNull(finishStatementCommand.TableCounts);
            Assert.AreEqual(3, finishStatementCommand.TableCounts.Count());
            CollectionAssert.AreEqual(new[] { "RDB$DATABASE", "RDB$RELATION_FIELDS", "RDB$CHARACTER_SETS" }, finishStatementCommand.TableCounts.Select(tc => tc.Name));
            CollectionAssert.AreEqual(new long?[] { 1L, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Natural));
            CollectionAssert.AreEqual(new long?[] { null, 6L, 2L }, finishStatementCommand.TableCounts.Select(tc => tc.Index));
            CollectionAssert.AreEqual(new long?[] { null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Update));
            CollectionAssert.AreEqual(new long?[] { null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Insert));
            CollectionAssert.AreEqual(new long?[] { null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Delete));
            CollectionAssert.AreEqual(new long?[] { null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Backout));
            CollectionAssert.AreEqual(new long?[] { null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Purge));
            CollectionAssert.AreEqual(new long?[] { null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Expunge));
            Assert.AreEqual(8L, finishStatementCommand.RecordsFetched);
            Assert.AreEqual(6, finishStatementCommand.ElapsedTime.TotalMilliseconds);
            Assert.AreEqual(3L, finishStatementCommand.Reads);
            Assert.AreEqual(null, finishStatementCommand.Writes);
            Assert.AreEqual(53L, finishStatementCommand.Fetches);
            Assert.AreEqual(null, finishStatementCommand.Marks);
        }

        [Test]
        public void RawTraceData_Statement_Start_NoPlan_Params_TableCounts()
        {
            var message = File.ReadAllText(@"Messages\RawTraceData_Statement_Start_NoPlan_Params_TableCounts.txt");
            ICommand rawTraceData = new MockTraceData(ExecuteStatementFinish, message);
            IParser parser = new Parser.Parser();
            var finishStatementCommand = parser.Parse(rawTraceData) as IStatementFinish;
            Assert.IsNotNull(finishStatementCommand);
            Assert.AreEqual(625L, finishStatementCommand.StatementId);
            Assert.AreEqual(@"execute procedure SYNC_RACE_STAT_IU(?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  , ?  )", finishStatementCommand.Text);
            Assert.AreEqual(null, finishStatementCommand.Plan);
            Assert.IsNotNull(finishStatementCommand.Params);
            Assert.AreEqual(21, finishStatementCommand.Params.Count());
            CollectionAssert.AreEqual(
                new[]
                {
                    @"param0 = integer, ""22""",
                    @"param1 = integer, ""510652""",
                    @"param2 = integer, ""12392854""",
                    @"param3 = integer, ""2619128""",
                    @"param4 = integer, ""1782664""",
                    @"param5 = varchar(80), ""<NULL>""",
                    @"param6 = integer, ""135""",
                    @"param7 = integer, ""1034""",
                    @"param8 = timestamp, ""1899-12-30T00:00:00.0000""",
                    @"param9 = integer, ""37056""",
                    @"param10 = integer, ""40192""",
                    @"param11 = integer, ""38034""",
                    @"param12 = integer, ""106""",
                    @"param13 = integer, ""14969337""",
                    @"param14 = smallint, ""1""",
                    @"param15 = integer, ""4""",
                    @"param16 = integer, ""2""",
                    @"param17 = integer, ""4""",
                    @"param18 = integer, ""37056""",
                    @"param19 = smallint, ""0""",
                    @"param20 = varchar(400), ""<NULL>"""
                },
                finishStatementCommand.Params);
            Assert.IsNotNull(finishStatementCommand.TableCounts);
            Assert.AreEqual(4, finishStatementCommand.TableCounts.Count());
            CollectionAssert.AreEqual(new[] { "T_RACE_STAT", "T_WC_VERSIONS", "T_WEB_SITE", "T_RECORD" }, finishStatementCommand.TableCounts.Select(tc => tc.Name));
            CollectionAssert.AreEqual(new long?[] { null, null, null, 430L }, finishStatementCommand.TableCounts.Select(tc => tc.Natural));
            CollectionAssert.AreEqual(new long?[] { 432L, 1L, 6L, null }, finishStatementCommand.TableCounts.Select(tc => tc.Index));
            CollectionAssert.AreEqual(new long?[] { 1L, 1L, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Update));
            CollectionAssert.AreEqual(new long?[] { null, null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Insert));
            CollectionAssert.AreEqual(new long?[] { null, null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Delete));
            CollectionAssert.AreEqual(new long?[] { null, null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Backout));
            CollectionAssert.AreEqual(new long?[] { null, null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Purge));
            CollectionAssert.AreEqual(new long?[] { null, null, null, null }, finishStatementCommand.TableCounts.Select(tc => tc.Expunge));
            Assert.AreEqual(0L, finishStatementCommand.RecordsFetched);
            Assert.AreEqual(59, finishStatementCommand.ElapsedTime.TotalMilliseconds);
            Assert.AreEqual(1081L, finishStatementCommand.Reads);
            Assert.AreEqual(6L, finishStatementCommand.Writes);
            Assert.AreEqual(3645L, finishStatementCommand.Fetches);
            Assert.AreEqual(9L, finishStatementCommand.Marks);
        }
    }
}
