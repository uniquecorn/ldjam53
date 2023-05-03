using System;
using System.Runtime.InteropServices;
using Castle.Core.CastleKit;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Castle.Core.Save
{
    public static class CastleKit
    {
        const string Lib = "__Internal";
        //public static async UniTask<bool> Login() => (await GKLocalPlayer.Authenticate()).IsAuthenticated;
        [DllImport(Lib)]
        private static extern void GK_Authenticate(IntPtr callback, IntPtr didModifySavedGameCallback, IntPtr hasConflictingSavedGamesCallback);
        [DllImport(Lib)]
        private static extern bool GK_IsAuthenticated(IntPtr pointer);
        public static void AuthenticateLocalPlayer(Action<CastleResult> callback,Action<CastleResult> modifiedSaves,Action<CastleResult> conflict)
        {
            
                GK_Authenticate(
                MonoPCallback.ActionToIntPtr(callback),
                MonoPCallback.ActionToIntPtr(modifiedSaves),
                MonoPCallback.ActionToIntPtr(conflict)
            );

        }
        //public static UniTask<>
    }
}
