using DrawChatApp.Data;
using System.Threading.Tasks;

namespace DrawChatApp.Infrastructure
{
    public interface IRoomSettingsService
    {
        Task<RoomSettings> GetRoomSettingsAsync(string roomId);
        Task<RoomSettings> CreateRoomSettingsAsync(string roomId, RoomSettings settings);
        Task<RoomSettings> UpdateRoomSettingsAsync(string roomId, RoomSettings settings);
        Task DeleteRoomSettingsAsync(string roomId);
    }
}
