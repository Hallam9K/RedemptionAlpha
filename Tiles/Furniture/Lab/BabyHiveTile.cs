using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.NPCs.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class BabyHiveTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileCut[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<SludgeDust>();
            MinPick = 10;
            MineResist = 5f;
            HitSound = SoundID.NPCHit13;
            AddMapEntry(new Color(54, 193, 59));
            AnimationFrameHeight = 18;
        }
        public override bool IsTileDangerous(int i, int j, Player player) => true;
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!WorldGen.gen && !fail && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = 0; k < Main.rand.Next(1, 3); k++)
                    NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16 + 8, j * 16 + 8, ModContent.NPCType<OozeBlob>());
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 30)
            {
                frameCounter = 0;
                frame++;
                if (frame > 1)
                    frame = 0;
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class BabyHive : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Baby Infection Hive");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<BabyHiveTile>();
        }
    }
}