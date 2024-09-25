using System.Net;
using CSharpFunctionalExtensions;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rittou.Preconditions;

namespace Rittou.SlashCommands {
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

    public struct DuckObject : IJsonDeserializable {
        public string Url { get; set; }
        public string Message { get; set; }

        public IJsonResult FromJson(string json) {
            var data = JsonConvert.DeserializeObject<DuckObject>(json);

            if (data.Url != null) {
                return Result.Success<IJsonDeserializable, string>(data);
            } else {
                return Result.Failure<IJsonDeserializable, string>($"API returned {data.Message}");
            }
        }

        public string GetUrl() {
            return Url;
        }
    }

    public struct FoxObject : IJsonDeserializable {
        public string Image { get; set; }
        public string Link { get; set; }

        public IJsonResult FromJson(string json) {
            var data = JsonConvert.DeserializeObject<FoxObject>(json);

            if (data.Image != null) {
                return Result.Success<IJsonDeserializable, string>(data);
            } else {
                return Result.Failure<IJsonDeserializable, string>($"Fox API failed");
            }
        }

        public string GetUrl() {
            return Image;
        }
    }

    struct MKBHDCollection : IJsonDeserializable {
        public string Version { get; set; }
        public Dictionary<string, MKBHDObject> Data { get; set; }

        public IJsonResult FromJson(string jsonString) {
            var data = JsonConvert.DeserializeObject<MKBHDCollection>(jsonString);
            if (data.Data != null || Version != "1") {
                return Result.Success<IJsonDeserializable, string>(data);
            } else {
                if (Version != "1") {
                    return Result.Failure<IJsonDeserializable, string>($"API failed: Unsupported version: {data.Version}");
                }

                return Result.Failure<IJsonDeserializable, string>($"API failed: error unknown: {data}");
            }
        }

        public string GetUrl() {
            string obj = string.Empty;
            
            while (string.IsNullOrWhiteSpace(obj)) {
                obj = Data.ElementAt(new Random().Next(0, Data.Count)).Value.GetUrl();
            }

            return obj;
        }
    }

    public struct MKBHDObject : IJsonDeserializable {

        //[JsonProperty(propertyName: "dhd")]
        public string Dhd { get; set; }
        public string Dsd { get; set; }
        public string S { get; set; }
        public string E { get; set; }
        
        public IJsonResult FromJson(string jsonString) {
            var data = JsonConvert.DeserializeObject<MKBHDObject>(jsonString);
            
            if (data.Dsd != null || data.Dsd != null || data.S != null) {
                return Result.Success<IJsonDeserializable, string>(data);
            } else {
                return Result.Failure<IJsonDeserializable, string>($"API failed: error unknown: {data}");
            }
        }

        public string GetUrl() {
            if (Dhd != null) {
                return Dhd;
            }
            
            if (Dsd != null) {
                return Dsd;
            }

            if (S != null) {
                return S;
            }

            return E;
        }
    }


    public enum ImageSendType {
        Random,
        Dog,
        Cat,
        Duck,
        Fox
    }

    [CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
    [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
    public class ImageSlashModule : InteractionModuleBase<SocketInteractionContext> {
        private const string DOG_URL = "https://dog.ceo/api/breeds/image/random";
        private const string CAT_URL = "https://api.thecatapi.com/v1/images/search";
        private const string DUCK_URL = "https://random-d.uk/api/v2/quack";
        private const string FOX_URL = "https://randomfox.ca/floof/";

        [RequireOwnerUserCommand]
        [SlashCommand("animal", "Sends an image/gif of an animal")]
        public async Task ImageCommandAsync([Summary(description: "The image you want to send")] ImageSendType image) {
            var maybeResult = Maybe<IJsonResult>.None;
            
            if (image == ImageSendType.Random) {
                image = (ImageSendType)new Random().Next(1, Enum.GetNames(typeof(ImageSendType)).Length - 1);
            }

            switch (image) {
                case ImageSendType.Dog:
                    maybeResult = await IJsonDeserializable.GetJson<DogObject>(DOG_URL);
                    break;
                case ImageSendType.Cat:
                    maybeResult = await IJsonDeserializable.GetJson<CatObject>(CAT_URL);
                    break;
                case ImageSendType.Duck:
                    maybeResult = await IJsonDeserializable.GetJson<DuckObject>(DUCK_URL);
                    break;
                case ImageSendType.Fox:
                    maybeResult = await IJsonDeserializable.GetJson<FoxObject>(FOX_URL);
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

        public static string GetFileExtensionFromUrl(string url) {
            url = url.Split('?')[0];
            url = url.Split('/').Last();
            
            return url.Contains('.') ? url.Substring(url.LastIndexOf('.')) : "";
        }

        [SlashCommand("mkbhd", "View images from MKBHD's \"Panels\" app")]   
        public async Task MkbhdAsync() {
            Maybe<IJsonResult> maybeResult = await IJsonDeserializable.GetJson<MKBHDCollection>("https://storage.googleapis.com/panels-api/data/20240916/media-1a-i-p~s");

            if (maybeResult.HasValue) {
                var result = maybeResult.Value;

                if (result.IsSuccess) {
                    if (result.Value is MKBHDCollection collection) {
                        int index = 0;

                        foreach (var file in collection.Data) {
                            Console.WriteLine(file.Key);
                            string url = file.Value.GetUrl();

                            Console.WriteLine($"{file.Key} ({index + 1}/{collection.Data.Count})");
                            Console.WriteLine(url);
                            Console.WriteLine(GetFileExtensionFromUrl(url));
                            
                            if (File.Exists($"mkbhd/{file.Key}{GetFileExtensionFromUrl(url)}")) {
                                Console.WriteLine($"   file for {file.Key} exists; skipping");
                                index += 1;
                                continue;
                            } else {
                                if (!Directory.Exists("mkbhd")) {
                                    Directory.CreateDirectory("mkbhd");
                                }
                            }

                            using (var client = new HttpClient()) {
                                if (string.IsNullOrWhiteSpace(url)) {
                                    Console.WriteLine($"   url for {file.Key} empty; skipping");
                                    index += 1;
                                    continue;
                                }
                                
                                var response = await client.GetAsync(url);

                                using (var stream = new FileStream($"mkbhd/{file.Key}{GetFileExtensionFromUrl(url)}", FileMode.Create, FileAccess.Write, FileShare.None)) {
                                    await response.Content.CopyToAsync(stream);
                                    Console.WriteLine($"  Downloaded {file.Key}{GetFileExtensionFromUrl(url)}");
                                }
                            }

                            index += 1;
                        }

                        string mkb = collection.GetUrl();
                        
                        Embed embed = new EmbedBuilder()
                        .WithImageUrl(mkb)
                        .WithFooter($"Panels CDN v{collection.Version}")
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