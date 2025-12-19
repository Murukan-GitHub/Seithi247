//Fancybox.bind("[data-fancybox='details']", {
//    type: "ajax",
//    src: function (trigger) {
//        const id = trigger.dataset.id;
//        return `/News/DetailsPartial/${id}`;
//    },
//    mainClass: "news-post-popup",
//    hideScrollbar: true,
//    dragToClose: false,
//    closeButton: "inside",
//    autoFocus: false,
//    on: {
//        done: (fbox, slide) => initNewsDetailInteractions()
//    }
//});
//$(document).ready(function () {
//    Fancybox.bind("[data-fancybox='details']", {
//        type: "ajax",
//        mainClass: "news-post-popup",
//        dragToClose: false,
//        hideScrollbar: true,
//        closeButton: "inside",
//        autoFocus: false,
//        ajax: {
//            settings: {
//                // optional: if you want GET only
//                type: "GET"
//            }
//        },
//        on: {
//            done: (fancybox, slide) => {
//                initNewsDetailInteractions()
//                console.log("Popup loaded:", slide.src);
//                // Allow internal scrolling
//                const content = slide.$content;
//                if (content) {
//                    content.style.overflow = "hidden"; // prevent double scroll
//                }
//                fancybox.$container.style.overflow = "hidden";
//            }
//        }
//    });
//    function initNewsDetailInteractions() {
//        $(".like-btn").off("click").on("click", function () {
//            const id = $(this).data("id");
//            $.post("/News/Like", { id }, res => {
//                $(`.like-count-${id}`).text(res.likeCount);
//            });
//        });

//        $("#commentForm").off("submit").on("submit", function (e) {
//            e.preventDefault();
//            const form = $(this);
//            $.post(form.attr("action"), form.serialize(), function (html) {
//                $("#comments-list").html(html);
//                form.trigger("reset");
//            });
//        });
//    }
//});