
using UnityEngine;

public static class GizmoUtility
{
    #region PUBLIC API

    public static void DrawArrow(Vector3 i_from, Vector3 i_to, float i_width, Color i_color, float i_arrowHeadLength = 0.25f, float i_arrowHeadAngle = 20.0f, float i_arrowPosition = 0f)
    {
        DrawLine(i_from, i_to, i_width, i_color);
        Vector3 direction = i_to - i_from;
        drawArrowEnd(i_to, direction, i_width, i_color, i_arrowHeadLength, i_arrowHeadAngle, i_arrowPosition);
    }

    public static void DrawLine(Vector3 i_p1, Vector3 i_p2, float i_width, Color i_color)
    {
        Color col = ApplyGizmoColor(i_color);

        drawLine(i_p1, i_p2, i_width);

        Gizmos.color = col;
    }

    public static Color ApplyGizmoColor(Color i_color)
    {
        Color originalCol = Gizmos.color;

        Gizmos.color = i_color;

        return originalCol;
    }

    #endregion

    #region PRIVATE

    private static void drawArrowEnd(Vector3 pos, Vector3 direction, float i_width, Color i_color, float i_arrowHeadLength = 0.25f, float i_arrowHeadAngle = 20.0f, float i_arrowPosition = 0f)
    {
        Quaternion lookRotation = direction == MathConstants.VECTOR_3_ZERO ? Quaternion.identity : Quaternion.LookRotation(direction);

        Vector3 right = (lookRotation * Quaternion.Euler(i_arrowHeadAngle, 0, 0) * MathConstants.VECTOR_3_BACK) * i_arrowHeadLength;
        Vector3 left = (lookRotation * Quaternion.Euler(-i_arrowHeadAngle, 0, 0) * MathConstants.VECTOR_3_BACK) * i_arrowHeadLength;
        Vector3 up = (lookRotation * Quaternion.Euler(0, i_arrowHeadAngle, 0) * MathConstants.VECTOR_3_BACK) * i_arrowHeadLength;
        Vector3 down = (lookRotation * Quaternion.Euler(0, -i_arrowHeadAngle, 0) * MathConstants.VECTOR_3_BACK) * i_arrowHeadLength;

        Vector3 arrowTip = pos + (direction * i_arrowPosition);

        DrawLine(arrowTip, arrowTip + right, i_width, i_color);
        DrawLine(arrowTip, arrowTip + left, i_width, i_color);
        DrawLine(arrowTip, arrowTip + up, i_width, i_color);
        DrawLine(arrowTip, arrowTip + down, i_width, i_color);
    }

    private static void drawLine(Vector3 i_p1, Vector3 i_p2, float i_width)
    {
        int count = 1 + Mathf.CeilToInt(i_width);
        if (count == 1)
        {
            Gizmos.DrawLine(i_p1, i_p2);
        }
        else
        {
            Camera c = Camera.current;
            if (c == null)
            {
                Debug.LogError("Camera.current is null");
                return;
            }
            var scp1 = c.WorldToScreenPoint(i_p1);
            var scp2 = c.WorldToScreenPoint(i_p2);

            Vector3 v1 = (scp2 - scp1).normalized; 
            Vector3 n = Vector3.Cross(v1, Vector3.forward); 

            for (int i = 0; i < count; i++)
            {
                Vector3 o = 0.99f * n * i_width * ((float)i / (count - 1) - 0.5f);
                Vector3 origin = c.ScreenToWorldPoint(scp1 + o);
                Vector3 destiny = c.ScreenToWorldPoint(scp2 + o);
                Gizmos.DrawLine(origin, destiny);
            }
        }
    }

    #endregion
}
