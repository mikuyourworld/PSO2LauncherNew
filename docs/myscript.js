function GetLatestGithubReleaseInfo() {
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
            var releaseInfo = release.name + " was updated " + timeAgo;
            $("#fafafaf").attr("href", asset.browser_download_url);
            $("#fafafaf").text(releaseInfo);
            $("#fafafaf").fadeIn("slow");
        }
    });
}


$(function() {
    GetLatestGithubReleaseInfo();
});