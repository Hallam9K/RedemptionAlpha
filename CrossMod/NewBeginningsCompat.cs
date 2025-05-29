using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Single;
using Redemption.Items.Armor.Vanity.SkySquire;
using Redemption.Items.Donator.Sneaklone;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Tiles.Furniture.Misc;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Redemption.CrossMod;

internal class NewBeginningsCompat : ModSystem
{
    public static Asset<Texture2D> GetIcon(string name) => Request<Texture2D>("Redemption/CrossMod/NewBeginningsOrigins/" + name);

    public override bool IsLoadingEnabled(Mod mod) => CrossMod.NewBeginnings.Enabled;
    public override void Load()
    {
        var beginnings = CrossMod.NewBeginnings.Instance;

        beginnings.Call("Delay", () =>
        {
            AddSkySquire();
            AddSpiritWalker();
            AddMadman();
            AddFacilityGuard();
        });

        void AddSkySquire()
        {
            object equip = beginnings.Call("EquipData", ItemType<SkySquiresHelm>(), ItemType<SkySquiresTabard>(), ItemType<SkySquiresGreaves>(),
                new int[] { ItemID.CloudinaBottle });
            object misc = beginnings.Call("MiscData", 100, 20, -1, -1, -1, -1, 10, 2);
            object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { }, () => true, (Func<Point16>)FindGoldenGatewaySpawnPoint);
            object result = beginnings.Call("ShortAddOrigin", GetIcon("SkySquire"), "MoRSkySquire",
                "Mods.Redemption.Origins.SkySquire", Array.Empty<(int, int)>(), equip, misc, dele);
        }

        void AddSpiritWalker()
        {
            object equip = beginnings.Call("EquipData", ItemID.None, ItemID.None, ItemID.None, new int[] { ItemType<TrappedSoulBauble>() });
            object misc = beginnings.Call("MiscData", 80, 20, -1, ItemType<CruxCardGathicSkeletons>(), -1, -1, 10, 4);
            object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { }, () => true, (Func<Point16>)FindGathicPortalSpawn,
                (Action<Player>)AddSpiritWalkerAbility);
            object result = beginnings.Call("ShortAddOrigin", GetIcon("SpiritWalker"), "MoRSpiritWalker",
                "Mods.Redemption.Origins.SpiritWalker", new (int, int)[] { (ItemType<EmptyCruxCard>(), 5), (ItemType<LostSoul>(), 50), (ItemID.BoneTorch, 25) }, equip, misc, dele);
        }

        void AddMadman()
        {
            int lanternType = 0;
            if (beginnings.TryFind("DeprivedLantern", out ModItem lantern))
                lanternType = lantern.Type;

            object equip = beginnings.Call("EquipData", ItemType<JollyHelm>(), ItemID.None, ItemID.None,
                new int[] { lanternType });
            object misc = beginnings.Call("MiscData", 60, 20, -1, ItemType<Zweihander>(), ItemType<GraveSteelPickaxe>(), ItemType<GraveSteelBattleaxe>(), 10, 3);
            object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { }, () => true, (Func<Point16>)FindGathicTombSpawn);
            object result = beginnings.Call("ShortAddOrigin", GetIcon("Madman"), "MoRMadman",
                "Mods.Redemption.Origins.Madman", new (int, int)[] { (ItemType<AncientGoldCoin>(), 20) }, equip, misc, dele);
        }

        void AddFacilityGuard()
        {
            object equip = beginnings.Call("EquipData", ItemType<SneakloneHelmet2>(), ItemType<SneakloneSuit>(), ItemType<SneakloneLegs>(),
                new int[] { ItemID.GPS });
            object misc = beginnings.Call("MiscData", 120, 0, -1, ItemType<LegoPistol>(), ItemType<LegoPistol>(), ItemType<LegoPistol>(), 10, 5);
            object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { });
            object result = beginnings.Call("ShortAddOrigin", GetIcon("FacilityGuard"), "MoRFacilityGuard",
                "Mods.Redemption.Origins.FacilityGuard", new (int, int)[] { (ItemType<IOLocator>(), 1), (ItemID.SilverBullet, 3), (ItemID.Chain, 99) }, equip, misc, dele);
        }
    }

    private static Point16 FindSkyIslandSpawnPoint()
    {
        List<Point16> spawns = [];

        for (int x = 200; x < Main.maxTilesX - 200; ++x)
        {
            for (int y = 20; y < Main.worldSurface + 120; ++y)
            {
                Tile tile = Main.tile[x, y];

                if (tile.HasTile && !Main.tile[x, y - 1].HasTile && (tile.TileType is TileID.Cloud or TileID.RainCloud))
                    spawns.Add(new Point16(x, y - 2));
            }
        }

        if (spawns.Count == 0)
            return new Point16(Main.spawnTileX, Main.spawnTileY - 2);

        return WorldGen.genRand.Next([.. spawns]);
    }

    private static Point16 FindGoldenGatewaySpawnPoint()
    {
        Point16 gatewayPos = new(Main.spawnTileX, Main.spawnTileY - 2);
        for (int x = 20; x < Main.maxTilesX - 20; x++)
        {
            for (int y = 20; y < 400; y++)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (!tile.HasTile || tile.TileType != TileType<GoldenGatewayTile>())
                    continue;
                gatewayPos = new Point16(x - 6, y + 1);
                break;
            }
        }

        return new Point16(gatewayPos.X, gatewayPos.Y);
    }

    private void AddSpiritWalkerAbility(Player player)
    {
        player.RedemptionAbility().Spiritwalker = true;
    }

    private static Point16 FindGathicPortalSpawn()
    {
        Point16 gathicPortalPos = new(Main.spawnTileX, Main.spawnTileY - 2);
        for (int x = 20; x < Main.maxTilesX - 20; x++)
        {
            for (int y = 20; y < Main.maxTilesY - 20; y++)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (!tile.HasTile || tile.TileType != TileType<GathuramPortalTile>())
                    continue;
                gathicPortalPos = new Point16(x + 6, y + 9);
                break;
            }
        }

        return new Point16(gathicPortalPos.X, gathicPortalPos.Y);
    }

    private static Point16 FindGathicTombSpawn()
    {
        List<Point16> spawns = [];

        for (int x = 50; x < Main.maxTilesX - 250; ++x)
        {
            for (int y = (int)(Main.maxTilesY * .4f); y < (int)(Main.maxTilesY * .8f); ++y)
            {
                Tile tile = Main.tile[x, y];

                if (tile.HasTile && TileLists.AncientTileArray.Contains(tile.TileType) && TileLists.AncientWallArray.Contains(Main.tile[x, y - 2].WallType))
                {
                    bool clear = true;
                    for (int i = 1; i < 4; i++)
                    {
                        if (Main.tile[x, y - i].HasTile)
                        {
                            clear = false;
                            break;
                        }
                        if (Main.tile[x + 1, y - i].HasTile)
                        {
                            clear = false;
                            break;
                        }
                        if (Main.tile[x - 1, y - i].HasTile)
                        {
                            clear = false;
                            break;
                        }
                    }

                    if (clear)
                        spawns.Add(new Point16(x, y - 2));
                }
            }
        }

        if (spawns.Count == 0)
            return new Point16(Main.spawnTileX, Main.spawnTileY - 2);

        return WorldGen.genRand.Next([.. spawns]);
    }
}