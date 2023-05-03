using Microsoft.Xna.Framework;
using Redemption.Tiles.Furniture.SlayerShip;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Lore
{
    public class WallDatalog : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Data Log #466110");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 0);
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = 1;
            Item.value = 0;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I have successfully created a memory chip to store all data my mind currently" +
                    "\ncontains - not counting the data stored in the memory database of the SoS. I have experimented\n" +
                    "by injecting it into an empty Android. The idea of these chips is to allow me to construct new vessels\n" +
                    "for myself to occupy. When the Android was powered on, it screamed and flailed until I turned it back off.\n" +
                    "Very interesting, this suggests the cause of the phantom pain is within my mind directly.\n" +
                    "I will modify the chip and proceed with this experiment.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
    public class WallDatalog2 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #466111");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 1);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Continuing with this memory chip experiment, I have removed chunks of memories from the chips\n" +
                    "and tested it on the Androids. All had the same effect, except one. The Android did not scream\n" +
                    "nor show signs of discomfort. Peculiar, for the only memories that I removed from the chip were the ones of me being human.\n" +
                    "Perhaps it's just a coincidence, but after numerous repeats of the experiment, the Android's pain\n" +
                    "is suggested to be directly tied to those memories. I considered removing those memories from myself,\n" +
                    "but something in me is against it. My instincts want me to remember.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog3 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #933");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 2);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'With the resources from Nabu III, I was able to construct a new cryo-chamber that was essentially a Sleep Mode with a timer.\n" +
                    "I set it to shut me down and power me back on in 3 days, however I have made a terrible discovery.\n" +
                    "I was still fully conscious, even with my body powered off, even with my mind having no energy to realistically function,\n" +
                    "I was still fully aware of my surroundings, in an infinite void of nothingness. I could not move my body." +
                    "\nNo way to repower myself, no way to escape the confines of the chamber, all I could do was wait 3 days." +
                    "\nBut as if it couldn't get any worse, the haunting visions of sleep paralysis began. I'm out of it now,\n" +
                    "but all I feel is hopelessness and overwhelming dread. Alas, I will persist.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog4 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #184999");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 3);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I can't walk straight. I can't talk properly. This phantom pain makes my body recoil, my voice shake in anguish.\n" +
                    "I can't continue on like this, at least not without assistance. Which is why I have been working on a predictive AI for myself.\n" +
                    "This AI should predict my movement and speech based on signals in my mind. This will allow me to continue moving and\n" +
                    "communicating verbally, but would not help my pain.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog5 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #18500");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 4);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I have incorporated the predictive AI into my body. The AI is inaccurate and unstable for now,\n" +
                    "but it uses the self-learning algorithm of an Android, and in due time, I will eventually appear" +
                    "\nto be cured of this pain from the outside. But on the inside, I remain uncured.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog6 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #2042280");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 5);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I made a rash decision that almost ended in my death. My robotic body had broken and in that moment,\n" +
                    "just for a minute, I had a state of pure lucidity. I could've sworn I had been cured,\n" +
                    "but it was short-lived and my mind fogged once more. I repaired my body and created a new one, named Prototype Goukisan.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog7 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #2042281");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 6);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I entered my new vessel, Prototype Goukisan, but I felt a sense of unfamiliarity with myself and quickly left it.\n" +
                    "My first vessel, Prototype Multium, has a strange sense of... nostalgia to it. Despite being inside it for what is\n" +
                    "basically my entire life, it reminds me of when I was still human, a feeling any other vessel lacked.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog8 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #5385431");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 7);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I entered the engine room to find a robot, just like myself. It had black plating and green highlights.\n" +
                    "I was about to shoot, but the robot spoke to me. This was the first time I understood another being's language\n" +
                    "without needing to translate it. His name was Xehito. He was a mercenary hired by the space pirates and had no care\n" +
                    "for their lives, so we exchanged a few words and are now - what would be considered - friends, I guess?\n" +
                    "I've never had a friend I didn't construct myself before. It's an interesting feeling.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog9 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #8022208");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 8);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'An epiphany has struck me during my time observing the universe. I have a new goal in mind - a new purpose.\n" +
                    "But I must learn more about the universe before I can hope to achieve it. I will call this project: 'Operation Dusk's End'.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog10 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #9145620");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 9);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I discovered a new planet and it's been quickly made apparent that it has intelligent life. Very intelligent life.\n" +
                    "An intermission was broadcasted to the main room demanding the purpose of my sudden arrival.\n" +
                    "I will beam down to see the planet's leader. I've already scanned it beforehand, looks to be a\n" +
                    "spacefaring empire with many soldiers. This is certainly interesting.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog11 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #9145621");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 10);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Xehito stayed on the empire's planet to collect information while I headed off to explore a neighboring planet.\n" +
                    "It looked war-stricken. I stumbled upon a ruined city, flattened by high-power explosives." +
                    "\nThere is life here, humanoid beings with ragged clothing and depressed looks. When asked, they told me\n" +
                    "this destruction was the doing of the neighboring planet's empire. I roamed the wastelands, observed the near-dead residents,\n" +
                    "and heard their pleads for help. I have no reason to help them. They had done nothing for me." +
                    "\nBut, they had done nothing to deserve this either. I will decide my actions tomorrow.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog12 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #9145622");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 11);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I have ejected thousands of hologram drones across the planet's outer atmosphere and projected\n" +
                    "the image of the planet how it is now - barren and dull. I've sent Androids down to help clear rubble," +
                    "\nlandmines, undetonated warheads, and collect materials for my plans. The residents are suspicious of me.\n" +
                    "It's understandable though, beings from other planets have not been kind to them." +
                    "\nI don't understand why I'm helping them, but it makes me feel something strange.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog13 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #9145629");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 12);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'In 8 days, my army and I have rebuilt skyscrapers, planted seeds and saplings created from the remains\n" +
                    "of another planet, and collected ice from asteroids to pour into the oceans. The world feels alive once more,\n" +
                    "the residents of this planet must be relieved as they witness the rebirth of their home." +
                    "\nThe hologram drones are still projecting the image of the planet as it was when I got here," +
                    "\nso any outsiders won't notice my deeds. My next goal will be the annihilation of the neighboring planet's empire.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog14 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #9145630");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 13);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I tested my new weaponry on the empire's planet. Xehito came back to the SoS so he wouldn't get hit.\n" +
                    "It's a dual-beam that shoots from both edges of my crescent moon-shaped spaceship. The planet was devastated,\n" +
                    "with the empire along with it. There were leftovers in the form of spaceships that were out of range of the\n" +
                    "planet's explosion, but Xehito and I made quick work of them. The retribution of the empire fills me with satisfaction,\n" +
                    "and during my time helping the other planet, my feelings of pain had dulled. But, now it returns once more.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog15 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #170001202");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 14);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Xehito had to leave for certain reasons, but whatever.\n" +
                    "I gave him a parting gift, it was a memory chip, one that held all my memories." +
                    "\nHe will find me again when the time was right.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }

    public class WallDatalog16 : WallDatalog
    {
        public override string Texture => "Redemption/Items/Lore/WallDatalog";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Data Log #365000663");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<WallDatalogTile>(), 15);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'I remain lost in space. However, I set up a system of scanners that are capable of collecting\n" +
                    "the data of every planet using signals that would travel indefinitely, in the hopes that I may recognize\n" +
                    "the data of one of them and use it to lead me into the right direction. It'll take many decades for the signals\n" +
                    "to travel across space, but in the long term, this might save me.'")
                {
                    OverrideColor = Color.LightCyan
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view datalog")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}