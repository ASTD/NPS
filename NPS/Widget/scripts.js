
function initNps(npsConfig) {

    if (!('withCredentials' in new XMLHttpRequest())) {
        return;
    }

    var myNps = {};
    myNps.engagementType = npsConfig.engagementType;
    myNps.masterCustomerId = npsConfig.masterCustomerId;
    myNps.referenceId = npsConfig.referenceId;

    function getTemplate(nps) {
        $.get(npsConfig.baseServiceUrl + "/Widget/Index?engagementType=" + nps.engagementType + "&masterCustomerId=" + nps.masterCustomerId + "&referenceId=" + nps.referenceId, function (data) {
            var $widgetContainer = $("<div id=\"nps-widget-x\"></div>");
            $("body").append($widgetContainer);
            $("#nps-widget-x")[0].innerHTML = data;
            bindEvents();
        });
    }

    function updateNps(nps, callback) {
        var json = JSON.stringify(nps);
        $.ajax({
            type: "PUT",
            url: npsConfig.baseServiceUrl + "/api/Nps",
            data: json,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                callback();
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    function bindEvents() {

        $(document).on("click", "#ratings-selector li", function (event) {
            var myRating = $(this).data('rate');
            $("#nps-rate").val(myRating);
            myNps.score = $("#nps-rate").val();
            myNps.npsEngagementId = $('.nps-widget').data('npsengagementid');

            updateNps(myNps, function () {
                $(".ratings").toggle();
                $(".comments").toggle();
            });
        });

        $(document).on("change", "#nps-rate", function (event) {
            var rate = $(this).val();
            if (rate == 8) {
                $("#pre-num").html("an");
            }
            else {
                $("#pre-num").html("a");
            }
            myNps.score = $("#nps-rate").val();
        });


        $(document).on("click", "#submit-nps-comments", function (event) {
            event.preventDefault();
            if (myNps.npsEngagementId) {
                // call endpoint
                myNps.comments = $("#nps-comment").val();
                updateNps(myNps, function () {
                    $(".comments").toggle();
                    $(".nps-thank-you").toggle();
                    setTimeout(function () {
                        $(".nps-widget").hide();
                    }, 3000);
                });
            }
            return false;
        });

        $(document).on("click", ".nps-widget .close", function (event) {
            $(".nps-widget").hide();
        });

        $(".nps-widget").show();

    }

    getTemplate(myNps);
}