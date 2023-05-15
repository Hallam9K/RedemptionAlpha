using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Redemption.NPCs.Soulless;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadesteelLeverTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            RedeTileHelper.CannotMineTileBelow[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<ShadesteelDust>();
            MinPick = 500;
            MineResist = 30f;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Shadesteel Lever");
            AddMapEntry(new Color(110, 111, 135), name);
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = -1;
            player.cursorItemIconText = "Call Lift";
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool CanExplode(int i, int j) => false;

        public override bool RightClick(int i, int j)
        {
            bool activate = false;
            int left = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            if (Main.tile[left, top].TileFrameX % 18 == 0 && Main.tile[left, top].TileFrameY == 0)
            {
                int lift = GetNearestNPC(new Vector2(i * 16, j * 16));
                if (lift > -1)
                {
                    NPC liftNPC = Main.npc[lift];
                    if (liftNPC.ModNPC is ShadestoneLift modLift && (modLift.buttonPressed == 0 || modLift.buttonY >= 14))
                    {
                        if (liftNPC.position.Y < (liftNPC.ai[2] - ((liftNPC.ai[2] / 2) - (liftNPC.ai[3] / 2))) * 16)
                        {
                            if (liftNPC.Center.Y < j * 16)
                            {
                                SoundEngine.PlaySound(CustomSounds.Switch1, liftNPC.position);
                                modLift.buttonPressed = 1;
                                activate = true;
                            }
                        }
                        else
                        {
                            if (liftNPC.Center.Y > (j + 6) * 16)
                            {
                                SoundEngine.PlaySound(CustomSounds.Switch1, liftNPC.position);
                                modLift.buttonPressed = 2;
                                activate = true;
                            }
                        }
                    }
                    else if (liftNPC.ModNPC is ShadestoneLiftBig modLiftBig && (modLiftBig.buttonPressed == 0 || modLiftBig.buttonY >= 14))
                    {
                        if (liftNPC.position.Y < (liftNPC.ai[2] - ((liftNPC.ai[2] / 2) - (liftNPC.ai[3] / 2))) * 16)
                        {
                            if (liftNPC.Center.Y < j * 16)
                            {
                                SoundEngine.PlaySound(CustomSounds.Switch1, liftNPC.position);
                                modLiftBig.buttonPressed = 1;
                                activate = true;
                            }
                        }
                        else
                        {
                            if (liftNPC.Center.Y > (j + 6) * 16)
                            {
                                SoundEngine.PlaySound(CustomSounds.Switch1, liftNPC.position);
                                modLiftBig.buttonPressed = 2;
                                activate = true;
                            }
                        }
                    }
                }
            }
            if (!activate)
                return false;
            if (Main.tile[left, top].TileFrameX == 0)
            {
                for (int x = left; x < left + 2; x++)
                {
                    for (int y = top; y < top + 2; y++)
                    {
                        if (Main.tile[x, y].TileFrameX < 36)
                            Main.tile[x, y].TileFrameX += 36;
                    }
                }
            }
            else
            {
                for (int x = left; x < left + 2; x++)
                {
                    for (int y = top; y < top + 2; y++)
                    {
                        if (Main.tile[x, y].TileFrameX >= 36)
                            Main.tile[x, y].TileFrameX -= 36;
                    }
                }
            }
            return true;
        }
        public static int GetNearestNPC(Vector2 point)
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;

                if (npc.type != ModContent.NPCType<ShadestoneLift>() && npc.type != ModContent.NPCType<ShadestoneLiftBig>())
                    continue;

                if (nearestNPCDist != -1 && !(npc.Distance(point) < nearestNPCDist))
                    continue;

                nearestNPCDist = npc.Distance(point);
                nearestNPC = npc.whoAmI;
            }

            return nearestNPC;
        }
    }
}