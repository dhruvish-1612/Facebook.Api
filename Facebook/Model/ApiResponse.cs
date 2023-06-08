namespace Facebook.Model
{
    /// <summary>
    /// ApiResponse.
    /// </summary>
    /// <typeparam name="T">.</typeparam>
    public class ApiResponse<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}" /> class.
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="validationsModels">The validations models.</param>
        public ApiResponse(T model, List<ValidationsModel> validationsModels)
        {
            this.Model = model;
            this.Validations = validationsModels;
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public T Model { get; set; } = default(T);

        /// <summary>
        /// Gets or sets the validations.
        /// </summary>
        /// <value>
        /// The validations.
        /// </value>
        public List<ValidationsModel> Validations { get; set; } = new List<ValidationsModel>();
    }
}
