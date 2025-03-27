using System;
using System.Threading.Tasks;

namespace Parking_Zone.Services.Interfaces
{
    public interface ICameraService
    {
        /// <summary>
        /// Captures a photo from the specified camera
        /// </summary>
        /// <param name="cameraId">Unique identifier of the camera</param>
        /// <returns>Byte array representing the captured image</returns>
        Task<byte[]> TakePhoto(Guid cameraId);

        /// <summary>
        /// Captures a photo from the specified camera with additional metadata
        /// </summary>
        /// <param name="cameraId">Unique identifier of the camera</param>
        /// <param name="metadata">Additional metadata for the photo capture</param>
        /// <returns>Byte array representing the captured image</returns>
        Task<byte[]> TakePhoto(Guid cameraId, object metadata);

        /// <summary>
        /// Gets the status of a specific camera
        /// </summary>
        /// <param name="cameraId">Unique identifier of the camera</param>
        /// <returns>Camera status</returns>
        Task<string> GetCameraStatus(Guid cameraId);
    }
}
