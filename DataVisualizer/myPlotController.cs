using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace DataVisualizer
{
    class myPlotController : PlotController
    {
        public myPlotController()
        {
            // Zoom rectangle bindings: MMB / control RMB / control+alt LMB
            this.BindMouseDown(OxyMouseButton.Middle, myPlotCommands.ZoomRectangle);
            this.BindMouseDown(OxyMouseButton.Right, OxyModifierKeys.Control, myPlotCommands.ZoomRectangle);
            this.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control | OxyModifierKeys.Alt, myPlotCommands.ZoomRectangle);

            // Reset bindings: Same as zoom rectangle, but double click / A key
            this.BindMouseDown(OxyMouseButton.Middle, OxyModifierKeys.None, 2, myPlotCommands.ResetAt);
            this.BindMouseDown(OxyMouseButton.Right, OxyModifierKeys.Control, 2, myPlotCommands.ResetAt);
            this.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control | OxyModifierKeys.Alt, 2, myPlotCommands.ResetAt);
            this.BindKeyDown(OxyKey.A, myPlotCommands.Reset);
            this.BindKeyDown(OxyKey.C, OxyModifierKeys.Control | OxyModifierKeys.Alt, myPlotCommands.CopyCode);
            this.BindKeyDown(OxyKey.R, OxyModifierKeys.Control | OxyModifierKeys.Alt, myPlotCommands.CopyTextReport);
            this.BindKeyDown(OxyKey.Home, myPlotCommands.Reset);
            this.BindCore(new OxyShakeGesture(), myPlotCommands.Reset);

            // Pan bindings: RMB / alt LMB / Up/down/left/right keys (panning direction on axis is opposite of key as it is more intuitive)
            this.BindMouseDown(OxyMouseButton.Right, myPlotCommands.PanAt);
            this.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Alt, myPlotCommands.PanAt);
            this.BindKeyDown(OxyKey.Left, myPlotCommands.PanLeft);
            this.BindKeyDown(OxyKey.Right, myPlotCommands.PanRight);
            this.BindKeyDown(OxyKey.Up, myPlotCommands.PanUp);
            this.BindKeyDown(OxyKey.Down, myPlotCommands.PanDown);
            this.BindKeyDown(OxyKey.Left, OxyModifierKeys.Control, myPlotCommands.PanLeftFine);
            this.BindKeyDown(OxyKey.Right, OxyModifierKeys.Control, myPlotCommands.PanRightFine);
            this.BindKeyDown(OxyKey.Up, OxyModifierKeys.Control, myPlotCommands.PanUpFine);
            this.BindKeyDown(OxyKey.Down, OxyModifierKeys.Control, myPlotCommands.PanDownFine);

            this.BindTouchDown(myPlotCommands.PanZoomByTouch);

            // Tracker bindings: LMB
            this.BindMouseDown(OxyMouseButton.Left, myPlotCommands.SnapTrack);
            this.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control, myPlotCommands.Track);
            this.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Shift, myPlotCommands.PointsOnlyTrack);

            // Tracker bindings: Touch
            this.BindTouchDown(myPlotCommands.SnapTrackTouch);

            // Zoom in/out binding: XB1 / XB2 / mouse wheels / +/- keys
            this.BindMouseDown(OxyMouseButton.XButton1, myPlotCommands.ZoomInAt);
            this.BindMouseDown(OxyMouseButton.XButton2, myPlotCommands.ZoomOutAt);
            this.BindMouseWheel(myPlotCommands.ZoomWheel);
            this.BindMouseWheel(OxyModifierKeys.Control, myPlotCommands.ZoomWheelFine);

            this.BindKeyDown(OxyKey.Add, myPlotCommands.ZoomIn);
            this.BindKeyDown(OxyKey.Subtract, myPlotCommands.ZoomOut);
            this.BindKeyDown(OxyKey.PageUp, myPlotCommands.ZoomIn);
            this.BindKeyDown(OxyKey.PageDown, myPlotCommands.ZoomOut);
            this.BindKeyDown(OxyKey.Add, OxyModifierKeys.Control, myPlotCommands.ZoomInFine);
            this.BindKeyDown(OxyKey.Subtract, OxyModifierKeys.Control, myPlotCommands.ZoomOutFine);
            this.BindKeyDown(OxyKey.PageUp, OxyModifierKeys.Control, myPlotCommands.ZoomInFine);
            this.BindKeyDown(OxyKey.PageDown, OxyModifierKeys.Control, myPlotCommands.ZoomOutFine);           
        }
    }
}