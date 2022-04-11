using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Redemption.StructureHelper.Items;
using System.IO;

namespace Redemption.StructureHelper.GUI
{
	class ManualGeneratorMenu : UIState
	{
		public static bool Visible => TestWand.UIVisible;
        public static StructureEntry selected;
        public static bool ignoreNulls = false;

        public static bool multiMode = false;
        public static int multiIndex;

        public static UIList structureElements = new UIList();
        public static UIScrollbar scrollBar = new UIScrollbar();

        public static UIImageButton refreshButton = new UIImageButton(ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/Refresh"));
        public static UIImageButton ignoreButton = new UIImageButton(ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/Null"));
        public static UIImageButton closeButton = new UIImageButton(ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/Cross"));

        public static void LoadStructures()
		{
            structureElements.Clear();
            selected = null;

            var folderPath = ModLoader.ModPath.Replace("Mods", "SavedStructures");
            Directory.CreateDirectory(folderPath);

            var filePaths = Directory.GetFiles(folderPath);

            foreach(string path in filePaths)
			{
                var name = path.Replace(folderPath + Path.DirectorySeparatorChar, "");
                structureElements.Add(new StructureEntry(name, path));
            }
        }

		public override void OnInitialize()
		{
            LoadStructures();
            SetDims(structureElements, -200, 0.5f, 0, 0.1f, 400, 0, 0, 0.8f);
            SetDims(scrollBar, 232, 0.5f, 0, 0.1f, 32, 0, 0, 0.8f);
            structureElements.SetScrollbar(scrollBar);
            Append(structureElements);
            Append(scrollBar);

            SetDims(refreshButton, -200, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			refreshButton.OnClick += RefreshButton_OnClick;
            Append(refreshButton);

            SetDims(ignoreButton, -150, 0.5f, -50, 0.1f, 32, 0, 32, 0);
            ignoreButton.OnClick += IgnoreButton_OnClick;
            Append(ignoreButton);

            SetDims(closeButton, 200 - 32, 0.5f, -50, 0.1f, 32, 0, 32, 0);
			closeButton.OnClick += CloseButton_OnClick;
            Append(closeButton);
        }

		private void CloseButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
            TestWand.UIVisible = false;
        }

		private void IgnoreButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
            ignoreNulls = !ignoreNulls;
            Main.isMouseLeftConsumedByUI = true;
        }

		private void RefreshButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
            LoadStructures();
            Main.isMouseLeftConsumedByUI = true;
        }

		public override void Update(GameTime gameTime)
		{
            Recalculate();

            if (Main.playerInventory)
                TestWand.UIVisible = false;

            if(ignoreButton.IsMouseHovering)
			{
                Main.hoverItemName = $"Place with null tiles: {ignoreNulls}";
                Main.LocalPlayer.mouseInterface = true;
            }

            if(refreshButton.IsMouseHovering)
			{
                Main.hoverItemName = "Reload structure folder";
                Main.LocalPlayer.mouseInterface = true;
            }

            if(closeButton.IsMouseHovering)
			{
                Main.hoverItemName = "Close";
                Main.LocalPlayer.mouseInterface = true;
            }

			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
            var color = new Color(49, 84, 141);
            DrawBox(spriteBatch, ignoreButton.GetDimensions().ToRectangle(), ignoreButton.IsMouseHovering ? color : color * 0.8f);
            DrawBox(spriteBatch, refreshButton.GetDimensions().ToRectangle(), refreshButton.IsMouseHovering ? color : color * 0.8f);
            DrawBox(spriteBatch, closeButton.GetDimensions().ToRectangle(), closeButton.IsMouseHovering ? color : color * 0.8f);

            var rect = structureElements.GetDimensions().ToRectangle();
            rect.Inflate(30, 10);
            DrawBox(spriteBatch, rect, new Color(20, 40, 60) * 0.8f);

            base.Draw(spriteBatch);

            if (!ignoreNulls)
            {
                var tex = ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/Cross").Value;
                spriteBatch.Draw(tex, ignoreButton.GetDimensions().ToRectangle(), ignoreButton.IsMouseHovering ? Color.White : Color.White * 0.5f);
            }
        }

		public static void SetDims(UIElement ele, int x, int y, int w, int h)
		{
            ele.Left.Set(x, 0);
            ele.Top.Set(y, 0);
            ele.Width.Set(w, 0);
            ele.Height.Set(h, 0);
		}

        public static void SetDims(UIElement ele, int x, float xp, int y, float yp, int w, float wp, int h, float hp)
        {
            ele.Left.Set(x, xp);
            ele.Top.Set(y, yp);
            ele.Width.Set(w, wp);
            ele.Height.Set(h, hp);
        }

        public static void DrawBox(SpriteBatch sb, Rectangle target, Color color = default)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Redemption/StructureHelper/GUI/Box").Value;

            if(color == default) 
                color = new Color(49, 84, 141) * 0.8f;

            Rectangle sourceCorner = new Rectangle(0, 0, 6, 6);
            Rectangle sourceEdge = new Rectangle(6, 0, 4, 6);
            Rectangle sourceCenter = new Rectangle(6, 6, 4, 4);

            Rectangle inner = target;
            inner.Inflate(-4, -4);

            sb.Draw(tex, inner, sourceCenter, color);

            sb.Draw(tex, new Rectangle(target.X + 2, target.Y, target.Width - 4, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
            sb.Draw(tex, new Rectangle(target.X, target.Y - 2 + target.Height, target.Height - 4, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            sb.Draw(tex, new Rectangle(target.X - 2 + target.Width, target.Y + target.Height, target.Width - 4, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
            sb.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 2, target.Height - 4, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            sb.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            sb.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            sb.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            sb.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
        }
    }

    class StructureEntry : UIElement
	{
        public string Name = "";
        public string Path;

        bool active => ManualGeneratorMenu.selected == this;

        public StructureEntry(string name, string path)
		{
            Name = name;
            Path = path;

            Width.Set(400, 0);
            Height.Set(32, 0);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            Rectangle mainBox = new Rectangle((int)pos.X, (int)pos.Y, 400, 32);

            Color color = Color.Gray;

            if (IsMouseHovering) 
                color = Color.White;

            if (active)
                color = Color.Yellow;

            ManualGeneratorMenu.DrawBox(spriteBatch, mainBox, IsMouseHovering || active ? new Color(49, 84, 141) : new Color(49, 84, 141) * 0.6f);
            Utils.DrawBorderString(spriteBatch, Name, mainBox.Center() + Vector2.UnitY * 4, color, 0.8f, 0.5f, 0.5f);

            base.Draw(spriteBatch);

            if(!active)
			{
                Height.Set(32, 0);
                RemoveAllChildren();
            }
		}

        public override void Click(UIMouseEvent evt)
        {
            ManualGeneratorMenu.selected = this;
            ManualGeneratorMenu.multiIndex = 0;

            if (!Generator.StructureDataCache.ContainsKey(Path))
                Generator.LoadFile(Path, Redemption.Instance, true);

            if(Generator.StructureDataCache[Path].ContainsKey("Structures"))
			{
                ManualGeneratorMenu.multiMode = true;

                var count = Generator.StructureDataCache[Path].Get<List<TagCompound>>("Structures").Count;
                Height.Set(36 + 36 * count, 0);

                UIList list = new UIList();

                for (int k = 0; k < count; k++)
                {
                    list.Add(new MultiSelectionEntry(k));
                }

                list.Width.Set(300, 0);
                list.Height.Set(36 * count, 0);
                list.Left.Set(50, 0);
                list.Top.Set(36, 0);
                Append(list);
            }
			else
                ManualGeneratorMenu.multiMode = false;
        }
	}

    class MultiSelectionEntry : UIElement
	{
        public int value;

        bool active => ManualGeneratorMenu.multiIndex == value;

        public MultiSelectionEntry(int index)
        {
            value = index;
            Width.Set(50, 0);
            Height.Set(32, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            Vector2 pos = GetDimensions().ToRectangle().Center();
            Color color = Color.Gray;

            if (IsMouseHovering)
                color = Color.White;

            if (active)
                color = Color.Yellow;

            ManualGeneratorMenu.DrawBox(spriteBatch, GetDimensions().ToRectangle(), IsMouseHovering || active ? new Color(49, 84, 141) : new Color(49, 84, 141) * 0.6f);
            Utils.DrawBorderString(spriteBatch, value.ToString(), pos + Vector2.UnitY * 4, color, 0.8f, 0.5f, 0.5f);

            base.Draw(spriteBatch);
        }

        public override void Click(UIMouseEvent evt)
        {
            ManualGeneratorMenu.multiIndex = value;
        }
    }
}
