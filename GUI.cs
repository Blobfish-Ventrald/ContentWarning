using ContentWarning.GUI;
using ExitGames.Client.Photon;
using Mono.Security.Authenticode;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityCheats_ContentWarning
{
    public class Class1 : MonoBehaviour
    {
        // For the GUI windows
        private static Rect windowRect = new Rect(10, 10, 320, 400);
        private static Rect titleRect = new Rect(0, 0, 320, 20);
        public static Rect mainTabRect = new Rect(10, 30, 100, 30);
        public static Rect settingsTabRect = new Rect(120, 30, 100, 30);
        public static bool showMain = true;
        public static bool showSettings = false;
        private Vector2 scrollPos = Vector2.zero;

        // Main toggles for mods
        public static bool infStamina = false;
        public static bool infJump = false;
        public static bool GodMode = false;
        public static bool UnlimitedOxy = false;
        public static bool UnlimitedBattery = false;
        public static bool Rainbowvisor = false;
        public static bool ReversePeople = false;
        public static bool UnlimitedFilm = false;
        public static bool LagAll = false;
        public static bool FlingAll = false;
        public static bool RagdallAll = false;
        public static bool NoMovement = false;
        public static bool FreezeAll = false;
        public static bool SpeedAll = false;
        public static bool Dragplayerstolocal = false;
        public static bool SpeedBoost = false;
        public static bool NoRagDoll = false;
        public static bool AntiKick = true;
        public static bool EarRape = false;
        public static bool SpinFace = false;
        public static int MoneyValue = 500;

        // Coloring for GUI
        public static Texture2D black;
        public Texture2D purpleLine;
        public static Texture2D toggleOn;
        public static Texture2D toggleOff;
        private static Texture2D settingsBackground;

        // Animation 
        private Rect currentPurpleLineRect = new Rect(10, 30, 100, 5);
        private Rect targetPurpleLineRect;
        public static float animationDuration = 0.3f;
        public static float animationStartTime;
        public static bool isAnimating = false;

        // Settings
        public static bool showSettingsWindow = false;
        public static float godModeHealthValue = 100f;
        public static Rect settingsWindowRect = new Rect(Screen.width - 320, 10, 300, 200);
        public static Rect targetSettingsWindowRect;
        public static bool isSettingsWindowAnimating = false;

        // Game
        public static List<BombItem> BombItem = new List<BombItem>();
        public static Player Player;
        public static PlayerController Controller;
        public static List<Player> players = new List<Player>();
        public static PhotonView PhotonView;
        public static List<PlayerInventory> playerinventory = new List<PlayerInventory>();
        private int InvSlot;
        private void Awake()
        {
            Player = FindObjectOfType<Player>();
            Controller = FindObjectOfType<PlayerController>();
        }

        private void OnGUI()
        {
            if (black == null) black = CreateTexture(Color.black);
            if (purpleLine == null) purpleLine = CreateTexture(Color.magenta, 1, 5);
            if (toggleOn == null || toggleOff == null)
            {
                toggleOn = CreateTexture(Color.green, 30, 30);
                toggleOff = CreateTexture(Color.red, 30, 30);
            }
            if (settingsBackground == null) settingsBackground = CreateTexture(Color.black, 300, 200);

            windowRect = GUI.Window(0, windowRect, WindowFunction, "");

            if (isAnimating)
            {
                float t = (Time.time - animationStartTime) / animationDuration;
                if (t >= 1f)
                {
                    t = 1f;
                    isAnimating = false;
                }
                currentPurpleLineRect = new Rect(
                    Mathf.Lerp(currentPurpleLineRect.x, targetPurpleLineRect.x, t),
                    Mathf.Lerp(currentPurpleLineRect.y, targetPurpleLineRect.y, t),
                    Mathf.Lerp(currentPurpleLineRect.width, targetPurpleLineRect.width, t),
                    Mathf.Lerp(currentPurpleLineRect.height, targetPurpleLineRect.height, t)
                );
            }

            if (isSettingsWindowAnimating)
            {
                float t = (Time.time - animationStartTime) / animationDuration;
                if (t >= 1f)
                {
                    t = 1f;
                    isSettingsWindowAnimating = false;
                }
                settingsWindowRect = new Rect(
                    Mathf.Lerp(settingsWindowRect.x, targetSettingsWindowRect.x, t),
                    Mathf.Lerp(settingsWindowRect.y, targetSettingsWindowRect.y, t),
                    settingsWindowRect.width,
                    settingsWindowRect.height
                );
            }

            if (showSettingsWindow)
            {
                settingsWindowRect = GUI.Window(1, settingsWindowRect, SettingsWindowFunction, "Settings");
            }
        }

        private void WindowFunction(int windowID)
        {
            GUI.DrawTexture(new Rect(0, 0, windowRect.width, windowRect.height), black);

            float lineY = Mathf.Max(mainTabRect.y + mainTabRect.height, settingsTabRect.y + settingsTabRect.height) + 5;
            GUI.DrawTexture(new Rect(currentPurpleLineRect.x, lineY, currentPurpleLineRect.width, currentPurpleLineRect.height), purpleLine);

            float contentHeight = 2500f; 
            scrollPos = GUI.BeginScrollView(new Rect(0, 20, windowRect.width, windowRect.height - 20), scrollPos, new Rect(0, 0, windowRect.width - 20, contentHeight));
            DrawTab(mainTabRect, "Main", ref showMain);
            DrawTab(settingsTabRect, "Settings", ref showSettings);

            if (showMain)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(70);

                ToggleButton("God Mode", ref GodMode);
                ToggleButton("Inf Stamina", ref infStamina);
                ToggleButton("Unlimited Oxygen", ref UnlimitedOxy);
                ToggleButton("Unlimited Battery", ref UnlimitedBattery);
                ToggleButton("Unlimited Film", ref UnlimitedFilm);
                ToggleButton("Speed Boost", ref SpeedBoost);
                ToggleButton("Inf Jumps", ref infJump);
                ToggleButton("NoRagDoll", ref NoRagDoll);
                ToggleButton("RainbowVisor", ref Rainbowvisor);
                ToggleButton("Spin Face", ref SpinFace);
                ToggleButton("Reverse Players(Works?)", ref ReversePeople);
                ToggleButton("Fling All", ref FlingAll);
                ToggleButton("No Movement All", ref NoMovement);
                ToggleButton("Freeze All", ref FreezeAll);
                ToggleButton("Ragdall All", ref RagdallAll);
                ToggleButton("Drag Players To You", ref Dragplayerstolocal);
                ToggleButton("EarRape", ref EarRape);
                ToggleButton("Spaz All", ref TaseAll);
                ToggleButton("Lag All", ref LagAll);
                ToggleButton("Speed All", ref SpeedAll);

                if (GUILayout.Button("Emote All"))
                {
                    MainUpdate.Emote(52);
                }
                if (GUILayout.Button("Clown All(Hat)"))
                {
                    foreach (Player Playa in MainUpdate.players)
                    {
                        if (!Playa.IsLocal)
                            Playa.Call_EquipHat(11);
                    }
                }
                if (GUILayout.Button("Break Game All"))
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<color=purple><size=1000>");
                    sb.Append('A', 9000);
                    sb.Append("</size></color>");
                    string longNickname = sb.ToString();
                    PhotonNetwork.LocalPlayer.NickName = longNickname;
                    PlayerPrefs.SetString("PhotonUsername", longNickname);
                    PhotonNetwork.LocalPlayer.NickName = longNickname;
                    MainUpdate.LocalHospitalBill(int.MaxValue);
                    string formattedFaceText = $"???";
                    foreach (Player player in UnityEngine.Object.FindObjectsOfType<Player>())
                    {
                        player.refs.view.RPC("RPCA_SetAllFaceSettings", RpcTarget.All, new object[]
                        {
                    1f,
                    0,
                    formattedFaceText,
                    1f,
                    3f
                        });
                    }
                }
                if (GUILayout.Button("Spawn Drones"))
                {
                    ShopHandler[] shops = FindObjectsOfType<ShopHandler>();
                    foreach (ShopHandler shop in shops)
                    {
                        var fieldInfo = typeof(ShopHandler).GetField("m_PhotonView", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fieldInfo != null)
                        {
                            PhotonView photonView = (PhotonView)fieldInfo.GetValue(shop);

                            byte[] items = new byte[] { byte.MaxValue };
                            photonView.RPC("RPCA_SpawnDrone", RpcTarget.All, items);
                        }
                    }
                }
                if (GUILayout.Button("Kick All(Works?)"))
                {
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        int actorNumber = player.ActorNumber;
                        PhotonNetwork.RaiseEvent(20, actorNumber, RaiseEventOptions.Default, SendOptions.SendReliable);
                    }
                    SurfaceNetworkHandler.Instance.photonView.RPC("RPC_LoadScene", RpcTarget.Others, "NewMainMenu");
                }
                if (GUILayout.Button("Kill All"))
                {
                    foreach (Player player in MainUpdate.players)
                    {
                        if (!player.IsLocal && !player.data.dead && player != null)
                        {
                            player.Sys<Player>().InvokeMethod("CallDie", Array.Empty<object>());
                            player.CallTakeDamage(999f);
                        }
                    }
                }
                if (GUILayout.Button("Give Money"))
                {
                    MainUpdate.HospitalBill(-MoneyValue);
                }
                if (GUILayout.Button("Remove Money"))
                {
                    MainUpdate.HospitalBill(MoneyValue);
                }
                if (GUILayout.Button("Give Money And Views"))
                {
                    var extractVideoMachine = Object.FindObjectOfType<ExtractVideoMachine>();
                    if (extractVideoMachine == null) return;

                    var photonViewField = typeof(ExtractVideoMachine).GetField("m_photonView", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (photonViewField == null) return;

                    var photonView = photonViewField.GetValue(extractVideoMachine) as PhotonView;
                    if (photonView == null) return;

                    var parameters = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, false };
                    photonView.RPC("RPC_ExtractDone", RpcTarget.All, parameters);
                }
                if (GUILayout.Button("Heal Others/ReviveAll"))
                {
                    foreach (Player player in UnityEngine.Object.FindObjectsOfType<Player>())
                    {
                        if (!player.IsLocal)
                        {
                            player.CallHeal(100f);
                        }
                        else if (player.data.dead && !player.IsLocal)
                        {
                            foreach (Player p in MainUpdate.players)
                            {
                                if (p.data.dead) p.CallRevive();
                            }
                        }
                    }
                }

                if (GUILayout.Button("BombAll"))
                {
                    foreach (Player player in UnityEngine.Object.FindObjectsOfType<Player>())
                    {
                        if (!player.IsLocal)
                        {
                            MainUpdate.SpawnItem("Bomb", player.refs.headPos.transform.position);
                        }
                    }
                }
                if (GUILayout.Button("Clear Inv All(Works?)"))
                {
                    foreach (PlayerInventory inventory in playerinventory)
                    {
                        foreach (Player player in players)
                        {
                            if (!player.IsLocal)
                            {
                                {
                                    inventory.SyncClearSlot(InvSlot);
                                }
                            }
                        }
                    }
                }
              

                GUILayout.EndVertical();
            }
            else if (showSettings)
            {
                GUILayout.Space(70f);
                GUILayout.Label("TODO: add color settings, add mods settings, add things on window like fps, ping, time");
                GUILayout.Space(10f);
                if (GUILayout.Button("Log Rpcs"))
                {
                    foreach (string Rpcs in PhotonNetwork.PhotonServerSettings.RpcList)
                    {
                        Debug.Log(Rpcs);
                    }
                }
                if (GUILayout.Button("How Many Online Players"))
                {
                    if (PhotonNetwork.IsConnected)
                    {
                        int playerCount = PhotonNetwork.CountOfPlayersInRooms;
                        Debug.Log($"players currently online: {playerCount}");
                    }
                    else
                    {
                        Debug.LogWarning("PhotonNetwork is not connected.");
                    }
                }
            }

            GUI.EndScrollView();
            GUI.DragWindow();
        }
        private void DrawTab(Rect rect, string label, ref bool showTab)
        {
            GUIStyle tabStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = black },
                active = { background = black },
                hover = { background = black },
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };

            if (GUI.Button(rect, label, tabStyle))
            {
                bool ShowMain = label == "Main";
                bool ShowSettings = label == "Settings";

                if (showMain != ShowMain || showSettings != ShowSettings)
                {
                    isAnimating = true;
                    animationStartTime = Time.time;
                    targetPurpleLineRect = new Rect(rect.x, rect.y + rect.height, rect.width, 5);
                    showMain = ShowMain;
                    showSettings = ShowSettings;
                }
            }
        }

        public static void ToggleButton(string label, ref bool toggle)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = black },
                hover = { background = black },
                active = { background = black }
            };

            Texture2D buttonTexture = toggle ? toggleOn : toggleOff;
            GUILayout.BeginHorizontal();

            GUIContent buttonContent = new GUIContent(buttonTexture);
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.Width(29), GUILayout.Height(29)))
            {
                toggle = !toggle;
            }
            GUILayout.Label(label, GUILayout.Width(150));

            if (GUILayout.Button("Settings", GUILayout.Width(100), GUILayout.Height(29)))
            {
                if (!showSettingsWindow)
                {
                    targetSettingsWindowRect = new Rect(Screen.width - 320, 10, 300, 200);
                    isSettingsWindowAnimating = true;
                    animationStartTime = Time.time;
                }
                showSettingsWindow = !showSettingsWindow;
            }

            GUILayout.EndHorizontal();
        }

        private void SettingsWindowFunction(int windowID)
        {
            GUI.DrawTexture(new Rect(0, 0, 300, 200), settingsBackground);
            GUILayout.BeginArea(new Rect(10, 30, 280, 160));

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16
            };

            GUILayout.Label("Adjust Mod Settings", labelStyle);

            godModeHealthValue = GUILayout.HorizontalSlider(godModeHealthValue, 5f, 100f);
            GUILayout.Label($"Speed Boost Speed: {SpeedValue:F1}", labelStyle);
            SpeedValue = GUILayout.HorizontalSlider(SpeedValue, 1f, 60f);
            GUILayout.Label($"Speed All Speed: {Anothervalue:F1}", labelStyle);
            MoneyValue = (int)GUILayout.HorizontalSlider(MoneyValue, 5f, 10000f);
            GUILayout.Label($"Money Value: {MoneyValue:F1}", labelStyle);

            GUILayout.EndArea();
            GUI.DragWindow();
        }

        private Texture2D CreateTexture(Color color, int width = 1, int height = 1)
        {
            var texture = new Texture2D(width, height);
            var colorArray = new Color[width * height];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = color;
            }
            texture.SetPixels(colorArray);
            texture.Apply();

            return texture;
        }
        public static float Anothervalue = 27f;
        public static float SpeedValue = 27f;
        private float SpingRot;
        public static bool TaseAll;
        private static readonly object keyByteZero;
        public static PlayerRagdoll PlayerRagdoll;
        public static ShopHandler ShopHandler;
        void Update()
        {
            if (Player == null || Controller == null || ShopHandler == null || PlayerRagdoll == null)
            {
                Player = FindObjectOfType<Player>();
                Controller = FindObjectOfType<PlayerController>();
                PlayerRagdoll = FindObjectOfType<PlayerRagdoll>();
                ShopHandler = FindObjectOfType<ShopHandler>();

                if (Player == null || Controller == null || ShopHandler == null || PlayerRagdoll == null)
                    return;
            }
            
            if (SpinFace)
            {
                PlayerVisor visor = Player.localPlayer.refs.visor;
                visor.SetAllFaceSettings(visor.hue.Value, visor.visorColorIndex, visor.visorFaceText.text, SpingRot, visor.FaceSize);
                SpingRot += 1f;
                MainUpdate.Clamp(SpingRot, 0f, 360f);
            }
            if (SpeedBoost)
            {
                Controller.movementForce = SpeedValue;
            }
            else if (!SpeedBoost)
            {
                Controller.movementForce = 17f;
            }
            if (NoMovement && !Player.IsLocal && !Player.ai)
            {
                foreach (Player player in MainUpdate.players)
                {
                    float slowAmount = (NoMovement && !player.IsLocal && !player.ai) ? 0.001f : 1f;
                    player.refs.view.RPC("RPCA_SlowFor", RpcTarget.Others, new object[] { slowAmount, 1f });
                }
            }

            if (SpeedAll && !Player.IsLocal && !Player.ai)
            {
                Player.refs.view.RPC("RPCA_SlowFor", RpcTarget.All, Anothervalue, 1f);
            }
            else if (!SpeedAll)
            {
                Player.refs.view.RPC("RPCA_SlowFor", RpcTarget.All, 1f, 1f);
            }

            if (infJump)
            {
                Player.localPlayer.data.sinceJump = 0.7f;
            }

            if (EarRape)
            {
                PhotonView.Get(this).RPC("RPCA_AddItemToCart", RpcTarget.All, 6);
            }

            if (TaseAll)
            {
                foreach (Player p in FindObjectsOfType<Player>())
                {
                    if (!p.IsLocal)
                    p.refs.view.RPC("RPCA_CallTakeDamageAndTase", RpcTarget.All, 0.1f, 1f);
                }
            }

            if (FreezeAll && Player != null && !Player.IsLocal)
            {
                Hashtable hashtable = new Hashtable { [0] = -1 };
                PhotonNetwork.NetworkingClient.OpRaiseEvent(207, hashtable, RaiseEventOptions.Default, SendOptions.SendReliable);
                PhotonNetwork.NetworkingClient.OpRaiseEvent(202, hashtable, RaiseEventOptions.Default, SendOptions.SendReliable);
            }

            if (infStamina)
            {
                Player.localPlayer.data.currentStamina = Player.localPlayer.refs.controller.maxStamina;
            }

            if (FlingAll)
            {
                foreach (Player p in FindObjectsOfType<Player>())
                {
                    
                    p.refs.view.RPC("RPCA_TakeDamageAndAddForce", RpcTarget.Others, 0f, Player.refs.cameraPos.up * 50f, 3f);
                }
            }

            if (LagAll)
            {
                MainUpdate.HospitalBill(int.MaxValue);
            }

            if (NoRagDoll)
            {
                Player.data.fallTime = 0f;
            }

            if (RagdallAll)
            {
                foreach (Player p in FindObjectsOfType<Player>())
                {
                    if (!p.IsLocal)
                    {
                        p.refs.view.RPC("RPCA_TakeDamageAndAddForce", RpcTarget.Others, 0f, Vector3.down * 20f, 1f);
                        p.refs.view.RPC("RPCA_Fall", RpcTarget.Others, 1f);
                        p.data.fallTime = 100f;
                    }
                }
            }

            if (UnlimitedFilm)
            {
                var item = Player.localPlayer?.data.currentItem;
                if (item != null)
                {
                    var camera = item.GetComponent<VideoCamera>();
                    if (camera != null)
                    {
                        var infoEntry = MainUpdate.GetField<VideoInfoEntry>(camera, "m_recorderInfoEntry");
                        if (infoEntry != null)
                        {
                            infoEntry.timeLeft = infoEntry.maxTime;
                        }
                    }
                }
            }

            if (Rainbowvisor)
            {
                float time = Time.time;
                float red = Mathf.Clamp01((Mathf.Sin(time) + 1f) * 0.5f);
                float green = Mathf.Clamp01((Mathf.Sin(time + Mathf.PI * 2f / 3f) + 1f) * 0.5f);
                float blue = Mathf.Clamp01((Mathf.Sin(time + Mathf.PI * 4f / 3f) + 1f) * 0.5f);

                Color visorColor = new Color(red, green, blue);
                Player.localPlayer.refs.visor.ApplyVisorColor(visorColor);
            }

            if (ReversePeople)
            {
                foreach (Player p in FindObjectsOfType<Player>())
                {
                    if (!p.IsLocal)
                        p.refs.view.RPC("RPCA_SlowFor", RpcTarget.Others, -1f, 1f);
                }
            }

            if (UnlimitedBattery)
            {
                var item = Player.localPlayer?.data.currentItem;
                if (item != null)
                {
                    var typesWithBattery = new[]
                    {
                    typeof(Flashlight),
                    typeof(Defib),
                    typeof(ShockStick),
                    typeof(RescueHook),
                    typeof(NorgGun)
                };

                    foreach (var type in typesWithBattery)
                    {
                        var component = item.GetComponent(type);
                        if (component != null)
                        {
                            var field = type.GetField("m_batteryEntry", BindingFlags.NonPublic | BindingFlags.Instance);
                            var battery = field?.GetValue(component) as BatteryEntry;
                            if (battery != null)
                            {
                                battery.AddCharge(10_000f);
                                break;
                            }
                        }
                    }

                    var partyPopper = item.GetComponent<PartyPopper>();
                    if (partyPopper != null)
                    {
                        var usedEntryField = typeof(PartyPopper).GetField("usedEntry", BindingFlags.NonPublic | BindingFlags.Instance);
                        var stashAbleEntryField = typeof(PartyPopper).GetField("stashAbleEntry", BindingFlags.NonPublic | BindingFlags.Instance);
                        var usedEntry = usedEntryField?.GetValue(partyPopper) as OnOffEntry;
                        var stashAbleEntry = stashAbleEntryField?.GetValue(partyPopper) as StashAbleEntry;

                        if (usedEntry != null && stashAbleEntry != null)
                        {
                            usedEntry.on = false;
                            stashAbleEntry.isStashAble = true;
                            stashAbleEntry.ClearDirty();
                            usedEntry.ClearDirty();
                            partyPopper.chargesLeftGO.SetActive(true);
                            var wasUsedOnConfigField = typeof(PartyPopper).GetField("wasUsedOnConfig", BindingFlags.NonPublic | BindingFlags.Instance);
                            wasUsedOnConfigField?.SetValue(partyPopper, false);
                        }
                    }

                    var flare = item.GetComponent<Flare>();
                    if (flare != null)
                    {
                        var lifeTimeEntryField = typeof(Flare).GetField("m_lifeTimeEntry", BindingFlags.NonPublic | BindingFlags.Instance);
                        var lifeTimeEntry = lifeTimeEntryField?.GetValue(flare) as LifeTimeEntry;
                        if (lifeTimeEntry != null)
                        {
                            lifeTimeEntry.m_lifeTimeLeft = flare.maxLifeTime;
                        }
                    }
                }
            }
            if (UnlimitedOxy)
            {
                Player.data.remainingOxygen = Player.data.maxOxygen;
            }

            if (Dragplayerstolocal && Player.localPlayer != null)
            {
                Vector3 localPlayerPosition = Player.localPlayer.refs.headPos.position;

                foreach (var p in FindObjectsOfType<Player>())
                {
                    if (p == Player.localPlayer) continue;

                    Vector3 direction = localPlayerPosition - p.refs.headPos.position;
                    p.refs.view.RPC("RPCA_TakeDamageAndAddForce", RpcTarget.All, 0f, direction.normalized * 8f, 1.5f);
                }
            }

            if (GodMode)
            {
                Player.data.health = 100f;
                Player.data.dead = false;

                if (Player.data.dead && Player.IsLocal)
                {
                    Player.CallRevive();
                    Player.CallHeal(100f);
                    Player.localPlayer.CallRevive();
                    foreach (var p in MainUpdate.players.Where(p => p.IsLocal))
                        p.Sys().InvokeMethod("RPCA_Heal", 100f);
                }
            }
        }
    }
}