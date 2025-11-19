using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temp
{
    public class OctreeTest : MonoBehaviour
    {
        [SerializeField] private float size = 100;
        [SerializeField] private int nodeCapcity = 10;
        [SerializeField] private float pointCount = 1000;

        private bool generated = false;
        private Octree octree;

        private void Start()
        {
            if (octree == null)
            {
                InitializeOctree();
            }

            GeneratePoints();
        }

        private void OnDrawGizmos()
        {
            if (generated) octree.DrawGizmos();
        }

        private void InitializeOctree()
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.one * size);
            octree = new Octree(bounds, nodeCapcity);
            Debug.Log("八叉树初始化完成");
        }

        private void GeneratePoints()
        {
            List<Vector3> allPoints = new List<Vector3>();
            float pointSpread = size * 0.5f;
            for (int i = 0; i < pointCount; i++)
            {
                Vector3 point = new Vector3(
                    Random.Range(-pointSpread, pointSpread),
                    Random.Range(-pointSpread, pointSpread),
                    Random.Range(-pointSpread, pointSpread)
                );
                allPoints.Add(point);
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.SetParent(transform);
                obj.transform.position = point;
                obj.transform.localScale = Vector3.one;
            }

            int insertCount = octree.InsertBatch(allPoints);
            generated = true;
            Debug.Log($"成功插入 {insertCount}/{allPoints.Count} 个点到八叉树");
        }
    }

}
