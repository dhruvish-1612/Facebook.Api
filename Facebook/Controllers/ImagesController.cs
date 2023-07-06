// <copyright file="ImagesController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// ImagesController.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        /// <summary>
        /// Gets the story image.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <returns>image.</returns>
        [HttpGet("Story/{imageName}")]
        public IActionResult GetStoryImage(string imageName)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Medias/UserStories", imageName);
            if (!System.IO.File.Exists(imagePath))
            {
                return this.NotFound("Path Not Found");
            }

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return this.File(imageBytes, "image/png"); // Adjust the MIME type according to your image file format
        }

        /// <summary>
        /// Gets the avatar image.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <returns>image.</returns>
        [HttpGet("Avatar/{imageName}")]
        public IActionResult GetAvatarImage(string imageName)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Medias/UserProfilePhoto", imageName);
            if (!System.IO.File.Exists(imagePath))
            {
                return this.NotFound("Path Not Found");
            }

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return this.File(imageBytes, "image/png"); // Adjust the MIME type according to your image file format
        }

        /// <summary>
        /// Gets the post image.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <returns>image.</returns>
        [HttpGet("Post/{imageName}")]
        public IActionResult GetPostImage(string imageName)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Medias/UsersPosts", imageName);
            if (!System.IO.File.Exists(imagePath))
            {
                return this.NotFound("Path Not Found");
            }

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return this.File(imageBytes, "image/png"); // Adjust the MIME type according to your image file format
        }
    }
}
