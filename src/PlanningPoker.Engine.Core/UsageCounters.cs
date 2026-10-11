using System.Threading;

namespace PlanningPoker.Engine.Core
{
    public interface IUsageCounters
    {
        void RoomCreated();
        void PlayerJoined();
        void RoundPlayed();
        UsageSnapshot ReadAndReset();
    }

    public readonly struct UsageSnapshot
    {
        public UsageSnapshot(long roomsCreated, long playersJoined, long roundsPlayed)
        {
            RoomsCreated = roomsCreated;
            PlayersJoined = playersJoined;
            RoundsPlayed = roundsPlayed;
        }

        public long RoomsCreated { get; }
        public long PlayersJoined { get; }
        public long RoundsPlayed { get; }
    }

    /// <summary>
    /// Anonymous, aggregated usage counts. No identifiers are recorded.
    /// </summary>
    public class UsageCounters : IUsageCounters
    {
        private long _roomsCreated;
        private long _playersJoined;
        private long _roundsPlayed;

        public void RoomCreated() => Interlocked.Increment(ref _roomsCreated);
        public void PlayerJoined() => Interlocked.Increment(ref _playersJoined);
        public void RoundPlayed() => Interlocked.Increment(ref _roundsPlayed);

        public UsageSnapshot ReadAndReset()
        {
            return new UsageSnapshot(
                Interlocked.Exchange(ref _roomsCreated, 0),
                Interlocked.Exchange(ref _playersJoined, 0),
                Interlocked.Exchange(ref _roundsPlayed, 0));
        }
    }
}
