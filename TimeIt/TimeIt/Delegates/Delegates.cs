namespace TimeIt.Delegates
{
    /// <summary>
    /// Used to call the invalidate surface method
    /// of the canvas view
    /// </summary>
    public delegate void InvalidateSurfaceEvent();

    public delegate void RequestRedraw(bool redraw);
}
