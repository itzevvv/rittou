using CSharpFunctionalExtensions;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace Yuki.SlashCommands {
    /*public enum ImageSendType {
        Dog,
        Cat
    }*/

    public enum TimeZoneAbbrev {

    }

    public class ToolsModule : InteractionModuleBase {
        [SlashCommand("timestamp", "Sends a Timestamp")]
        public async Task TimestampCommandAsync([Summary(description: "The date")] DateTime date) {
            await RespondAsync("OK");
            /*var maybeResult = Maybe<IJsonResult>.None;
            TimeZoneInfo

            switch (image) {
                case ImageSendType.Dog:
                    maybeResult = await IJsonDeserializable.GetJson<DogObject>(DOG_URL);
                    break;
                case ImageSendType.Cat:
                    maybeResult = await IJsonDeserializable.GetJson<CatObject>(CAT_URL);
                    break;
            };

            if (maybeResult.HasValue) {
                var result = maybeResult.Value;

                if (result.IsSuccess) {
                    var jsonResult = result.Value;
                    
                    await RespondAsync(jsonResult.GetUrl());
                } else {
                    await RespondAsync(result.Error);
                }
            } else {
                await RespondAsync("Cannot fetch image (unimplemented type)");
            }*/
        }
    }
}