using System.Collections;
using UnityEngine;

namespace TeamFourteen.AI
{
    public partial class EnemyMovement
    {
        abstract class BehaviourLoop
        {
            public BehaviourLoop(EnemyMovement enemyMovement)
            {
                this.enemyMovement = enemyMovement;
            }

            private const float lookAroundSpeed = 0.3f;
            protected readonly EnemyMovement enemyMovement;

            public abstract void Start();
            public abstract void Stop();

            protected IEnumerator LookAround()
            {
                float initialYRotation = enemyMovement.transform.localEulerAngles.y;
                for (float i = 0; i < 1; i += Time.deltaTime * lookAroundSpeed)//0.00225f)
                {
                    enemyMovement.transform.localEulerAngles = new Vector3(0, initialYRotation + enemyMovement.lookBehaviourCurve.Evaluate(i) * 360, 0);
                    yield return null;
                }

                enemyMovement.transform.localEulerAngles = new Vector3(0, initialYRotation, 0);
            }
        }
    }
}