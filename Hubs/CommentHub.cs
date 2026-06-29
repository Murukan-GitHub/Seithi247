using Microsoft.AspNetCore.SignalR;
using Seithi247.Services;
using System.Threading.Tasks;

namespace Seithi247.Hubs
{
    public class CommentHub : Hub
    {
        private readonly ICommentService _commentService;

        public CommentHub(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // Join a news group
        public async Task JoinNewsGroup(int newsId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"news_{newsId}");
        }

        // Leave a news group
        public async Task LeaveNewsGroup(int newsId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"news_{newsId}");
        }

        // Send new comment to all connected users
        public async Task SendNewComment(int newsId, object comment)
        {
            await Clients.Group($"news_{newsId}")
                .SendAsync("ReceiveNewComment", comment);
        }

        // Send reaction update
        public async Task SendReaction(int newsId, int commentId, object reactionSummary)
        {
            await Clients.Group($"news_{newsId}")
                .SendAsync("ReceiveReaction", commentId, reactionSummary);
        }

        // Notify comment deletion
        public async Task NotifyCommentDeleted(int newsId, int commentId)
        {
            await Clients.Group($"news_{newsId}")
                .SendAsync("ReceiveCommentDeleted", commentId);
        }

        // Notify comment edit
        public async Task NotifyCommentEdited(int newsId, int commentId, string newText)
        {
            await Clients.Group($"news_{newsId}")
                .SendAsync("ReceiveCommentEdited", commentId, newText);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
