using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;
using Redemption.Buffs.Debuffs;
using Redemption.Tiles.Natural;

namespace Redemption.Tiles.Ores
{
    public class XenomiteShardTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 300;
            DustType = DustID.GreenTorch;
            ItemDrop = ModContent.ItemType<XenomiteShard>();
            MinPick = 100;
            MineResist = 4f;
            SoundStyle = 27;
            SoundType = SoundID.Item;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Xenomite Shard");
            AddMapEntry(new Color(54, 193, 59), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.1f;
            b = 0.0f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (dist <= 1f)
                player.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 20);
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(70))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>());
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>(), 0, 0, -1, -1);
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}