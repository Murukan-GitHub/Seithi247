//    $(document).on("click", ".like-btn", function () {
//    const btn = $(this);
//    const id = btn.data("id");
//    $.post("/News/Like", {id: id }, function (data) {
//        if (data.success) btn.find(".like-count").text(data.likes);
//    });
//});

//    $("#commentForm").submit(function (e) {
//        e.preventDefault();
//    $.post("/News/AddComment", $(this).serialize(), function (html) {
//        $("#commentsList").html(html);
//    $("#commentForm textarea").val("");
//    });
//});
