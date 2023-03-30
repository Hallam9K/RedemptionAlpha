using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Tiles.Tiles;
using Redemption.Base;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Redemption.WorldGeneration;

namespace Redemption.Items.Usable
{
    public class RoomsCreator : ModItem
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Room Layout Generator");
            /* Tooltip.SetDefault("Creates a bunch of 25x25 room layouts\n" +
                "WILL DESTROY TILES IN A LARGE AREA"); */
		}
		
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 45;
            Item.useTime = 45;
        }

		public override bool? UseItem(Player player)
        {
            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(0, 0, 255)] = ModContent.TileType<GathicStoneTile>(),
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Misc/RoomLayouts", AssetRequestMode.ImmediateLoad).Value;
            Point origin = player.Center.ToPoint();
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X / 16, origin.Y / 16, true, true);
            });
            return true;
		}
	}
}