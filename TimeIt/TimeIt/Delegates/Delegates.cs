namespace TimeIt.Delegates
{
    /// <summary>
    /// Used to call the invalidate surface method
    /// of the canvas view
    /// </summary>
    public delegate void InvalidateSurfaceEvent();

    public delegate void RequestRedraw(bool redraw);

    /// <summary>
    /// Used to indicate wether the ok or cancel button was clicked
    /// </summary>
    /// <param name="ok">True for ok, false for cancel / background click / dismiss</param>
    public delegate void OnConfirmDialogButtonClick(bool ok);
}
