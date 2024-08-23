using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Yuki.Preconditions {
    public class RequireOwnerUserCommand : PreconditionAttribute {
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services) {
            bool isUserInteraction = context.Interaction.IntegrationOwners.Any(integration => integration.Key is ApplicationIntegrationType.UserInstall);
            bool isInServerOrBotDm = (context.Channel is SocketDMChannel && context.Channel.Id == Yuki.Client.CurrentUser.Id) || context.Interaction.GuildId != null;
            bool isBotOwnerUserInteration = context.Interaction.IntegrationOwners.Any(integration => integration.Key is ApplicationIntegrationType.UserInstall && integration.Value == Yuki.Config.OwnerId);

            if (isUserInteraction && !isInServerOrBotDm) {
                if (isBotOwnerUserInteration) {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }

                return Task.FromResult(PreconditionResult.FromError("This command can only be run by a bot owner"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}