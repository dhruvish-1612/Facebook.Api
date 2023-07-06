// <copyright file="GetUserId.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Helpers
{
    using System.Net;
    using Facebook.CustomException;
    using Facebook.Model;

    /// <summary>
    /// GetUserId.
    /// </summary>
    public class GetUserId
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserId"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public GetUserId(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the login user identifier.
        /// </summary>
        /// <returns>GetLoginUserId.</returns>
        public long GetLoginUserId()
        {
            long userId = 0;
            if (this.httpContextAccessor.HttpContext is not null)
            {
                string? result = this.httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(result))
                {
                    throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.Unauthorized, "User Is Invalid.") } };
                }

                userId = long.Parse(result);
            }

            return userId;
        }
    }
}
