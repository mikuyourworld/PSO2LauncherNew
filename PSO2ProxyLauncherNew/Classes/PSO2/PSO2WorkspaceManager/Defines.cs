namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2WorkspaceManager
{
    public static class CharacterPreset
    {
        public sealed class Gender
        {
            private string code;
            public override string ToString() => this.code;
            private Gender(string codename) { this.code = codename; }

            public static readonly Gender Male = new Gender("m");
            public static readonly Gender Female = new Gender("f");
        }

        public sealed class Race
        {
            private string code;
            public override string ToString() => this.code;
            private Race(string codename) { this.code = codename; }

            public static readonly Race Human = new Race("h");
            public static readonly Race Newman = new Race("n");
            public static readonly Race Cast = new Race("c");
            public static readonly Race Dewman = new Race("d");
        }

        public static string[] GetFileExts()
        {
            string[] result = new string[8];
            result[0] = GetFileExt(Gender.Male, Race.Human);
            result[1] = GetFileExt(Gender.Male, Race.Newman);
            result[2] = GetFileExt(Gender.Male, Race.Cast);
            result[3] = GetFileExt(Gender.Male, Race.Dewman);

            result[4] = GetFileExt(Gender.Female, Race.Human);
            result[5] = GetFileExt(Gender.Female, Race.Newman);
            result[6] = GetFileExt(Gender.Female, Race.Cast);
            result[7] = GetFileExt(Gender.Female, Race.Dewman);
            return result;
        }

        public static string GetFileExt(Gender g, Race r)
        {
            return (g.ToString() + r.ToString() + "p");
        }
    }
}
