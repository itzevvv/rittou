using CSharpFunctionalExtensions;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Yuki.Preconditions;

namespace Yuki.SlashCommands {
    using IJsonResult = Result<IJsonDeserializable, string>;

    public interface IJsonDeserializable {
        public static async Task<IJsonResult> GetJson<T>(string url) where T : IJsonDeserializable, new() {
            using HttpClient client = new();

            HttpResponseMessage responseMessage;

            try {
                responseMessage = await client.GetAsync(url);
            } catch (Exception exp) {
                return Result.Failure<IJsonDeserializable, string>($"Http request failed with the following error: {exp.Message}");
            }

            if (responseMessage.IsSuccessStatusCode) {
                var jsonString = await responseMessage.Content.ReadAsStringAsync();

                try {
                    var data = new T().FromJson(jsonString);
                
                    if (data.IsSuccess) {
                        return data;
                    }
                    else {
                        return Result.Failure<IJsonDeserializable, string>($"Json parse failed with the following error: {data.Error}");
                    }
                } catch(Exception exp) {
                    return Result.Failure<IJsonDeserializable, string>($"Could not convert json to the specified data type: {exp.Message}");
                }
            }
            else {
                return Result.Failure<IJsonDeserializable, string>($"Json fetch returned status code {responseMessage.StatusCode}");
            }
        }

        public IJsonResult FromJson(string jsonString);
        
        public string GetUrl();
    }

    public struct CatObject : IJsonDeserializable {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public IJsonResult FromJson(string jsonString) {
            var data = JsonConvert.DeserializeObject<CatObject[]>(jsonString);
            
            if (data != null && data.Length > 0) {
                return Result.Success<IJsonDeserializable, string>(data[0]);
                // do something
            } else {
                return Result.Failure<IJsonDeserializable, string>("Data is null or has a length of 0");
            }
        }

        public string GetUrl() {
            return Url;
        }
    }

    public struct DogObject : IJsonDeserializable {
        public string Message { get; set; }
        public string Status { get; set; }

        public IJsonResult FromJson(string jsonString) {
            var data = JsonConvert.DeserializeObject<DogObject>(jsonString);
            
            if (data.Status == "success") {
                return Result.Success<IJsonDeserializable, string>(data);
            } else {
                return Result.Failure<IJsonDeserializable, string>($"API returned status {data.Status}");
            }
        }

        public string GetUrl() {
            return Message;
        }
    }

    struct NekoBestCollection : IJsonDeserializable {
        public NekoBestObject[] Results { get; set; }

        public IJsonResult FromJson(string jsonString) {
            var data = JsonConvert.DeserializeObject<NekoBestCollection>(jsonString);
            
            if (data.Results != null) {
                return Result.Success<IJsonDeserializable, string>(data);
            } else {
                return Result.Failure<IJsonDeserializable, string>($"API failed: error unknown: {data}");
            }
        }

        public string GetUrl() {
            return Results[new Random().Next(0, Results.Length - 1)].GetUrl();
        }
    }

    public struct NekoBestObject : IJsonDeserializable {

        [JsonProperty(propertyName: "anime_name")]
        public string AnimeName { get; set; }
        public string Url { get; set; }

        public IJsonResult FromJson(string jsonString) {
            var data = JsonConvert.DeserializeObject<NekoBestObject>(jsonString);
            
            if (data.Url != null) {
                return Result.Success<IJsonDeserializable, string>(data);
            } else {
                return Result.Failure<IJsonDeserializable, string>($"API failed: error unknown: {data}");
            }
        }

        public string GetUrl() {
            return Url;
        }
    }


    public enum ImageSendType {
        Dog,
        Cat
    }

    [CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
    [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
    public class ImageSlashModule : InteractionModuleBase<SocketInteractionContext> {
        private const string DOG_URL = "https://dog.ceo/api/breeds/image/random";
        private const string CAT_URL = "https://api.thecatapi.com/v1/images/search";

        [RequireOwnerUserCommand]
        [SlashCommand("image", "Sends an image")]
        public async Task ImageCommandAsync([Summary(description: "The image you want to send")] ImageSendType image) {
            var maybeResult = Maybe<IJsonResult>.None;
            
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
            }
        }

        [SlashCommand("goodnight", "Goodnight!")]   
        public async Task GoodnightCommandAsync() {
            Maybe<IJsonResult> maybeResult = await IJsonDeserializable.GetJson<NekoBestCollection>("https://nekos.best/api/v2/sleep");

            if (maybeResult.HasValue) {
                var result = maybeResult.Value;

                if (result.IsSuccess) {
                    if (result.Value is NekoBestCollection collection) {
                        Random rng = new Random();
                        NekoBestObject neko = collection.Results[rng.Next(0, collection.Results.Length - 1)];
                        
                        Embed embed = new EmbedBuilder()
                        .WithAuthor($"Goodnight, {Context.User.GlobalName}!")
                        .WithImageUrl(neko.GetUrl())
                        .WithFooter($"{neko.AnimeName}")
                        .Build();
                    
                    await RespondAsync(embed: embed);
                    }
                } else {
                    await RespondAsync(result.Error);
                }
            } else {
                await RespondAsync("Cannot fetch image (unimplemented type)");
            }
        }
    }
}