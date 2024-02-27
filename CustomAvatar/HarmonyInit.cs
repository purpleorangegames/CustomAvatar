using System;
using HarmonyLib;
using Verse;

namespace CustomAvatar
{
	// Token: 0x02000002 RID: 2
	[StaticConstructorOnStartup]
	public static class HarmonyInit
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		static HarmonyInit()
		{
			new Harmony("AvatarMod").PatchAll();
		}
	}
}
