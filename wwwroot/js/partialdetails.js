// Partial details JS: handlers for recommend, share, comments sheet, and reactions

(function ($) {
    'use strict';

    function getShareUrl(id) {
        return window.location.origin + '/News/Details/' + id;
    }

    function increaseShareCount(id) {
        $.post('/News/IncreaseShare', { id: id }, function (res) {
            $('#shareCount').text(res.count);
        });
    }

    // Sharing helpers
    window.shareFB = function (id) {
        const url = getShareUrl(id);
        window.open('https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(url));
        increaseShareCount(id);
    };

    window.shareWA = function (id) {
        const url = getShareUrl(id);
        window.open('https://wa.me/?text=' + encodeURIComponent(url));
        increaseShareCount(id);
    };

    window.shareTW = function (id) {
        const url = getShareUrl(id);
        window.open('https://twitter.com/intent/tweet?url=' + encodeURIComponent(url));
        increaseShareCount(id);
    };

    window.copyLink = function (id) {
        const url = getShareUrl(id);
        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(url).then(function () {
                alert('Link copied!');
            });
        } else {
            // fallback
            const el = document.createElement('textarea');
            el.value = url;
            document.body.appendChild(el);
            el.select();
            document.execCommand('copy');
            document.body.removeChild(el);
            alert('Link copied!');
        }
        increaseShareCount(id);
    };

    window.showSharedUsers = function (id) {
        if (window.Fancybox) {
            Fancybox.show([{ src: '/News/GetSharedUsers?newsId=' + id, type: 'ajax' }]);
        } else {
            window.open('/News/GetSharedUsers?newsId=' + id, '_blank');
        }
    };

    window.nativeShare = function (id) {
        const url = getShareUrl(id);
        if (navigator.share) {
            navigator.share({ title: document.title, text: 'Check this news', url: url })
                .then(function () { increaseShareCount(id); })
                .catch(function (err) { console.log(err); });
        } else {
            window.copyLink(id);
        }
    };

    // Document handlers
    $(document)
        .off('click', '.react-btn')
        .on('click', '.react-btn', function () {
            var commentId = $(this).data('comment');
            var emoji = $(this).data('emoji');
            $.post('/News/ReactToComment', { commentId: commentId, emoji: emoji }, function (data) {
                var summaryText = data.map(function (r) { return r.emoji + ' ' + r.count; }).join(' ');
                $('#reaction-summary-' + commentId).text(summaryText);
            }).fail(function () { alert('Error reacting to comment.'); });
        });

    $(document)
        .off('click', '.recommend-btn')
        .on('click', '.recommend-btn', function (e) {
            e.preventDefault();
            var btn = $(this);
            var newsId = btn.data('id');
            if (btn.data('loading')) return;
            btn.data('loading', true);
            $.ajax({
                url: '/News/Recommend',
                type: 'POST',
                data: { newsId: newsId },
                success: function () {
                    btn.addClass('recommended').html('⭐ Recommended');
                    $('#recommendedContainer').load('/News/Stories');
                },
                error: function () { alert('Unable to recommend'); },
                complete: function () { btn.data('loading', false); }
            });
        });

    $(document)
        .off('click', '.recommend-icon')
        .on('click', '.recommend-icon', function () {
            var el = $(this);
            var id = el.data('id');
            if (el.hasClass('active')) return;
            el.addClass('animate');
            setTimeout(function () { el.removeClass('animate'); }, 400);
            $.post('/News/Recommend', { newsId: id }, function () {
                el.addClass('active');
                el.find('i').removeClass('fa-regular').addClass('fa-solid');
                $('#recommendedContainer').load('/News/Stories');
            });
        });

    // document.addEventListener("click", e => {

    //     if (e.target.closest(".comments-trigger")) {

    //         document
    //             .querySelector("#commentDrawer")
    //             .classList.add("open");

    //     }

    //     if (e.target.closest("#closeComments")) {

    //         document
    //             .querySelector("#commentDrawer")
    //             .classList.remove("open");

    //     }

    // });
    // // Open/close mobile comment sheet
    // document.addEventListener('click', function (e) {
    //     if (e.target.closest('#openComments')) {
    //         var sheet = document.querySelector('#commentSheet');
    //         if (sheet) sheet.classList.add('active');
    //     }
    //     if (e.target.closest('#closeComments')) {
    //         var sheet = document.querySelector('#commentSheet');
    //         if (sheet) sheet.classList.remove('active');
    //     }
    // });

    // Optional: handler for post-comment to forward to server (basic example)
    $(document).on('click', '.post-comment', function (e) {
        e.preventDefault();
        var btn = $(this);
        var newsId = btn.data('id');
        var parent = btn.closest('.add-comment');
        var author = parent.find('input[id^="authorName"]').val();
        var text = parent.find('textarea[id^="commentText"]').val();
        if (!text || !author) { alert('Please enter name and comment'); return; }
        $.post('/News/PostComment', { newsId: newsId, authorName: author, text: text }, function (html) {
            // Server should return rendered comment list or new comment HTML
            // If HTML returned, replace the appropriate list
            if (parent.closest('#commentSheet').length) {
                $('#comments-list-mobile').prepend(html);
            } else {
                $('#comments-list').prepend(html);
            }
            parent.find('textarea[id^="commentText"]').val('');
        }).fail(function () { alert('Unable to post comment'); });
    });

//     document.addEventListener("click", function(e){

//     if(e.target.closest("#openComments")){

//         document
//         .getElementById("commentDrawer")
//         .classList.add("open");
//     }

//     if(e.target.closest("#closeComments")){

//         document
//         .getElementById("commentDrawer")
//         .classList.remove("open");
//     }

// });
const drawer = document.getElementById("commentDrawer");

document.addEventListener("click", e => {

    if (e.target.closest("#openComments")) {

        drawer.classList.add("open");

        document.body.style.overflow="hidden";

    }

    if (
        e.target.closest("#closeComments") ||
        e.target.closest(".drawer-overlay")
    ) {

        drawer.classList.remove("open");

        document.body.style.overflow="";

    }

});
})(jQuery);
