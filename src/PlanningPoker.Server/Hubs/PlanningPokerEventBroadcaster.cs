using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Engine.Core;
using PlanningPoker.Engine.Core.Events;
using PlanningPoker.Hub.Client.Abstractions;
using PlanningPoker.Hub.Client.Abstractions.ViewModels;
using PlanningPoker.Server.ViewModelMappers;

namespace PlanningPoker.Server.Hubs
{
    public interface IPlanningPokerEventBroadcaster
    {
    }

    public class PlanningPokerEventBroadcaster : IPlanningPokerEventBroadcaster
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHubContext<PlanningPokerHub> _hubContext;

        public PlanningPokerEventBroadcaster(
            IHubContext<PlanningPokerHub> hubContext,
            IPlanningPokerEngine pokerEngine,
            IDateTimeProvider dateTimeProvider)
        {
            _hubContext = hubContext;
            _dateTimeProvider = dateTimeProvider;

            pokerEngine.LogUpdated += OnLogUpdated;
            pokerEngine.PlayerKicked += OnPlayerKicked;
            pokerEngine.RoomCleared += OnRoomCleared;
            pokerEngine.RoomUpdated += OnRoomUpdated;
        }

        private void OnRoomUpdated(object? sender, RoomUpdatedEventArgs e)
        {
            _hubContext.Clients.Group(e.ServerId.ToString()).SendAsync(BroadcastChannels.UPDATED, e.UpdatedServer.Map());
        }

        private void OnRoomCleared(object? sender, RoomClearedEventArgs e)
        {
            _hubContext.Clients.Group(e.ServerId.ToString()).SendAsync(BroadcastChannels.CLEAR);
        }

        private void OnPlayerKicked(object? sender, PlayerKickedEventArgs e)
        {
            _hubContext.Clients.Group(e.ServerId.ToString()).SendAsync(BroadcastChannels.KICKED, e.KickedPlayer.Map(false));
        }

        private void OnLogUpdated(object? sender, LogUpdatedEventArgs e)
        {
            var now = _dateTimeProvider.GetUtcNow();
            var logMessage = new LogMessage
            {
                User = e.InitiatingPlayer,
                Message = e.LogMessage,
                Timestamp = now
            };

            _hubContext.Clients.Group(e.ServerId.ToString()).SendAsync(BroadcastChannels.LOG, logMessage);
        }
    }
}