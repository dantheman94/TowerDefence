using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/7/2018
//
//******************************

namespace TowerDefence {

    public static class Settings {

        // Camera Movement
        public static int ScreenOffset { get { return 50; } }
        public static float ScrollSpeed { get { return 10000; } }
        public static float MovementSpeed { get; set; }
        public static float CameraWalkSpeed { get { return 100; } }
        public static float CameraSprintSpeed { get { return 300; } }
        public static float RotateSpeed { get { return 100; } }
        public static float RotateZoomSpeed { get { return 100; } }

        // Camera bounds
        public static float MinCameraHeight { get { return 160; } }
        public static float MaxCameraHeight { get { return 160; } }
        public static float MinFov { get { return 33; } }
        public static float MaxFov { get { return 50; } }
        public static float ZoomSpeed { get { return 160; } }

        // Selecting world objects
        private static Vector3 invalidPosition = new Vector3(-99999, -99999, -99999);
        public static Vector3 InvalidPosition { get { return invalidPosition; } }

        private static GameObject selectBoxObject;
        public static GameObject SelectBoxObjects { get { return selectBoxObject; } }
        public static void StoreSelectBoxItems(GameObject skin) { selectBoxObject = skin; }

        private static Bounds invalidBounds = new Bounds(new Vector3(-99999, -99999, -99999), new Vector3(0, 0, 0));
        public static Bounds InvalidBounds { get { return invalidBounds; } }

        public static Rect CalculateSelectionBox(Bounds selectionBounds, Rect playingArea) {

            // Shorthand for the coordinates of the center of the selection bounds
            float cx = selectionBounds.center.x;
            float cy = selectionBounds.center.y;
            float cz = selectionBounds.center.z;

            // Shorthand for the coordinates of the extents of the selection bounds
            float ex = selectionBounds.extents.x;
            float ey = selectionBounds.extents.y;
            float ez = selectionBounds.extents.z;

            // Determine the screen coordinates for the corners of the selection bounds
            List<Vector3> corners = new List<Vector3>();
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz + ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz - ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz + ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz + ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz - ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz + ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz - ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz - ez)));

            // Determine the bounds on screen for the selection bounds
            Bounds screenBounds = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < corners.Count; i++) {
                screenBounds.Encapsulate(corners[i]);
            }

            // Screen coordinates start in the bottom left corner, rather than the top left corner
            // this correction is needed to make sure the selection box is drawn in the correct place
            float selectBoxTop = playingArea.height - (screenBounds.center.y + screenBounds.extents.y);
            float selectBoxLeft = screenBounds.center.x - screenBounds.extents.x;
            float selectBoxWidth = 2 * screenBounds.extents.x;
            float selectBoxHeight = 2 * screenBounds.extents.y;

            return new Rect(selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);
        }
    }
}
