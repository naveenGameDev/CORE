using System.Collections.Generic;
using UnityEngine;

namespace CORE
{
    /// <summary>
    /// class where non reflection monobehaviour updates are handled
    /// </summary>
    public class ViewContext : MonoBehaviour
    {
        private readonly List<View> Subscriptions = new();

        private void Update()
        {
            for (int i = 0; i < Subscriptions.Count; i++)
            {
                Subscriptions[i].OnUpdate();
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < Subscriptions.Count; i++)
            {
                Subscriptions[i].OnLateUpdate();
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < Subscriptions.Count; i++)
            {
                Subscriptions[i].OnFixedUpdate();
            }
        }

        public void SubscribeForLifeCycleUpdates(View view)
        {
            if (!Subscriptions.Contains(view))
            {
                Subscriptions.Add(view);
            }
        }

        public void UnsubscribeForLifeCycleUpdates(View view)
        {
            Subscriptions.Remove(view);
        }

    }
}
