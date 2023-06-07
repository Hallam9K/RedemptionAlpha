using System;
using Terraria.ModLoader;
using Terraria;
using Redemption.Globals.World;
using Terraria.ID;
using Redemption.Globals;

namespace Redemption
{
    partial class Redemption
    {
        public override object Call(params object[] args)
        {
            try
            {
                string code = args[0].ToString();

                switch (code)
                {
                    case "AbominationnClearEvents":
                        bool eventOccurring = FowlMorningWorld.FowlMorningActive;
                        bool canClearEvents = Convert.ToBoolean(args[1]);
                        if (eventOccurring && canClearEvents)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FowlMorningWorld.FowlMorningActive = false;
                                FowlMorningWorld.ChickPoints = 0;
                                FowlMorningWorld.ChickWave = 0;

                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }

                            FowlMorningWorld.SendInfoPacket();
                        }
                        return eventOccurring;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Call Error: " + e.StackTrace + e.Message);
            }

            if (args is null)
                throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
            if (args.Length == 0)
                throw new ArgumentException("Arguments cannot be empty!");

            if (args[0] is string content)
            {
                switch (content)
                {
                    case "addElementNPC":
                        if (args[1] is not int elementID)
                            throw new Exception($"Expected an argument of type int when setting element ID, but got type {args[1].GetType().Name} instead.");
                        if (args[2] is not int npcID)
                            throw new Exception($"Expected an argument of type int when setting NPC type, but got type {args[2].GetType().Name} instead.");
                        return elementID switch
                        {
                            1 => ElementID.NPCArcane[npcID] = true,
                            2 => ElementID.NPCFire[npcID] = true,
                            3 => ElementID.NPCWater[npcID] = true,
                            4 => ElementID.NPCIce[npcID] = true,
                            5 => ElementID.NPCEarth[npcID] = true,
                            6 => ElementID.NPCWind[npcID] = true,
                            7 => ElementID.NPCThunder[npcID] = true,
                            8 => ElementID.NPCHoly[npcID] = true,
                            9 => ElementID.NPCShadow[npcID] = true,
                            10 => ElementID.NPCNature[npcID] = true,
                            11 => ElementID.NPCPoison[npcID] = true,
                            12 => ElementID.NPCBlood[npcID] = true,
                            13 => ElementID.NPCPsychic[npcID] = true,
                            14 => ElementID.NPCCelestial[npcID] = true,
                            15 => ElementID.NPCExplosive[npcID] = true,
                            _ => false,
                        };
                    case "addElementItem":
                        if (args[1] is not int elementID2)
                            throw new Exception($"Expected an argument of type int when setting element ID, but got type {args[1].GetType().Name} instead.");
                        if (args[2] is not int itemID)
                            throw new Exception($"Expected an argument of type int when setting Item type, but got type {args[2].GetType().Name} instead.");
                        return elementID2 switch
                        {
                            1 => ElementID.ItemArcane[itemID] = true,
                            2 => ElementID.ItemFire[itemID] = true,
                            3 => ElementID.ItemWater[itemID] = true,
                            4 => ElementID.ItemIce[itemID] = true,
                            5 => ElementID.ItemEarth[itemID] = true,
                            6 => ElementID.ItemWind[itemID] = true,
                            7 => ElementID.ItemThunder[itemID] = true,
                            8 => ElementID.ItemHoly[itemID] = true,
                            9 => ElementID.ItemShadow[itemID] = true,
                            10 => ElementID.ItemNature[itemID] = true,
                            11 => ElementID.ItemPoison[itemID] = true,
                            12 => ElementID.ItemBlood[itemID] = true,
                            13 => ElementID.ItemPsychic[itemID] = true,
                            14 => ElementID.ItemCelestial[itemID] = true,
                            15 => ElementID.ItemExplosive[itemID] = true,
                            _ => false,
                        };
                    case "addElementProj":
                        if (args[1] is not int elementID3)
                            throw new Exception($"Expected an argument of type int when setting element ID, but got type {args[1].GetType().Name} instead.");
                        if (args[2] is not int projID)
                            throw new Exception($"Expected an argument of type int when setting Projectile type, but got type {args[2].GetType().Name} instead.");
                        return elementID3 switch
                        {
                            1 => ElementID.ProjArcane[projID] = true,
                            2 => ElementID.ProjFire[projID] = true,
                            3 => ElementID.ProjWater[projID] = true,
                            4 => ElementID.ProjIce[projID] = true,
                            5 => ElementID.ProjEarth[projID] = true,
                            6 => ElementID.ProjWind[projID] = true,
                            7 => ElementID.ProjThunder[projID] = true,
                            8 => ElementID.ProjHoly[projID] = true,
                            9 => ElementID.ProjShadow[projID] = true,
                            10 => ElementID.ProjNature[projID] = true,
                            11 => ElementID.ProjPoison[projID] = true,
                            12 => ElementID.ProjBlood[projID] = true,
                            13 => ElementID.ProjPsychic[projID] = true,
                            14 => ElementID.ProjCelestial[projID] = true,
                            15 => ElementID.ProjExplosive[projID] = true,
                            _ => false,
                        };
                    case "addItemToBluntSwing": // Disables automatic Slash Bonus
                        if (args[1] is not int ItemID)
                            throw new Exception($"Expected an argument of type int when setting Projectile type, but got type {args[1].GetType().Name} instead.");
                        ItemLists.BluntSwing.Add(ItemID);
                        break;
                    case "addNPCToElementTypeList":
                        if (args[1] is not string typeString)
                            throw new Exception($"Expected an argument of type string when setting Type Name, but got type {args[1].GetType().Name} instead.");
                        if (args[2] is not int NPCID)
                            throw new Exception($"Expected an argument of type int when setting NPC type, but got type {args[2].GetType().Name} instead.");
                        switch (typeString)
                        {
                            case "Skeleton":
                                NPCLists.Skeleton.Add(NPCID);
                                break;
                            case "SkeletonHumanoid":
                                NPCLists.SkeletonHumanoid.Add(NPCID);
                                break;
                            case "Humanoid": // Doesn't have to include SkeletonHumanoid
                                NPCLists.Humanoid.Add(NPCID);
                                break;
                            case "Undead":
                                NPCLists.Undead.Add(NPCID);
                                break;
                            case "Spirit":
                                NPCLists.Spirit.Add(NPCID);
                                break;
                            case "Plantlike":
                                NPCLists.Plantlike.Add(NPCID);
                                break;
                            case "Demon":
                                NPCLists.Demon.Add(NPCID);
                                break;
                            case "Cold":
                                NPCLists.Cold.Add(NPCID);
                                break;
                            case "Hot":
                                NPCLists.Hot.Add(NPCID);
                                break;
                            case "Wet":
                                NPCLists.Wet.Add(NPCID);
                                break;
                            case "Dragonlike":
                                NPCLists.Dragonlike.Add(NPCID);
                                break;
                            case "Inorganic":
                                NPCLists.Inorganic.Add(NPCID);
                                break;
                            case "Robotic": // Also add these into Inorganic
                                NPCLists.Robotic.Add(NPCID);
                                break;
                            case "Armed":
                                NPCLists.Armed.Add(NPCID);
                                break;
                            case "Hallowed":
                                NPCLists.Hallowed.Add(NPCID);
                                break;
                            case "Dark":
                                NPCLists.Dark.Add(NPCID);
                                break;
                            case "Blood":
                                NPCLists.Blood.Add(NPCID);
                                break;
                            case "Slime":
                                NPCLists.IsSlime.Add(NPCID);
                                break;
                        }
                        break;
                }
            }
            /*
            // In SetStaticDefaults() of ModItem, ModProjectile, or ModNPC
            if (!ModLoader.TryGetMod("Redemption", out var redemption))
                return;

            // For ModItem
            redemption.Call("addElementItem", 13, Type); // Psychic element ID
            // For ModProjectile
            redemption.Call("addElementProj", 4, Type); // Ice element ID
            // For ModNPC
            redemption.Call("addElementNPC", 6, Type); // Wind element ID
            
            1 = Arcane
            2 = Fire
            3 = Water
            4 = Ice
            5 = Earth
            6 = Wind
            7 = Thunder
            8 = Holy
            9 = Shadow
            10 = Nature
            11 = Poison
            12 = Blood
            13 = Psychic
            14 = Celestial
            15 = Explosive
            */
            return false;
        }
    }
}