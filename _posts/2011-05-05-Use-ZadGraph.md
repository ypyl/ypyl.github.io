---
layout: post
title: Use ZadGraph
date: 2011-05-05

tags: dotnet
categories: programming
---
Library can be downloaded. License is LPGL 2.1.

```cs
if (dates.Count > 0 && dates.Count == values.Count)
{
    double max = FindMax(values);
    if (!Directory.Exists(_imageFolderName))
    {
        Directory.CreateDirectory(_imageFolderName);
    }
    GraphPane myPane = new GraphPane();

    // Set the titles and axis labels
    myPane.Title.Text = "Statistic of errors";
    myPane.XAxis.Title.Text = "Day";
    myPane.YAxis.Title.Text = "Percent";
    // Make up some data points based on the Sine function
    PointPairList list = new PointPairList();
    for (int i = 0; i < dates.Count; i++)
    {
        double x = dates[i].Date.ToOADate();
        double y = values[i];
        list.Add(x, y);
    }
    myPane.XAxis.CrossAuto = true;
    myPane.XAxis.Type = AxisType.Date;
    myPane.XAxis.Scale.MinorStep = 1;
    myPane.XAxis.Scale.MajorStep = 1;
    myPane.XAxis.Scale.Format = "dd.MM.yy";
    myPane.XAxis.MajorTic.IsBetweenLabels = true;
    myPane.XAxis.Scale.Min = dates[0].ToOADate();
    myPane.XAxis.Scale.Max = dates[dates.Count - 1].ToOADate();
    myPane.XAxis.AxisGap = 3f;

    myPane.YAxis.Scale.Max = max + 4;
    myPane.YAxis.Scale.MinorStep = myPane.YAxis.Scale.Max/10;
    myPane.YAxis.MajorGrid.IsVisible = true;
    myPane.YAxis.Type = AxisType.Linear;
    LineItem myCurve2 = myPane.AddCurve("Overall",
            list, Color.Blue, SymbolType.Diamond);
    myPane.YAxis.Scale.MajorStep = myPane.YAxis.Scale.Max / 5;

    myPane.Legend.Position = ZedGraph.LegendPos.Bottom;
    int width = 800;
    if (27 * dates.Count > width)
        width = 27 * dates.Count;
    myPane.GetImage(width, 600, 800f).Save(Path.Combine(_imageFolderName, _imageFileName), System.Drawing.Imaging.ImageFormat.Jpeg);
}
```
