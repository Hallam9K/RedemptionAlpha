using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs;
using Redemption.Buffs.Cooldowns;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.Prefixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public abstract class BaseCruxCard : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SoulCost);
        protected int[] SpiritTypes;
        protected int SoulCost = 1;
        //protected int MoveCost = 1;
        public bool Infernal = false;
        public bool BossCard = false;
        protected int[] SpiritHealth;
        protected int[] SpiritDefense;
        protected int[] SpiritDamage;
        protected float SpawnDustSize = 2f;
        public virtual void SafeSetDefaults() { }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Summon;
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 1;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.NPCDeath6;
            SafeSetDefaults();
            Item.damage = SpiritDamage[0];
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.HasBuff<CruxCardCooldown>())
                return false;
            int soul = player.FindItem(ItemType<LostSoul>());
            if (player.altFunctionUse == 2)
            {
                bool active2 = false;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (!npc.active || !SpiritTypes.Contains(npc.type))
                        continue;

                    if (npc.ai[3] == player.whoAmI)
                        active2 = true;
                }
                if (active2)
                    return true;

                return false;
            }
            if (!player.HasBuff<CruxCardBuff>() && soul >= 0 && player.inventory[soul].stack >= SoulCost)
            {
                player.inventory[soul].stack -= SoulCost;
                if (player.inventory[soul].stack <= 0)
                    player.inventory[soul] = new Item();
            }
            else
                return false;
            bool active = false;
            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (!npc.active || !npc.Redemption().spiritSummon)
                    continue;

                if (npc.ai[3] == player.whoAmI)
                    active = true;
            }
            return !active;
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.altFunctionUse == 2)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (!npc.active || !SpiritTypes.Contains(npc.type) || npc.ai[3] != player.whoAmI)
                            continue;

                        for (int i = 0; i < (BossCard ? 20 : 10); i++)
                        {
                            int dust = Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, Infernal ? DustID.InfernoFork : DustID.DungeonSpirit, 0, 0, Scale: SpawnDustSize);
                            Main.dust[dust].velocity *= 2f;
                            Main.dust[dust].noGravity = true;
                        }
                        npc.ai[0] = 10;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    player.AddBuff(BuffType<CruxCardBuff>(), 2);
                    for (int i = 0; i < 16; i++)
                    {
                        int dust = Dust.NewDust(player.Center - Vector2.One, 1, 1, DustType<GlowDust>(), 0, 0, 0, default, SpawnDustSize);
                        Main.dust[dust].noGravity = true;
                        Color dustColor = Infernal ? new(255, 162, 17, 0) : new(188, 244, 227, 0);
                        Main.dust[dust].color = dustColor;
                    }
                    SpawnSpirits(player);
                }
            }
            return true;
        }
        public void NewSpirit(Player player, int x, int y, int typeID = 0)
        {
            // BEGGING for this method to be public
            MethodInfo method = typeof(Item).GetMethod("TryGetPrefixStatMultipliersForItem", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] args = new object[] { Item.prefix, 1f, 1f, 1f, 1f, 1f, 1f, 0 };
            method.Invoke(Item, args);
            float prefixDmg = (float)args[1];

            int n = NPC.NewNPC(new EntitySource_BossSpawn(player), x, y, SpiritTypes[typeID], ai3: player.whoAmI);
            Main.npc[n].lifeMax = (int)(SpiritHealth[typeID] * Item.Redemption().CruxHealthPrefix);
            Main.npc[n].life = (int)(SpiritHealth[typeID] * Item.Redemption().CruxHealthPrefix);
            Main.npc[n].defense = (int)(SpiritDefense[typeID] * Item.Redemption().CruxDefensePrefix);
            Main.npc[n].damage = (int)(GetOtherDamage(SpiritDamage[typeID]) * prefixDmg);
        }
        public virtual void SpawnSpirits(Player player) { }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Item.prefix > 0)
            {
                TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
                if (tt != null)
                {
                    string[] splitText = tt.Text.Split(' ');
                    string prefixValue = splitText.First();
                    tt.Text = splitText[1] + " " + splitText[2] + " " + prefixValue;
                    for (int i = 3; i < splitText.Length; i++)
                        tt.Text += " " + splitText[i];
                }
            }
            if (SpiritDamage.Length > 1)
            {
                TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
                if (tt != null)
                {
                    MethodInfo method = typeof(Item).GetMethod("TryGetPrefixStatMultipliersForItem", BindingFlags.NonPublic | BindingFlags.Instance);
                    object[] args = new object[] { Item.prefix, 1f, 1f, 1f, 1f, 1f, 1f, 0 };
                    method.Invoke(Item, args);
                    float prefixDmg = (float)args[1];

                    string damage = string.Empty;
                    for (int i = 1; i < SpiritDamage.Length; i++)
                    {
                        int otherDmg = GetOtherDamage(SpiritDamage[i]);
                        damage += "/" + (int)(otherDmg * prefixDmg);
                    }
                    string[] splitText = tt.Text.Split(' ');
                    string damageValue = splitText.First();
                    string damageWord = splitText[^2] + " " + splitText.Last();
                    tt.Text = damageValue + damage + " " + damageWord;
                }
            }
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Damage"));
            if (tooltipLocation != -1)
            {
                string health = ((int)(SpiritHealth[0] * Item.Redemption().CruxHealthPrefix)).ToString();
                if (SpiritHealth.Length > 1)
                {
                    for (int i = 1; i < SpiritHealth.Length; i++)
                        health += "/" + (int)(SpiritHealth[i] * Item.Redemption().CruxHealthPrefix);
                }
                string defense = ((int)(SpiritDefense[0] * Item.Redemption().CruxDefensePrefix)).ToString();
                if (SpiritDefense.Length > 1)
                {
                    for (int i = 1; i < SpiritDefense.Length; i++)
                        defense += "/" + (int)(SpiritDefense[i] * Item.Redemption().CruxDefensePrefix);
                }
                tooltips.Insert(tooltipLocation, new TooltipLine(Mod, "MaxLife", health + " base health"));
                tooltips.Insert(tooltipLocation + 2, new TooltipLine(Mod, "Defense", defense + " defense"));
            }
        }
        public int GetOtherDamage(int value)
        {
            StatModifier modifier = Main.LocalPlayer.GetTotalDamage(Item.DamageType);
            CombinedHooks.ModifyWeaponDamage(Main.LocalPlayer, Item, ref modifier);

            float baseDamage = value * ItemID.Sets.ToolTipDamageMultiplier[Item.type];
            return Math.Max(0, (int)(modifier.ApplyTo(baseDamage) + 5E-06f));
        }
        public override int ChoosePrefix(UnifiedRandom rand)
        {
            WeightedRandom<int> prefix = new(rand);
            prefix.Add(PrefixType<GladPrefix>());
            prefix.Add(PrefixType<GlumPrefix>());
            prefix.Add(PrefixType<HardyPrefix>());
            prefix.Add(PrefixType<JovialPrefix>());
            prefix.Add(PrefixType<IrkedPrefix>());
            prefix.Add(PrefixType<HealthyPrefix>(), .9);
            prefix.Add(PrefixType<LoyalPrefix>(), .8);
            prefix.Add(PrefixType<DepressedPrefix>(), .8);
            prefix.Add(PrefixType<FrenziedPrefix>(), .7);
            prefix.Add(PrefixType<CrestfallenPrefix>(), .5);
            prefix.Add(PrefixType<StalwartPrefix>(), .7);
            prefix.Add(PrefixType<VexedPrefix>(), .5);
            prefix.Add(PrefixType<UnyieldingPrefix>(), .4);
            prefix.Add(PrefixType<DeterminedPrefix>(), .7);
            return prefix;
        }
        private float drawTimer;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            if (BossCard)
            {
                Texture2D bossTex = Request<Texture2D>("Redemption/Textures/CruxCardBoss").Value;
                RedeDraw.DrawTreasureBagEffect(spriteBatch, bossTex, ref drawTimer, position - new Vector2(2, 2), new Rectangle(0, 0, bossTex.Width, bossTex.Height), RedeColor.EnergyPulse * .5f, 0, origin, scale);
            }
            else
                RedeDraw.DrawTreasureBagEffect(spriteBatch, texture, ref drawTimer, position, new Rectangle(0, 0, texture.Width, texture.Height), Color.LightBlue, 0, origin, scale);
            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), drawColor, 0, origin, scale, 0, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;

            if (BossCard)
            {
                Texture2D bossTex = Request<Texture2D>("Redemption/Textures/CruxCardBoss").Value;
                RedeDraw.DrawTreasureBagEffect(spriteBatch, bossTex, ref drawTimer, Item.Center - new Vector2(2, 2) - Main.screenPosition, frame, RedeColor.EnergyPulse * .5f, rotation, origin, scale);
            }
            else
                RedeDraw.DrawTreasureBagEffect(spriteBatch, texture, ref drawTimer, Item.Center - Main.screenPosition, frame, Color.LightBlue, rotation, origin, scale);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}