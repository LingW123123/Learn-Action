using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Temp
{
    public class OctreeNode
    {
        public Bounds bounds;
        public int capcity;
        public int depth;
        public bool isDivided = false;

        public OctreeNode[] children;
        public List<Vector3> points;

        public OctreeNode(Bounds bounds, int capcity, int depth)
        {
            this.bounds = bounds;
            this.capcity = capcity;
            this.depth = depth;

            children = new OctreeNode[8];
            points = new List<Vector3>(capcity);
        }

        public bool Insert(Vector3 point)
        {
            if (!bounds.Contains(point)) return false;

            if (!isDivided && points.Count < capcity)
            {
                points.Add(point);
                return true;
            }

            if (!isDivided)
            {
                Subdivide();
            }

            return InsertToChildren(point);
        }

        public bool InsertToChildren(Vector3 point)
        {
            foreach(var child in children)
            {
                if (child.bounds.Contains(point))
                {
                    return child.Insert(point);
                }
            }

            return false;
        }

        private void Subdivide()
        {
            if (isDivided) return;
            int index = 0;
            Vector3 quarterSize = bounds.size * 0.25f;
            for(int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        Vector3 offset = Vector3.Scale(quarterSize, new Vector3(x, y, z));
                        Vector3 childCenter = bounds.center + offset;
                        Bounds childBounds = new Bounds(childCenter, bounds.extents);

                        children[index] = new OctreeNode(childBounds, capcity, depth + 1);
                        index++;
                    }
                }
            }

            foreach(var point in points)
            {
                InsertToChildren(point);
            }

            isDivided = true;
            points.Clear();
            points = null;
        }

        public List<Vector3> QueryRange(Bounds bounds)
        {
            List<Vector3> results = new List<Vector3>();
            if (!bounds.Intersects(bounds)) return results;

            if (isDivided)
            {
                foreach(var child in children)
                {
                    results.AddRange(child.QueryRange(bounds));
                }
            }
            else
            {
                foreach(var point in points)
                {
                    if (bounds.Contains(point))
                    {
                        results.Add(point);
                    }
                }
            }

            return results;
        }

        public void DrawGizmos()
        {
            if (isDivided)
            {
                foreach(var child in children)
                {
                    child.DrawGizmos();
                }
            }
            else
            {
                if(points.Count > 0)
                {
                    Color color = Color.Lerp(Color.red, Color.blue, depth / 8f);

                    Gizmos.color = color;
                    Gizmos.DrawWireCube(bounds.center, bounds.size);
                }

            }
        }
    }

    public class Octree
    {
        public OctreeNode octree;
        public int pointCount;

        public Octree(Bounds bounds, int capcity = 10)
        {
            octree = new OctreeNode(bounds, capcity, 0);
        }

        public bool Insert(Vector3 point)
        {
            if (octree.Insert(point))
            {
                pointCount++;
                return true;
            }

            return false;
        }

        public int InsertBatch(IEnumerable<Vector3> points)
        {
            int insertCount = 0;
            foreach(var point in points)
            {
                octree.Insert(point);
                insertCount++;
            }

            return insertCount;
        }

        public void DrawGizmos()
        {
            octree.DrawGizmos();
        }
    }

}
