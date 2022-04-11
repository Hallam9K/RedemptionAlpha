using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.StructureHelper.Items
{
    class CopyWand : ModItem
    {
        public bool SecondPoint;
        public Point16 TopLeft;
        public int Width;
        public int Height;

        public override bool AltFunctionUse(Player player) => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Structure Wand");
            Tooltip.SetDefault("Select 2 points in the world, then right click to save a structure");
        }

        public override void SetDefaults()
        {
            Item.useStyle = 1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.rare = 1;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && !SecondPoint && TopLeft != default)
                Saver.SaveToFile(new Rectangle(TopLeft.X, TopLeft.Y, Width, Height));

            else if (!SecondPoint)
            {
                TopLeft = (Main.MouseWorld / 16).ToPoint16();
                Width = 0;
                Height = 0;
                Main.NewText("Select Second Point");
                SecondPoint = true;
            }

            else
            {
                Point16 bottomRight = (Main.MouseWorld / 16).ToPoint16();
                Width = bottomRight.X - TopLeft.X - 1;
                Height = bottomRight.Y - TopLeft.Y - 1;
                Main.NewText("Ready to save! Right click to save this structure...");
                SecondPoint = false;
            }

            return true;
        }  
    }
}
