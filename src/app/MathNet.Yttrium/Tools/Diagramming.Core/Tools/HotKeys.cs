using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This class implementing the <see cref="IKeyboardListener"/> collects all the hotkeys.
    /// </summary>
    class HotKeys : IKeyboardListener
    {
        private IController mController;

        public IController Controller
        {
            get { return mController; }
        }

        public HotKeys(IController controller)
        {
            mController = controller;
        }

        public void KeyDown(System.Windows.Forms.KeyEventArgs e)
        {
             
        }

        public void KeyUp(System.Windows.Forms.KeyEventArgs e)
        {  
            ICommand cmd = null;
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.A:
                    break;
                case System.Windows.Forms.Keys.Add:
                    break;
                case System.Windows.Forms.Keys.Alt:
                    break;
                
                case System.Windows.Forms.Keys.Attn:
                    break;
                case System.Windows.Forms.Keys.B:
                    break;
                case System.Windows.Forms.Keys.Back:
                    break;
                case System.Windows.Forms.Keys.BrowserBack:
                    break;
                case System.Windows.Forms.Keys.BrowserFavorites:
                    break;
                case System.Windows.Forms.Keys.BrowserForward:
                    break;
                case System.Windows.Forms.Keys.BrowserHome:
                    break;
                case System.Windows.Forms.Keys.BrowserRefresh:
                    break;
                case System.Windows.Forms.Keys.BrowserSearch:
                    break;
                case System.Windows.Forms.Keys.BrowserStop:
                    break;
                case System.Windows.Forms.Keys.C:
                    if (e.Control)
                    {
                        this.Controller.ActivateTool("Copy Tool");
                    }
                    break;
                case System.Windows.Forms.Keys.Cancel:
                    break;
                case System.Windows.Forms.Keys.Capital:
                    break;
              
                case System.Windows.Forms.Keys.Clear:
                    break;
                case System.Windows.Forms.Keys.Control:
                    break;
                case System.Windows.Forms.Keys.ControlKey:
                    break;
                case System.Windows.Forms.Keys.Crsel:
                    break;
                case System.Windows.Forms.Keys.D:
                    break;
                case System.Windows.Forms.Keys.D0:
                    break;
                case System.Windows.Forms.Keys.D1:
                    break;
                case System.Windows.Forms.Keys.D2:
                    break;
                case System.Windows.Forms.Keys.D3:
                    break;
                case System.Windows.Forms.Keys.D4:
                    break;
                case System.Windows.Forms.Keys.D5:
                    break;
                case System.Windows.Forms.Keys.D6:
                    break;
                case System.Windows.Forms.Keys.D7:
                    break;
                case System.Windows.Forms.Keys.D8:
                    break;
                case System.Windows.Forms.Keys.D9:
                    break;
                case System.Windows.Forms.Keys.Decimal:
                    break;
                case System.Windows.Forms.Keys.Delete:
                    cmd = new DeleteCommand(this.mController, Selection.SelectedItems.Copy());
                    this.Controller.UndoManager.AddUndoCommand(cmd);
                    cmd.Redo();
                    break;
                case System.Windows.Forms.Keys.Divide:
                    break;
                case System.Windows.Forms.Keys.Down:
                    break;
                case System.Windows.Forms.Keys.E:
                    break;
                case System.Windows.Forms.Keys.End:
                    break;
                case System.Windows.Forms.Keys.Enter:
                    break;
                case System.Windows.Forms.Keys.EraseEof:
                    break;
                case System.Windows.Forms.Keys.Escape:
                    
                    break;
                case System.Windows.Forms.Keys.Execute:
                    break;
              
                case System.Windows.Forms.Keys.F:
                    break;
                case System.Windows.Forms.Keys.F1:
                    break;
                case System.Windows.Forms.Keys.F10:
                    break;
                case System.Windows.Forms.Keys.F11:
                    break;
                case System.Windows.Forms.Keys.F12:
                    break;
                case System.Windows.Forms.Keys.F13:
                    break;
                case System.Windows.Forms.Keys.F14:
                    break;
                case System.Windows.Forms.Keys.F15:
                    break;
                case System.Windows.Forms.Keys.F16:
                    break;
                case System.Windows.Forms.Keys.F17:
                    break;
                case System.Windows.Forms.Keys.F18:
                    break;
                case System.Windows.Forms.Keys.F19:
                    break;
                case System.Windows.Forms.Keys.F2:
                    break;
                case System.Windows.Forms.Keys.F20:
                    break;
                case System.Windows.Forms.Keys.F21:
                    break;
                case System.Windows.Forms.Keys.F22:
                    break;
                case System.Windows.Forms.Keys.F23:
                    break;
                case System.Windows.Forms.Keys.F24:
                    break;
                case System.Windows.Forms.Keys.F3:
                    break;
                case System.Windows.Forms.Keys.F4:
                    break;
                case System.Windows.Forms.Keys.F5:
                    break;
                case System.Windows.Forms.Keys.F6:
                    break;
                case System.Windows.Forms.Keys.F7:
                    break;
                case System.Windows.Forms.Keys.F8:
                    break;
                case System.Windows.Forms.Keys.F9:
                    break;
                case System.Windows.Forms.Keys.FinalMode:
                    break;
                case System.Windows.Forms.Keys.G:
                    break;
                case System.Windows.Forms.Keys.H:
                    break;
                
              
                
                case System.Windows.Forms.Keys.Help:
                    break;
                case System.Windows.Forms.Keys.Home:
                    break;
                case System.Windows.Forms.Keys.I:
                    break;
                
                case System.Windows.Forms.Keys.Insert:
                    break;
                case System.Windows.Forms.Keys.J:
                    break;
                
                case System.Windows.Forms.Keys.K:
                    break;
           
                case System.Windows.Forms.Keys.KeyCode:
                    break;
                case System.Windows.Forms.Keys.L:
                    break;
                case System.Windows.Forms.Keys.LButton:
                    break;
                case System.Windows.Forms.Keys.LControlKey:
                    break;
                case System.Windows.Forms.Keys.LMenu:
                    break;
                case System.Windows.Forms.Keys.LShiftKey:
                    break;
                case System.Windows.Forms.Keys.LWin:
                    break;
                
                case System.Windows.Forms.Keys.Left:
                    break;
                case System.Windows.Forms.Keys.LineFeed:
                    break;
                case System.Windows.Forms.Keys.M:
                    break;
                case System.Windows.Forms.Keys.MButton:
                    break;
                
                case System.Windows.Forms.Keys.Menu:
                    break;
                case System.Windows.Forms.Keys.Modifiers:
                    break;
                case System.Windows.Forms.Keys.Multiply:
                    break;
                case System.Windows.Forms.Keys.N:
                    if (e.Control)
                        this.Controller.ParentControl.NewDocument();
                    break;
                case System.Windows.Forms.Keys.Next:
                    break;
                case System.Windows.Forms.Keys.NoName:
                    break;
                case System.Windows.Forms.Keys.None:
                    break;
                case System.Windows.Forms.Keys.NumLock:
                    break;
                case System.Windows.Forms.Keys.NumPad0:
                    break;
                case System.Windows.Forms.Keys.NumPad1:

                    break;
                case System.Windows.Forms.Keys.NumPad2:
                    break;
                case System.Windows.Forms.Keys.NumPad3:
                    break;
                case System.Windows.Forms.Keys.NumPad4:
                    break;
                case System.Windows.Forms.Keys.NumPad5:
                    break;
                case System.Windows.Forms.Keys.NumPad6:
                    break;
                case System.Windows.Forms.Keys.NumPad7:
                    break;
                case System.Windows.Forms.Keys.NumPad8:
                    break;
                case System.Windows.Forms.Keys.NumPad9:
                    break;
                case System.Windows.Forms.Keys.O:
                    this.Controller.ParentControl.Open(@"c:\temp\test.netron");
                    break;
                
               
                case System.Windows.Forms.Keys.Oemcomma:
                    break;
                case System.Windows.Forms.Keys.Oemplus:
                    break;
                
                case System.Windows.Forms.Keys.P:
                   
                    break;
              
                case System.Windows.Forms.Keys.Packet:
                    break;
               
                case System.Windows.Forms.Keys.PageUp:
                    break;
                case System.Windows.Forms.Keys.Pause:
                    break;
                case System.Windows.Forms.Keys.Play:
                    break;
                case System.Windows.Forms.Keys.Print:
                    break;
                case System.Windows.Forms.Keys.PrintScreen:
                    break;
                
                case System.Windows.Forms.Keys.ProcessKey:
                    break;
                case System.Windows.Forms.Keys.Q:
                    break;
                case System.Windows.Forms.Keys.R:
                    break;
                case System.Windows.Forms.Keys.RButton:
                    break;
                case System.Windows.Forms.Keys.RControlKey:
                    break;
                case System.Windows.Forms.Keys.RMenu:
                    break;
                case System.Windows.Forms.Keys.RShiftKey:
                    break;
                case System.Windows.Forms.Keys.RWin:
                    break;
               
                case System.Windows.Forms.Keys.Right:
                    break;
                case System.Windows.Forms.Keys.S:
                    this.Controller.ParentControl.SaveAs(@"c:\temp\test.netron");
                    break;
                case System.Windows.Forms.Keys.Scroll:
                    break;
                case System.Windows.Forms.Keys.Select:
                    break;
                case System.Windows.Forms.Keys.SelectMedia:
                    break;
                case System.Windows.Forms.Keys.Separator:
                    break;
                case System.Windows.Forms.Keys.Shift:
                    break;
                case System.Windows.Forms.Keys.ShiftKey:
                    break;
                case System.Windows.Forms.Keys.Sleep:
                    break;
               
                case System.Windows.Forms.Keys.Space:
                    break;
                case System.Windows.Forms.Keys.Subtract:
                    break;
                case System.Windows.Forms.Keys.T:
                    break;
                case System.Windows.Forms.Keys.Tab:
                    break;
                case System.Windows.Forms.Keys.U:
                    break;
                case System.Windows.Forms.Keys.Up:
                    break;
                case System.Windows.Forms.Keys.V:
                    if (e.Control)
                    {
                        this.Controller.ActivateTool("Paste Tool");
                    }
                    break;
                case System.Windows.Forms.Keys.VolumeDown:
                    break;
                case System.Windows.Forms.Keys.VolumeMute:
                    break;
                case System.Windows.Forms.Keys.VolumeUp:
                    break;
                case System.Windows.Forms.Keys.W:
                    break;
                case System.Windows.Forms.Keys.X:
                    break;
                
                case System.Windows.Forms.Keys.Y:
                    break;
                case System.Windows.Forms.Keys.Z:
                    break;
                case System.Windows.Forms.Keys.Zoom:
                    break;
                default:
                    break;
            }
        }

        public void KeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            
        }
    }
}
