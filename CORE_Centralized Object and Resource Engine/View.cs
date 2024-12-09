using UnityEngine;

namespace CORE
{
    public class View : MonoBehaviour
    {
        public virtual void OnUpdate()
        {

        }

        public virtual void OnLateUpdate()
        {

        }

        public virtual void OnFixedUpdate()
        {

        }

        public void OnEnable()
        {
            if (!Bootstrap)
            {
                Bootstrap = FindFirstObjectByType<Bootstrap>();
            }
            Bootstrap.AddView(this);
        }

        public void OnDisable()
        {
            Bootstrap.RemoveView(this);
        }

        private Bootstrap Bootstrap;
    }
}
