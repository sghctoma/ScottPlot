﻿using ScottPlot;
using System.Data;

namespace WinForms_Demo.Demos;

public partial class AxisRules : Form, IDemoWindow
{
    public string Title => "Axis Rules";

    public string Description => "Configure rules that limit how far the user " +
        "can zoom in or out or enforce equal axis scaling";

    public AxisRules()
    {
        InitializeComponent();
        UnlockButtons();

        Coordinates[] coordinates = Generate.RandomCoordinates(1000);
        var sp = formsPlot1.Plot.Add.Scatter(coordinates);
        sp.LineWidth = 0;
        sp.MarkerStyle.Size = 5;
    }

    private void LockButtons()
    {
        groupBox1.Enabled = false;
        groupBox2.Enabled = false;
        groupBox3.Enabled = false;
        groupBox4.Enabled = false;
        btnReset.Enabled = true;
    }

    private void UnlockButtons()
    {
        groupBox1.Enabled = true;
        groupBox2.Enabled = true;
        groupBox3.Enabled = true;
        groupBox4.Enabled = true;
        btnReset.Enabled = false;
    }

    private void btnBoundaryMin_Click(object sender, EventArgs e)
    {
        ScottPlot.AxisRules.MinimumBoundary rule = new(
            xAxis: formsPlot1.Plot.Axes.Bottom,
            yAxis: formsPlot1.Plot.Axes.Left,
            limits: new AxisLimits(0, 1, 0, 1));

        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.Rules.Add(rule);
        formsPlot1.Plot.Title("Disable zooming in beyond [0, 1]");
        formsPlot1.Refresh();
        LockButtons();
    }

    private void btnBoundaryMax_Click(object sender, EventArgs e)
    {
        ScottPlot.AxisRules.MaximumBoundary rule = new(
            xAxis: formsPlot1.Plot.Axes.Bottom,
            yAxis: formsPlot1.Plot.Axes.Left,
            limits: new AxisLimits(0, 1, 0, 1));

        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.Rules.Add(rule);
        formsPlot1.Plot.Title("Disable zooming out beyond [0, 1]");
        formsPlot1.Refresh();
        LockButtons();
    }

    private void btnScalePreserveX_Click(object sender, EventArgs e)
    {
        ScottPlot.AxisRules.SquarePreserveX rule = new(
            xAxis: formsPlot1.Plot.Axes.Bottom,
            yAxis: formsPlot1.Plot.Axes.Left);

        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.Rules.Add(rule);
        formsPlot1.Plot.Title("Square axes zooming Y to preserve X");
        formsPlot1.Refresh();
        LockButtons();
    }

    private void btnScalePreserveY_Click(object sender, EventArgs e)
    {
        ScottPlot.AxisRules.SquarePreserveY rule = new(
            xAxis: formsPlot1.Plot.Axes.Bottom,
            yAxis: formsPlot1.Plot.Axes.Left);

        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.Rules.Add(rule);
        formsPlot1.Plot.Title("Square axes zooming X to preserve Y");
        formsPlot1.Refresh();
        LockButtons();
    }

    private void btnScaleZoom_Click(object sender, EventArgs e)
    {
        ScottPlot.AxisRules.SquareZoomOut rule = new(
            xAxis: formsPlot1.Plot.Axes.Bottom,
            yAxis: formsPlot1.Plot.Axes.Left);

        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.Rules.Add(rule);
        formsPlot1.Plot.Title("Square axes by zooming out the smaller axis");
        formsPlot1.Refresh();
        LockButtons();
    }

    private void btnSpanMin_Click(object sender, EventArgs e)
    {
        ScottPlot.AxisRules.MinimumSpan rule = new(
            xAxis: formsPlot1.Plot.Axes.Bottom,
            yAxis: formsPlot1.Plot.Axes.Left,
            xSpan: 1,
            ySpan: 1);

        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.Rules.Add(rule);
        formsPlot1.Plot.Title("Disabled zooming in beyond an axis span of 1");
        formsPlot1.Refresh();
        LockButtons();
    }

    private void btnSpanMax_Click(object sender, EventArgs e)
    {
        ScottPlot.AxisRules.MaximumSpan rule = new(
            xAxis: formsPlot1.Plot.Axes.Bottom,
            yAxis: formsPlot1.Plot.Axes.Left,
            xSpan: 1,
            ySpan: 1);

        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.Rules.Add(rule);
        formsPlot1.Plot.Title("Disabled zooming out beyond an axis span of 1");
        formsPlot1.Refresh();
        LockButtons();
    }

    private void btnLockHorizontal_Click(object sender, EventArgs e)
    {
        ScottPlot.AxisRules.LockedHorizontal rule = new(formsPlot1.Plot.Axes.Bottom);

        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.Rules.Add(rule);
        formsPlot1.Plot.Title("Horizontal Axis is Locked");
        formsPlot1.Refresh();
        LockButtons();
    }

    private void btnLockVertical_Click(object sender, EventArgs e)
    {
        ScottPlot.AxisRules.LockedVertical rule = new(formsPlot1.Plot.Axes.Left);

        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.Rules.Add(rule);
        formsPlot1.Plot.Title("Vertical Axis is Locked");
        formsPlot1.Refresh();
        LockButtons();
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        formsPlot1.Plot.Axes.Rules.Clear();
        formsPlot1.Plot.Axes.AutoScale();
        formsPlot1.Plot.Title("No axis rules are in effect");
        formsPlot1.Refresh();
        UnlockButtons();
    }
}
