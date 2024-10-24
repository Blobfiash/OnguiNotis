using UnityEngine;
using System.Collections.Generic;

namespace UnityCheatsGorillaTag.Mods
{
    public class NotiOngui : MonoBehaviour
    {
        private static float displayTime = 2f;
        private static float fadeDuration = 0.5f;
        private static float slideDuration = 0.6f;

        private static List<NotificationData> activeNotis = new List<NotificationData>();
        private const int MaxNotis = 5;

        private void Update()
        {
            if (activeNotis.Count > 0)
            {
                Display();
            }
        }

        private void OnGUI()
        {
            ShowNotifications();
        }

        private void Display()
        {
            List<NotificationData> toRemove = new List<NotificationData>();

            foreach (var notification in activeNotis)
            {
                if (notification.IsFading)
                {
                    notification.Alpha -= Time.deltaTime / fadeDuration;
                    notification.Scale = Mathf.Lerp(1.0f, 0.5f, 1 - (notification.Alpha / 1.0f));
                    if (notification.Alpha <= 0)
                    {
                        toRemove.Add(notification);
                    }
                }
                else
                {
                    notification.Timer += Time.deltaTime;
                    notification.Alpha = Mathf.Clamp(notification.Timer / fadeDuration, 0, 1);
                    notification.PositionOffset = Mathf.Lerp(-100, 0, notification.Timer / slideDuration);
                    notification.Scale = Mathf.Lerp(0.5f, 1.0f, notification.Timer / fadeDuration);

                    if (notification.Timer > displayTime)
                    {
                        notification.IsFading = true;
                        notification.Timer = 0f;
                    }
                }
            }

            foreach (var notification in toRemove)
            {
                activeNotis.Remove(notification);
            }
        }

        private void ShowNotifications()
        {
            float width = 320;
            float height = 140;
            float x = Screen.width - width - 10;
            float spacing = 20f;
            float padding = 15f;

            for (int i = 0; i < activeNotis.Count; i++)
            {
                var notification = activeNotis[i];
                float y = 10 + (i * (height + spacing));
                float positionX = x + notification.PositionOffset;
                float scaleFactor = notification.Scale;
                float scaledWidth = width * scaleFactor;
                float scaledHeight = height * scaleFactor;

                GUI.color = new Color(0f, 0f, 0f, 0.3f * notification.Alpha);
                Rec(new Rect(positionX + 4, y + 4, scaledWidth, scaledHeight), 10);

                GUI.color = new Color(0.1f, 0.1f, 0.2f, notification.Alpha);
                Rec(new Rect(positionX, y, scaledWidth, scaledHeight), 10);

                GUI.color = Color.white;

                GUILayout.BeginArea(new Rect(positionX + padding * scaleFactor, y + padding * scaleFactor, scaledWidth - padding * 2 * scaleFactor, scaledHeight - padding * 2 * scaleFactor));
                GUI.color = Color.white;

                var titleStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 22,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState { textColor = Color.white }
                };

                var NotiStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 16,
                    normal = new GUIStyleState { textColor = Color.white }
                };

                GUILayout.Label("NOTIFICATION", titleStyle);
                GUILayout.Space(15);
                GUILayout.Label(notification.Message, NotiStyle);
                GUILayout.EndArea();
            }

            GUI.color = Color.white;
        }

        private void Rec(Rect rect, float radius)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            GUI.DrawTexture(rect, texture);
        }

        public static void Notify(string message)
        {
            if (activeNotis.Count >= MaxNotis)
            {
                var oldest = activeNotis[0];
                oldest.IsFading = true;
                oldest.Timer = 0;
            }
            else
            {
                activeNotis.Add(new NotificationData
                {
                    Message = message,
                    Timer = 0f,
                    Alpha = 0f,
                    IsFading = false,
                    PositionOffset = -100,
                    Scale = 1.0f
                });
            }
        }

        public static void ClearNotifications()
        {
            activeNotis.Clear();
        }

        private class NotificationData
        {
            public string Message;
            public float Timer;
            public float Alpha;
            public bool IsFading;
            public float PositionOffset;
            public float Scale;
        }
    }
}
