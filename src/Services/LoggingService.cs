using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Rittou.Services {
    public class LoggingService {
        public LoggingService(DiscordSocketClient client, CommandService cmd) {
            client.Log += LogAsync;
            cmd.Log += LogAsync;
        }

        private Task LogAsync(LogMessage msg) {
            if (msg.Exception is CommandException cmdEx) {
                Console.WriteLine($"[Command/{msg.Severity}] {cmdEx.Command.Aliases[0]} failed to execute in {cmdEx.Context.Channel}.");
                Console.WriteLine(cmdEx);
            } else {
                Console.WriteLine($"[General/{msg.Severity}] {msg}");
            }

            return Task.CompletedTask;
        }
    }
}