using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Lore;
using Redemption.Items.Placeable.Furniture.SlayerShip;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.SlayerShip
{
    public class KSBattlestationTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.Width = 8;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(3, 4);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 500;
            MineResist = 30f;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Observatory Station");
            AddMapEntry(new Color(151, 153, 160), name);
            AnimationFrameHeight = 90;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 128, 80, ModContent.ItemType<KSBattlestation>());
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<WallDatalog>();
        }
        public override bool RightClick(int i, int j)
        {
            if (!Main.dedServ)
            {
                Tile tile = Main.tile[i, j];
                switch (tile.TileFrameX / 144)
                {
                    case 0:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Planetary Observations #1\n" +
                            "'I've began to study Epidotra for any signs of instability. A tiny drone was sent down to the very center of the planet,\n" +
                            "which would relay back the temperature, and was made with highly heat-resistant materials. The temperatures were the\n" +
                            "expected results, until the drone was no less than 30kms away from the absolute center point. It suddenly sent nothing.\n" +
                            "It was like it just stopped existing. After more testing, the most logical conclusion was a void at the center of the planet.\n" +
                            "This is concerning. I will test again at a future date to see if the void has changed shape.'");
                        break;
                    case 1:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Planetary Observations #2\n" +
                            "'It has been 2 days after the first test. The results showed proof that the void was bigger than previously,\n" +
                            "if only by millimeters. I had alerted the Demigod of this news but he was unable to think of what to do about it.\n" +
                            "Future tests concluded that the void in the center of the planet was growing extremely slowly, but growing nonetheless.'");
                        break;
                    case 2:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Planetary Observations #3\n" +
                            "'The world will not last another Great Reset. I have calculated the instability and this is the conclusion I have come to.\n" +
                            "I am partially to blame, as I am not even meant to be alive. The Resets were created by the great unknown due to things\n" +
                            "overtime growing unstable. Whether it be living or unliving, to exist beyond intended is a strain upon the universe.\n" +
                            "The other planet, Liden, is a foreign virus caused by the instability, and may be a sign of no return.\n" +
                            "Neither it nor I should exist.\n" +
                            "I was not meant to survive. Nothing was.'");
                        break;
                }
            }
            return true;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.7f;
            g = 0.7f;
            b = 1f;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 1)
                    frame = 0;
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
}