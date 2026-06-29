/**
 * Comment System - Enhanced Version with Edit & Delete
 * This file shows how to extend the basic comment system
 */

class CommentSystemManagerEnhanced extends CommentSystemManager {

    // ============ EDIT COMMENT ============

    async handleEditComment(event) {
        const btn = event.target.closest('.btn-edit-comment');
        const commentCard = btn.closest('.comment-card');
        const commentId = commentCard.dataset.commentId;
        const commentText = commentCard.querySelector('.comment-text');

        // Show edit form
        const editForm = this.createEditForm(commentId, commentText.textContent);
        commentText.parentElement.replaceWith(editForm);
        editForm.querySelector('.edit-textarea').focus();
    }

    createEditForm(commentId, currentText) {
        const form = document.createElement('div');
        form.className = 'edit-form';
        form.innerHTML = `
            <textarea class="form-control edit-textarea" maxlength="2000">${currentText}</textarea>
            <div class="edit-actions mt-2">
                <button class="btn btn-sm btn-primary btn-save-edit" data-comment-id="${commentId}">Save</button>
                <button class="btn btn-sm btn-secondary btn-cancel-edit">Cancel</button>
            </div>
        `;

        form.querySelector('.btn-save-edit').addEventListener('click', (e) => {
            this.handleSaveEdit(e, commentId, form);
        });

        form.querySelector('.btn-cancel-edit').addEventListener('click', () => {
            location.reload(); // Simple reload - can be improved
        });

        return form;
    }

    async handleSaveEdit(event, commentId, form) {
        const newText = form.querySelector('.edit-textarea').value.trim();

        if (!newText || newText.length < 5) {
            alert('Comment must be at least 5 characters');
            return;
        }

        try {
            const response = await fetch(`${this.apiBase}/edit`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    commentId: commentId,
                    newText: newText
                })
            });

            if (response.ok) {
                const result = await response.json();
                location.reload(); // Refresh to show edited state
            }
        } catch (error) {
            console.error('Error editing comment:', error);
            alert('Failed to edit comment');
        }
    }

    // ============ DELETE COMMENT ============

    async handleDeleteComment(event) {
        if (!confirm('Are you sure you want to delete this comment?')) {
            return;
        }

        const btn = event.target.closest('.btn-delete-comment');
        const commentCard = btn.closest('.comment-card');
        const commentId = commentCard.dataset.commentId;

        try {
            const response = await fetch(`${this.apiBase}/${commentId}`, {
                method: 'DELETE'
            });

            if (response.ok) {
                commentCard.style.animation = 'slideOut 0.3s ease-out forwards';
                setTimeout(() => {
                    commentCard.remove();
                    this.updateCommentCount();
                }, 300);
            }
        } catch (error) {
            console.error('Error deleting comment:', error);
            alert('Failed to delete comment');
        }
    }

    // ============ INLINE REPLY (Advanced) ============

    handleInlineReply(event) {
        const replyBtn = event.target.closest('.reply-btn');
        const commentCard = replyBtn.closest('.comment-card');

        // Scroll to reply input
        const replyInput = commentCard.querySelector('.reply-input-container');
        if (replyInput) {
            replyInput.classList.remove('d-none');
            replyInput.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        }
    }

    // ============ PAGINATION ============

    async loadMoreComments() {
        const btn = document.getElementById('loadMoreBtn');
        // Implement pagination logic here
    }

    // ============ UTILITIES ============

    attachEditDeleteListeners() {
        document.addEventListener('click', (e) => {
            if (e.target.closest('.btn-edit-comment')) {
                this.handleEditComment(e);
            }
            if (e.target.closest('.btn-delete-comment')) {
                this.handleDeleteComment(e);
            }
        });
    }
}

// Animation for delete
const style = document.createElement('style');
style.textContent = `
    @keyframes slideOut {
        to {
            opacity: 0;
            transform: translateX(100%);
        }
    }
`;
document.head.appendChild(style);
