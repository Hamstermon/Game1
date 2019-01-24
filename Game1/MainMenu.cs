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
using Steropes.UI.Platform;
using System.Threading;

namespace Game1
{
    public class Playing : Grid
    {
        public MapWidget mapWidget;
        public TextField trans;

        public Playing(IUIStyle s, Game1 parent, GraphicsDeviceManager man) : base(s)
        {
            mapWidget = new MapWidget(s);
            trans = new TextField(s)
            {
                //Alignment = Alignment.Center,
                
                Visibility = Visibility.Hidden,
                Anchor = AnchoredRect.CreateFixed(0,0,1080,800),
                Color = Color.Black,
                TextColor = Color.White
            };
            this.Add(mapWidget);
            this.Add(trans);
        }

        public void TransitionVisible(bool visible)
        {
            if (visible)
            {
                trans.Visibility = Visibility.Visible;
                mapWidget.Visibility = Visibility.Hidden;
            }
            else
            {
                trans.Visibility = Visibility.Hidden;
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
        int width = 1080;
        int height = 800;
        public MainMenu(IUIStyle s, Game1 parent) : base(s)
        {
            Texture2D background = parent.Content.Load<Texture2D>("characterSpritesheet");
            //txt.Rebase(background, new Rectangle(0, 0, width, height), "");
            var bg = new Image(s)
            {
                //Texture = txt
            };
            var title = new Label(s, "Game Name", Alignment.Center)
            {
                TextColor = Color.Black,
                Anchor = AnchoredRect.CreateFixed(0, 0, width, 80)
            };
            var play = new Button(s, "Play")
            {
                Anchor = AnchoredRect.CreateFixed(5, 80, 100, 60),
                Color = Color.Aquamarine,
                OnActionPerformed = (se, a) =>
                {
                    parent.State = Game1.GameState.Playing;
                }
            };
            this.Add(bg);
            this.Add(title);
            this.Add(play);
        }
    }
}
