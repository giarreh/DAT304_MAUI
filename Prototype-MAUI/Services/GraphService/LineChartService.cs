﻿using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using Newtonsoft.Json.Linq;
using MauiApp8.Services.Health;
using MauiApp8.Services.PublishSubscribeService;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp8.Model;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using static System.Net.Mime.MediaTypeNames;

namespace MauiApp8.Services.GraphService
{
    internal partial class LineChartService<T> : ObservableObject, IChartService<T>, IRecipient<GlucoseDataMessage>, IRecipient<InsulinDataMessage>
    {
        Publish _publish;
        IHealthService _healthService;
        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        ObservableCollection<Model.GlucoseInfo> glucosesChart;
        [ObservableProperty]
        ObservableCollection<Model.InsulinInfo> insulinsChart;

     
        public T LastPointInData { get; private set; }

        [ObservableProperty]
        DateTimeOffset fromDate;
        [ObservableProperty]
        DateTimeOffset toDate;

        public LineChartService(Publish publish, IHealthService healthService)
        {
            _publish = publish;
            GlucosesChart = new ObservableCollection<Model.GlucoseInfo>();
            InsulinsChart = new ObservableCollection<Model.InsulinInfo>();
            _healthService = healthService;
            FromDate = DateTimeOffset.UtcNow.AddDays(-1);
            ToDate = DateTimeOffset.UtcNow;

            WeakReferenceMessenger.Default.Register<GlucoseDataMessage>(this);
            WeakReferenceMessenger.Default.Register<InsulinDataMessage>(this);




            Task.Run(async () =>
            {
                await  _publish.HealthSub(); // Assuming this is a synchronous method

            });



            //_publish.GlucoseDataAvailable += (sender, e) =>
            //{
            //    foreach (var glucose in e.GlucoseData)
            //    {
            //        if (!GlucosesChart.Contains(glucose))
            //        {
            //            // Add the new Glucose object to the GlucosesChart collection
            //            GlucosesChart.Add(glucose);
            //            Console.WriteLine($"Glucose: {glucose.Glucose} Timestamp : {glucose.Timestamp}");

            //        }
            //    }

            //    switch (typeof(T).Name)
            //    {
            //        case nameof(HealthData):
            //            LastPointInData = (T)(object)new HealthData
            //            {
            //                LastGlucose = int.TryParse(GlucosesChart.Where(g => g.Timestamp <= ToDate)
            //                .LastOrDefault()?.Glucose.ToString() ?? "0", out int lastGlucoseLevel) ? lastGlucoseLevel : 0,

            //                SecondLastGlucose = int.TryParse(GlucosesChart.Where(g => g.Timestamp <= ToDate)
            //                .Reverse()
            //                .Skip(1)
            //                .LastOrDefault()?.Glucose.ToString() ?? "0", out int secondLastGlucoseLevel) ? secondLastGlucoseLevel : 0,



            //    };
            //            break;



            //        default:
            //            throw new NotImplementedException($"Unknown data type: {typeof(T).Name}");
            //    }
            //    IsDataChanged = true;
            //    OnDataChanged();

            //};

            //_publish.InsulinDataAvailable += (sender, e) =>
            //{
            //    foreach (var insulin in e.InsulinData)
            //    {
            //        if (!InsulinsChart.Contains(insulin))
            //        {
            //            InsulinsChart.Add(insulin);
            //            Console.WriteLine($"Insulin: {insulin.Insulin} Timestamp : {insulin.Timestamp} Basal : {insulin.Basal} ");

            //        }
            //        //Console.WriteLine($"Insulin: {insulin.Insulin} Timestamp : {insulin.Timestamp} Basal : {insulin.Basal} ");
            //    }

              


            

             

        }

     

        






        public async Task<ISeries> AddBasalSeries()
        {

            double threshold = 10; // Set threshold value


            var basalSeries = new LineSeries<Model.InsulinInfo>
            {
                Values = InsulinsChart,
                LineSmoothness = 1,
                GeometrySize = 0,
                GeometryStroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
                Fill = new SolidColorPaint(SKColors.Transparent), // Set Fill to transparent
                ScalesYAt = 0,
                Name = "Basal",
                TooltipLabelFormatter = (point) => $"Basal: {point.PrimaryValue.ToString("0.0000")}U, Time: {DateTime.FromOADate(point.TertiaryValue):dd/MM HH:mm}",
                Mapping = (insulins, point) =>
                {
                    if (insulins != null && point != null && point.Context != null && point.Context.Entity != null)
                    {
                        point.PrimaryValue = (float)insulins.Basal;
                        point.SecondaryValue = point.Context.Entity.EntityIndex;
                        point.TertiaryValue = insulins.Timestamp.ToLocalTime().DateTime.ToOADate();
                    }
                }

        };
            
            

            return basalSeries;
        }

        public async Task<ISeries> AddGlucosesSeries()
        {

            double threshold = 10; // Set threshold value

            var glucoseseries = new LineSeries<GlucoseInfo>
            {
                Values = GlucosesChart,
                GeometrySize = 12,
                GeometryStroke =  new SolidColorPaint(SKColors.Green) { StrokeThickness = 2 },
                Fill = new SolidColorPaint(SKColors.Transparent), // Set Fill to transparent
                ScalesYAt = 0,
                Name = "Glucose",
                Stroke = null, // remove the line between points

                TooltipLabelFormatter = (point) => $"Glucose: {point.PrimaryValue}mg/dl, Time: {DateTime.FromOADate(point.TertiaryValue):dd/MM HH:mm}",
                Mapping = (glucoses, point) =>
                {
                    if (glucoses != null && point != null && point.Context != null && point.Context.Entity != null)
                    {
                        point.PrimaryValue = (float)glucoses.Glucose;
                        point.SecondaryValue = point.Context.Entity.EntityIndex;
                        point.TertiaryValue = glucoses.Timestamp.ToLocalTime().DateTime.ToOADate();

                        

                    }
                   
                },


            };
           

            var diffs = GlucosesChart.Zip(GlucosesChart.Skip(1), (prev, curr) => Math.Abs(curr.Glucose - prev.Glucose));
            var hasLargeDiff = diffs.Any(d => d > threshold);
            glucoseseries.Stroke = hasLargeDiff ? new SolidColorPaint(SKColors.Green) { StrokeThickness = 2 } : null;

           

            return glucoseseries;
        }

        public async Task<ISeries> AddInsulinSeries()
        {

            double threshold = 10; // Set threshold value


            var insulinSeries = new LineSeries<Model.InsulinInfo>
            {
                Values = InsulinsChart,
                GeometrySize = 4,
                GeometryStroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
                Fill = new SolidColorPaint(SKColors.Transparent), // Set Fill to transparent
                ScalesYAt = 0,
                Name = "Insulin",
                Stroke = null, // remove the line between points

                TooltipLabelFormatter = (point) => $"Insulin: {point.QuaternaryValue} units, Time: {DateTime.FromOADate(point.TertiaryValue):dd/MM HH:mm}",
                Mapping = (insulins, point) =>
                {
                    if (insulins != null && point != null && point.Context != null && point.Context.Entity != null && insulins.Insulin != 0)
                    {
                        point.PrimaryValue = (float)insulins.Insulin;
                        point.SecondaryValue = -1;
                        point.TertiaryValue = insulins.Timestamp.ToLocalTime().DateTime.ToOADate();

                        // Match with glucose timestamps
                        var glucose = GlucosesChart.FirstOrDefault(g => g.Timestamp == insulins.Timestamp);
                        var index = GlucosesChart.IndexOf(glucose);

                        if (glucose != null)
                        {
                            point.PrimaryValue += glucose.Glucose;
                            point.QuaternaryValue = (float)insulins.Insulin; // Store the insulin value

                            point.SecondaryValue = point.TertiaryValue;
                        }
                        else
                        {
                            // If no exact match, find closest glucose reading
                            glucose = GlucosesChart.OrderBy(g => Math.Abs(g.Timestamp.Ticks - insulins.Timestamp.Ticks)).FirstOrDefault();
                            glucose = GlucosesChart.OrderBy(g => Math.Abs(g.Timestamp.Ticks - insulins.Timestamp.Ticks)).FirstOrDefault();


                            var index2 = GlucosesChart.IndexOf(glucose);

                            if (glucose != null)
                            {
                                point.PrimaryValue += glucose.Glucose;
                                point.SecondaryValue = index2;
                                point.QuaternaryValue = (float)insulins.Insulin; // Store the insulin value

                            }

                        }
                    }

                },
            
        };

            var insulinRange = InsulinsChart.Max(i => i.Insulin) - InsulinsChart.Min(i => i.Insulin);
            insulinSeries.GeometrySize = (float)(insulinRange * 6);


            return insulinSeries;
        }



        [RelayCommand]
        void AddInsulin(List<InsulinInfo> insulin)
        {
            InsulinsChart.Clear();
            foreach (var item in insulin)
            {
                Console.WriteLine($" insulin : {item.Insulin} {item.Basal} {item.Timestamp}");
                InsulinsChart.Add(item);

            }
        }


        [RelayCommand]
        void AddGlucose(List<GlucoseInfo> glucose)
        {
            GlucosesChart.Clear();
            foreach (var item in glucose)
            {

                
                Console.WriteLine($"{item.Glucose} {item.Timestamp}");

                GlucosesChart.Add(item);

            }
        }
        public void Receive(InsulinDataMessage message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                AddInsulin(message.Value);
                WeakReferenceMessenger.Default.Send(new InsulinChartMessage(await AddBasalSeries()));
            });
        }

        public void Receive(GlucoseDataMessage message)
        {
           
            
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    AddGlucose(message.Value);
                    WeakReferenceMessenger.Default.Send(new GlucoseChartMessage(await AddGlucosesSeries()));

                });

           
        }
    }
}

