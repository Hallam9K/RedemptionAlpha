using Mono.Cecil.Cil;
using MonoMod.Cil;
using Redemption.Globals.Player;
using Terraria.ModLoader;

namespace Redemption.Globals.ILEdits
{
	class KnockbackStatEdit : ILEdit // Code by Spirit Mod
	{
		public override void Load(Mod mod) => IL.Terraria.Player.Hurt += Player_Hurt;

		private static void Player_Hurt(ILContext il)
		{
			ILCursor c = new(il);

			for (int i = 0; i < 2; ++i)
				if (!c.TryGotoNext(x => x.MatchLdfld<Terraria.Player>("mount"))) //match ldfld for mount twice to get to the right call
					return;
			if (!c.TryGotoNext(x => x.MatchLdcR4(4.5f))) //then match the 4.5 ldcr4
				return;

			if (!c.TryGotoNext(x => x.MatchMul())) //go to mul instruction, then move past it.
				return;
			c.Index++;

			//Now we do some shenanigains.
			c.Emit(OpCodes.Ldarg_0); //Push player onto stack so KnockbackMultiplier can consume it
			c.Emit(OpCodes.Ldc_I4_0); //Push 0 (true) to the stack, since this is horizontal knockback
			c.EmitDelegate(BuffPlayer.KnockbackMultiplier); //Get the kb modifier
			c.Emit(OpCodes.Mul); //and multiply the value already on the stack by the modifier

			//and we move to the velocity.Y 
			if (!c.TryGotoNext(x => x.MatchLdcR4(-3.5f))) //match the -3.5f ldcr4
				return;
			c.Index++;

			//and match the same things
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldc_I4_1); //Only difference is this pushes 1, because this is vertical knockback
			c.EmitDelegate(BuffPlayer.KnockbackMultiplier);
			c.Emit(OpCodes.Mul);
		}
	}
}
