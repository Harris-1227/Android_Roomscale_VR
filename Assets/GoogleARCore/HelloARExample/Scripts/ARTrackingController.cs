
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using GoogleARCore;
	using UnityEngine.UI;

    /// <summary>
    /// Controlls the HelloAR example.
    /// </summary>
    public class ARTrackingController : MonoBehaviour
    {
		public Text m_camPoseText;

		public GameObject m_CameraParent;

		public float m_XZScaleFactor = 10;

		public float m_YScaleFactor = 2;

		public bool m_showPoseData = true;

		private bool trackingStarted = false;

		private Vector3 m_prevARPosePosition;

        
        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update ()
        {
            _QuitOnConnectionErrors();

            // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
            if (Frame.TrackingState != FrameTrackingState.Tracking)
            {
				trackingStarted = false;  // if tracking lost or not initialized
				m_camPoseText.text = "Lost tracking, wait ...";
                const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
                Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
                return;
            }else{
				m_camPoseText.text = "";
			}

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Vector3 currentARPosition = Frame.Pose.position;
			if(!trackingStarted)
			{
				trackingStarted = true;
				m_prevARPosePosition = Frame.Pose.position;
			}

			// Remember the previous position so we can apply deltas

			Vector3 deltaPosition = currentARPosition - m_prevARPosePosition;

			m_prevARPosePosition = currentARPosition;

			if(m_CameraParent != null)
			{
				Vector3 scaledTranslation = new Vector3 (m_XZScaleFactor * deltaPosition.x, m_YScaleFactor * deltaPosition.y, m_XZScaleFactor * deltaPosition.z);
				m_CameraParent.transform.Translate (scaledTranslation);
				if (m_showPoseData)
				{
					m_camPoseText.text = "Pose = " + currentARPosition + "\n" + GetComponent<FPSARCoreScript> ().FPSstring + "\n" + m_CameraParent.transform.position;
				}
			}
        }

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            // Do not update if ARCore is not tracking.
            if (Session.ConnectionState == SessionConnectionState.DeviceNotSupported)
            {
                m_camPoseText.text = "This device does not support ARCore.";
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
            {
                m_camPoseText.text = "Camera permission is needed to run this application.";
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
            {
                m_camPoseText.text = "ARCore encountered a problem connecting. Please start the app again.";
                Application.Quit();
            }
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        /// <param name="length">Toast message time length.</param>
        private static void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }

