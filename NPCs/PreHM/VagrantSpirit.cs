using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Redemption.NPCs.Friendly;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.PreHM
{
    public class VagrantSpirit : ModNPC
    {
        public static Asset<Texture2D> back;
        public static Asset<Texture2D> lantern;
        public static Asset<Texture2D> mask;
        public static Asset<Texture2D> trail;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            back = Request<Texture2D>(Texture + "_Back");
            lantern = Request<Texture2D>(Texture + "_Lantern");
            mask = Request<Texture2D>(Texture + "_Mask");
            trail = Request<Texture2D>(Texture + "_Trail");
        }

        public enum ActionState
        {
            Wander,
            Vanish
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public float[] oldrot = new float[10];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Velocity = 1f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 76;
            NPC.height = 44;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.alpha = 100;
            NPC.chaseable = false;
            Banner = NPC.type;
            BannerItem = ItemType<VagrantSpiritBanner>();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 5f;
                    Main.dust[dust].noGravity = true;
                }
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        private int vanishCounter;
        private bool HasMask;
        private bool HasCloth;
        public override void OnSpawn(IEntitySource source)
        {
            HasMask = Main.rand.NextBool();
            HasCloth = Main.rand.NextBool();

            TimerRand = Main.rand.Next(180, 420);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = NPC.rotation;

            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            NPC.LookByVelocity();

            NPC.position.Y += (float)Math.Sin(NPC.localAI[0]++ / 40);

            if (Main.rand.NextBool(1500) && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Ghost3, NPC.position);

            if (NPC.alpha < 150)
            {
                if (Main.LocalPlayer.DistanceSQ(NPC.Center) < 160 * 160)
                    Main.LocalPlayer.AddBuff(BuffID.Chilled, 30);
                if (Main.LocalPlayer.Hitbox.Intersects(NPC.Hitbox))
                    Main.LocalPlayer.AddBuff(BuffType<PureChillDebuff>(), 30);

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (!npc.dontTakeDamage && npc.Hitbox.Intersects(NPC.Hitbox))
                        npc.AddBuff(BuffType<PureChillDebuff>(), 30);
                }
            }

            switch (AIState)
            {
                case ActionState.Wander:
                    if (NPC.velocity.X == 0)
                    {
                        NPC.LookAtEntity(player);
                        NPC.velocity.X = 1 * NPC.spriteDirection;
                    }
                    NPC.alpha -= 2;
                    NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 10, 255);

                    NPC.velocity.X *= 1.02f;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -1.4f, 1.4f);

                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(180, 420);
                        AIState = ActionState.Vanish;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Vanish:
                    NPC.alpha++;
                    if (NPC.alpha >= 255)
                    {
                        if (((player.ZoneOverworldHeight || player.ZoneSkyHeight) && !Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive) || vanishCounter > 4)
                            NPC.active = false;

                        vanishCounter++;
                        NPC.position = new Vector2(player.Center.X + (600 * NPC.spriteDirection), player.Center.Y + Main.rand.Next(-400, 401));
                        NPC.LookAtEntity(player);
                        NPC.velocity.X = 1 * NPC.spriteDirection;
                        AIState = ActionState.Wander;
                    }
                    break;
            }
        }

        Vector2 SetLantern2Offset(ref int frameHeight)
        {
            return (NPC.frame.Y / frameHeight) switch
            {
                1 => new Vector2(-2 * NPC.spriteDirection, 0),
                2 => new Vector2(-2 * NPC.spriteDirection, -2),
                3 => new Vector2(-2 * NPC.spriteDirection, -4),
                _ => new Vector2(0, 0),
            };
        }
        Vector2 SetLantern1Offset(ref int frameHeight)
        {
            return (NPC.frame.Y / frameHeight) switch
            {
                1 => new Vector2(-2 * NPC.spriteDirection, 0),
                2 => new Vector2(-4 * NPC.spriteDirection, -4),
                3 => new Vector2(-2 * NPC.spriteDirection, -4),
                _ => new Vector2(0, 0),
            };
        }
        Vector2 Lantern2Offset;
        Vector2 Lantern1Offset;
        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.05f;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
            Lantern1Offset = SetLantern1Offset(ref frameHeight);
            Lantern2Offset = SetLantern2Offset(ref frameHeight);
        }

        public override bool? CanBeHitByItem(Player player, Item item) => RedeHelper.CanHitSpiritCheck(player, item);
        public override bool? CanBeHitByProjectile(Projectile projectile) => RedeHelper.CanHitSpiritCheck(projectile);
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Arcane);
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            RedeQuest.SetBonusDiscovered(RedeQuest.Bonuses.Arcane);
        }

        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<LostSoulNPC>(), Main.rand.NextFloat(1f, 2f));
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ItemType<LostSoul>()));
            npcLoot.Add(ItemDropRule.Food(ItemType<Soulshake>(), 20));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CustomCollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], false, 25);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.VagrantSpirit"))
            });
        }

        private Vector2 LanternPos1() => NPC.Center + new Vector2(11 * NPC.spriteDirection, 28) + Lantern1Offset;
        private Vector2 LanternPos2() => NPC.Center + new Vector2(23 * NPC.spriteDirection, 20) + Lantern2Offset;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!NPC.IsABestiaryIconDummy)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(trail.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.White with { A = 0 }) * 0.1f, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }
            }

            spriteBatch.Draw(lantern.Value, LanternPos1() - screenPos, lantern.Frame(2, 1, 1), NPC.ColorTintedAndOpacity(drawColor), NPC.rotation + (.2f * NPC.spriteDirection) + (float)(Math.Sin(NPC.localAI[0] / 30) / 5), new Vector2(lantern.Width() / 4 - 1, 0), NPC.scale, effects, 0);
            spriteBatch.Draw(lantern.Value, LanternPos2() - screenPos, lantern.Frame(2, 1), NPC.ColorTintedAndOpacity(drawColor), NPC.rotation + (.2f * NPC.spriteDirection) + (float)(Math.Sin((NPC.localAI[0] - 15) / 30) / 4), new Vector2(lantern.Width() / 4 - 1, 0), NPC.scale, effects, 0);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(drawColor with { A = 200 }), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (HasCloth)
                spriteBatch.Draw(back.Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(drawColor with { A = 100 }), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (HasMask)
                spriteBatch.Draw(mask.Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(drawColor with { A = 100 }), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy)
            {
                Texture2D LightGlow = Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
                float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, .9f, .7f, .9f);
                Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(255, 246, 182), new Color(234, 133, 66), new Color(255, 246, 182));

                Main.spriteBatch.End();
                Main.spriteBatch.BeginAdditive();

                spriteBatch.Draw(LightGlow, LanternPos1() + new Vector2(-4 * NPC.spriteDirection - (float)(Math.Sin(NPC.localAI[0] / 30) * 4), 19) - screenPos, null, NPC.GetAlpha(color) * 1.5f, MathHelper.PiOver2, LightGlow.Size() / 2, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(LightGlow, LanternPos2() + new Vector2(-4 * NPC.spriteDirection - (float)(Math.Sin((NPC.localAI[0] - 15) / 30) * 4), 19) - screenPos, null, NPC.GetAlpha(color) * 1.5f, MathHelper.PiOver2, LightGlow.Size() / 2, scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.BeginDefault();
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.RedemptionAbility().SpiritwalkerActive && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
                return 0.6f;

            if (spawnInfo.PlayerSafe)
                return 0;
            return SpawnCondition.Cavern.Chance * 0.006f;
        }
    }
}