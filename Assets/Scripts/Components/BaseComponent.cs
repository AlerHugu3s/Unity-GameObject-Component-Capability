using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityGCC.Capabilities;

namespace UnityGCC.Components
{
    public class BaseComponent : MonoBehaviour
    {
        public Dictionary<Instigator,HashSet<TagEnum>> BlockedTags;

        public bool IsTagBlocked(TagEnum blockTag,Instigator instigator = null)
        {
            instigator ??= CapabilitiesController.Instance.Instigator;
            if (BlockedTags.TryGetValue(instigator, out var blockedTag))
            {
                return true;
            }
            return false;
        }

        public void BlockTags(IEnumerable<TagEnum> blockTags,Instigator instigator = null)
        {
            instigator ??= CapabilitiesController.Instance.Instigator;
            if (BlockedTags.ContainsKey(instigator))
            {
                BlockedTags[instigator].AddRange(blockTags);
            }
            else
            {
                BlockedTags.Add(instigator,new HashSet<TagEnum>());
                BlockedTags[instigator].AddRange(blockTags);
            }
        }
        
        public void UnBlockTags(IEnumerable<TagEnum> blockTags,Instigator instigator = null)
        {
            instigator ??= CapabilitiesController.Instance.Instigator;
            if (BlockedTags.ContainsKey(instigator))
            {
                BlockedTags[instigator].RemoveWhere(blockTags.Contains);
            }
        }

        protected virtual void Awake()
        {
            BlockedTags = new Dictionary<Instigator,HashSet<TagEnum>> { { CapabilitiesController.Instance.Instigator, new HashSet<TagEnum>() } };
        }
    }
}