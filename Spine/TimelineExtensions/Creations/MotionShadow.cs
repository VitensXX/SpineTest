using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

namespace Game.Render.Object
{
    public class MotionShadow : MonoBehaviour
    {
        public GameObject originalGo;
        public float lifeTime;
        private float initialAlpha;

        private MeshRenderer[] _originalRenderers;

        public void CreateShadow(GameObject originalGo, float lifeTime, float initialAlpha)
        {
            this.originalGo = originalGo;
            this.lifeTime = lifeTime;
            this.initialAlpha = initialAlpha;

            MeshRenderer[] originalRenderers = originalGo.GetComponentsInChildren<MeshRenderer>();

            if (originalRenderers == null)
                return;

            for (int i = 0; i < originalRenderers.Length; i++)
            {
                MeshFilter meshFilter = originalGo.GetComponent<MeshFilter>();
                Mesh copyMesh = Instantiate(meshFilter.sharedMesh);
                MeshFilter copyMeshFilter = gameObject.AddComponent<MeshFilter>();
                copyMeshFilter.mesh = copyMesh;

                MeshRenderer renderer = originalRenderers[i];
                MeshRenderer copyRenderer = gameObject.AddComponent<MeshRenderer>();

                int count = renderer.sharedMaterials.Length;
                Material[] copyMaterials = new Material[count];
                for (int j = 0; j < count; j++)
                {
                    Material material = Instantiate(renderer.sharedMaterials[j]);
                    copyMaterials[j] = material;
                }
                copyRenderer.materials = copyMaterials;

                copyRenderer.transform.position = renderer.transform.position;
                copyRenderer.transform.localScale = renderer.transform.localScale;
                copyRenderer.transform.rotation = renderer.transform.rotation;

                for (int j = 0; j < copyMaterials.Length; j++)
                {
                    copyMaterials[j].DOColor(new Color(1, 1, 1, 0), "_Color", lifeTime).SetEase(Ease.Linear);
                }
            }

            Sequence seq = DOTween.Sequence().AppendInterval(lifeTime).AppendCallback(()=> { Complete(); });
        }

        private void Complete()
        {
            GameObject.Destroy(gameObject);
        }
    }
}