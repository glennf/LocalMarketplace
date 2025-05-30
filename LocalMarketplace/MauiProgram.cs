﻿using Microsoft.Extensions.Logging;
using LocalMarketplace.Services;
using LocalMarketplace.ViewModels;
using LocalMarketplace.Views;

namespace LocalMarketplace;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        // Register services
        builder.Services.AddSingleton<DatabaseService>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
