/**
** Control panel function

**/

$(function() {
    initInput();
    $("#datetype").change(function() {
        initInput();
    });

    search();
    $("#search").click(function() {
        search();
    });
    
    /* 省份选择 */
    $('#region').bind('click', function() {
        $('#provinces').toggle(100);
    });
    $('#provinces a').bind('click', function() {
        $('#region').val($(this).attr("accesskey"));
        $('#provinces').hide(100);
    });
    
    $("input[name='industry']:checkbox").change(function() {
        setindustry();
    });
    $(".type-select").ruleSingleSelect();
});


//日期框初始化
function initInput() {
    if ($("#datetype").val() == "年") {
        $("#bdate").val("2013"); $("#edate").val("");
        $(".date-input").css("display", "inline-block");
        $(".date-select").css("display", "none");
        $("#bdate").attr("onfocus", "WdatePicker({skin:'twoer', dateFmt:'yyyy'})");
        $("#edate").attr("onfocus", "WdatePicker({skin:'twoer', dateFmt:'yyyy'})");
    }
    if ($("#datetype").val() == "季") {
        $("#qbdate,#bdate").val("20131"); $("#qedate,#edate").val("");
        $(".date-input").css("display", "none");
        $(".date-select").css("display", "inline-block");
        $(".date-select").ruleSingleSelect();
        $("#qbdate").unbind().bind('change', function() { $("#bdate").val(this.value); });
        $("#qedate").unbind().bind('change', function() { $("#edate").val(this.value); });
    }
    if ($("#datetype").val() == "月") {
        $("#bdate").val("201301"); $("#edate").val("");
        $(".date-input").css("display", "inline-block");
        $(".date-select").css("display", "none");
        $("#bdate").attr("onfocus", "WdatePicker({skin:'twoer', dateFmt:'yyyyMM'})");
        $("#edate").attr("onfocus", "WdatePicker({skin:'twoer', dateFmt:'yyyyMM'})");
    }
    if ($("#datetype").val() == "日") {
        $("#bdate").val("20130101"); $("#edate").val("");
        $(".date-input").css("display", "inline-block");
        $(".date-select").css("display", "none");
        $("#bdate").attr("onfocus", "WdatePicker({skin:'twoer', dateFmt:'yyyyMMdd'})");
        $("#edate").attr("onfocus", "WdatePicker({skin:'twoer', dateFmt:'yyyyMMdd'})");
    }

    $("#qbdate,#qedate").empty();
    $("#qbdate,#qedate").append('<option value="" ">请选择季度</option>');
    for (var i = 2010; i <= 2014; i++) {
        $("#qbdate,#qedate").append('<option value="' + i + '1" >' + i + '一季度</option>');
        $("#qbdate,#qedate").append('<option value="' + i + '2" >' + i + '二季度</option>');
        $("#qbdate,#qedate").append('<option value="' + i + '3" >' + i + '三季度</option>');
        $("#qbdate,#qedate").append('<option value="' + i + '4" >' + i + '四季度</option>');
    }
}


//选中相关的checkbox
function checkItems(chkobj, className) {
    if ($(chkobj).text() == "全选") {
        $(chkobj).text("取消");
        $("." + className).prop("checked", true);
    } else {
        $(chkobj).text("全选");
        $("." + className).prop("checked", false);
    }
    setindustry();
}

//清空相关的checkbox
function clearItems(name) {
    $("input[name='" + name + "']:checked").prop("checked", false);
    setindustry();
}

//设置行业显示
function setindustry() {
    $("#industry_lbl dl").empty();
    $("input[name='industry']:checked").each(function() {
        $("#industry_lbl dl").append("<dt>" + $(this).val() + "</dt>");
    });
    $("#menu li").css("background-color", "");
    $("input[name='industry']:checked").each(function() {
        var cl = $(this).attr("class");
        $(".p_" + cl).css("background-color", "red");
    });
}



//查询方法
function search() {
    var province = $("#region").val();
    if ((province) == "全国") {
        province = "";
    }

    var industry = [];
    $("input[name='industry']:checked").each(function() {
        industry.push($(this).val());
    });

    var datetype = $("#datetype").val();
    var bdate = $("#bdate").val();
    var edate = $("#edate").val();
    if (bdate.length == 0) {
        alert('你还没有选择日期！');
        return;
    }
    //默认卖出
    var ruOrchu = "outprovince";
    //设置买入还是卖出
    if (document.getElementById("chu").checked) {
        ruOrchu = "outprovince";
    }
    else if (document.getElementById("ru").checked) {
        ruOrchu = "inprovince";
    }
    var myChart = echarts.init(document.getElementById("main"));
    myChart.clear();
    myChart.showLoading({
        text: '正在努力的读取数据中...'    //loading
    });
    var sendurl = "/tools/submit_ajax2.ashx?action=trade";
    var postdata = { datetype: "" + datetype + "", province: "" + province + "", industry: "" + industry + "", bdate: "" + bdate + "", edate: "" + edate + "", ruOrchu: "" + ruOrchu + "" };
    $.post(sendurl, postdata, function(data, textStatus) {
        if (data.status == 0) {
            myChart.hideLoading();
            alert(data.msg);
        }
        else {
            $("#ajax_data").prop("src", "js/ajax_data.js");
            $("#index").prop("src", "js/index.js");
            $.getScript("js/ajax_data.js?t=" + Math.random(), function() {
                $.getScript("js/index.js?t=" + Math.random(), function() {
                    myChart.hideLoading();
                    //如果结束时间为空
                    if (edate.length == 0) {
                        //如果没有选择城市 则加载全国地图  有线有点
                        if (province == "") {
                            //用来实现地图的省份选择响应事件
                            myChart.on(echarts.config.EVENT.MAP_SELECTED, function(param) {
                                var temp = param.selected;
                                for (var p in temp) {
                                    if (temp[p]) {
                                        $(".area_r").find("a").each(function() {
                                            if ($(this).html() == p) {
                                                $("#region").val($(this).attr("accesskey"));
                                                $("#search").click();
                                            }
                                        });
                                    }
                                }
                            });
                            //赋值
                            option = option;
                        }
                        //否则记载全国地图和对应的省份的地图  有线有点
                        else {
                            //用来实现地图的省份选择响应事件
                            myChart.on(echarts.config.EVENT.MAP_SELECTED, function(param) {
                                var temp = param.selected;
                                for (var p in temp) {
                                    if (temp[p]) {
                                        $(".area_r").find("a").each(function() {
                                            if ($(this).html() == p) {
                                                $("#region").val($(this).attr("accesskey"));
                                                $("#search").click();
                                            }
                                        });
                                    }
                                }
                            });
                            //赋值
                            option = option_province;
                        }
                    }
                    //如果结束时间不为空
                    else {
                        //有时间轴无图例的全国地图
                        if (document.getElementById("isOrno_time").checked) {
                            //用来实现地图的省份选择响应事件
                            myChart.on(echarts.config.EVENT.MAP_SELECTED, function(param) {
                                var temp = param.selected;
                                for (var p in temp) {
                                    if (temp[p]) {
                                        $(".area_r").find("a").each(function() {
                                            if ($(this).html() == p) {
                                                $("#region").val($(this).attr("accesskey"));
                                                $("#search").click();
                                            }
                                        });
                                    }
                                }
                            });
                            //向option中追加参数
                            $.each(markPoint_datas, function(index, item) {
                                options.push(
                                {
                                    dataRange: {
                                        max: max_datas[index]
                                    },
                                    series: [
                                        {
                                            mapLocation: {
                                                x: '350'
                                            },
                                            markPoint: {
                                                symbolSize: function(v) {
                                                    return v / (max_datas[index] / 5) + 1;
                                                },
                                                data: item
                                            }
                                        }
                                    ]
                                }
                            );
                            });
                            //设置option并且赋值
                            option = {
                                timeline:
                                {
                                    data: timeline_data,
                                    type: 'number',
                                    lineStyle:
                                    {
                                        type: 'solid'
                                    },
                                    label:
                                    {
                                        formatter: function(s) {
                                            return s;
                                        },
                                        textStyle: {
                                            color: '#828181'
                                        }
                                    },
                                    checkpointStyle:
                                    {
                                        color: '#c30404',
                                        symbolSize: 8

                                    },
                                    symbolSize: 5,
                                    controlPosition: 'right',
                                    realtime: true,
                                    autoPlay: false,
                                    playInterval: 5000
                                },
                                options: options
                            };
                        }
                        //有图例无时间轴的全国地图
                        else {
                            //用来实现地图的省份选择响应事件
                            myChart.on(echarts.config.EVENT.MAP_SELECTED, function(param) {
                                var temp = param.selected;
                                for (var p in temp) {
                                    if (temp[p]) {
                                        $(".area_r").find("a").each(function() {
                                            if ($(this).html() == p) {
                                                $("#region").val($(this).attr("accesskey"));
                                                $("#search").click();
                                            }
                                        });
                                    }
                                }
                            });
                            //向option中追加参数
                            $.each(markPoint_datas, function(index, item) {
                                series.push(
                               {
                                   name: timeline_data[index + 1],
                                   type: 'map',
                                   hoverable: false,
                                   mapType: 'china',
                                   itemStyle: {
                                       normal: {
                                           borderColor: 'rgba(100,149,237,1)',
                                           borderWidth: 0.5,
                                           areaStyle: {
                                               color: '#1b1b1b'
                                           }
                                       }
                                   },
                                   data: [],
                                   markLine: {
                                       smooth: true,
                                       symbol: 'circle',
                                       symbolSize: function(v) {
                                           return v / (max_data / 5) + 1;
                                       },
                                       itemStyle: {
                                           normal: {
                                               borderWidth: 0.09,
                                               lineStyle: { type: 'solid' },
                                               label: { show: false }
                                           }
                                       },
                                       data: markLine_datas[index]
                                   },
                                   markPoint:
                                   {
                                       symbol: 'circle',
                                       symbolSize: function(v) {
                                           return v / (max_data / 5) + 1;
                                       },
                                       itemStyle: {
                                           normal: {
                                               label: { show: false }
                                           }
                                       },
                                       data: item
                                   }
                               }
                            );
                            });
                            //赋值
                            option = optionx;
                        }
                    }
                    //渲染
                    myChart.setOption(option);
                    myChart.restore();
                });
            });
        }
    }, "json");
}
