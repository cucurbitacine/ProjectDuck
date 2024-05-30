using UnityEngine;

namespace Game.Utils
{
    public static class Tools
    {
        public static void DrawBox(Vector2 center, Vector2 size, float angle)
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

        public static bool TryGet<T>(this Collider2D collider2D, out T component) where T : Component
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
    }
}