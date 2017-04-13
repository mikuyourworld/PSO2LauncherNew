function GetLatestGithubReleaseInfo() {
    $("#progressRing").show();
    $("#downloadcontent").hide();
    $("#downloadButton").attr("href", "");
    SetLoading($("#progressRing"));
    $.ajax({
        cache: false,
        url: "https://api.github.com/repos/Leayal/PSO2LauncherNew/releases",
        dataType: "json",
        success: function(json) {
            var release = json[0];
            var asset = release.assets[0];
            var oneHour = 60 * 60 * 1000;
            var oneDay = 24 * oneHour;
            var oneMonth = oneHour * 30;
            //tag_name for ver only
            //name for the title
            var dateDiff = new Date() - new Date(asset.updated_at);
            var timeAgo;
            if (dateDiff < oneDay) {
                timeAgo = (dateDiff / oneHour).toFixed(1) + " hours ago";
            } else if (dateDiff < oneMonth) {
                timeAgo = (dateDiff / oneDay).toFixed(1) + " days ago";
            } else {
                timeAgo = (dateDiff / oneMonth).toFixed(1) + " months ago";
            }
            $("#downloadButton").attr("href", asset.browser_download_url);
            $("#downloadVersion").text(release.tag_name);
            $("#downloadLastUpload").text(timeAgo);
            $("#downloadButton").fadeIn("slow");

            for (i = 1; i < json.length; i++) {
                release = json[i];
                if (release && release.assets && release.assets.length > 0) {
                    var theLi = $("<li>");
                    theLi.append($("<a>").attr("href", release.assets[0].browser_download_url).text(release.tag_name));
                    theLi.appendTo($("#olderversionlisting"));
                }
            }
            $("#progressRing").hide();
            $("#downloadcontent").show();
            RemoveLoading($("#progressRing"));
        }
    });
};

function getRandomIntInclusive(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
};

function SetLoading(target) {
    var found = target.find($("div[metroloading]"));
    if (found && found.length > 0) return;
    //target.append($("<div metroloading class=\"midcenter\"><div class=\"windows8-loading\"><b></b><b></b><b></b><b></b><b></b></div></div>"));
    var aaasdwaf = $("<div>").addClass("stretch").addClass("windows8-loading");
    aaasdwaf.append($("<b>"));
    aaasdwaf.append($("<b>"));
    aaasdwaf.append($("<b>"));
    aaasdwaf.append($("<b>"));
    aaasdwaf.append($("<b>"));
    var ddd = $("<div metroloading>").addClass("midcenter").append(aaasdwaf);
    ($("<div metroloading>")
        .addClass("fixedDiv")
        .addClass("stretch")
        .addClass("disabled")
        .addClass("opacity50")).prependTo(target);
    target.append(ddd);
};

function RemoveLoading(target) {
    target.children("div[metroloading]").remove();
};