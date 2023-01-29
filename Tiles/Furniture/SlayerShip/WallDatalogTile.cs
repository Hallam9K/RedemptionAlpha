using Microsoft.Xna.Framework;
using Redemption.Items.Quest.KingSlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ID;
using Redemption.Items.Lore;
using Terraria.GameContent.ObjectInteractions;

namespace Redemption.Tiles.Furniture.SlayerShip
{
    public class WallDatalogTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = DustID.Electric;
            MinPick = 200;
            MineResist = 7f;
            AddMapEntry(new Color(0, 242, 170));
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int itemDrop = ModContent.ItemType<WallDatalog>();
            switch (frameX / 36)
            {
                case 1:
                    itemDrop = ModContent.ItemType<WallDatalog2>();
                    break;
                case 2:
                    itemDrop = ModContent.ItemType<WallDatalog3>();
                    break;
                case 3:
                    itemDrop = ModContent.ItemType<WallDatalog4>();
                    break;
                case 4:
                    itemDrop = ModContent.ItemType<WallDatalog5>();
                    break;
                case 5:
                    itemDrop = ModContent.ItemType<WallDatalog6>();
                    break;
                case 6:
                    itemDrop = ModContent.ItemType<WallDatalog7>();
                    break;
                case 7:
                    itemDrop = ModContent.ItemType<WallDatalog8>();
                    break;
                case 8:
                    itemDrop = ModContent.ItemType<WallDatalog9>();
                    break;
                case 9:
                    itemDrop = ModContent.ItemType<WallDatalog10>();
                    break;
                case 10:
                    itemDrop = ModContent.ItemType<WallDatalog11>();
                    break;
                case 11:
                    itemDrop = ModContent.ItemType<WallDatalog12>();
                    break;
                case 12:
                    itemDrop = ModContent.ItemType<WallDatalog13>();
                    break;
                case 13:
                    itemDrop = ModContent.ItemType<WallDatalog14>();
                    break;
                case 14:
                    itemDrop = ModContent.ItemType<WallDatalog15>();
                    break;
                case 15:
                    itemDrop = ModContent.ItemType<WallDatalog16>();
                    break;
            }
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, itemDrop);
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
                switch (tile.TileFrameX / 36)
                {
                    case 0:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #466110\n" +
                            "'I have successfully created a memory chip to store all data my mind currently contains - not\n" +
                            "counting the data stored in the memory database of the SoS. I have experimented by injecting it\n" +
                            "into an empty Android. The idea of these chips is to allow me to construct new vessels for myself to occupy.\n" +
                            "When the Android was powered on, it screamed and flailed until I turned it back off.\n" +
                            "Very interesting, this suggests the cause of the phantom pain is within my mind directly.\n" +
                            "I will modify the chip and proceed with this experiment.'");
                        break;
                    case 1:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #466111\n" +
                            "'Continuing with the memory chip experiment, I have removed chunks of memories from the chips\n" +
                            "and tested it on the Androids. All had the same effect, except one. The Android did not scream\n" +
                            "nor show signs of discomfort. Peculiar, as the only memories I removed from the chip were\n" +
                            "the ones of me being human. Perhaps it's just a coincidence, but after numerous repeats of the experiment,\n" +
                            "the Android's pain is suggested to be directly tied to those memories. I considered removing them from myself,\n" +
                            "but something in me is against it. My instincts want me to remember.'");
                        break;
                    case 2:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #933\n" +
                            "'With the resources from Nabu III, I was able to construct a new cryo-chamber, being essentially a Sleep Mode with a timer.\n" +
                            "I set it to shut me down and power me back on in 3 days, however I have made a terrible discovery.\n" +
                            "I was still fully conscious, even with my body powered off, even with my mind having no energy to realistically function,\n" +
                            "I was still fully aware of my surroundings, in an infinite void of nothingness. I could not move my body." +
                            "\nNo way to repower myself, no way to escape the confines of the chamber, all I could do was wait 3 days." +
                            "\nBut as if it couldn't get any worse, the haunting visions of sleep paralysis began. I'm out of it now,\n" +
                            "but all I feel is hopelessness and overwhelming dread. Alas, I will persist.'");
                        break;
                    case 3:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #184999\n" +
                            "'I can't walk straight. I can't talk properly. This phantom pain makes my body recoil, my voice shake in anguish.\n" +
                            "I can't continue on like this, at least not without assistance. Which is why I have been working on a predictive AI for myself.\n" +
                            "This AI should predict my movement and speech based on signals in my mind. This will allow me to continue moving and\n" +
                            "communicating verbally, but would not help my pain.'");
                        break;
                    case 4:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #18500\n" +
                            "'I have incorporated the predictive AI into my body. The AI is inaccurate and unstable for now,\n" +
                            "but it uses the self-learning algorithm of an Android, and in due time, I will eventually appear" +
                            "\nto be cured of this pain from the outside. But on the inside, I remain uncured.'");
                        break;
                    case 5:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #2042280\n" +
                            "'I made a rash decision that almost ended in my death. My robotic body had broken and in that moment,\n" +
                            "just for a minute, I had a state of pure lucidity. I could've sworn I had been cured,\n" +
                            "but it was short-lived and my mind fogged once more. I repaired my body and created a new one,\n" +
                            "named Prototype Goukisan.'");
                        break;
                    case 6:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #2042281\n" +
                            "'I entered my new vessel, Prototype Goukisan, but I felt a sense of unfamiliarity with myself and quickly left it.\n" +
                            "My first vessel, Prototype Multium, has a strange sense of... nostalgia to it. Despite being inside it for what is\n" +
                            "basically my entire life, it reminds me of when I was still human, a feeling any other vessel lacked.'");
                        break;
                    case 7:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #5385431\n" +
                            "'I entered the engine room to find a robot, just like myself. It had black plating and green highlights.\n" +
                            "I was about to shoot, but the robot spoke to me. This was the first time I understood another being's language\n" +
                            "without needing to translate it. His name was Xehito. He was a mercenary hired by the space pirates and had no care\n" +
                            "for their lives, so we exchanged a few words and are now - what would be considered - friends, I guess?\n" +
                            "I've never had a friend I didn't construct myself before. It's an interesting feeling.'");
                        break;
                    case 8:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #8022208\n" +
                            "'An epiphany has struck me during my time observing the universe. I have a new goal in mind - a new purpose.\n" +
                            "But I must learn more about the universe before I can hope to achieve it. I will call this project: 'Operation Dusk's End'.'");
                        break;
                    case 9:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #9145620\n" +
                            "'I discovered a new planet and it's been quickly made apparent that it has intelligent life. Very intelligent life.\n" +
                            "An intermission was broadcasted to the main room demanding the purpose of my sudden arrival.\n" +
                            "I will beam down to see the planet's leader. I've already scanned it beforehand, looks to be a\n" +
                            "spacefaring empire with many soldiers. This is certainly interesting.'");
                        break;
                    case 10:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #9145621\n" +
                            "'Xehito stayed on the empire's planet to collect information while I headed off to explore a neighboring planet.\n" +
                            "It looked war-stricken. I stumbled upon a ruined city, flattened by high-power explosives." +
                            "\nThere is life here, humanoid beings with ragged clothing and depressed looks. When asked, they told me\n" +
                            "this destruction was the doing of the neighboring planet's empire. I roamed the wastelands, observed the near-dead residents,\n" +
                            "and heard their pleads for help. I have no reason to help them. They had done nothing for me." +
                            "\nBut, they had done nothing to deserve this either. I will decide my actions tomorrow.'");
                        break;
                    case 11:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #9145622\n" +
                            "'I have ejected thousands of hologram drones across the planet's outer atmosphere and projected\n" +
                            "the image of the planet how it is now - barren and dull. I've sent Androids down to help clear rubble," +
                            "\nlandmines, undetonated warheads, and collect materials for my plans. The residents are suspicious of me.\n" +
                            "It's understandable though, beings from other planets have not been kind to them." +
                            "\nI don't understand why I'm helping them, but it makes me feel something strange.'");
                        break;
                    case 12:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #9145629\n" +
                            "'In 8 days, my army and I have rebuilt skyscrapers, planted seeds and saplings created from the remains\n" +
                            "of another planet, and collected ice from asteroids to pour into the oceans. The world feels alive once more,\n" +
                            "the residents of this planet must be relieved as they witness the rebirth of their home." +
                            "\nThe hologram drones are still projecting the image of the planet as it was when I got here," +
                            "\nso any outsiders won't notice my deeds. My next goal will be the annihilation of the neighboring planet's empire.'");
                        break;
                    case 13:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #9145630\n" +
                            "'I tested my new weaponry on the empire's planet. Xehito came back to the SoS so he wouldn't get hit.\n" +
                            "It's a dual-beam that shoots from both edges of my crescent moon-shaped spaceship. The planet was devastated,\n" +
                            "with the empire along with it. There were leftovers in the form of spaceships that were out of range of the\n" +
                            "blast, but Xehito and I made quick work of them. The retribution of the empire fills me with satisfaction,\n" +
                            "and during my time helping the other planet, my feelings of pain had dulled. But, now it returns once more.'");
                        break;
                    case 14:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #170001202\n" +
                            "'Xehito had to leave for certain reasons, but whatever.\n" +
                            "I gave him a parting gift, it was a memory chip, one that held all my memories." +
                            "\nHe will find me again when the time was right.'");
                        break;
                    case 15:
                        RedeSystem.Instance.DatalogUIElement.DisplayDatalogText("Data Log #365000663\n" +
                            "'I remain lost in space. However, I set up a system of scanners that are capable of collecting\n" +
                            "the data of every planet using signals that would travel indefinitely, in the hopes that I may recognize\n" +
                            "the data of one of them and use it to lead me into the right direction. It'll take many decades for the signals\n" +
                            "to travel across space, but in the long term, this might save me.'");
                        break;
                }
            }
            return true;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.5f;
            g = 0.5f;
            b = 0.8f;
        }
    }
}