using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zorro.Core;
using Zorro.UI;

namespace ContentWarning.GUI
{
    public class MainUpdate : MonoBehaviour
    {
        public static Player localPlayer;
        public static PlayerController PlayerController;
        public static Camera cam;
        public static List<Player> players = new List<Player>();

        public static T GetField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return field != null ? (T)field.GetValue(obj) : default;
        }
        public static string GeneratePackedString(int repetitions)
        {
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < repetitions; i++)
            {
                sb.Append("PACKED");
            }

            return sb.ToString();
        }
        public static void Emote(byte id)
        {
            foreach (Player Playa in UnityEngine.Object.FindObjectsOfType<Player>())
            {
                if (!Playa.IsLocal)
                {
                    Playa.refs.view.RPC("RPC_PlayEmote", RpcTarget.All, new object[] { id });
                }
            }
        }
        public static void SpawnItem(string ItemName, Vector3 pos)
        {
            Item item = SingletonAsset<ItemDatabase>.Instance.lastLoadedItems.FirstOrDefault((Item i) => i.name == ItemName);
            if (!(item == null))
            {
                Player.localPlayer.RequestCreatePickup(item, new ItemInstanceData(Guid.NewGuid()), pos, Quaternion.identity);
            }
        }
            public static void HospitalBill(int amount) // Use this too add or remove money and if you spam it lags people (: does not need host btw
        {
            var otherPlayers = PhotonNetwork.PlayerListOthers;
            Photon.Realtime.Player Players = otherPlayers.Length > 0 ? otherPlayers[0] : PhotonNetwork.LocalPlayer;
            var actorDict = new Dictionary<int, int>
    {
        { Players.ActorNumber, amount }
    };

            var actorList = actorDict
                .Select(entry => (ValueTuple<int, int>)new ValueTuple<int, int>(entry.Key, entry.Value))
                .ToList();

            var handler = SurfaceNetworkHandler.Instance;

            MethodInfo sendBillMethod = typeof(SurfaceNetworkHandler).GetMethod(
                "SendHospitalBill",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (sendBillMethod != null)
            {
                sendBillMethod.Invoke(handler, new object[] { actorList });
            }
        }
        public static string itemName;

        public static string GetName()
        {
            return itemName;
        }
        public static Item FindItemByName(string itemName)
        {
            var items = ItemDatabase.Instance.Objects;

            Item item = Array.Find(items, x => GetName().Equals(itemName, StringComparison.OrdinalIgnoreCase));

            if (item != null)
            {
                return item;
            }
            else
            {
                Debug.LogWarning("Item not found: " + itemName);
                return null;
            }
        }
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static void SpawnItem(string itemName)
        {
            Debug.Log("spawning item: " + itemName);

            Item item = FindItemByName(itemName);
            if (item == null) return;
            Photon.Realtime.Player[] allPlayers = PhotonNetwork.PlayerList;

            foreach (var player in allPlayers)
            {
                if (player == null) continue;

                PhotonView photonView = PhotonView.Find(player.ActorNumber);
                if (photonView == null) continue;

                PlayerController playerController = photonView.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    Vector3 playerPosition = playerController.transform.position;

                    Player.localPlayer.RequestCreatePickup(item, new ItemInstanceData(Guid.NewGuid()), playerPosition, Quaternion.identity);
                }
            }
        }

        public static Vector3 GetDebugItemSpawnPos()
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out var hitInfo, 10f, HelperFunctions.GetMask(HelperFunctions.LayerType.All)))
            {
                return hitInfo.point + hitInfo.normal * 0.25f;
            }

            return ray.GetPoint(10f);
        }
    
        public static T RetrieveValue<T>(object instance, string name, bool isStatic = false, bool isProperty = false)
        {
            BindingFlags bindingFlags = BindingFlags.Instance |
                                         BindingFlags.NonPublic |
                                         BindingFlags.Public |
                                         (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                                         (isProperty ? BindingFlags.GetProperty : BindingFlags.GetField);

            return isProperty ? FetchProperty<T>(instance, name, bindingFlags) : FetchField<T>(instance, name, bindingFlags);
        }

        private static T FetchField<T>(object instance, string fieldName, BindingFlags bindingFlags)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance), "Instance cannot be null.");
            }

            Type objectType = instance.GetType();
            FieldInfo field = objectType.GetField(fieldName, bindingFlags);

            if (field == null)
            {
                throw new ArgumentException($"Field named '{fieldName}' is not available.");
            }

            return (T)field.GetValue(instance);
        }
        public static void TransitionToPage<T>() where T : UIPage
        {
            UnityEngine.Object .FindObjectOfType<UIPageHandler>().TransistionToPage<T>();
        }
        private static T FetchProperty<T>(object instance, string propertyName, BindingFlags bindingFlags)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance), "Instance cannot be null.");
            }

            Type objectType = instance.GetType();
            PropertyInfo property = objectType.GetProperty(propertyName, bindingFlags);

            if (property == null)
            {
                throw new ArgumentException($"Property named '{propertyName}' is not available.");
            }

            return (T)property.GetValue(instance);
        }
        public static void LocalHospitalBill(int amount) // only using this for breakgameall
        {
            var actorDict = new Dictionary<int, int>
        {
            { PhotonNetwork.LocalPlayer.ActorNumber, amount }
        };
            var actorList = actorDict
                .Select(entry => (ValueTuple<int, int>)new ValueTuple<int, int>(entry.Key, entry.Value))
                .ToList();

            var handler = SurfaceNetworkHandler.Instance;

            MethodInfo sendBillMethod = typeof(SurfaceNetworkHandler).GetMethod(
                "SendHospitalBill",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (sendBillMethod != null)
            {
                sendBillMethod.Invoke(handler, new object[] { actorList });
            }
        }
            public static void Update() //Using this for some random things 
        {
            RPCManager.ClearRPCs();
        }
    }
}
