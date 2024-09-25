namespace Rittou {
    public class Config {
        public string? Token { get; set; }

        public string? TwitchId { get; set; }
        public string? TwitchSecret { get; set; }

        public ulong DevGuild { get; set; }
        public ulong OwnerId { get; set; }

        public static Config FromFile(string filePath) {
            var text = File.ReadAllText(filePath);
            var conf = Tomlyn.Toml.ToModel<Config>(text);
            
            return conf;
        }
    }
}