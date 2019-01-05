function drawChart(keyword, runAvg, divID) {
    var params = {
        request: 'TOTAL',
        keyword: keyword,
        runningAvg: runAvg,
        interval: 0,
        sinceID: '0'
    };
    var posCount = 0;
    var negCount = 0;
    var url = '/api/socialmonitor?' + jQuery.param(params);
    $.getJSON(
      url,
      function (data) {
          var chartData = new google.visualization.DataTable();
          chartData.addColumn('date', 'Date');
          chartData.addColumn('number', 'Perception');
          chartData.addColumn('string', 'title1');
          chartData.addColumn('string', 'text1');
          chartData.addColumn('number', 'Volume');
          chartData.addColumn('string', 'title2');
          chartData.addColumn('string', 'text2');
          var mood = null;
          var vol = null;
          var numpoints = 0;
          for (var i = 0; i < data['dates'].length; i++) {
              mood = data['normalizedMood'][i];
              volume = data['totalHits'][i];
              if (volume < 2) {
                  if (volume == 0) {
                      //mood = 5;
                  }
                  volume = null;
                  mood = 0;
                  continue;
              }
              else {
                  numpoints++;
              }
              if (data['annotations'][i] == '') {
                  chartData.addRow([new Date(data['dates'][i]), mood, undefined, undefined, volume, undefined, undefined]);
              }
              else {
                  chartData.addRow([new Date(data['dates'][i]), mood, data['annotationTitles'][i], data['annotations'][i], volume, undefined, undefined]);
                  //alert(data['annotations'][i]);
              }
          }

          var options = {
              'displayAnnotations': true,
              //'displayAnnotationsFilter': true,
              //'allowHtml': true,
              //'scaleColumns': [0],
              //'scaleType': 'allmaximized',
              'min': -1,
              'max': 1,
              'colors': ['blue', 'red']
          };

          var chart = new google.visualization.AnnotatedTimeLine(document.getElementById(divID));
          chart.draw(chartData, options);
      }
    );
}
function startLiveChart(keyword, divID) {
    var sinceID = -1
    var volChartData = new google.visualization.DataTable();
    var volChart = new google.visualization.LineChart(document.getElementById(divID));
    volChartData.addColumn('string', 'Date');
    volChartData.addColumn('number', 'Volume');

    //var moodChart = new google.visualization.LineChart(document.getElementById(divID + 'mood'));
    //var moodChartData = new google.visualization.DataTable();
    //moodChartData.addColumn('string', 'Date');
    //moodChartData.addColumn('number', 'Mood');
    var interval = 5000;
    var lastPass = new Date(new Date().getTime());
    $("#tweets").show("fade", "slow");
    function drawLiveCharts() {
        var volumeOptions = {
            width: 850, height: 200,
            legend: {
                position: 'off'
            },
            animation: {
                duration: 1000,
                easing: 'linear'
            },
            vAxis: {
                maxValue: 30, minValue: 0,
                title: 'Volume'
            },
            chartArea: {
                left: 60,
                right: 0,
                top: 8,
                bottom: 9,
                width: 800,
                height: 175
            },
            hAxis: {

            }
        };

        volChart.draw(volChartData, volumeOptions);

        //moodChart.draw(moodChartData, moodOption);
    }

    function addRow() {
        var params = {
            request: 'TWITTER_FEED',
            keyword: keyword,
            runningAvg: 0,
            interval: 0,
            sinceID: sinceID
        };
        var url = '/api/socialmonitor?' + jQuery.param(params);
        $.get(url, '', function (data) {
            //$('#tweets').html('')
            //alert(data['sentimentList'][i]['timeString']);
            var date = new Date().toLocaleTimeString();
            
                //new Date(data['sentimentList'][i]['timeString']).toLocaleTimeString();
            volChartData.addRow([date, data['sentimentList'].length]);
            if (data['sentimentList'].length > 0) {
                //$("#tweets").hide("fade", "fast");
            }
            if (data['sentimentList'].length > 0) {
                $('#tweets').html('<hr style="height: 2px;background-color: #ccc">' + $('#tweets').html());
            }
            for (var i = data['sentimentList'].length - 1; i >= 0;  i--) {
                var text = data['sentimentList'][i]['text'];
                var author = data['sentimentList'][i]['author'];
                var datePosted = data['sentimentList'][i]['timeString'];
                var tweetID = data['sentimentList'][i]['twitterID'];
                var authorURL = 'https://twitter.com/' + author;
                var tweetURL = authorURL + '/status/' + tweetID;
                if (i != data['sentimentList'].length - 1) {
                    $('#tweets').html('<hr style="height: 2px;background-color: #ccc">' + $('#tweets').html());
                }
                $('#tweets').html('<b><a href="' + authorURL + '" target="_blank">' + author + '</a></b><br /><font color="grey">' + new Date(parseInt(data['sentimentList'][i]['timeString'])).toLocaleTimeString() + '</font><br />' + text + '<br />' + $('#tweets').html());
                //'#' + divID + 'TwitFeed'
            }
            if (data['sentimentList'].length > 0) {
                //$("#tweets").show("fade", "slow");
            }
            //moodChartData.addRow([data['dates'][0], data['normalizedMood'][0]]);
            if (volChartData.getNumberOfRows() > 5) {
                volChartData.removeRow(0);
            }
            drawLiveCharts();
            //addRow();
            sinceID = data['lastTwitterIDString'];
        });

    }
    setInterval(addRow, interval);
    addRow();
}
function maximize(keyword, divID) {
    $('#newKeywordForm').html('');
    $('#keyword_list').html('');
    var content = '<tr><td>';
    content += '<span style="float:left; font-weight:bold;font-size:large">' + keyword + '</span>';
    content += '<span style="float:right"><button onclick="location.reload()">return</button></span><br><br>';
    content += '<span id="' + divID + '" style="width: 700px; height: 400px; float:left"></span></td></tr>';
    $('#keyword_list').html(content);
    drawChart(keyword, 1, divID);

}
function live(keyword, divID) {
    $('#newKeywordForm').html('');
    //$('#keyword_list').html('<p>here</p>');
    var content = '<tr><td>';
    content += '<span style="float:left; font-weight:bold;font-size:large">' + keyword + '</span>';
    content += '<span style="float:right" class="button"><a onclick="location.reload()">return</a></span><br><br>';
    //content += '<span id="' + divID + 'mood" style="width: 1050px; height: 400px; float:left"></span>';
    content += '<span id="' + divID + '"></span><br><br><br>';
    content += '</td></tr>';
    $('#keyword_list').html(content);
    startLiveChart(keyword, divID);
    //startTwitterFeed(keyword, divID);

}
function annotate(keyword,id) {
    var options = {
        keyword: keyword,
        userID: id,
    };
    window.location = '/Home/Annotation?' + $.param(options);
    return false;
}