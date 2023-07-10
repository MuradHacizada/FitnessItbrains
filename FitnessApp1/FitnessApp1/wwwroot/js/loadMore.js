let skip = 6;
let packageCount = $("#loadMore").next().val()
$(document).on("click", "#loadMore", function () {
    $.ajax({
        url: "/Package/LoadMore/",
        type: "get",
        data: {
            "skipCount": skip
        },
        success: function (res) {
            $("#myPackage").append(res)
            skip += 6;

            if (packageCount <= skip) {
                $("#loadMore").remove()
            }
        }
    });
}); 