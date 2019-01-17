using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Squared.Tiled;
using System.Collections.Generic;
using Steropes.UI;
using Steropes.UI.Components;
using Steropes.UI.Input;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.TextWidgets;
using System.Threading;

namespace Game1
{
    public class Playing : Grid
    {
        public MapWidget mapWidget;
        public Grid griddy;
        public Label trans;

        public Playing(IUIStyle s, Game1 parent, GraphicsDeviceManager man) : base(s)
        {
            mapWidget = new MapWidget(s)
            {
                Visibility = Visibility.Hidden
            };
            trans = new Label(s)
            {
                Anchor = AnchoredRect.CreateFull(1),
                Color = Color.Black,
                TextColor = Color.White,
                Visibility = Visibility.Visible
            };
            griddy = new Grid(s)
            {
                Color = Color.Black,
                Visibility = Visibility.Visible,
                Anchor = AnchoredRect.CreateFull(1)
            };
            this.Add(mapWidget);
            this.Add(griddy);
            griddy.Add(trans);
        }

        public void TransitionVisible(bool visible)
        {
            if (visible)
            {
                Console.WriteLine("MAKING TRANSITION SCREEN VISIBLE");
                griddy.Visibility = Visibility.Visible;
                mapWidget.Visibility = Visibility.Hidden;
            }
            else
            {
                Console.WriteLine("MAKING MAP SCREEN VISIBLE");
                griddy.Visibility = Visibility.Hidden;
                mapWidget.Visibility = Visibility.Visible;
            }
        }        
    }

    class OverWorldMenu:Grid
    {
        public OverWorldMenu(IUIStyle s, Game1 parent) : base(s)
        {
            
            var button = new Button(s, "Exit")
            {
                Anchor = AnchoredRect.CreateBottomRightAnchored(),
                Color = Color.Red,
                OnActionPerformed = (se, a) =>
                {
                    parent.State = Game1.GameState.Playing;
                }
            };
            this.AddChildAt(button, 0, 0);
        }
    }

    class MainMenu:Grid
    {
        public MainMenu(IUIStyle s, Game1 parent) : base(s)
        {
            var tf = new TextField(s)
            {
                Text = " Enter Here ",
                Anchor = AnchoredRect.CreateTopAnchored()
            };

            var lab = new Label(s, "Starting Text")
            {
                Anchor = AnchoredRect.CreateCentered()
            };

            var bt = new Button(s, "Test")
            {
                Anchor = AnchoredRect.CreateBottomAnchored(),
                Color = Color.Aquamarine,
                OnActionPerformed = (se, a) =>
                {
                    lab.Text = tf.Text;
                    System.Console.WriteLine("Click");
                }
            };

            var mess = new TextField(s)
            {
                Color = Color.AliceBlue,
                Anchor = AnchoredRect.CreateFixed(150, 20, 150, 150),
                Text = "Hello World!"
            };

            var play = new Button(s, "Play")
            {
                Anchor = AnchoredRect.CreateFixed(0, 0, 100, 60),
                Color = Color.Aquamarine,
                OnActionPerformed = (se, a) =>
                {
                    parent.State = Game1.GameState.Playing;
                }
            };

            this.AddChildAt(tf, 0, 0);
            this.AddChildAt(lab, 0, 5);
            this.AddChildAt(bt, 0, 10);
            this.AddChildAt(mess, 0, 15);
            this.AddChildAt(play, 0, 20);
        }
    }
}
