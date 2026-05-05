using LibTvsPlayer.Helpers;
using LibTvsPlayer.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;
using TvsPlayer.Helpers;

namespace TvsPlayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton<CollectTimestampedTvsChunkRefs>();
            builder.Services.AddSingleton<ParseKey>();
            builder.Services.AddSingleton<ParseTvsStruc>();
            builder.Services.AddSingleton<ReadTvsChunk>();
            builder.Services.AddSingleton(TileConfHelperV2.Default);
            builder.Services.AddSingleton<ApplyFrameSkipping>();
            builder.Services.AddSingleton(ParseRect21.Default);
            builder.Services.AddSingleton(ParsePackedRectBlockHelper.Default);
            builder.Services.AddSingleton(ParsePackedBlocksHelper.Default);
            builder.Services.AddSingleton<DecodeKeyRecord>();

            builder.Services.AddSingleton(new AppInfo(
                Version: (typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "?")
                    .Split('+')
                    .First(), // remove trailing hash-bang
                AboutUrl: "http://github.com/HiraokaHyperTools/TvsPlayer"
            ));

            await builder.Build().RunAsync();
        }
    }
}
