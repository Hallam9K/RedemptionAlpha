using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Usable;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public abstract class HangingTiedTileBase : ModTile
    {
        public override string Texture => "Redemption/Tiles/Furniture/Misc/HangingTiedTile";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            DustType = DustID.Bone;
            HitSound = CustomSounds.BoneHit;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            RegisterItemDrop(ModContent.ItemType<OldTophat>());

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(81, 81, 81), name);
        }
    }
    public class HangingTiedTileFake : HangingTiedTileBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<OldTophat>(), Type, 0);
        }
    }
    public class HangingTiedTile : HangingTiedTileBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            TileObjectData.GetTileData(Type, 0).LavaDeath = false;
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            if (RedeTileHelper.CanDeadRing(player))
                player.cursorItemIconID = ModContent.ItemType<DeadRinger>();
            else
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (RedeTileHelper.CanDeadRing(player))
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Bell, new Vector2(i, j) * 16);

                for (int n = 0; n < 25; n++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(i * 16, j * 16), 2, 2, DustID.DungeonSpirit, 0f, 0f, 100, default, 2);
                    Main.dust[dustIndex].velocity *= 2f;
                    Main.dust[dustIndex].noGravity = true;
                }
                DustHelper.DrawDustImage(new Vector2(i * 16, j * 16), DustID.DungeonSpirit, 0.5f, "Redemption/Effects/DustImages/DeadRingerDust", 2, true, 0);

                SoundEngine.PlaySound(SoundID.Item74, new Vector2(i * 16, j * 16));
                WorldGen.KillTile(i, j);
            }
            return true;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            RedeTileHelper.DrawSpiritFlare(spriteBatch, i, j, 0, 1.5f, 1.5f);
            return true;
        }
    }
}