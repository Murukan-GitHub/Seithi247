/**
 * Comment System - SignalR Integration (Optional)
 * Enables real-time comment notifications
 */

class CommentSystemSignalR {
    constructor(hubUrl = '/commentHub', newsId) {
        this.newsId = newsId;
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect()
            .build();

        this.setupHubMethods();
        this.connect();
    }

    setupHubMethods() {
        // Receive new comment from other users
        this.connection.on('ReceiveNewComment', (comment) => {
            console.log('New comment from SignalR:', comment);
            // Update UI in real-time
            this.updateCommentUI(comment);
        });

        // Receive reaction update
        this.connection.on('ReceiveReaction', (commentId, reactionSummary) => {
            console.log('Reaction update:', commentId, reactionSummary);
            // Update reactions in real-time
            this.updateReactionsUI(commentId, reactionSummary);
        });

        // Receive comment deletion
        this.connection.on('ReceiveCommentDeleted', (commentId) => {
            console.log('Comment deleted:', commentId);
            document.querySelector(`.comment-card[data-comment-id="${commentId}"]`)?.remove();
        });

        // Receive comment edit
        this.connection.on('ReceiveCommentEdited', (commentId, newText) => {
            console.log('Comment edited:', commentId);
            const card = document.querySelector(`.comment-card[data-comment-id="${commentId}"]`);
            if (card) {
                card.querySelector('.comment-text').textContent = newText;
                card.classList.add('comment-edited-highlight');
                setTimeout(() => card.classList.remove('comment-edited-highlight'), 2000);
            }
        });
    }

    async connect() {
        try {
            await this.connection.start();
            console.log('SignalR connected');

            // Join news group
            await this.connection.invoke('JoinNewsGroup', this.newsId);
        } catch (error) {
            console.error('SignalR connection error:', error);
            setTimeout(() => this.connect(), 5000);
        }
    }

    disconnect() {
        this.connection.stop();
    }

    // Notify other users of new comment
    async notifyNewComment(comment) {
        try {
            await this.connection.invoke('SendNewComment', this.newsId, comment);
        } catch (error) {
            console.error('Error sending comment via SignalR:', error);
        }
    }

    // Notify other users of reaction
    async notifyReaction(commentId, reactionSummary) {
        try {
            await this.connection.invoke('SendReaction', this.newsId, commentId, reactionSummary);
        } catch (error) {
            console.error('Error sending reaction via SignalR:', error);
        }
    }

    updateCommentUI(comment) {
        // Similar to addCommentToDOM in comment-system.js
        const commentsList = document.getElementById('commentsList');
        if (!commentsList) return;

        const commentHTML = this.createCommentHTML(comment);
        commentsList.insertAdjacentHTML('afterbegin', commentHTML);
    }

    updateReactionsUI(commentId, reactionSummary) {
        // Similar to updateReactionsSummary
        const commentCard = document.querySelector(`.comment-card[data-comment-id="${commentId}"]`);
        if (!commentCard) return;

        const summaryContainer = commentCard.querySelector('.reaction-summary');
        if (summaryContainer) {
            summaryContainer.innerHTML = Object.entries(reactionSummary)
                .map(([emoji, count]) => 
                    `<span class="reaction-badge" data-emoji="${emoji}">${emoji} <span class="reaction-count">${count}</span></span>`
                )
                .join('');
        }
    }

    createCommentHTML(comment) {
        // Same as comment-system.js createCommentHTML
        return `<div class="comment-card" data-comment-id="${comment.id}">...</div>`;
    }
}

// Usage:
// In Details.cshtml:
// <script src="~/lib/signalr/signalr.min.js"></script>
// <script>
//     const signalRManager = new CommentSystemSignalR('/commentHub', @Model.Id);
// </script>
