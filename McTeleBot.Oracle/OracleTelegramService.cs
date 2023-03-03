using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace McTeleBot.Oracle
{
    internal class OracleTelegramService : IHostedService
    {
        private readonly ILogger<OracleTelegramService> _logger;
        private readonly OracleConnection dbConn;
        private readonly int _delaySecs;

        public OracleTelegramService(ILogger<OracleTelegramService> logger)
        {
            _logger = logger;
            dbConn = new OracleConnection(Program.DbConnStr);
            _delaySecs = 60;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            int sessionNum = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Run session {0}", sessionNum++);
                await dbConn.OpenAsync();
                _logger.LogInformation("Main db opened.");

                var dbList = (await dbConn.QueryAsync<OracleBases>("select tbb.db_conn as dbconn, tbb.note from T_BOT_BASES tbb")).ToList();

                await Parallel.ForEachAsync(dbList, async (db, cancellationToken) => await ServiceServer(db));

                await dbConn.CloseAsync();
                _logger.LogInformation("Delay [{0}]", _delaySecs);
                await Task.Delay(1000 * _delaySecs, cancellationToken);
            }
        }

        private async Task ServiceServer(OracleBases oracleBase)
        {
            _logger.LogInformation("Service start : {0}", oracleBase.Note);
            var dbConnStr = $"Data Source={oracleBase.DbConn};User Id=RWC;Password=RWC";
            using OracleConnection dbConn = new OracleConnection(dbConnStr);
            await dbConn.OpenAsync();
            _logger.LogInformation("Service base opened: {0}", oracleBase.Note);
            OracleTransaction? tr = null;
            try
            {
                tr = dbConn.BeginTransaction();
                var ids = (await dbConn.QueryAsync<int>("SELECT tbm.ide\n" +
                                    "  FROM t_bot_messages tbm\n" +
                                    " WHERE tbm.status = 0\n" +
                                    "   AND rownum <= 10000\n" +
                                    "   FOR UPDATE ORDER BY 1", transaction: tr)).ToList();

                foreach (var id in ids)
                {
                    var mess = await dbConn.QueryFirstOrDefaultAsync<OracleMessages>(GetMessSql(id));
                    var sts = (await SendMessage(mess)) ? 1 : -1;
                    await dbConn.ExecuteAsync("update t_bot_messages set status = :sts where ide = :id", new { sts, id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                if (tr != null)
                    await tr.CommitAsync();
                await dbConn.CloseAsync();
            }
        }

        private async Task<bool> SendMessage(OracleMessages mess)
        {
            try
            {
                if (mess.ChatId == null || mess.Token == null || string.IsNullOrEmpty(mess.MessageTxt))
                    return false;
                var botClient = new TelegramBotClient(mess.Token);
                var chtId = new ChatId(mess.ChatId.Value);

                _logger.LogInformation("Sending message ide: {0}", mess.Ide);
#if (!DEBUG)
                var pm = mess.GetParseMode();
                await botClient.SendTextMessageAsync(chtId, mess.MessageTxt, parseMode: pm);
#endif

                _logger.LogInformation("Done sending message ide: {0}", mess.Ide);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error sending message ide: {0}", mess.Ide);
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        private string GetMessSql(int ide) => "SELECT m.ide\n" +
"      ,m.bot_ide AS botide\n" +
"      ,NVL(m.chat_id, b.chat_id) AS chatid\n" +
"      ,m.message_txt AS messagetxt\n" +
"      ,m.parse_mode AS parsemode\n" +
"      ,m.status\n" +
"      ,b.token\n" +
"  FROM t_bot_messages m\n" +
"  JOIN t_bot_list b\n" +
"    ON b.ide = m.bot_ide\n" +
" WHERE m.ide = " + ide;


        public Task StopAsync(CancellationToken cancellationToken)
        {
            dbConn?.Close();
            dbConn?.Dispose();
            _logger.LogInformation($"{nameof(OracleTelegramService)} stopped");
            return Task.CompletedTask;
        }
    }
}
