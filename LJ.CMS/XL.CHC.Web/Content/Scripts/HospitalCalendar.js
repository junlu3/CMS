
$(window).resize(function ()
{
    InitCalendarFontSize();
});

$(window).load(function ()
{
    InitCalendarFontSize();
});

function InitCalendarFontSize()
{
    if ($(window).width() <= 600)
    {
        $(".fc-day-number").css("font-size", "12px");
        $("td[class*='fc-day']").css("font-size", "6px");
    }
    else
    {
        $(".fc-day-number").css("font-size", "22px");
        $("td[class*='fc-day']").css("font-size", "12px");
    }
}

var HospitalCalendar = function ()
{


    return {

        //main function to initiate the module
        init: function ()
        {
            HospitalCalendar.initCalendar();
        },

        initCalendar: function ()
        {

            if (!jQuery().fullCalendar)
            {
                return;
            }

            var h = {};

            if (Metronic.isRTL())
            {
                if ($('#calendar').parents(".portlet").width() <= 720)
                {
                    $('#calendar').addClass("mobile");
                    h = {
                        right: 'title, prev, next',
                        center: '',
                        left: 'agendaDay, agendaWeek, month, today'
                    };
                } else
                {
                    $('#calendar').removeClass("mobile");
                    h = {
                        right: 'title',
                        center: '',
                        left: 'agendaDay, agendaWeek, month, today, prev,next'
                    };
                }
            } else
            {
                if ($('#calendar').parents(".portlet").width() <= 720)
                {
                    $('#calendar').addClass("mobile");
                    h = {
                        left: 'title, prev, next',
                        center: '',
                        right: 'today,month,agendaWeek,agendaDay'
                    };
                } else
                {
                    $('#calendar').removeClass("mobile");
                    h = {
                        left: 'title',
                        center: '',
                        right: 'prev,next,today,month,agendaWeek,agendaDay'
                    };
                }
            }

            //predefined events
            $('#event_box').html("");
            $('#calendar').fullCalendar('destroy'); // destroy the calendar
            $('#calendar').fullCalendar({ //re-initialize the calendar
                header: null,
                defaultView: 'month', // change default view with available options from http://arshaw.com/fullcalendar/docs/views/Available_Views/ 
                slotMinutes: 15,
                editable: true,
                droppable: false, // this allows things to be dropped onto the calendar !!!
                lang: "zh-cn",
                firstDay: 1,
                firstHour: 0,
                titleFormat: {
                    month: 'yyyy年 MM月',

                },
                defaultDate: $("#txtDate").val(),
                dayClick: function (a, b, c)
                {
                    //alert('a day has been clicked!');
                }

            });

        },

        InitStyle: function ()
        {
            $(".fc-day").css("cursor", "not-allowed");
            $(".fc-day").css("color", "black");
            $(".fc-day").text("未开放预约");
            $(".fc-day-number").css("font-size", "22px");
            $(".fc-other-month").css("cursor", "not-allowed");
            $(".fc-other-month").css("background-color", "#E5E5E5");
            $(".fc-day").css("vertical-align", "middle");
            $(".fc-day").css("text-align", "center");

            $("td[class*='fc-other-month'][class*='fc-day fc-widget-content']").text("");
        }

    };

}();

Date.prototype.format = function (format)
{
    /* 
     * 使用例子:format="yyyy-MM-dd hh:mm:ss"; 
     */
    var o = {
        "M+": this.getMonth() + 1, // month  
        "d+": this.getDate(), // day  
        "h+": this.getHours(), // hour  
        "m+": this.getMinutes(), // minute  
        "s+": this.getSeconds(), // second  
        "q+": Math.floor((this.getMonth() + 3) / 3), // quarter  
        "S": this.getMilliseconds()
        // millisecond  
    }

    if (/(y+)/.test(format))
    {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4
                        - RegExp.$1.length));
    }

    for (var k in o)
    {
        if (new RegExp("(" + k + ")").test(format))
        {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1
                            ? o[k]
                            : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}