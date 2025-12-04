namespace Simulator.Application.SimulateRawData.Command
{
    /// <summary>
    /// Creates the send raw test result result
    /// </summary>
    /// <seealso cref="System.IEquatable&lt;Simulator.Application.SimulateRawData.Command.SendRawTestResultResult&gt;" />
    public record SendRawTestResultResult(bool Success, string? MessageId);
}
