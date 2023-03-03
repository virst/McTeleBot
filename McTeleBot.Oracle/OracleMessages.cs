using Telegram.Bot.Types.Enums;

namespace McTeleBot.Oracle
{
    internal record OracleMessages
    {
        public int Ide { get; set; }
        public int BotIde { get; set; }
        public long? ChatId { get; set; }
        public string? MessageTxt { get; set; }
        public int ParseMode { get; set; }
        public int Status { get; set; }
        public string? Token { get; set; }

        public ParseMode? GetParseMode()
        {
            if (ParseMode == 0)
                return null;
            return (ParseMode)ParseMode;
        }

    }
}
