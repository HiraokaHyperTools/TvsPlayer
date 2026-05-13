using Microsoft.Extensions.DependencyInjection;
using VideoConv4Win.Helpers;

namespace VideoConv4Win
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();
            services.AddTransient<Form1>();
            services.AddTransient<ConvertProgressForm>();
            services.AddTransient<Func<ConvertProgressForm>>(
                sp =>
                    () =>
                        sp.GetRequiredService<ConvertProgressForm>()
            );
            services.AddVideoConv4Win();
            using var resolver = services.BuildServiceProvider();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(resolver.GetRequiredService<Form1>());
        }
    }
}