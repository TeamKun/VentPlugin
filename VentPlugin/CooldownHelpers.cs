﻿using UnityEngine;

namespace VentPlugin
{
    public static class CooldownHelpers
    {
        public static void SetCooldownNormalizedUvs(this SpriteRenderer myRend)
        {
            Vector2[] uv = myRend.sprite.uv;
            Vector4 vector = new Vector4(2f, -1f, 2f, -1f);
            for (int i = 0; i < uv.Length; i++)
            {
                if (vector.x > uv[i].x)
                {
                    vector.x = uv[i].x;
                }
                if (vector.y < uv[i].x)
                {
                    vector.y = uv[i].x;
                }
                if (vector.z > uv[i].y)
                {
                    vector.z = uv[i].y;
                }
                if (vector.w < uv[i].y)
                {
                    vector.w = uv[i].y;
                }
            }
            vector.y -= vector.x;
            vector.w -= vector.z;
            myRend.material.SetVector("_NormalizedUvs", vector);
        }
    }
}