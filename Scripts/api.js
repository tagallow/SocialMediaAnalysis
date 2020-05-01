google.load("visualization", "1", {
    packages: ["annotatedtimeline", "corechart"]
});
var protocol = location.protocol;
var api = {
    socialMonitorCloudURL:
        protocol + "//socialmonitor.whateverittakes.com/api/socialmonitor?",
    liveInstance: undefined,
    currentRequest: undefined,
    latestDate: new Date().toLocaleString(),
    currentPage: 2,
    historyDiv_old: undefined,
    drawChart: function (keyword, divID) {
        var params = {
            request: "TOTAL",
            keyword: keyword,
            runningAvg: 1,
            interval: 0,
            sinceID: "0"
        };
        var posCount = 0;
        var negCount = 0;
        var url = api.socialMonitorCloudURL + jQuery.param(params);
        $.getJSON(url, function (data) {
            keyword = data["keyword"];
            var chartData = new google.visualization.DataTable();
            chartData.addColumn("date", "Date");
            chartData.addColumn("number", "Perception");
            chartData.addColumn("string", "title1");
            chartData.addColumn("string", "text1");
            chartData.addColumn("number", "Volume");
            chartData.addColumn("string", "title2");
            chartData.addColumn("string", "text2");
            var mood = null;
            var vol = null;
            var numpoints = 0;
            for (var i = 0; i < data["dates"].length; i++) {
                mood = data["normalizedMood"][i];
                volume = data["totalHits"][i];
                if (volume < 2) {
                    if (volume == 0) {
                        //mood = 5;
                    }
                    volume = null;
                    mood = 0;
                    continue;
                } else {
                    numpoints++;
                }
                if (data["annotations"][i] == "") {
                    chartData.addRow([
                        new Date(data["dates"][i]),
                        mood,
                        undefined,
                        undefined,
                        volume,
                        undefined,
                        undefined
                    ]);
                } else {
                    chartData.addRow([
                        new Date(data["dates"][i]),
                        mood,
                        data["annotationTitles"][i],
                        data["annotations"][i],
                        volume,
                        undefined,
                        undefined
                    ]);
                    //alert(data['annotations'][i]);
                }
            }

            var options = {
                displayAnnotations: true,
                //'displayAnnotationsFilter': true,
                //'allowHtml': true,
                //'scaleColumns': [0],
                //'scaleType': 'allmaximized',
                min: -1,
                max: 1,
                colors: ["blue", "red"],
                displayZoomButtons: false,
                allowRedraw: true
            };

            var chart = new google.visualization.AnnotatedTimeLine(
                document.getElementById(divID)
            );
            chart.draw(chartData, options);
            //$('#keywordTitle').html(data['keyword']);
        });
    },
    drawChart_noflash: function (keyword, volumeDiv, moodDiv, historyDiv) {
        var params = {
            request: "TOTAL",
            keyword: keyword,
            runningAvg: 1,
            interval: 0,
            sinceID: "0"
        };
        historyDiv_old = historyDiv;
        var posCount = 0;
        var negCount = 0;
        api.currentPage = 2;
        var url = api.socialMonitorCloudURL + jQuery.param(params);
        $("#smpageControls").hide();
        $.getJSON(url, function (data) {
            keyword = data["keyword"];
            var moodData = new google.visualization.DataTable();
            var volumeData = new google.visualization.DataTable();
            moodData.addColumn("date", "Date");
            moodData.addColumn("number", "Perception");
            moodData.addColumn({ type: "string", role: "annotation" });
            moodData.addColumn({
                type: "string",
                role: "annotationText",
                p: { html: true }
            });

            volumeData.addColumn("date", "Date");
            volumeData.addColumn("number", "Volume");

            var mood = null;
            var vol = null;
            var numpoints = 0;
            for (var i = 0; i < data["dates"].length; i++) {
                mood = Number(data["normalizedMood"][i].toFixed(2));
                volume = data["totalHits"][i];

                moodData.addRow([
                    new Date(data["dates"][i]),
                    mood,
                    data["annotationTitles"][i],
                    data["annotations"][i]
                ]);
                volumeData.addRow([new Date(data["dates"][i]), volume]);
                //alert(data['annotations'][i]);
            }

            var moodoptions = {
                legend: { position: "left" },
                //hAxis: { textPosition: 'out' },
                vAxis: {
                    textPosition: "left",
                    gridlines: { count: 2 },
                    title: "Mood",
                    maxValue: 1,
                    minValue: -1
                },
                chartArea: { left: 100, top: 10, width: "90%", height: "90%" }
                //height: 250
            };
            var volumeoptions = {
                legend: { position: "none" },
                hAxis: { textPosition: "out" },
                vAxis: {
                    textPosition: "left",
                    gridlines: { count: 3 },
                    title: "Volume"
                },
                chartArea: { left: 100, top: 10, width: "90%", height: "70%" }
            };

            var moodChart = new google.visualization.LineChart(
                document.getElementById(moodDiv)
            );
            var volumeChart = new google.visualization.AreaChart(
                document.getElementById(volumeDiv)
            );

            function selectHandlerMood() {
                selectHandler(true);
            }

            function selectHandlerVol() {
                selectHandler(false);
            }

            function selectHandler(isMood) {
                //api.currentPage = 1;
                if (api.currentRequest != undefined) {
                    api.currentRequest.abort();
                }
                $(historyDiv).html("Loading data...");
                $("#historyPage").html(" " + api.currentPage + " ");
                if (isMood) selectedItem = moodChart.getSelection()[0];
                else selectedItem = volumeChart.getSelection()[0];

                if (selectedItem) {
                    var topping = moodData.getValue(selectedItem.row, 0);
                    var params = {
                        request: "DAY_VALUES",
                        keyword: keyword,
                        date: new Date(topping).toLocaleDateString()
                    };
                    var url = api.socialMonitorCloudURL + jQuery.param(params);
                    api.currentRequest = $.get(url, "", function (data) {
                        $("#smpageControls").show();
                        $(historyDiv).html("");
                        for (var i = 0; i < data["sentimentList"].length; i++) {
                            var text = data["sentimentList"][i]["text"];
                            var author = data["sentimentList"][i]["author"];
                            var datePosted = new Date(
                                data["sentimentList"][i]["time"]
                            ).toLocaleString();
                            var authorURL =
                                protocol + "//twitter.com/" + author;
                            var tweetURL = authorURL + "/status/";
                            if (i != 0) {
                                $(historyDiv).append(
                                    '<br><br /><hr style="height: 2px;background-color: #ccc">'
                                );
                            }
                            if (data["sentimentList"][i]["source"] != "Unknown")
                                $(historyDiv).append(
                                    '<img src="' +
                                        protocol +
                                        "//socialmonitor.whateverittakes.com/images/social_media_icons/" +
                                        data["sentimentList"][i]["source"] +
                                        '.png" width="75" height="75" align="left">'
                                );
                            //$(historyDiv).append('<img src="/images/social_media_icons/' + data["sentimentList"][i]['source'] + '.png" width="75" height="75" align="left">');

                            $(historyDiv).append(
                                '<b><a href="' +
                                    authorURL +
                                    '" target="_blank">' +
                                    author +
                                    '</a></b><br /><font color="grey">' +
                                    datePosted +
                                    "</font><br />" +
                                    text +
                                    "<br /><br />"
                            );
                            //'#' + divID + 'TwitFeed'
                        }
                        if (data["sentimentList"].length == 0) {
                            $(historyDiv).html("No Data");
                        }
                    }).fail(function () {
                        $(historyDiv).html("History Unavailable");
                    });
                }
            }

            google.visualization.events.addListener(
                moodChart,
                "select",
                selectHandlerMood
            );
            google.visualization.events.addListener(
                volumeChart,
                "select",
                selectHandlerVol
            );
            moodChart.draw(moodData, moodoptions);
            volumeChart.draw(volumeData, volumeoptions);
            //$('#keywordTitle').html(data['keyword']);
        });
    },

    getTimeRange: function (keyword, range, volumeDiv, moodDiv) {
        var params = {
            request: "TIME_RANGE",
            keyword: keyword,
            unit: range
        };
        var url = api.socialMonitorCloudURL + jQuery.param(params);
        $.getJSON(url, "", function (data) {
            keyword = data["keyword"];
            var moodData = new google.visualization.DataTable();
            var volumeData = new google.visualization.DataTable();
            moodData.addColumn("date", "Date");
            moodData.addColumn("number", "Perception");
            moodData.addColumn({ type: "string", role: "annotation" });
            moodData.addColumn({
                type: "string",
                role: "annotationText",
                p: { html: true }
            });

            volumeData.addColumn("date", "Date");
            volumeData.addColumn("number", "Volume");

            var mood = null;
            var vol = null;
            var numpoints = 0;
            for (var i = 0; i < data["dates"].length; i++) {
                mood = data["normalizedMood"][i];
                volume = data["totalHits"][i];

                moodData.addRow([
                    new Date(data["dates"][i]),
                    mood,
                    data["annotationTitles"][i],
                    data["annotations"][i]
                ]);
                volumeData.addRow([new Date(data["dates"][i]), volume]);
                //alert(data['annotations'][i]);
            }

            var moodoptions = {
                legend: { position: "left" },
                //hAxis: { textPosition: 'none' },
                vAxis: {
                    textPosition: "left",
                    gridlines: { count: 2 },
                    title: "Mood",
                    maxValue: 1,
                    minValue: -1
                },
                chartArea: { left: 100, top: 10, width: "90%", height: "90%" }
                //height: 250
            };
            var volumeoptions = {
                legend: { position: "none" },
                hAxis: { textPosition: "out" },
                vAxis: {
                    textPosition: "left",
                    gridlines: { count: 3 },
                    title: "Volume"
                },
                chartArea: { left: 100, top: 10, width: "90%", height: "70%" }
            };

            var moodChart = new google.visualization.LineChart(
                document.getElementById(moodDiv)
            );
            var volumeChart = new google.visualization.AreaChart(
                document.getElementById(volumeDiv)
            );

            function selectHandler() {
                api.currentPage = 1;
                $("#historyPage").html(" " + api.currentPage + " ");
                var selectedItem = moodChart.getSelection()[0];
                if (selectedItem) {
                    var topping = moodData.getValue(selectedItem.row, 0);
                    var params = {
                        request: "DAY_VALUES",
                        keyword: keyword,
                        date: new Date(topping).toLocaleDateString()
                    };
                    var url = api.socialMonitorCloudURL + jQuery.param(params);
                    $.get(url, "", function (data) {
                        $("#tweets").html("");
                        for (var i = 0; i < data["sentimentList"].length; i++) {
                            if (i == 0) {
                                api.latestDate = new Date(
                                    data["sentimentList"][i]["time"]
                                ).toLocaleString();
                            }
                            var text = data["sentimentList"][i]["text"];
                            var author = data["sentimentList"][i]["author"];
                            var datePosted = new Date(
                                data["sentimentList"][i]["time"]
                            ).toLocaleString();
                            var authorURL =
                                protocol + "//twitter.com/" + author;
                            var tweetURL = authorURL + "/status/";
                            if (i != 0) {
                                $("#tweets").append(
                                    '<br><br /><hr style="height: 2px;background-color: #ccc">'
                                );
                            }
                            if (data["sentimentList"][i]["source"] != "Unknown")
                                $("#tweets").append(
                                    '<img src="' +
                                        protocol +
                                        "//socialmonitor.whateverittakes.com/images/social_media_icons/" +
                                        data["sentimentList"][i]["source"] +
                                        '.png" width="75" height="75" align="left">'
                                );
                            //$('#tweets').append('<img src="/images/social_media_icons/' + data["sentimentList"][i]['source'] + '.png" width="75" height="75" align="left">');

                            $("#tweets").append(
                                '<b><a href="' +
                                    authorURL +
                                    '" target="_blank">' +
                                    author +
                                    '</a></b><br /><font color="grey">' +
                                    datePosted +
                                    "</font><br />" +
                                    text +
                                    "<br /><br />"
                            );
                            //'#' + divID + 'TwitFeed'
                        }
                        if (data["sentimentList"].length == 0) {
                            $("#tweets").html("History Unavailable");
                        }
                    }).fail(function () {
                        $("#tweets").html("History Unavailable");
                    });
                }
            }
            google.visualization.events.addListener(
                moodChart,
                "select",
                selectHandler
            );
            google.visualization.events.addListener(
                volumeChart,
                "select",
                selectHandler
            );
            moodChart.draw(moodData, moodoptions);
            volumeChart.draw(volumeData, volumeoptions);
            if (data["sentimentList"] != null) {
                api.currentPage = 1;
                $("#historyPage").html(" " + api.currentPage + " ");
                for (var i = 0; i < data["sentimentList"].length; i++) {
                    if (i == 0) {
                        api.latestDate = new Date(
                            data["dates"][i]
                        ).toLocaleString();
                    }
                    var text = data["sentimentList"][i]["text"];
                    var author = data["sentimentList"][i]["author"];
                    var datePosted = data["sentimentList"][i]["timeString"];
                    var tweetID = data["sentimentList"][i]["twitterID"];
                    var authorURL = protocol + "//twitter.com/" + author;
                    var tweetURL = authorURL + "/status/" + tweetID;
                    if (i != 0) {
                        $("#tweets").html(
                            '<br><br><hr style="height: 2px;background-color: #ccc"><br>' +
                                $("#tweets").html()
                        );
                    }

                    $("#tweets").html(
                        '<b><a href="' +
                            authorURL +
                            '" target="_blank">' +
                            author +
                            '</a></b><br /><font color="grey">' +
                            new Date(
                                parseInt(data["sentimentList"][i]["timeString"])
                            ).toLocaleTimeString() +
                            "</font><br />" +
                            text +
                            "<br /><br>" +
                            $("#tweets").html()
                    );
                    if (data["sentimentList"][i]["source"] != "Unknown")
                        $("#tweets").html(
                            '<img src="' +
                                protocol +
                                "//socialmonitor.whateverittakes.com/images/social_media_icons/" +
                                data["sentimentList"][i]["source"] +
                                '.png" width="75" height="75" align="left">' +
                                $("#tweets").html()
                        );
                    //'#' + divID + 'TwitFeed'
                }
                if (data["sentimentList"].length > 0) {
                    $("#tweets").show("fade", "slow");
                }
            }
            //$('#keywordTitle').html(data['keyword']);
        });
    },
    getLiveDataRow: function (keyword, sinceID, callback) {
        var params = {
            request: "TWITTER_FEED",
            keyword: keyword,
            sinceID: sinceID
        };
        var url = api.socialMonitorCloudURL + jQuery.param(params);
        $.get(url, "", function (data) {
            callback(data);
        });
    },
    live: function (keyword, source, tweetDiv, historyDiv) {
        if (!historyDiv) {
            historyDiv = historyDiv_old;
        }
        //var tweetDiv = tweetDiv;
        var tweetCount = 0;
        $(historyDiv).html("");
        $(tweetDiv).html("Retrieving data from " + source + "...");
        if (api.liveInstance != undefined) {
            clearInterval(api.liveInstance);
            api.liveInstance = undefined;
        }
        $("#moodChart").hide();
        $("#volumeChart").hide();
        $("#pageControls").hide();
        //$(tweetDiv).html('');

        var interval = 10000;

        $(tweetDiv).show("fade", "slow");

        function addRow() {
            var params = {
                request: "TIME_RANGE",
                keyword: keyword,
                unit: "10sec",
                source: source
            };
            var url = api.socialMonitorCloudURL + jQuery.param(params);
            $.getJSON(url, "", function (data) {
                var date = new Date().toLocaleTimeString();
                if (data["sentimentList"].length > 0) {
                    $(tweetDiv).hide("fade", "fast");
                    //volChartData.addRow([date, data['sentimentList'].length]);
                    //moodChartData.addRow([data['dates'][0], data['normalizedMood'][0]]);

                    //moodChartData.removeRow(0);
                    //volChartData.removeRow(0);

                    $(tweetDiv).html(
                        '<hr style="height: 2px;background-color: #ccc">' +
                            $(tweetDiv).html()
                    );
                }
                tweetCount = tweetCount + data["sentimentList"].length;
                if (tweetCount > 10) {
                    $(tweetDiv).html("");
                    tweetCount = data["sentimentList"].length;
                }
                if (data["sentimentList"] != null) {
                    for (
                        var i = 0;
                        i < data["sentimentList"].length && i < 10;
                        i++
                    ) {
                        var text = data["sentimentList"][i]["text"];
                        var author = data["sentimentList"][i]["author"];
                        var datePosted = data["sentimentList"][i]["timeString"];
                        var tweetID = data["sentimentList"][i]["twitterID"];
                        var authorURL = protocol + "//twitter.com/" + author;
                        var tweetURL = authorURL + "/status/" + tweetID;
                        if (i != 0) {
                            $(tweetDiv).html(
                                '<br><hr style="height: 2px;background-color: #ccc"><br>' +
                                    $(tweetDiv).html()
                            );
                        }

                        $(tweetDiv).html(
                            '<b><a href="' +
                                authorURL +
                                '" target="_blank">' +
                                author +
                                '</a></b><br /><font color="grey">' +
                                new Date().toLocaleTimeString() +
                                "</font><br />" +
                                text +
                                "<br /><br><br><br>" +
                                $(tweetDiv).html()
                        );
                        if (data["sentimentList"][i]["source"] != "Unknown")
                            $(tweetDiv).html(
                                '<br><img src="' +
                                    protocol +
                                    "//socialmonitor.whateverittakes.com/images/social_media_icons/" +
                                    data["sentimentList"][i]["source"] +
                                    '.png" width="75" height="75" align="left">' +
                                    $(tweetDiv).html()
                            );
                        //'#' + divID + 'TwitFeed'
                    }
                    if (data["sentimentList"].length > 0) {
                        $(tweetDiv).show("fade", "slow");
                    }
                }
                //drawLiveCharts();
                //addRow();
                //sinceID = data['lastTwitterIDString'];
            });
        }
        addRow();
        api.liveInstance = setInterval(addRow, interval);

        //drawLiveCharts();
    },

    nextPage: function (keyword, div) {
        if (api.currentPage == -1) {
            api.latestDate = new Date();
        }
        api.currentPage++;
        api.getHistoryPage(
            keyword,
            api.latestDate.toLocaleString(),
            api.currentPage,
            div
        );
    },
    prevPage: function (keyword, div) {
        if (api.currentPage > 0) {
            api.currentPage--;
            api.getHistoryPage(keyword, api.latestDate, api.currentPage, div);
        }
    },
    getHistoryPage: function (keyword, date, page, div) {
        if (page < 0) {
            page = 0;
        }
        var params = {
            request: "PAGE",
            keyword: keyword,
            interval: page,
            date: date
        };
        var url = api.socialMonitorCloudURL + jQuery.param(params);
        $.get(url, "", function (data) {
            keyword = data["keyword"];
            $(div).html("");
            var date = new Date().toLocaleTimeString();

            if (data["sentimentList"].length > 0) {
                $(div).hide("fade", "fast");
                $(div).html(
                    '<hr style="height: 2px;background-color: #ccc">' +
                        $(div).html()
                );
            }
            if (data["sentimentList"] != null) {
                for (var i = 0; i < data["sentimentList"].length; i++) {
                    var text = data["sentimentList"][i]["text"];
                    var author = data["sentimentList"][i]["author"];
                    var datePosted = data["sentimentList"][i]["timeString"];
                    var tweetID = data["sentimentList"][i]["twitterID"];
                    var authorURL = protocol + "//twitter.com/" + author;
                    var tweetURL = authorURL + "/status/" + tweetID;
                    if (i != 0) {
                        $(div).html(
                            '<br><hr style="height: 2px;background-color: #ccc"><br>' +
                                $(div).html()
                        );
                    }

                    $(div).html(
                        '<b><a href="' +
                            authorURL +
                            '" target="_blank">' +
                            author +
                            '</a></b><br /><font color="grey">' +
                            data["sentimentList"][i]["timeString"] +
                            "</font><br />" +
                            text +
                            "<br /><br/><br/>" +
                            $(div).html()
                    );
                    if (data["sentimentList"][i]["source"] != "Unknown")
                        $(div).html(
                            '<img src="' +
                                protocol +
                                "//socialmonitor.whateverittakes.com/images/social_media_icons/" +
                                data["sentimentList"][i]["source"] +
                                '.png" width="75" height="75" align="left">' +
                                $(div).html()
                        );
                    //'#' + divID + 'TwitFeed'
                }
                if (data["sentimentList"].length > 0) {
                    $(div).show("fade", "fast");
                }
            }
        });
    },
    getAnnotations: function (keyword) {
        $("#annotationList").html("");
        var params = {
            request: "GET_ANNOTATIONS",
            keyword: keyword
        };
        var url = api.socialMonitorCloudURL + jQuery.param(params);

        $.getJSON(url, "", function (data) {
            return data;
        });
    },
    deleteAnnotation: function (id, keyword) {
        var params = {
            request: "DELETE_ANNOTATION",
            UserID: id,
            keyword: keyword
        };
        var url = api.socialMonitorCloudURL + jQuery.param(params);
        $.post(url, "", function () {});
    },
    insertAnnotation: function (keyword, title, caption, date) {
        var params = {
            request: "INSERT_ANNOTATION",
            title: title,
            caption: caption,
            date: date,
            keyword: keyword
        };
        var url = api.socialMonitorCloudURL + jQuery.param(params);
        $.post(url, "", function () {});
    },
    addKeyword: function (userid, keyword) {
        var params = {
            request: "ADD_KEYWORD",
            UserID: userid,
            keyword: keyword
        };
        var url = api.socialMonitorCloudURL + jQuery.param(params);
        $.post(url, "", function () {
            location.reload();
        });
    },
    removeKeyword: function (userid, keyword) {
        var params = {
            request: "REMOVE_KEYWORD",
            UserID: userid,
            keyword: keyword
        };
        var url = api.socialMonitorCloudURL + jQuery.param(params);
        $.post(url, "", function () {
            location.reload();
        });
    }
};
