using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Redemption.Items.Placeable.Furniture.ElderWood;

namespace Redemption.Tiles.Furniture.ElderWood
{
    public class ElderWoodSofaTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.addTile(Type);
            TileID.Sets.CanBeSatOnForNPCs[Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            AddMapEntry(new Color(109, 87, 78), Language.GetText("ItemName.Sofa"));
            DustType = -1;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
            => settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconID = ModContent.ItemType<ElderWoodSofa>();
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }
        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) => info.VisualOffset.Y += 2;
    }
}
