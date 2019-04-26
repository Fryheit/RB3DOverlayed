//!CompilerOption:AddRef:SlimDx.dll
namespace RB3DOverlayed
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using ff14bot;
    using ff14bot.AClasses;
    using ff14bot.Enums;
    using ff14bot.Managers;
    using ff14bot.Objects;
    using Overlay;
    using Vector3 = SlimDX.Vector3;
    using NVector3 = Clio.Utilities.Vector3;

    public class Plugin : BotPlugin
    {
        public override string Name => "3D Overlayed";
        public override string Description => "Modified RebornBuddy 3d Overlay";
        public override string Author => "The Buddy Team, ZZI, Freiheit";
        public override Version Version => new Version(1, 0, 0);
        public override string ButtonText => "Settings";
        public override bool WantButton => true;

        private RenderForm _renderForm;

        private readonly List<uint> _listSMobs = new List<uint>
        {
            2953, // Laideronnette
            2954, // Wulgaru
            2955, // Mindflayer
            2956, // Thousand-cast Theda
            2957, // Zona Seeker
            2958, // Brontes
            2959, // Lampalagua
            2960, // Nunyunuwi
            2961, // Minhocao
            2962, // Croque-mitaine
            2963, // Croakadile
            2964, // The Garlok
            2965, // Bonnacon
            2966, // Nandi
            2967, // Chernobog
            2968, // Safat
            2969, // Agrippa The Mighty
            4374, // Kaiser Behemoth
            4375, // Senmurv
            4376, // The Pale Rider
            4377, // Gandarewa
            4378, // Bird of Paradise
            4380, // Leucrotta
            5984, // Okina
            5985, // Gamma
            5986, // Orghana
            5987, // Udumbara
            5988, // Bone Crawler
            5989, // Salt and Light
        };

        private readonly List<uint> _listAMobs = new List<uint>
        {
            2936, // Forneus
            2937, // Melt
            2938, // Ghede Ti Malice
            2939, // Girtab
            2940, // Alectryon
            2941, // Sabotender Bailarina
            2942, // Maahes
            2943, // Zanig'oh
            2944, // Dalvag's Final Flame
            2945, // Vogaal Ja
            2946, // Unktehi
            2947, // Hellsclaw
            2948, // Nahn
            2949, // Marberry
            2950, // Cornu
            2951, // Marraco
            2952, // Kurrea
            4362, // Mirka
            4363, // Lyuba
            4364, // Pylraster
            4365, // Lord of the Wyverns
            4366, // Slipkinx Steeljoints
            4367, // Stolas
            4368, // Bune
            4369, // Agathos
            4370, // Enkelados
            4371, // Sisiutl
            4372, // Campacti
            4373, // Stench Blossom
            5990, // Orcus
            5991, // Erle
            5992, // Vochstein
            5993, // Aqrabuamelu
            5994, // Mahisha
            5995, // Luminare
            5996, // Funa Yurei
            5997, // Oni Yumemi
            5998, // Gajasura
            5999, // Angada
            6000, // Girimekhala
            6001, // Sum
        };

        private readonly List<uint> _listBMobs = new List<uint>
        {
            2919, // White Joker
            2920, // Stinging Sophie
            2921, // Monarch Ogrefly
            2922, // Phecda
            2923, // Sewer Syrup
            2924, // Ovjang
            2925, // Gatling
            2926, // Albin the Ashen
            2927, // Flame Sergeant Dalvag
            2928, // Skogs Fru
            2929, // Barbastelle
            2930, // Bloody Mary
            2931, // Dark Helmet
            2932, // Myradrosh
            2933, // Vuokho
            2934, // Naul
            2935, // Leech King
            4350, // Alteci
            4351, // Kreutzet
            4352, // Gnath Cometdrone
            4353, // Thextera
            4354, // Pterygotus
            4355, // False Gigantopithecus
            4356, // Scitalis
            4357, // The Scarecrow
            4358, // Squonk
            4359, // Sanu Vali of Dancing Wings
            4360, // Lycidas
            4361, // Omni
            6002, // Gauki Strongblade
            6003, // Guhuo Niao
            6004, // Deidar
            6005, // Gyorai Quickstrike
            6006, // Kurma
            6007, // Aswang
            6008, // Shadow-dweller Yamini
            6009, // Ouzelum
            6010, // Gwas-y-neidr
            6011, // Buccaboo
            6012, // Manes
            6013, // Kiwa
        };

        public override void OnPulse()
        {
        }

        public override void OnInitialize()
        {

        }

        public override void OnShutdown()
        {
            Task.Run(OnDisableAsync);
        }

        public override void OnEnabled()
        {
            Task.Factory.StartNew(RunRenderForm, TaskCreationOptions.LongRunning);
        }

        private void RunRenderForm()
        {
            OverlayManager.Drawing += Drawing;

            IntPtr targetWindow = Core.Memory.Process.MainWindowHandle;

            _renderForm = new RenderForm(targetWindow);

            Application.Run(_renderForm);
        }

        public override void OnDisabled()
        {
            Task.Run(OnDisableAsync);
        }

        private async Task OnDisableAsync()
        {
            OverlayManager.Drawing -= Drawing;

            if (_renderForm == null)
                return;

            await _renderForm.ShutdownAsync();
        }

        private SettingsForm _settingsForm;
        public override void OnButtonPress()
        {
            if (_settingsForm == null)
                _settingsForm = new SettingsForm();

            _settingsForm.ShowDialog();
        }

        private void Drawing(DrawingContext ctx)
        {
            OverlaySettings settings = OverlaySettings.Instance;

            if (settings.OnlyDrawInForeground &&
                Imports.GetForegroundWindow() != Core.Memory.Process.MainWindowHandle)
                return;

            if (QuestLogManager.InCutscene)
                return;

            // Gameobject list is threadstatic, always need to pulse otherwise new objects wont show up
            GameObjectManager.Update();

            if (settings.EnableInfoPanel)
            {
                DrawGameStats(ctx);
            }

            if (settings.DrawHuntSMobs || settings.DrawHuntAMobs || settings.DrawHuntBMobs)
            {
                DrawHuntMobs(ctx);
            }

            if (settings.DrawSelfBox)
            {
                DrawSelfCircle(ctx);
            }

            if (settings.DrawHostilityBoxes || settings.DrawUnitLines || settings.DrawGameObjectBoxes ||
                settings.DrawGameObjectLines)
            {
                foreach (GameObject obj in GameObjectManager.GameObjects)
                {
                    if (!obj.IsVisible)
                        continue;

                    if (settings.OnlyRenderTargetable)
                    {
                        if (obj.Type != GameObjectType.EventObject)
                        {
                            if (!obj.IsTargetable)
                                continue;
                        }
                    }

                    if (obj.Type == GameObjectType.Mount)
                        continue;

                    var name = obj.Name;
                    var vecCenter = obj.Location.Convert() + new Vector3(0, 1, 0);


                    //.Where(i => i.Type == GameObjectType.GatheringPoint || i.Type == GameObjectType.BattleNpc || i.Type == GameObjectType.EventObject || i.Type == GameObjectType.Treasure || i.Type == GameObjectType.Pc)


                    var color = Color.FromArgb(150, Color.Blue);

                    //some generic objects. If you want to add a specific object it should probably go here or in it's own block below this.
                    if (obj.Type == GameObjectType.GatheringPoint || obj.Type == GameObjectType.EventObject ||
                        obj.Type == GameObjectType.Treasure)
                    {
                        if (obj.Type == GameObjectType.GatheringPoint)
                            color = Color.FromArgb(150, Color.BlueViolet);
                        if (obj.Type == GameObjectType.EventObject)
                            color = Color.FromArgb(150, Color.Fuchsia);
                        if (obj.Type == GameObjectType.Treasure)
                            color = Color.SandyBrown;

                        if (settings.DrawGameObjectNames && !string.IsNullOrEmpty(name))
                            ctx.Draw3DText(name, vecCenter);

                        if (settings.DrawGameObjectBoxes)
                        {
                            ctx.DrawOutlinedBox(vecCenter, new Vector3(1), Color.FromArgb(150, color));
                        }


                        //if (settings.DrawGameObjectLines)
                        //{
                        //    if (!settings.DrawGameObjectLinesLos || obj.InLineOfSight())
                        //        ctx.DrawLine(vecStart, vecCenter, Color.FromArgb(150, color));
                        //}
                    }

                    var u = obj as Character;
                    if (u != null)
                    {
                        var playerOrPlayerOwned = (!u.IsNpc || u.SummonerObjectId != GameObjectManager.EmptyGameObject);
                        if (!settings.DrawPlayers && playerOrPlayerOwned)
                        {
                            continue;
                        }

                        var hostilityColor = Color.FromArgb(150, Color.Green);

                        var uStatusFlags = u.StatusFlags;
                        if (uStatusFlags.HasFlag(StatusFlags.Hostile))
                        {
                            hostilityColor = Color.FromArgb(150, Color.Red);

                            //if (settings.DrawAggroRangeCircles)
                            //    ctx.DrawCircle(vecCenter, u.MyAggroRange, 64,
                            //                   Color.FromArgb(75, Color.DeepSkyBlue));
                        }

                        if (uStatusFlags == StatusFlags.None)
                            hostilityColor = Color.FromArgb(150, Color.Yellow);

                        if (uStatusFlags.HasFlag(StatusFlags.Friend) || uStatusFlags.HasFlag(StatusFlags.PartyMember) ||
                            uStatusFlags.HasFlag(StatusFlags.AllianceMember))
                            hostilityColor = Color.FromArgb(150, Color.Green);


                        if (playerOrPlayerOwned)
                        {
                            if (settings.DrawPlayerNames)
                            {
                                ctx.Draw3DText(name, vecCenter);
                            }
                        }
                        else
                        {
                            if (settings.DrawUnitNames)
                            {
                                if (!string.IsNullOrEmpty(name) && obj.IsTargetable)
                                    ctx.Draw3DText(name, vecCenter);
                            }
                        }


                        ctx.DrawOutlinedBox(vecCenter, new Vector3(1), Color.FromArgb(255, hostilityColor));
                    }
                }
            }
        }


        private void DrawHuntMobs(DrawingContext ctx)
        {
            OverlaySettings settings = OverlaySettings.Instance;

            NVector3 mypos = Core.Me.Location;
            Vector3 vecStart = new Vector3(mypos.X, mypos.Y + 1, mypos.Z);

            foreach (GameObject obj in GameObjectManager.GameObjects)
            {
                if (   (!settings.DrawHuntSMobs || !_listSMobs.Contains(obj.NpcId))
                    && (!settings.DrawHuntAMobs || !_listAMobs.Contains(obj.NpcId))
                    && (!settings.DrawHuntBMobs || !_listBMobs.Contains(obj.NpcId)))
                    continue;

                var name = obj.Name;
                var vecCenter = obj.Location.Convert() + new Vector3(0, 1, 0);
                var color = Color.White;

                if (_listSMobs.Contains(obj.NpcId))
                    color = Color.Yellow;
                if (_listAMobs.Contains(obj.NpcId))
                    color = Color.Red;
                if (_listBMobs.Contains(obj.NpcId))
                    color = Color.Green;

                if (!string.IsNullOrEmpty(name))
                    ctx.Draw3DText(name, vecCenter);

                ctx.DrawOutlinedBox(vecCenter, new Vector3(1), Color.FromArgb(255, color));
                ctx.DrawBox(vecCenter, new Vector3(1), Color.FromArgb(150, color));

                // Lines toward the hunt to make it easier to spot
                NVector3 end = obj.Location;
                Vector3 vecEnd = new Vector3(end.X, end.Y + 1, end.Z);

                ctx.DrawLine(vecStart, vecEnd, Color.FromArgb(150, color));
            }
        }

        private void DrawSelfCircle(DrawingContext ctx)
        {
            ctx.DrawCircleOutline(Core.Me.Location, 1, Color.FromArgb(255, Color.Blue));
            ctx.DrawCircle(Core.Me.Location, 1, Color.FromArgb(150, Color.Blue));
        }

        private void DrawGameStats(DrawingContext ctx)
        {
            OverlaySettings settings = OverlaySettings.Instance;
            StringBuilder sb = new StringBuilder();

            if (settings.ShowGameStats)
            {
                NVector3 mypos = Core.Me.Location;
                Vector3 vecStart = new Vector3(mypos.X, mypos.Y, mypos.Z);
                int myLevel = Core.Me.ClassLevel;

                GameObject currentTarget = Core.Me.CurrentTarget;

                sb.AppendLine($@"My Position: {Core.Me.Location}");
                if (currentTarget != null)
                {
                    sb.AppendLine(
                        $@"Current Target: {currentTarget.Name}, Distance: {Math.Round(currentTarget.Distance(), 3)}");

                    NVector3 end = currentTarget.Location;
                    Vector3 vecEnd = new Vector3(end.X, end.Y, end.Z);

                    ctx.DrawLine(vecStart, vecEnd, Color.DeepSkyBlue);
                }
                else
                {
                    sb.AppendLine(@"Current Target: None");
                }

                sb.AppendLine();
                sb.AppendLine($@"XP Per Hour: {GameStatsManager.XPPerHour:F0}");
                sb.AppendLine($@"Deaths Per Hour: {GameStatsManager.DeathsPerHour:F0}");

                if (myLevel < 70)
                    sb.AppendLine($@"Time to Level: {GameStatsManager.TimeToLevel}");

                sb.AppendLine($@"TPS: {GameStatsManager.TicksPerSecond:F2}");
                sb.AppendLine();
            }

            if (settings.ShowHuntList)
            {
                sb.AppendLine("Nearby Hunts:");

                bool huntFound = false;
                foreach (GameObject obj in GameObjectManager.GameObjects)
                {
                    if (_listSMobs.Contains(obj.NpcId))
                    {
                        sb.AppendLine(@"[S] " + obj.Name);
                        huntFound = true;
                    }
                    if (_listAMobs.Contains(obj.NpcId))
                    {
                        sb.AppendLine(@"[A] " + obj.Name);
                        huntFound = true;
                    }
                    if (_listBMobs.Contains(obj.NpcId))
                    {
                        sb.AppendLine(@"[B] " + obj.Name);
                        huntFound = true;
                    }
                }

                if (!huntFound)
                    sb.AppendLine("None");
            }

            sb.AppendLine();

            if (settings.UseShadowedText)
            {
                ctx.DrawOutlinedText(sb.ToString(),
                    settings.GameStatsPositionX,
                    settings.GameStatsPositionY,
                    settings.GameStatsForegroundColor,
                    settings.GameStatsShadowColor,
                    settings.GameStatsFontSize
                );
            }
            else
            {
                ctx.DrawText(sb.ToString(),
                    settings.GameStatsPositionX,
                    settings.GameStatsPositionY,
                    settings.GameStatsForegroundColor,
                    settings.GameStatsFontSize
                );
            }
        }
    }
}
