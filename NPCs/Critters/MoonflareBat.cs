using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Critters;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.Critters
{
    public class MoonflareBat : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.Shimmerfly;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 18;
            NPC.defense = 0;
            NPC.lifeMax = 25;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath4;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.chaseable = false;
            NPC.catchItem = (short) ModContent.ItemType<MoonflareBatItem>();
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<MoonflareBatBanner>();
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.HasElement(ElementID.Shadow))
                modifiers.FinalDamage *= 1.25f;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.HasElement(ElementID.Shadow))
                modifiers.FinalDamage *= 1.25f;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonflareFragment>(), 1, 2, 4));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.OverworldNight.Chance;
            float multiplier = LanternNight.LanternsUp || Main.moonPhase == 0 ? 0.4f : 0.01f;

            return baseChance * multiplier;
        }

        public Vector2 moveTo;

        public override void AI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = NPC.rotation;

            NPC.LookByVelocity();

            if (NPC.collideX && NPC.velocity.X != NPC.oldVelocity.X)
                NPC.velocity.X = -NPC.oldVelocity.X;

            if (NPC.collideY && NPC.velocity.Y != NPC.oldVelocity.Y)
                NPC.velocity.Y = -NPC.oldVelocity.Y;

            if (NPC.velocity.Length() == 0)
                NPC.velocity.Y = -6;

            if (NPC.velocity.X == 0)
                NPC.velocity.X = 3 * Main.rand.NextFloatDirection();

            int tilePosY = BaseWorldGen.GetFirstTileFloor((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16), true, true);
            int dist = (tilePosY * 16) - (int)NPC.Center.Y;

            NPC.velocity.X *= 1.04f;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -4, 4);

            if (dist < 32)
                NPC.velocity.Y = -1f; 
            else if (dist < 200)
                NPC.velocity.Y -= 1f;
            else if (dist > 800)
                NPC.velocity.Y += 0.02f;

            if (NPC.velocity.Y < 2 && NPC.velocity.Y > -2)
                NPC.velocity.Y *= 1.01f;
            else if (NPC.velocity.Y < -4 || NPC.velocity.Y > 4)
                NPC.velocity.Y *= 0.92f;

            if (!Main.rand.NextBool(20) || Main.dayTime || Main.moonPhase == 4)
                return;

            int sparkle = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<MoonflareDust>());
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.05f;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 3)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > 4 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Moon,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.MoonflareBat"))
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D Trail = ModContent.Request<Texture2D>(Texture + "_Trail").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!Main.dayTime && Main.moonPhase != 4 && !NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.BeginAdditive();

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(Trail, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, Color.White * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (!Main.dayTime && Main.moonPhase != 4)
                spriteBatch.Draw(Glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                int gore1 = ModContent.Find<ModGore>("Redemption/MoonflareBatGore1").Type;
                int gore2 = ModContent.Find<ModGore>("Redemption/MoonflareBatGore2").Type;

                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, gore1);

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, gore2);

                for (int i = 0; i < 8; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<MoonflareDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
            }

            for (int i = 0; i < 4; i++)
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<MoonflareDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
        }
    }
}