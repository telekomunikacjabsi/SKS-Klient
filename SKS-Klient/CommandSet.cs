namespace SKS_Klient
{
    public static class CommandSet
    {
        // liczby oznaczają liczbę argumentów danej komendy
        public static readonly Command Auth = new Command("AUTH", 1);
        public static readonly Command Connect = new Command("CONNECT", 3);
        public static readonly Command AdminConnect = new Command("CONNECT", 1);
        public static readonly Command Disconnect = new Command("DISCONNECT");
        public static readonly Command VerifyList = new Command("VERIFYLIST", 2);
        public static readonly Command OK = new Command("OK");
        public static readonly Command List = new Command("LIST", 2);
        public static readonly Command Port = new Command("PORT", 1);
        public static readonly Command Screenshot = new Command("SCREENSHOT");
        public static readonly Command Message = new Command("MESSAGE", 1);
        public static readonly Command Warn = new Command("WARN", 2);
    }
}
