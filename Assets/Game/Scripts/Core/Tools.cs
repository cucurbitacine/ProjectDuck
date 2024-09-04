using UnityEngine;

namespace Game.Scripts.Core
{
    public static class Tools
    {
        public static void DrawBox2D(Vector2 center, Vector2 size, float angle = 0f)
        {
            var rotor = Quaternion.Euler(0, 0, angle);

            var a = new Vector2(size.x, size.y) * 0.5f;
            var b = new Vector2(size.x, -size.y) * 0.5f;
            var c = new Vector2(-size.x, -size.y) * 0.5f;
            var d = new Vector2(-size.x, size.y) * 0.5f;

            a = center + (Vector2)(rotor * a);
            b = center + (Vector2)(rotor * b);
            c = center + (Vector2)(rotor * c);
            d = center + (Vector2)(rotor * d);

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }

        public static void DrawCircle2D(Vector2 center, float radius, int number = 36)
        {
            var first = Vector2.zero;
            var last = Vector2.zero;
            for (var i = 0; i < number; i++)
            {
                var phi = 2f * Mathf.PI * i / number;
                var point = center + new Vector2(Mathf.Sin(phi), Mathf.Cos(phi)) * radius;
                
                if (i > 0)
                {
                    Gizmos.DrawLine(last, point);
                }
                else
                {
                    first = point;
                }

                last = point;
            }
            
            Gizmos.DrawLine(last, first);
        }
        
        public static bool TryGet<T>(this Collider2D collider2D, out T component)
        {
            if (collider2D.TryGetComponent(out component))
            {
                return true;
            }

            if (collider2D.attachedRigidbody && collider2D.attachedRigidbody.TryGetComponent(out component))
            {
                return true;
            }

            return false;
        }
        
        public static string GetName(this Component component)
        {
            return $"{component.name} ({component.GetType().Name})";
        }
        
        public static string GetName(this object obj)
        {
            return obj is Component component ? component.GetName() : obj.GetType().Name;
        }
    }
}