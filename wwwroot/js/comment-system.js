/**
 * Comment System - AJAX Manager
 * Handles all AJAX operations for comments, replies, and reactions
 */

class CommentSystemManager {
    constructor(config = {}) {
        this.apiBase = config.apiBase || '/api/comments';
        this.newsId = config.newsId;
        this.userIdentifier = this.getUserIdentifier();
        this.currentEditingCommentId = null;

        this.init();
    }

    init() {
        this.attachEventListeners();
    }

    // ============ EVENT LISTENERS ============

    attachEventListeners() {
        // Post comment
        document.addEventListener('click', (e) => {
            if (e.target.closest('.btn-post-comment')) {
                this.handlePostComment();
            }
        });

        // Send reply
        document.addEventListener('click', (e) => {
            if (e.target.closest('.btn-send-reply')) {
                this.handleSendReply(e);
            }
        });

        // Show/hide replies
        document.addEventListener('click', (e) => {
            if (e.target.closest('.btn-show-replies')) {
                this.handleToggleReplies(e);
            }
        });

        // Reply button
        document.addEventListener('click', (e) => {
            if (e.target.closest('.reply-btn')) {
                this.handleShowReplyInput(e);
            }
        });

        // Cancel reply
        document.addEventListener('click', (e) => {
            if (e.target.closest('.btn-cancel-reply')) {
                this.handleCancelReply(e);
            }
        });

        // Emoji reactions
        document.addEventListener('click', (e) => {
            if (e.target.closest('.reaction-btn')) {
                this.handleOpenEmojiPicker(e);
            }
        });

        // React to existing reaction badges
        document.addEventListener('click', (e) => {
            if (e.target.closest('.reaction-badge')) {
                this.handleReactionBadgeClick(e);
            }
        });

        // Character counter
        document.addEventListener('input', (e) => {
            if (e.target.closest('#commentText')) {
                const count = e.target.value.length;
                document.getElementById('charCount').textContent = count;
            }
        });
    }

    // ============ POST COMMENT ============

    async handlePostComment() {
        const authorName = document.getElementById('authorName')?.value?.trim();
        const text = document.getElementById('commentText')?.value?.trim();

        if (!authorName || !text) {
            this.showError('commentError', 'Please fill in all fields');
            return;
        }

        if (text.length < 5) {
            this.showError('commentError', 'Comment must be at least 5 characters');
            return;
        }

        try {
            this.setButtonLoading('.btn-post-comment', true);

            const response = await fetch(`${this.apiBase}/add`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    newsId: this.newsId,
                    authorName: authorName,
                    text: text,
                    parentCommentId: null
                })
            });

            if (response.ok) {
                const comment = await response.json();
                this.addCommentToDOM(comment);
                this.clearCommentForm();
                this.showSuccess('commentSuccess', 'Comment posted successfully!');
                setTimeout(() => this.hideSuccess('commentSuccess'), 3000);
            } else {
                this.showError('commentError', 'Failed to post comment');
            }
        } catch (error) {
            console.error('Error posting comment:', error);
            this.showError('commentError', 'An error occurred while posting comment');
        } finally {
            this.setButtonLoading('.btn-post-comment', false);
        }
    }

    // ============ REPLY HANDLING ============

    async handleSendReply(event) {
        const btn = event.target.closest('.btn-send-reply');
        const container = btn.closest('.reply-input-wrapper');
        const parentCommentId = parseInt(container.querySelector('.parent-comment-id').value);
        const authorName = container.querySelector('.reply-name-input')?.value?.trim();
        const text = container.querySelector('.reply-text-input')?.value?.trim();

        if (!authorName || !text) {
            alert('Please fill in all fields');
            return;
        }

        try {
            this.setButtonLoading('.btn-send-reply', true, btn);

            const response = await fetch(`${this.apiBase}/add`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    newsId: this.newsId,
                    authorName: authorName,
                    text: text,
                    parentCommentId: parentCommentId
                })
            });

            if (response.ok) {
                const reply = await response.json();
                this.addReplyToDOM(reply, parentCommentId);
                container.querySelector('.reply-name-input').value = '';
                container.querySelector('.reply-text-input').value = '';
                container.classList.add('d-none');
            }
        } catch (error) {
            console.error('Error sending reply:', error);
            alert('Failed to send reply');
        } finally {
            this.setButtonLoading('.btn-send-reply', false, btn);
        }
    }

    handleShowReplyInput(event) {
        const btn = event.target.closest('.reply-btn');
        const commentCard = btn.closest('.comment-card');
        const parentCommentId = commentCard.dataset.commentId;
        const replyInputContainer = commentCard.querySelector('.reply-input-container');

        if (replyInputContainer) {
            replyInputContainer.classList.toggle('d-none');
        }
    }

    handleCancelReply(event) {
        const btn = event.target.closest('.btn-cancel-reply');
        const container = btn.closest('.reply-input-wrapper');
        container.classList.add('d-none');
    }

    handleToggleReplies(event) {
        const btn = event.target.closest('.btn-show-replies');
        const parentCommentId = btn.dataset.parentId;
        const repliesContainer = document.querySelector(`.replies-container[data-parent-id="${parentCommentId}"]`);

        if (repliesContainer) {
            repliesContainer.classList.toggle('d-none');
            btn.classList.toggle('collapsed');
        }
    }

    // ============ EMOJI REACTIONS ============

    handleOpenEmojiPicker(event) {
        const btn = event.target.closest('.reaction-btn');
        const commentCard = btn.closest('.comment-card');
        const commentId = commentCard.dataset.commentId;

        // Check if picker already exists
        let picker = commentCard.querySelector('.emoji-picker');
        if (picker) {
            picker.remove();
            return;
        }

        // Create emoji picker
        picker = this.createEmojiPicker(commentId);
        btn.parentElement.insertBefore(picker, btn.nextSibling);

        // Auto close on click outside
        setTimeout(() => {
            document.addEventListener('click', (e) => {
                if (!e.target.closest('.emoji-picker') && !e.target.closest('.reaction-btn')) {
                    picker.remove();
                }
            }, { once: true });
        }, 0);
    }

    createEmojiPicker(commentId) {
        const emojis = ['👍', '❤️', '😂', '😮', '😢', '😠'];
        const picker = document.createElement('div');
        picker.className = 'emoji-picker';

        emojis.forEach(emoji => {
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'emoji-option';
            btn.innerText = emoji;
            btn.addEventListener('click', () => this.handleAddReaction(commentId, emoji));
            picker.appendChild(btn);
        });

        return picker;
    }

    async handleAddReaction(commentId, emoji) {
        try {
            const response = await fetch(`${this.apiBase}/react`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    commentId: commentId,
                    emoji: emoji
                })
            });

            if (response.ok) {
                const data = await response.json();
                this.updateReactionsSummary(commentId, data.reactionSummary);

                // Remove picker
                document.querySelector('.emoji-picker')?.remove();
            }
        } catch (error) {
            console.error('Error adding reaction:', error);
        }
    }

    handleReactionBadgeClick(event) {
        const badge = event.target.closest('.reaction-badge');
        const emoji = badge.dataset.emoji;
        const commentCard = badge.closest('.comment-card');
        const commentId = commentCard.dataset.commentId;

        // Toggle reaction (click again to remove)
        this.handleAddReaction(commentId, emoji);
    }

    updateReactionsSummary(commentId, reactionSummary) {
        const commentCard = document.querySelector(`.comment-card[data-comment-id="${commentId}"]`);
        const summaryContainer = commentCard.querySelector('.reaction-summary');

        if (!summaryContainer) {
            // Create summary container if it doesn't exist
            const newSummary = document.createElement('div');
            newSummary.className = 'reaction-summary';
            commentCard.querySelector('.comment-body').insertAdjacentElement('afterend', newSummary);
        }

        const updatedSummary = commentCard.querySelector('.reaction-summary');
        updatedSummary.innerHTML = '';

        Object.entries(reactionSummary).forEach(([emoji, count]) => {
            const badge = document.createElement('span');
            badge.className = 'reaction-badge';
            badge.dataset.emoji = emoji;
            badge.title = `${count} reactions`;
            badge.innerHTML = `${emoji} <span class="reaction-count">${count}</span>`;
            updatedSummary.appendChild(badge);
        });
    }

    // ============ DOM MANIPULATION ============

    addCommentToDOM(comment) {
        const commentsList = document.getElementById('commentsList');
        if (commentsList.querySelector('.empty-state')) {
            commentsList.innerHTML = '';
        }

        const commentHTML = this.createCommentHTML(comment);
        commentsList.insertAdjacentHTML('afterbegin', commentHTML);

        // Update comment count
        this.updateCommentCount();
    }

    addReplyToDOM(reply, parentCommentId) {
        const repliesContainer = document.querySelector(`.replies-container[data-parent-id="${parentCommentId}"]`);
        const replyHTML = this.createReplyHTML(reply);
        repliesContainer.insertAdjacentHTML('beforeend', replyHTML);

        // Update reply count
        this.updateReplyCount(parentCommentId);
    }

    createCommentHTML(comment) {
        const reactionBadges = Object.entries(comment.reactionSummary || {})
            .map(([emoji, count]) => `<span class="reaction-badge" data-emoji="${emoji}">${emoji} <span class="reaction-count">${count}</span></span>`)
            .join('');

        return `
            <div class="comment-card" data-comment-id="${comment.id}">
                <div class="comment-header">
                    <div class="comment-author-info">
                        <div class="author-avatar">${comment.authorName.substring(0, 1).toUpperCase()}</div>
                        <div class="author-details">
                            <strong class="author-name">${comment.authorName}</strong>
                            <span class="comment-date"><small class="text-muted">${new Date(comment.postedDate).toLocaleDateString()}</small></span>
                        </div>
                    </div>
                </div>
                <div class="comment-body">
                    <p class="comment-text">${this.escapeHtml(comment.text)}</p>
                </div>
                ${reactionBadges ? `<div class="reaction-summary">${reactionBadges}</div>` : ''}
                <div class="comment-actions-bar">
                    <button class="action-btn reaction-btn" data-comment-id="${comment.id}"><span>😊</span> React</button>
                    <button class="action-btn reply-btn" data-comment-id="${comment.id}"><span>↩️</span> Reply</button>
                </div>
                <div class="reply-input-container d-none" data-parent-id="${comment.id}"></div>
            </div>
        `;
    }

    createReplyHTML(reply) {
        return `
            <div class="reply-card" data-comment-id="${reply.id}">
                <div class="reply-content-wrapper">
                    <div class="reply-header">
                        <div class="reply-author-info">
                            <div class="author-avatar-small">${reply.authorName.substring(0, 1).toUpperCase()}</div>
                            <div class="reply-author-details">
                                <strong>${reply.authorName}</strong>
                                <span class="reply-date text-muted"><small>${new Date(reply.postedDate).toLocaleDateString()}</small></span>
                            </div>
                        </div>
                    </div>
                    <div class="reply-body">
                        <p class="reply-text">${this.escapeHtml(reply.text)}</p>
                    </div>
                </div>
            </div>
        `;
    }

    // ============ UTILITIES ============

    getUserIdentifier() {
        let userIdentifier = localStorage.getItem('userIdentifier');
        if (!userIdentifier) {
            userIdentifier = `user_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
            localStorage.setItem('userIdentifier', userIdentifier);
        }
        return userIdentifier;
    }

    clearCommentForm() {
        document.getElementById('authorName').value = '';
        document.getElementById('commentText').value = '';
        document.getElementById('charCount').textContent = '0';
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    showError(elementId, message) {
        const element = document.getElementById(elementId);
        if (element) {
            element.textContent = message;
            element.classList.remove('d-none');
        }
    }

    hideError(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.classList.add('d-none');
        }
    }

    showSuccess(elementId, message) {
        const element = document.getElementById(elementId);
        if (element) {
            element.textContent = message;
            element.classList.remove('d-none');
        }
    }

    hideSuccess(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.classList.add('d-none');
        }
    }

    setButtonLoading(selector, isLoading, targetBtn = null) {
        const btn = targetBtn || document.querySelector(selector);
        if (!btn) return;

        if (isLoading) {
            btn.disabled = true;
            btn.classList.add('loading');
            const spinner = btn.querySelector('.spinner-border');
            if (spinner) spinner.classList.remove('d-none');
        } else {
            btn.disabled = false;
            btn.classList.remove('loading');
            const spinner = btn.querySelector('.spinner-border');
            if (spinner) spinner.classList.add('d-none');
        }
    }

    updateCommentCount() {
        const count = document.querySelectorAll('.comment-card').length;
        const badge = document.querySelector('.comment-count');
        if (badge) badge.textContent = count;
    }

    updateReplyCount(parentCommentId) {
        const replies = document.querySelectorAll(`.replies-container[data-parent-id="${parentCommentId}"] .reply-card`);
        const btn = document.querySelector(`.btn-show-replies[data-parent-id="${parentCommentId}"]`);
        if (btn) {
            btn.innerHTML = `<span class="toggle-icon">▼</span> View ${replies.length} ${replies.length === 1 ? 'reply' : 'replies'}`;
        }
    }
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    // Get newsId from a global variable or data attribute
    const newsId = document.querySelector('[data-news-id]')?.dataset.newsId;
    if (newsId) {
        window.commentSystem = new CommentSystemManager({ newsId: parseInt(newsId) });
    }
});
