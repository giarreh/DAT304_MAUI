﻿using CommunityToolkit.Maui;
using MauiApp8.Services.BackgroundServices.Realm;
using MauiApp8.Services.GraphService;
using MauiApp8.Services.Health;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using MauiApp8.Services.Food;

namespace MauiApp8;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp(true)
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // ViewModels
        builder.Services.AddTransient<ViewModel.HomePageModel>();
        builder.Services.AddSingleton<ViewModel.SettingsPageModel>();
        builder.Services.AddSingleton<ViewModel.LoginPageModel>();
        builder.Services.AddSingleton<ViewModel.LogFoodModel>();
        builder.Services.AddSingleton<ViewModel.FoodDetailsModel>();
        builder.Services.AddSingleton<ViewModel.FoodViewModel>();
        builder.Services.AddSingleton<ViewModel.GraphPageModel>();


        //Views
        builder.Services.AddTransient<Views.HomePage>();
        builder.Services.AddSingleton<Views.LoginPage>();
        builder.Services.AddSingleton<Views.SettingsPage>();
        builder.Services.AddSingleton<Views.FoodPage>();
        builder.Services.AddSingleton<Views.FoodDetailsPage>();
        builder.Services.AddSingleton<Views.GraphPage>();
        builder.Services.AddTransient<Views.FoodItemView>();


        builder.Services.AddTransient<Model.Food>();


        // Services
        builder.Services.AddSingleton<IChartService>((e) => new LineChartService());
        builder.Services.AddTransient<Services.BackgroundServices.Realm.ICRUD>((e) => new Services.BackgroundServices.Realm.CRUD());
        builder.Services.AddTransient<Services.BackgroundServices.Realm.IUtils>((e) => new Services.BackgroundServices.Realm.Utils());
        builder.Services.AddTransient<IHealthService>((e) => new HealthService(e.GetService<IUtils>(), e.GetService<ICRUD>()));
        builder.Services.AddSingleton<Services.Authentication.IAuthenticationService>((e) => new Services.Authentication.Authenticated_stub());
        builder.Services.AddTransient<Services.DataServices.IDataService>((e) => new Services.DataServices.FoodService_stub(e.GetService<Services.BackgroundServices.IBackgroundService>()));
        builder.Services.AddTransient<Realms.Realm>(e => Services.DBService.CreateDB.RealmCreate());
        builder.Services.AddTransient<Services.BackgroundServices.IBackgroundService>((e) => new Services.BackgroundServices.DataBase());
        builder.Services.AddSingleton<IFoodService, FoodService>(); // Add this line








        var app = builder.Build();
        return app;
    }
}