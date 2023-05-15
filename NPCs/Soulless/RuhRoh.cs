using CollisionLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Soulless
{
    public class RuhRoh : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public CollisionSurface[] colliders = null;
        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 320;
            NPC.lifeMax = 1000;
            NPC.damage = 100;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.behindTiles = true;
        }
        public override bool CheckActive() => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.velocity.X < -2;
        public override bool PreAI()
        {
            if (colliders == null || colliders.Length != 2)
            {
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.BottomLeft, new int[] { 1, 1, 1, 1 }, true),
                    new CollisionSurface(NPC.BottomLeft, NPC.BottomRight, new int[] { 1, 1, 1, 1 }, true) };
            }
            return true;
        }
        private bool Flare;
        private float FlareTimer;
        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (Main.LocalPlayer.DistanceSQ(NPC.Center) < 400 * 400)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player2 = Main.player[i];
                    if (!player2.active || player2.dead || !player2.Hitbox.Intersects(NPC.Hitbox))
                        continue;

                    if (player2.position.Y > NPC.Bottom.Y - 20)
                        player2.position.Y = NPC.position.Y + NPC.height;
                    else if (player2.Bottom.Y < NPC.position.Y + 20)
                        player2.position.Y = NPC.position.Y + -player2.height;
                    else
                        player2.position.X = NPC.position.X + (player2.RightOf(NPC) ? NPC.width + 20 : -20);
                }
            }
            if (Flare)
            {
                FlareTimer++;
                if (FlareTimer > 60)
                {
                    Flare = false;
                    FlareTimer = 0;
                }
            }
            switch (NPC.ai[0])
            {
                case 0:
                    Rectangle sight = new((int)NPC.position.X - 200, (int)NPC.position.Y, 220, NPC.height);
                    if (player.Hitbox.Intersects(sight) && !player.dead && player.active)
                    {
                        Flare = true;
                        NPC.ai[0] = 1;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    Flare = true;
                    FlareTimer = MathHelper.Min(30, FlareTimer);
                    NPC.velocity.X -= 0.01f;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -10, 0);
                    break;
            }

            if (colliders != null && colliders.Length == 2)
            {
                colliders[0].Update();
                colliders[0].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[0].endPoints[1] = NPC.Center + (NPC.BottomLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[1].Update();
                colliders[1].endPoints[0] = NPC.Center + (NPC.BottomLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[1].endPoints[1] = NPC.Center + (NPC.BottomRight - NPC.Center).RotatedBy(NPC.rotation);
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = Redemption.WhiteFlare.Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = NPC.Center - screenPos + new Vector2(8 * NPC.spriteDirection, -10 + NPC.gfxOffY);
            Color colour = Color.Lerp(Color.White, Color.White, 1f / FlareTimer * 10f) * (1f / FlareTimer * 10f);
            if (Flare)
            {
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour, 0, origin, 1.3f, SpriteEffects.None, 0);
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour * 0.4f, 0, origin, 1.3f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                    collider.PostUpdate();
            }
        }
    }
}