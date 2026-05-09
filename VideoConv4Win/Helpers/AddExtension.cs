using LibTvsPlayer.Helpers;
using LibTvsPlayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace VideoConv4Win.Helpers
{
    public static class AddExtension
    {
        public static IServiceCollection AddVideoConv4Win(this IServiceCollection services)
        {
            services.AddSingleton<DetectVideoFrameSize>();
            services.AddSingleton<ParseTvsStruc>();
            services.AddSingleton<ParseKey>();
            services.AddSingleton(ParsePackedBlocksHelper.Default);
            services.AddSingleton(ParsePackedRectBlockHelper.Default);
            services.AddSingleton(ParseRect21.Default);
            services.AddSingleton(TileConfHelperV2.Default);
            services.AddSingleton<DecodeKeyRecord>();
            services.AddSingleton<DecodeMouseRecord>();
            //NextService: services.AddSingleton<$ClassName$>();
            return services;
        }
    }
}