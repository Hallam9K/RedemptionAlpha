using Terraria;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Redemption
{
    public static class AdditiveCallManager
	{
		private static uint MaxCalls => 1000;

		private static IDrawAdditive[] AdditiveCalls;

		public static void DrawAdditiveCalls(SpriteBatch sb)
		{
			List<IDrawAdditive> CallList = new();

			foreach (Projectile p in Main.projectile)
			{
				var mP = p.ModProjectile;
				if (mP is IDrawAdditive && p.active)
					CallList.Add(mP as IDrawAdditive);
			}

			foreach (NPC n in Main.npc)
			{
				var nP = n.ModNPC;
				if (nP is IDrawAdditive && n.active)
					CallList.Add(nP as IDrawAdditive);
			}

			// For particles and such
			for (int i = 0; i < MaxCalls; i++)
			{
				if (AdditiveCalls != null && AdditiveCalls[i] != null)
					CallList.Add(AdditiveCalls[i]);
			}

			if (CallList.Count > 0)
			{
				Main.spriteBatch.Begin(default, BlendState.Additive, default, default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
				foreach (IDrawAdditive drawAdditive in CallList)
					drawAdditive.AdditiveCall(sb, Main.screenPosition);

				Main.spriteBatch.End();
			}
		}

		//Use this return to Dispose
		public static int ManualAppend(IDrawAdditive IDA)
		{
			for (int i = 0; i < AdditiveCalls.Length; i++) {
				if (AdditiveCalls[i] == null) {
					AdditiveCalls[i] = IDA;
					return i;
				}
			}
			throw new NullReferenceException("E");
		}

		//A bit difficult to manage, but better for performance. 
		public static void RemoveCall(int Index) => AdditiveCalls[Index] = null;

		public static void Load() => AdditiveCalls = new IDrawAdditive[MaxCalls];
		public static void Unload() => AdditiveCalls = null;
    }
}
