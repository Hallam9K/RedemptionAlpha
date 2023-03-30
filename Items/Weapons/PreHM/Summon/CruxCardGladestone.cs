using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Cooldowns;
using Redemption.Buffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.NPCs.Friendly.SpiritSummons;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardGladestone : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crux Card: Crumbling Automatons");
            /* Tooltip.SetDefault("Summons the spirits of 2 Ancient Gladestone Golems\n" +
                "Right-click to tug the spirits back to your position, consuming 3 [i:" + ModContent.ItemType<LostSoul>() + "]\n" +
                "Consumes 18 [i:" + ModContent.ItemType<LostSoul>() + "] on use\n" +
                "Can only use one Spirit Card at a time"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 7;
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 3, 44, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.NPCDeath6;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.HasBuff<CruxCardCooldown>())
                return false;

            int soul = player.FindItem(ModContent.ItemType<LostSoul>());
            if (player.altFunctionUse == 2)
            {
                bool active2 = false;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (!npc.active || npc.type != ModContent.NPCType<AncientGladestoneGolem_SS>())
                        continue;

                    if (npc.ai[3] == player.whoAmI)
                        active2 = true;
                }
                if (active2 && soul >= 0 && player.inventory[soul].stack >= 3)
                {
                    player.inventory[soul].stack -= 3;
                    if (player.inventory[soul].stack <= 0)
                        player.inventory[soul] = new Item();
                    return true;
                }
                return false;

            }
            if (soul >= 0 && player.inventory[soul].stack >= 18)
            {
                player.inventory[soul].stack -= 18;
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
                player.AddBuff(ModContent.BuffType<CruxCardBuff>(), 2);
                if (player.altFunctionUse == 2)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (!npc.active || npc.type != ModContent.NPCType<AncientGladestoneGolem_SS>() || npc.ai[3] != player.whoAmI)
                            continue;

                        for (int i = 0; i < 10; i++)
                        {
                            int dust = Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, DustID.DungeonSpirit, 0, 0, Scale: 2);
                            Main.dust[dust].velocity *= 2f;
                            Main.dust[dust].noGravity = true;
                        }
                        npc.ai[0] = 5;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    for (int i = 0; i < 16; i++)
                    {
                        int dust = Dust.NewDust(player.Center - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 2f);
                        Main.dust[dust].noGravity = true;
                        Color dustColor = new(188, 244, 227) { A = 0 };
                        Main.dust[dust].color = dustColor;
                    }
                    int type = ModContent.NPCType<AncientGladestoneGolem_SS>();

                    for (int i = 0; i < 2; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.Center.X + 10, (int)player.Center.Y + 10, type, ai3: player.whoAmI);
                        else
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                    }
                }
            }
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Damage"));
            if (tooltipLocation != -1)
            {
                tooltips.Insert(tooltipLocation, new TooltipLine(Mod, "MaxLife", "250 base health"));
                tooltips.Insert(tooltipLocation + 2, new TooltipLine(Mod, "Defense", "20 defense"));
            }
        }
        private float drawTimer;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
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

            RedeDraw.DrawTreasureBagEffect(spriteBatch, texture, ref drawTimer, Item.Center - Main.screenPosition, frame, Color.LightBlue, rotation, origin, scale);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}