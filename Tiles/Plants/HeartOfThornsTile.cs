using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.GameContent.Metadata;
using Redemption.Items.Usable.Summons;

namespace Redemption.Tiles.Plants
{
    public class HeartOfThornsTile : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16, 16
            };
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Heart of Thorns");
            AddMapEntry(new Color(144, 244, 144), name);
            DustType = DustID.GrassBlades;
            HitSound = SoundID.Grass;
            RegisterItemDrop(ModContent.ItemType<HeartOfThorns>());
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        }
        public override bool IsTileDangerous(int i, int j, Player player) => true;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (player.active && !player.dead && dist <= 0.2f)
                player.AddBuff(ModContent.BuffType<EnsnaredDebuff>(), 20);
        }
    }
}
