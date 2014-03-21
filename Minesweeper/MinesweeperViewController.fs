﻿namespace Minesweeper

open System
open System.Drawing

open MonoTouch.UIKit
open MonoTouch.Foundation
open utils

[<Register ("MinesweeperViewController")>]
type MinesweeperViewController () =
    inherit UIViewController ()

    let mutable actionMode = ActionMode.Digging

    override this.ViewDidLoad () =
        let mines, neighbors = setMinesAndGetNeighbors

        let getButton i j = 
            let MinesweeperButtonClicked = 
                new EventHandler(fun sender eventargs -> 
                    let ms = sender :?> MinesweeperButton
                    if (actionMode = ActionMode.Flagging) then
                        if (ms.CurrentImage = UIImage.FromBundle("Flag.png")) then
                            ms.SetImage(null, UIControlState.Normal)
                        else
                            ms.SetImage(UIImage.FromBundle("Flag.png"), UIControlState.Normal)
                    elif (actionMode = ActionMode.Digging && ms.IsMine) then
                        ms.BackgroundColor <- UIColor.Red //dead
                    else
                        ms.SetImage(null, UIControlState.Normal)
                        ms.BackgroundColor <- UIColor.DarkGray
                        if (ms.SurroundingMines = 0) then
                            ms.SetTitle("", UIControlState.Normal)
                        else 
                            ms.SetTitle(ms.SurroundingMines.ToString(), UIControlState.Normal)
                    )
            
            let b = new MinesweeperButton(mines.[i,j], neighbors.[i,j])
            b.BackgroundColor <- UIColor.LightGray
            b.Frame <- new RectangleF((float32)i*35.f+25.f, (float32)j*35.f+25.f, (float32)32.f, (float32)32.f)
            b.TouchUpInside.AddHandler MinesweeperButtonClicked
            b

        let CreateButtonView i j = 
            this.View.Add (getButton i j)

        let CreateSliderView = 
            let s = new UISegmentedControl(new RectangleF((float32)50.f, (float32)Height*35.f+50.f, (float32)200.f, (float32)50.f))

            let HandleSegmentChanged = 
                new EventHandler(fun sender eventargs -> 
                    let s = sender :?> UISegmentedControl
                    actionMode <- match s.SelectedSegment with 
                                    | 0 -> ActionMode.Flagging
                                    | 1 -> ActionMode.Digging
                                    | _ -> ActionMode.Flagging
                    )

            s.InsertSegment(UIImage.FromBundle("Flag.png"), 0, false)
            s.InsertSegment(UIImage.FromBundle("Bomb.png"), 1, false)
            s.SelectedSegment <- 1
            s.ValueChanged.AddHandler HandleSegmentChanged
            this.View.Add s

        let boardTiles = Array2D.init Width Height CreateButtonView
        CreateSliderView

        base.ViewDidLoad ()

    override this.ShouldAutorotateToInterfaceOrientation (orientation) =
        orientation <> UIInterfaceOrientation.PortraitUpsideDown
    