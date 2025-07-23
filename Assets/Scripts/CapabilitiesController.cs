using System.Collections.Generic;
using UnityEngine;
using UnityGCC.Capabilities;

namespace UnityGCC
{
    public class CapabilitiesController : MonoBehaviour
    {
        public static CapabilitiesController Instance;

        public Instigator Instigator;

        public Dictionary<TickGroup,List<BaseCapabilities>> Capabilities;
        private void Awake()
        {
            Instigator = new Instigator(this);
            Capabilities = new Dictionary<TickGroup, List<BaseCapabilities>>();
            for (int i = 0; i < (int)TickGroup.PostWork; i++)
            {
                Capabilities.Add((TickGroup)i,new List<BaseCapabilities>());
            }
            Instance = this;
        }

        public void RegisterCapability(BaseCapabilities capability)
        {
            Capabilities ??= new Dictionary<TickGroup, List<BaseCapabilities>>();
            Capabilities.TryAdd(capability.TickGroup, new List<BaseCapabilities>());
            Capabilities[capability.TickGroup].Add(capability);
        }
        
        public void RemoveCapability(BaseCapabilities capability)
        {
            Capabilities ??= new Dictionary<TickGroup, List<BaseCapabilities>>();
            Capabilities.TryAdd(capability.TickGroup, new List<BaseCapabilities>());
            Capabilities[capability.TickGroup].Remove(capability);
        }

        public void Update()
        {
            for (int i = 0; i <= (int)TickGroup.PostWork; i++)
            {
                var tickGroup = (TickGroup)i;
                if(!Capabilities.ContainsKey(tickGroup))
                    Capabilities.Add(tickGroup, new List<BaseCapabilities>());
                var tickGroupCapabilities = Capabilities[tickGroup];
                foreach (var capability in tickGroupCapabilities)
                {
                    if (!capability.bActive && capability.ShouldActivated())
                    {
                        capability.OnActivated();
                    }
                    else if (capability.bActive)
                    {
                        if(capability.ShouldDeActivated())
                            capability.OnDeActivated();
                    }
                    capability.TickActive(Time.deltaTime);
                }
            }
        }
    }

    public enum TagEnum
    {
        None = 0,
    }
    
    public enum TickGroup
    {
        SeparatedTickOrder = 0,
        Input = 1,
        BeforeMovement = 2,
        InfluenceMovement = 3,
        ActionMovement = 4,
        Movement = 5,
        LastMovement = 6,
        BeforeGameplay = 7,
        Gameplay = 8,
        AfterGameplay = 9,
        AfterPhysics = 10,
        Audio = 11,
        PostWork = 12
    }
}