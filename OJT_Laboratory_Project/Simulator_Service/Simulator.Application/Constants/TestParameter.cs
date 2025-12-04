namespace Simulator.Application.Constants
{
    /// <summary>
    /// Create Property for Test Parameter
    /// </summary>
    public class TestParameter
    {
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public string Parameter { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the abbreviation.
        /// </summary>
        /// <value>
        /// The abbreviation.
        /// </value>
        public string Abbreviation { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public string Unit { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the normal range.
        /// </summary>
        /// <value>
        /// The normal range.
        /// </value>
        public string NormalRange { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public double MinValue { get; set; }
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public double MaxValue { get; set; }
    }
}
