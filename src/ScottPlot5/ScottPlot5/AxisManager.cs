﻿using ScottPlot.AxisPanels;

namespace ScottPlot;

public class AxisManager
{
    private readonly Plot Plot;

    /// <summary>
    /// Logic that determines padding around the data area when <see cref="AutoScale()"/> is called
    /// </summary>
    public IAutoScaler AutoScaler { get; set; } = new AutoScalers.FractionalAutoScaler(.1, .15);

    /// <summary>
    /// Horizontal axes
    /// </summary>
    internal List<IXAxis> XAxes { get; } = new();

    /// <summary>
    /// Vertical axes
    /// </summary>
    internal List<IYAxis> YAxes { get; } = new();

    /// <summary>
    /// Panels take up spce on one side of the data area (like a colorbar)
    /// </summary>
    internal List<IPanel> Panels { get; } = new();

    /// <summary>
    /// A special panel
    /// </summary>
    public Panels.TitlePanel Title { get; } = new();

    /// <summary>
    /// All axes
    /// </summary>
    public IEnumerable<IAxis> GetAxes() => XAxes.Cast<IAxis>().Concat(YAxes);

    /// <summary>
    /// All axes with the given edge
    /// </summary>
    public IEnumerable<IAxis> GetAxes(Edge edge) => GetAxes().Where(x => x.Edge == edge).ToArray();

    /// <summary>
    /// Returns all axes, panels, and the title
    /// </summary>
    /// <returns></returns>
    internal IPanel[] GetPanels() => GetAxes().Concat(Panels).Concat(new[] { Title }).ToArray();

    /// <summary>
    /// The primary horizontal axis above the plot
    /// </summary>
    public IXAxis Top => XAxes.First(x => x.Edge == Edge.Top);

    /// <summary>
    /// The primary horizontal axis below the plot
    /// </summary>
    public IXAxis Bottom => XAxes.First(x => x.Edge == Edge.Bottom);

    /// <summary>
    /// The primary vertical axis to the left of the plot
    /// </summary>
    public IYAxis Left => YAxes.First(x => x.Edge == Edge.Left);

    /// <summary>
    /// The primary vertical axis to the right of the plot
    /// </summary>
    public IYAxis Right => YAxes.First(x => x.Edge == Edge.Right);

    /// <summary>
    /// All grids
    /// </summary>
    public List<IGrid> Grids { get; } = new();

    /// <summary>
    /// Rules that are applied before each render
    /// </summary>
    public List<IAxisRule> Rules { get; } = new();

    /// <summary>
    /// Contains state and logic for axes
    /// </summary>
    public AxisManager(Plot plot)
    {
        Plot = plot;

        // setup the default primary X and Y axes
        IXAxis xAxisPrimary = new BottomAxis();
        IYAxis yAxisPrimary = new LeftAxis();
        XAxes.Add(xAxisPrimary);
        YAxes.Add(yAxisPrimary);

        // add labeless secondary axes to get right side ticks and padding
        IXAxis xAxisSecondary = new TopAxis();
        IYAxis yAxisSecondary = new RightAxis();
        XAxes.Add(xAxisSecondary);
        YAxes.Add(yAxisSecondary);

        // add a default grid using the primary axes
        IGrid grid = new Grids.DefaultGrid(xAxisPrimary, yAxisPrimary);
        Grids.Add(grid);
    }

    public void Clear()
    {
        Grids.Clear();
        Panels.Clear();
        YAxes.Clear();
        XAxes.Clear();
    }

    /// <summary>
    /// Remove all axes that lie on the given edge.
    /// </summary>
    public void Remove(Edge edge)
    {
        foreach (IAxis axis in GetAxes(edge).ToArray())
        {
            if (axis is IXAxis xAxis)
                Plot.Axes.XAxes.Remove(xAxis);

            if (axis is IYAxis yAxis)
                Plot.Axes.YAxes.Remove(yAxis);
        }
    }

    /// <summary>
    /// Remove all axes on the given edge and add a new one that displays DateTime ticks
    /// </summary>
    public void DateTimeTicks(Edge edge)
    {
        Remove(edge);

        IXAxis dateAxis = edge switch
        {
            Edge.Left => throw new NotImplementedException(), // TODO: support vertical DateTime axes
            Edge.Right => throw new NotImplementedException(),
            Edge.Bottom => new DateTimeXAxis(),
            Edge.Top => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };

        Plot.Axes.XAxes.Add(dateAxis);

        foreach (IGrid grid in Plot.Axes.Grids)
        {
            grid.Replace(dateAxis);
        }
    }

    /// <summary>
    /// Crete a new axis, add it to the plot, and return it
    /// </summary>
    public LeftAxis AddLeftAxis()
    {
        LeftAxis axis = new();
        YAxes.Add(axis);
        return axis;
    }

    /// <summary>
    /// Crete a new axis, add it to the plot, and return it
    /// </summary>
    public RightAxis AddRightAxis()
    {
        RightAxis axis = new();
        YAxes.Add(axis);
        return axis;
    }

    /// <summary>
    /// Crete a new axis, add it to the plot, and return it
    /// </summary>
    public BottomAxis AddBottomAxis()
    {
        BottomAxis axis = new();
        XAxes.Add(axis);
        return axis;
    }

    /// <summary>
    /// Crete a new axis, add it to the plot, and return it
    /// </summary>
    public TopAxis AddTopAxis()
    {
        TopAxis axis = new();
        XAxes.Add(axis);
        return axis;
    }


    public void SetLimitsX(double left, double right, IXAxis xAxis)
    {
        xAxis.Min = left;
        xAxis.Max = right;
    }

    public void SetLimitsY(double bottom, double top, IYAxis yAxis)
    {
        yAxis.Min = bottom;
        yAxis.Max = top;
    }

    public void SetLimitsX(double left, double right)
    {
        SetLimitsX(left, right, Bottom);
    }

    public void SetLimitsY(double bottom, double top)
    {
        SetLimitsY(bottom, top, Left);
    }

    public void SetLimits(double left, double right, double bottom, double top)
    {
        SetLimitsX(left, right, Bottom);
        SetLimitsY(bottom, top, Left);
    }

    public void SetLimits(double left, double right, double bottom, double top, IXAxis xAxis, IYAxis yAxis)
    {
        SetLimitsX(left, right, xAxis);
        SetLimitsY(bottom, top, yAxis);
    }

    public void SetLimits(double? left = null, double? right = null, double? bottom = null, double? top = null)
    {
        SetLimitsX(left ?? Bottom.Min, right ?? Bottom.Max);
        SetLimitsY(bottom ?? Left.Min, top ?? Left.Max);
    }

    public void SetLimits(CoordinateRect rect)
    {
        SetLimits(rect.Left, rect.Right, rect.Bottom, rect.Top);
    }

    public void SetLimitsX(CoordinateRect limits)
    {
        SetLimitsX(limits.Left, limits.Right, Bottom);
    }

    public void SetLimitsY(CoordinateRect limits)
    {
        SetLimitsY(limits.Bottom, limits.Top, Left);
    }

    public void SetLimitsX(AxisLimits limits)
    {
        SetLimitsX(limits.Left, limits.Right);
    }

    public void SetLimitsX(AxisLimits limits, IXAxis xAxis)
    {
        SetLimitsX(limits.Left, limits.Right, xAxis);
    }

    public void SetLimitsY(AxisLimits limits)
    {
        SetLimitsY(limits.Bottom, limits.Top);
    }

    public void SetLimitsY(AxisLimits limits, IYAxis yAxis)
    {
        SetLimitsY(limits.Bottom, limits.Top, yAxis);
    }

    public void SetLimits(AxisLimits limits)
    {
        SetLimits(limits, Bottom, Left);
    }

    public void SetLimits(AxisLimits limits, IXAxis xAxis, IYAxis yAxis)
    {
        SetLimitsX(limits.Left, limits.Right, xAxis);
        SetLimitsY(limits.Bottom, limits.Top, yAxis);
    }

    public void SetLimits(CoordinateRange xRange, CoordinateRange yRange)
    {
        AxisLimits limits = new(xRange.Min, xRange.Max, yRange.Min, yRange.Max);
        SetLimits(limits);
    }

    /// <summary>
    /// Return the 2D axis limits for the default axes
    /// </summary>
    public AxisLimits GetLimits()
    {
        return new AxisLimits(
            Bottom.Min,
            Bottom.Max,
            Left.Min,
            Left.Max);
    }

    /// <summary>
    /// Return the 2D axis limits for the given X/Y axis pair
    /// </summary>
    public AxisLimits GetLimits(IXAxis xAxis, IYAxis yAxis)
    {
        return new AxisLimits(xAxis.Min, xAxis.Max, yAxis.Min, yAxis.Max);
    }

    /// <summary>
    /// Adds the default X and Y axes to all plottables with unset axes
    /// </summary>
    internal void ReplaceNullAxesWithDefaults()
    {
        foreach (var plottable in Plot.PlottableList)
        {
            if (plottable.Axes.XAxis is null)
                plottable.Axes.XAxis = Bottom;

            if (plottable.Axes.YAxis is null)
                plottable.Axes.YAxis = Left;
        }
    }

    /// <summary>
    /// Automatically scale all axes to fit the data in all plottables
    /// </summary>
    public void AutoScale()
    {
        ReplaceNullAxesWithDefaults();
        AutoScaler.AutoScaleAll(Plot.PlottableList);
    }

    public void AutoScaleX()
    {
        AutoScaleX(Bottom);
    }

    public void AutoScaleY()
    {
        AutoScaleY(Left);
    }

    public void AutoScaleX(IXAxis xAxis)
    {
        ReplaceNullAxesWithDefaults();
        AxisLimits limits = AutoScaler.GetAxisLimits(Plot, xAxis, Left);
        SetLimitsX(limits.Left, limits.Right, xAxis);
    }

    public void AutoScaleY(IYAxis yAxis)
    {
        ReplaceNullAxesWithDefaults();
        AxisLimits limits = AutoScaler.GetAxisLimits(Plot, Bottom, yAxis);
        SetLimitsY(limits.Bottom, limits.Top, yAxis);
    }

    /// <summary>
    /// Autoscale the given axes to accommodate the data from all plottables that use them
    /// </summary>
    public void AutoScale(IXAxis xAxis, IYAxis yAxis, bool horizontal = true, bool vertical = true)
    {
        ReplaceNullAxesWithDefaults();

        AxisLimits limits = AutoScaler.GetAxisLimits(Plot, xAxis, yAxis);

        if (horizontal)
        {
            SetLimitsX(limits.Left, limits.Right, xAxis);
        }

        if (vertical)
        {
            SetLimitsY(limits.Bottom, limits.Top, yAxis);
        }
    }

    /// <summary>
    /// Adjust limits all axes to pan by the given distance in coordinate space
    /// </summary>
    public void Pan(CoordinateSize distance)
    {
        XAxes.ForEach(x => x.Range.Pan(distance.Width));
        YAxes.ForEach(x => x.Range.Pan(distance.Height));
    }

    /// <summary>
    /// Adjust limits all axes to pan by the given distance in pixel space
    /// </summary>
    public void Pan(PixelSize distance)
    {
        if (Plot.RenderManager.LastRender.Count == 0)
            throw new InvalidOperationException("at least one render is required before pixel panning is possible");

        XAxes.ForEach(ax => ax.Range.Pan(ax.GetCoordinateDistance(distance.Width, Plot.RenderManager.LastRender.DataRect)));
        YAxes.ForEach(ax => ax.Range.Pan(ax.GetCoordinateDistance(distance.Height, Plot.RenderManager.LastRender.DataRect)));
    }

    /// <summary>
    /// Modify limits of all axes to apply the given zoom.
    /// Fractional values >1 zoom in and <1 zoom out.
    /// </summary>
    public void Zoom(double fracX = 1.0, double fracY = 1.0)
    {
        XAxes.ForEach(xAxis => xAxis.Range.ZoomFrac(fracX));
        YAxes.ForEach(yAxis => yAxis.Range.ZoomFrac(fracY));
    }
    /// <summary>
    /// Reset plot data margins to their default value.
    /// </summary>
    public void Margins()
    {
        AutoScaler = new AutoScalers.FractionalAutoScaler();
        AutoScale();
    }

    /// <summary>
    /// Define the amount of whitespace to place around the data area when calling <see cref="AutoScale()"/>.
    /// Values are a fraction from 0 (tightly fit the data) to 1 (lots of whitespace).
    /// </summary>
    public void Margins(double horizontal = 0.1, double vertical = .15)
    {
        AutoScaler = new AutoScalers.FractionalAutoScaler(horizontal, vertical);
        AutoScale();
    }

    /// <summary>
    /// Define the amount of whitespace to place around the data area when calling <see cref="AutoScale()"/>.
    /// Values are a fraction from 0 (tightly fit the data) to 1 (lots of whitespace).
    /// </summary>
    public void Margins(double left = .05, double right = .05, double bottom = .07, double top = .07)
    {
        AutoScaler = new AutoScalers.FractionalAutoScaler(left, right, bottom, top);
        AutoScale();
    }
}
