using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items
{
    public class SpriteSpawner : ModItem
    {
        public int x;
        public int y;
        public int divisions;
        public Vector2 offset = new(0f, 0f);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sprite Spawner");
            Tooltip.SetDefault("Spawns sprites at the cursor and continuously draws them at that location.\n" +
                               "Can be used to test shaders. You should use Edit and Continue to do this.");
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Purple;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                x = 0;
                y = 0;
                Talk("Coordinates cleared.", new Color(218, 70, 70));
                return true;
            }
            x = (int)(Main.MouseWorld.X / 16);
            y = (int)(Main.MouseWorld.Y / 16);
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, new Color(218, 70, 70), null);
            Talk($"Drawing sprites at [{x}, {y}]. Right-click to discard.", new Color(218, 70, 70));
            return true;
        }
        public void Talk(string message, Color color) => Main.NewText(message, color.R, color.G, color.B);
    }
}
